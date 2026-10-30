# Pacote de DLLs para Integração com o Linx Microvix (Outbound)

## O que é o Linx Microvix?
O **Linx Microvix** é o ERP da Linx para o varejo (lojas, estoque, vendas, fiscal e financeiro), com um e-commerce
próprio, o **B2C**. Ele expõe os dados por dois web services de leitura, os dois em XML:

* **Web Service de Saída Padrão (ERP)**, documentado na **v274**: 191 métodos de leitura (`Linx*`);
* **WS B2C**, documentado na **V64**: 69 métodos de consulta (`B2CConsulta*`). Os métodos de escrita do B2C usam outra
  URL e pertencem ao projeto **Inbound**.

### Conceitos usados pela integração
* **Portal**: o ambiente de um cliente no Microvix. Um cliente pode ter várias lojas no mesmo portal.
* **Chave**: GUID de ativação de cada web service, gerado pela Linx por cliente. Vai no corpo de toda chamada, como o
  token nas APIs REST, e **nunca** é gravado em log.
* **Loja (`cnpjEmp`)**: CNPJ da loja. Os métodos de loja são chamados uma vez por loja; as lojas do cliente vêm do
  `LinxGrupoLojas` (ERP) e do `B2CConsultaEmpresas` (B2C).
* **Timestamp**: versão do registro no Microvix. Cada chamada pede só os registros com timestamp maior que o
  informado; a leitura incremental parte do maior timestamp gravado.

---

## Sobre o pacote

Módulo do sistema de integrações (ETL) responsável por **ler** os dados dos dois web services do Microvix e gravá-los
no banco SQL Server:

* de forma **recorrente**, pelos jobs do Hangfire, com leitura incremental pelo timestamp. A primeira execução de cada
  método (`timestamp = 0`) traz o histórico inteiro;
* **sob demanda**, a partir de um sistema externo, buscando um registro por chave e registrando quem pediu cada
  chamada.

O módulo **não escreve** no Microvix.

| Item | Valor |
|---|---|
| Plataforma | .NET 10 (`net10.0`) |
| Arquitetura | Clean Architecture em três assemblies: Domain, Application e Infrastructure |
| API consumida | `POST https://webapi.microvix.com.br/1.0/api/integracao/<Comando>`, corpo XML (`application/xml`, UTF-8) com usuário, senha, comando e parâmetros |
| Documentação da API | Web Service de Saída Padrão v274 e WS B2C V64 (PDFs da Linx); o método `LinxMetodos` devolve o catálogo de comandos e parâmetros aceitos hoje |
| Banco de dados | SQL Server (schemas `untreated`, `linxmicrovix` e `auditing`; o `Script001` cria `linxmicrovix` quando não existe) |
| Multiempresa | Sim: cada CNPJ de `linxmicrovix.LinxMicrovixParameters` tem as chaves e credenciais de cada API. O CNPJ (`doc_company`) é a primeira coluna da chave de todas as tabelas |
| Host | `src/Backend/Hosts/Hangfire` (controller `LinxMicrovixController`, `partial` por service) |

### Comportamentos da API que a integração trata
A documentação não descreve tudo; o que segue foi conferido com chamadas reais (detalhes em
[`src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md`](src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md), §4.2 e §9):

* **Os erros chegam com HTTP 200**, com `ResponseSuccess = False` e a mensagem em `<Message>`.
* **Erro de parâmetro também vem com `ResponseSuccess = True`**: `<C />` vazio e a mensagem numa linha (visto no
  `LinxFaturas`). Os métodos tratam isso como erro.
* **A resposta vem com BOM** e em ordem crescente de timestamp. A próxima página começa no maior timestamp da página
  anterior, e a leitura repete até a resposta vir vazia.
* **O limite de linhas por chamada muda por método** (5.000 no `LinxMovimento`, 100 no `LinxUsuarios`). O **B2C não
  tem limite**: com timestamp 0 o `B2CConsultaPedidosStatus` trouxe 408.663 linhas numa chamada.
* **Datas obrigatórias**: vão de 2000-01-01 até hoje. Com `filtra_somente_timestamp = 1` a API ignora as datas, mas
  elas continuam obrigatórias.
* **`filtra_somente_timestamp = 1` também desliga o filtro da busca** (`documento`, `doc_origem`...). As leituras sob
  demanda mandam 0.
* **`<IdPortal>` não tem efeito** e não é enviado; o filtro de portal do `LinxGrupoLojas` é o parâmetro `portal`.
* **Escopo do método**: um método só é de portal quando duas lojas devolvem os mesmos dados (testado método a
  método). Esses são lidos pela primeira loja; os demais são lidos por loja, com `cnpj_emp` na chave.
* **Coluna repetida na resposta** (ex.: `valor` no `LinxPagamentoParcialDav`): a segunda ocorrência vira `<nome>_2`.
* **Parte da chave vazia** vira o valor padrão (0, texto vazio ou 1900-01-01); só o registro sem nenhuma parte da
  chave é descartado.

Os XMLs usados para conferir a modelagem estão na pasta [`Responsed XMLs`](#pasta-docsresponsed-xmls), com os dados reais
trocados por dados fictícios.

---

## Arquitetura

```
LinxMicrovix - Outbound Web Service/
├── Domain.LinxMicrovix.Outbound          ← contratos, DTOs, entidades e enums (sem dependência de infraestrutura)
├── Application.LinxMicrovix.Outbound     ← services (leituras), queries SQL e validações
├── Infrastructure.LinxMicrovix.Outbound  ← repositórios, injeção de dependência e scripts de banco
└── docs                                  ← propostas, planos e implementações do módulo
    └── Responsed XMLs  ← uma resposta de exemplo por método (dados fictícios)
```

### Referências entre projetos

```
Infrastructure.LinxMicrovix.Outbound ──► Application.LinxMicrovix.Outbound ──► Domain.LinxMicrovix.Outbound
        │                                         │                                    │
        ▼                                         ▼                                    ▼
Infrastructure.Shared                     Application.Shared                     Domain.Shared
```

| Assembly | Pacotes NuGet | Referências |
|---|---|---|
| `Domain.LinxMicrovix.Outbound` | — | `Domain.Shared` |
| `Application.LinxMicrovix.Outbound` | `Microsoft.Extensions.Caching.Memory` 10.0.8 | `Application.Shared`, `Domain.LinxMicrovix.Outbound` |
| `Infrastructure.LinxMicrovix.Outbound` | — | `Infrastructure.Shared`, `Application.LinxMicrovix.Outbound`, `Domain.LinxMicrovix.Outbound` |

Do projeto Shared o módulo usa:
* `IApiClient.SendAsync` (cliente HTTP que devolve status, corpo e cabeçalhos);
* `ILoggerService` (auditoria);
* `ISharedRepository` (consultas, bulk insert e chamada das procedures de merge).

---

## Domain.LinxMicrovix.Outbound

```
Domain.LinxMicrovix.Outbound/
├── Abstractions/Repositorys/   ← ILinxMicrovixRepository (comum) e um ILinxMicrovix<Service>Repository por service
├── Dtos/Reads/                 ← uma classe por método, com as colunas da resposta
│   ├── Erp/<Service>/          (Stores, Customers, Products, Movements...)
│   └── B2C/<Service>/          (B2CStores, B2CCustomers, B2COrders...)
├── Entities/
│   ├── Parameters/             (Company → linxmicrovix.LinxMicrovixParameters)
│   ├── Reads/                  ← o que é gravado, nas mesmas pastas dos DTOs
│   └── Writes/
│       └── Requests/           (Request → linxmicrovix.LinxMicrovixRequests)
├── Enums/                      (EnumApi: Erp e B2C; EnumJob)
└── Extensions/                 (MicrovixConverters: texto da resposta → tipos das entidades)
```

### Convenções

* **O nome do método é o nome do comando da API**, sem prefixo (`LinxClientesEnderecosEntrega()`,
  `B2CConsultaPedidos()`). Tabela, job, rota e DTO levam o mesmo nome.
* **DTOs** têm uma propriedade `string` por coluna da resposta, com o nome da coluna. É **uma classe por arquivo**.
* **Entidades** têm os tipos das colunas e os **mesmos nomes das colunas**, porque o bulk insert mapeia as colunas
  pelo nome da propriedade.
  * Convertem os campos do DTO no construtor (`MicrovixConverters`) e cortam os textos no tamanho da coluna.
  * Os tipos e tamanhos vêm da v274/V64; um tipo foi alargado quando a resposta real não cabia.
* Toda entidade tem `doc_company` (CNPJ do cliente) e `integrated_or_updated_at`, e as de loja têm `cnpj_emp`. Os
  registros de uma gravação recebem **o mesmo horário de lote** (`LinxMicrovixService.GetBatchTime()`).
* `EnumJob` deve ficar em sincronia com os scripts de jobs de cada service (`Script0NN_Insert_LinxMicrovix_*_Jobs_Into_Jobs_Table.sql`).

---

## Application.LinxMicrovix.Outbound

```
Application.LinxMicrovix.Outbound/
├── Abstractions/
│   ├── Handlers/Commands/      (ILinxMicrovixCommandHandler)
│   └── Services/               (ILinxMicrovixService e um por service)
├── Handlers/Commands/          (LinxMicrovixCommandHandler: monta as queries SQL)
├── Services/                   (LinxMicrovixService e os 29 services por assunto)
└── Validations/                (FluentValidation por DTO, nas mesmas pastas dos DTOs)
```

Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo timestamp, leitura da resposta,
validação e gravação), como no Totvs; não há classe base nem transporte central.

### `LinxMicrovixService`: operações comuns aos outros services

| Método | O que faz |
|---|---|
| `GetBatchTime` | Horário do lote (horário de Brasília), gravado igual em todos os registros de uma gravação |
| `GetCompanys` / `GetCompany` | Clientes de `linxmicrovix.LinxMicrovixParameters` (em cache, chave `LinxMicrovix:Parameters`, 2 h absoluto / 30 min deslizante) / um cliente pelo CNPJ |
| `GetStores` | CNPJs das lojas do cliente em cada API (`LinxGrupoLojas` no ERP, `B2CConsultaEmpresas` no B2C), sem cache |
| `GetLastTimestamp` | Maior timestamp gravado do método para o cliente e a loja; com uma coluna de filtro, por valor (ex.: por `id_tabela`) |
| `GetSourceValues` | Valores de uma coluna de outro método já gravado, para os métodos que exigem um valor por chamada; pode trazer só os gravados depois da última leitura |
| `SaveRequest` | Grava uma chamada sob demanda em `linxmicrovix.LinxMicrovixRequests`, ligada ao `Execution` do log. O XML enviado vai em `request_body` com a chave trocada por `(oculta)` |

### Services e métodos (leituras recorrentes)

Escopo: **loja** = uma chamada por loja, com `cnpj_emp` na chave; **portal (1ª loja)** = dado do portal, lido pela
primeira loja; **portal** = sem `cnpjEmp`. Parâmetros: além de `chave`, `cnpjEmp` e `timestamp`. Chave: PK da tabela,
sem `doc_company`.

#### `LinxMicrovixStoresService` (ERP, lojas, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxGrupoLojas` | portal | `grupo` = LinxMicrovixParameters.grupo (opcional) | `portal`, `empresa` | 8000 | 8500 |
| `LinxLojas` | loja | — | `cnpj_emp` | 8001 | 8501 |
| `LinxLojasParametros` | loja | — | `cnpj_emp` | 8002 | 8502 |
| `LinxUsuarios` | portal | — | `portal`, `usuario_id` | 8003 | 8503 |
| `LinxMetodos` | catálogo | — | `metodo` | 8004 | 8504 |
| `LinxEntrantes` | portal | — | `portal`, `id_entrantes` | 8005 | 8505 |

#### `LinxMicrovixB2CStoresService` (B2C, empresas, vendedores, transportadores e planos do B2C, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `B2CConsultaEmpresas` | loja | — | `cnpj_emp`, `empresa` | 8250 | 8691 |
| `B2CConsultaCNPJsChave` | portal | — | `portal`, `CNPJ` | 8251 | 8692 |
| `B2CConsultaVendedores` | portal (1ª loja) | — | `portal`, `cod_vendedor` | 8252 | 8693 |
| `B2CConsultaTransportadores` | portal (1ª loja) | — | `portal`, `cod_transportador` | 8253 | 8694 |
| `B2CConsultaFormasPagamento` | portal (1ª loja) | — | `portal`, `cod_forma_pgto` | 8254 | 8695 |
| `B2CConsultaPlanos` | portal (1ª loja) | — | `portal`, `plano` | 8255 | 8696 |
| `B2CConsultaPlanosParcelas` | portal (1ª loja) | — | `portal`, `id_planos_parcelas` | 8256 | 8697 |
| `B2CConsultaTiposCobrancaFrete` | portal (1ª loja) | — | `portal`, `codigo_tipo_cobranca_frete` | 8257 | 8698 |
| `B2CConsultaTipoEncomenda` | portal (1ª loja) | — | `portal`, `id_tipo_encomenda` | 8258 | 8699 |

#### `LinxMicrovixProductAttributesService` (ERP, atributos de produto, a cada 1 dia)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxSetores` | portal (1ª loja) | — | `portal`, `id_setor` | 8034 | 8534 |
| `LinxLinhas` | portal (1ª loja) | — | `portal`, `id_linha` | 8035 | 8535 |
| `LinxMarcas` | portal (1ª loja) | — | `portal`, `id_marca` | 8036 | 8536 |
| `LinxColecoes` | portal (1ª loja) | — | `portal`, `id_colecao` | 8037 | 8537 |
| `LinxTamanhos` | portal (1ª loja) | — | `portal`, `id_tamanho` | 8038 | 8538 |
| `LinxCores` | portal (1ª loja) | — | `portal`, `id_cor` | 8039 | 8539 |
| `LinxEspessuras` | portal (1ª loja) | — | `portal`, `id_espessura` | 8040 | 8540 |
| `LinxUnidades` | portal (1ª loja) | — | `portal`, `unidade` | 8041 | 8541 |
| `LinxClassificacoes` | portal (1ª loja) | — | `portal`, `id_classificacao` | 8042 | 8542 |
| `LinxSubClasses` | loja | — | `cnpj_emp`, `id_subclasses` | 8043 | 8543 |

#### `LinxMicrovixTaxesService` (ERP, tabelas fiscais, a cada 1 dia)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxNaturezaOperacao` | portal (1ª loja) | — | `portal`, `cod_natureza_operacao` | 8063 | 8563 |
| `LinxCfopFiscal` | portal (1ª loja) | — | `portal`, `id_cfop_fiscal` | 8064 | 8564 |
| `LinxCstIcmsFiscal` | portal (1ª loja) | — | `portal`, `id_cst_icms_fiscal` | 8065 | 8565 |
| `LinxCstIpiFiscal` | portal (1ª loja) | — | `portal`, `id_cst_ipi_fiscal` | 8066 | 8566 |
| `LinxCstPisFiscal` | portal (1ª loja) | — | `portal`, `id_cst_pis_fiscal` | 8067 | 8567 |
| `LinxCstCofinsFiscal` | portal (1ª loja) | — | `portal`, `id_cst_cofins_fiscal` | 8068 | 8568 |
| `LinxCsosnFiscal` | portal (1ª loja) | — | `portal`, `id_csosn_fiscal` | 8069 | 8569 |
| `LinxClasseFiscal` | portal (1ª loja) | — | `portal`, `id_classe_fiscal` | 8070 | 8570 |
| `LinxCest` | portal (1ª loja) | — | `portal`, `id_cest` | 8071 | 8571 |
| `LinxNcm` | portal (1ª loja) | — | `portal`, `codigo` | 8072 | 8572 |
| `LinxEnquadramentoIpi` | portal (1ª loja) | — | `portal`, `id_enquadramento_ipi` | 8073 | 8573 |
| `LinxMotivosDesoneracaoIcms` | portal (1ª loja) | — | `portal`, `id_motivos_desoneracao_icms` | 8074 | 8574 |
| `LinxSpedTipoItem` | portal (1ª loja) | — | `portal`, `id_sped_tipo_item` | 8075 | 8575 |
| `LinxSpedTipoBaseCredito` | portal (1ª loja) | — | `portal`, `id_sped_tipo_base_credito` | 8076 | 8576 |
| `LinxRotinaOrigem` | portal (1ª loja) | — | `portal`, `codigo_rotina` | 8077 | 8577 |
| `LinxNfeIbgeMunicipio` | portal (1ª loja) | — | `portal`, `id_nfe_ibge_municipio` | 8078 | 8578 |

#### `LinxMicrovixTaxSettingsService` (ERP, configurações tributárias, a cada 1 dia)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxConfiguracoesTributarias` | loja | — | `cnpj_emp`, `id_config_tributaria` | 8079 | 8579 |
| `LinxConfiguracoesTributariasDetalhes` | loja | `id_config_tributaria` de `LinxConfiguracoesTributarias` | `cnpj_emp`, `id_config_tributaria`, `cod_natureza_operacao`, `id_classe_fiscal` | 8080 | 8580 |
| `LinxConfiguracoesTributariasDetalhesSimplificado` | portal (1ª loja) | — | `portal`, `id_config_tributaria_detalhe` | 8081 | 8581 |
| `LinxConfiguracoesTributariasEmpresas` | loja | — | `cnpj_emp`, `id_config_tributaria`, `empresa` | 8082 | 8582 |

#### `LinxMicrovixProductsService` (ERP, produtos, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxProdutos` | portal (1ª loja) | `dt_update_inicio` = 2000-01-01; `dt_update_fim` = hoje | `portal`, `cod_produto` | 8024 | 8524 |
| `LinxProdutosAssociados` | loja | — | `cnpj_emp`, `codigoproduto_origem`, `codigoproduto_associado` | 8025 | 8525 |
| `LinxProdutosCamposAdicionais` | loja | `data_mov_ini` = 2000-01-01; `data_mov_fim` = hoje; `filtra_somente_timestamp` = 1 | `cnpj_emp`, `cod_produto`, `campo` | 8026 | 8526 |
| `LinxProdutosCodBar` | portal (1ª loja) | — | `portal`, `cod_produto`, `cod_barra` | 8027 | 8527 |
| `LinxProdutosFornec` | loja | — | `cnpj_emp`, `id_prod_forn` | 8028 | 8528 |
| `LinxProdutosInformacoes` | portal (1ª loja) | — | `portal`, `codigoproduto` | 8029 | 8529 |
| `LinxProdutosImagensURL` | portal (1ª loja) | — | `portal`, `id_imagem`, `codigoproduto` | 8030 | 8530 |
| `LinxProdutosLotes` | loja | — | `cnpj_emp`, `codigoproduto`, `empresa`, `lote`, `deposito` | 8031 | 8531 |
| `LinxLotesProdutos` | loja | — | `cnpj_emp`, `id_lote` | 8032 | 8532 |
| `LinxProdutosSerial` | loja | — | `cnpj_emp`, `codigoproduto`, `empresa`, `serial`, `id_deposito` | 8033 | 8533 |

#### `LinxMicrovixStocksService` (ERP, estoque e balanços, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxProdutosDetalhes` | loja | `data_mov_ini` = 2000-01-01; `data_mov_fim` = hoje | `cnpj_emp`, `cod_produto` | 8044 | 8544 |
| `LinxProdutosDetalhesSimplificado` | loja | — | `cnpj_emp`, `cod_produto` | 8045 | 8545 |
| `LinxProdutosDetalhesDepositos` | loja | `cod_deposito` de `LinxProdutosDepositos` | `cnpj_emp`, `cod_deposito`, `cod_produto` | 8046 | 8546 |
| `LinxProdutosDepositos` | portal (1ª loja) | — | `portal`, `cod_deposito` | 8047 | 8547 |
| `LinxProdutosInventario` | loja | `data_inventario` = hoje | `cnpj_emp`, `cod_produto` | 8048 | 8548 |
| `LinxListagemBalanco` | loja | — | `cnpj_emp`, `id_balanco` | 8049 | 8549 |
| `LinxBalancos` | loja | — | `cnpj_emp`, `id_balancos` | 8050 | 8550 |
| `LinxBalancosArquivos` | loja | — | `cnpj_emp`, `id_balancos_arquivos` | 8051 | 8551 |
| `LinxBalancosSecoes` | portal (1ª loja) | — | `portal`, `id_balancos_secoes` | 8052 | 8552 |

#### `LinxMicrovixPricesService` (ERP, preços, promoções e cupons, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxProdutosTabelas` | loja | — | `cnpj_emp`, `id_tabela` | 8053 | 8553 |
| `LinxProdutosTabelasPrecos` | loja | `id_tabela` de `LinxProdutosTabelas` | `cnpj_emp`, `id_tabela`, `cod_produto` | 8054 | 8554 |
| `LinxProdutosPromocoes` | loja | `data_cad_inicial` = 2000-01-01; `data_cad_fim` = hoje; `data_vig_inicial` = 2000-01-01; `data_vig_fim` = hoje; `filtra_somente_timestamp` = 1; `promocao_ativa` = S e N | `cnpj_emp`, `cod_produto`, `data_inicio_promocao`, `id_campanha` | 8055 | 8555 |
| `LinxAcoesPromocionais` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `id_acoes_promocionais` | 8056 | 8556 |
| `LinxAcoesPromocionaisCombinacaoProdutosItens` | portal (1ª loja) | — | `portal`, `id_acoes_promocionais_combinacao_produtos_itens` | 8057 | 8557 |
| `LinxAcoesPromocionaisProdutosCortesia` | portal (1ª loja) | — | `portal`, `id_acoes_promocionais_produtos_cortesia` | 8058 | 8558 |
| `LinxMovimentoAcoesPromocionais` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `transacao`, `id_acoes_promocionais`, `timestamp` | 8059 | 8559 |
| `LinxCupomDesconto` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `cupom` | 8060 | 8560 |
| `LinxCupomDescontoVendas` | loja | — | `cnpj_emp`, `identificador` | 8061 | 8561 |
| `LinxMotivoDesconto` | portal (1ª loja) | — | `portal`, `id_motivo_desconto` | 8062 | 8562 |

#### `LinxMicrovixCustomersService` (ERP, clientes e fornecedores, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxClientesFornec` | portal (1ª loja) | `data_inicial` = 2000-01-01; `data_fim` = hoje; `dt_update_inicial` = 2000-01-01; `dt_update_fim` = hoje | `portal`, `cod_cliente` | 8006 | 8506 |
| `LinxClientesFornecCamposAdicionais` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `cod_cliente`, `campo` | 8007 | 8507 |
| `LinxClientesFornecClasses` | loja | — | `cnpj_emp`, `cod_cliente`, `cod_classe` | 8008 | 8508 |
| `LinxClientesFornecContatos` | portal (1ª loja) | `data_inicial` = 2000-01-01; `data_fim` = hoje | `portal`, `cod_cliente`, `nome_contato`, `tipo_contato` | 8009 | 8509 |
| `LinxClientesFornecContatosParentesco` | portal (1ª loja) | — | `portal`, `id_parentesco` | 8010 | 8510 |
| `LinxClientesFornecCreditoAvulso` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `cod_cliente`, `controle` | 8011 | 8511 |
| `LinxClientesFornecVendedores` | loja | — | `cnpj_emp`, `cod_cliente`, `cod_vendedor` | 8012 | 8512 |
| `LinxClientesEnderecosEntrega` | portal (1ª loja) | — | `portal`, `id_endereco_entrega` | 8013 | 8513 |
| `LinxClientesRede` | loja | — | `cnpj_emp`, `id_clientes_rede` | 8014 | 8514 |
| `LinxRamosAtividade` | loja | — | `cnpj_emp`, `id_ramo_atividade` | 8015 | 8515 |

#### `LinxMicrovixSellersService` (ERP, vendedores e funcionários, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxVendedores` | loja | `data_upd_inicial` = 2000-01-01; `data_upd_fim` = hoje | `cnpj_emp`, `cod_vendedor` | 8016 | 8516 |
| `LinxVendedoresFerias` | loja | — | `cnpj_emp`, `cod_vendedor`, `data_inicial` | 8017 | 8517 |
| `LinxMetasVendedores` | loja | `data_inicial_meta` = 2000-01-01; `data_fim_meta` = hoje | `cnpj_emp`, `id_meta`, `cod_vendedor` | 8018 | 8518 |
| `LinxMetasVendedoresDia` | loja | `data_inicial_meta` = 2000-01-01; `data_fim_meta` = hoje | `cnpj_emp`, `id_meta`, `cod_vendedor`, `dia` | 8019 | 8519 |
| `LinxComissoesVendedores` | loja | — | `cnpj_emp`, `vendedor`, `empresa`, `data_origem`, `valor_base`, `valor_comissao`, `cancelado` | 8020 | 8520 |
| `LinxListaDaVez` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `cod_vendedor`, `data`, `timestamp` | 8021 | 8521 |
| `LinxFuncionariosFuncoes` | portal (1ª loja) | — | `portal`, `id_funcionarios_funcao` | 8022 | 8522 |
| `LinxRegistroProfissional` | loja | — | `cnpj_emp`, `id_tipo_registro_profissional` | 8023 | 8523 |

#### `LinxMicrovixPaymentsService` (ERP, pagamentos e caixa, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxMovimentoCartoes` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `identificador`, `ordem_cartao` | 8110 | 8610 |
| `LinxMovimentoPlanos` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `identificador`, `plano`, `ordem_cartao` | 8111 | 8611 |
| `LinxPlanos` | portal (1ª loja) | `data_upd_inicial` = 2000-01-01; `data_upd_fim` = hoje | `portal`, `plano` | 8112 | 8612 |
| `LinxPlanosParcelas` | portal (1ª loja) | — | `portal`, `id_planos_parcelas` | 8113 | 8613 |
| `LinxPlanosBandeiras` | loja | `data_upd_inicial` = 2000-01-01; `data_upd_fim` = hoje | `cnpj_emp`, `plano`, `bandeira` | 8114 | 8614 |
| `LinxMovimentoGiftCard` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `identificador_movimento`, `nsu_cliente` | 8115 | 8615 |
| `LinxValesComprasEnviadosAPI` | loja | — | `cnpj_emp`, `numero_controle` | 8116 | 8616 |
| `LinxSangriaSuprimentos` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje; `filtra_somente_timestamp` = 1 | `cnpj_emp`, `id_sangria_suprimentos` | 8117 | 8617 |
| `LinxFechamentoCaixa` | loja | — | `cnpj_emp`, `empresa`, `data`, `usuario` | 8118 | 8618 |

#### `LinxMicrovixLoyaltyService` (ERP, fidelidade, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxFidelidade` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `id_fidelidade_parceiro_log` | 8119 | 8619 |
| `LinxMovimentoReshop` | portal (1ª loja) | — | `portal`, `id_movimento_campanha_reshop` | 8120 | 8620 |
| `LinxMovimentoReshopItens` | portal (1ª loja) | — | `portal`, `id_movimento_campanha_reshop_item` | 8121 | 8621 |

#### `LinxMicrovixMovementsService` (ERP, movimento, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxMovimento` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje; `dt_update_inicial` = 2000-01-01; `dt_update_fim` = hoje; `filtra_somente_timestamp` = 1 | `cnpj_emp`, `transacao` | 8095 | 8595 |
| `LinxMovimentoPrincipal` | loja | — | `cnpj_emp`, `id_movimento_principal` | 8096 | 8596 |
| `LinxMovimentoExtensao` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `transacao` | 8097 | 8597 |
| `LinxMovimentoCamposAdicionais` | loja | — | `cnpj_emp`, `transacao` | 8098 | 8598 |
| `LinxMovimentoObservacoes` | portal (1ª loja) | — | `portal`, `id_obs` | 8099 | 8599 |
| `LinxMovimentoObservacaoCF` | portal (1ª loja) | — | `portal`, `identificador` | 8100 | 8600 |
| `LinxMovimentoIndicacoes` | loja | — | `cnpj_emp`, `identificador`, `cod_cliente` | 8101 | 8601 |
| `LinxMovimentoSerial` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `identificador`, `transacao`, `serial` | 8102 | 8602 |
| `LinxSerialVenda` | loja | — | `cnpj_emp`, `id_serial_venda` | 8103 | 8603 |
| `LinxMovimentoVendaConjugada` | loja | — | `cnpj_emp`, `identificador_produto`, `identificador_servico` | 8104 | 8604 |
| `LinxPerguntaVenda` | loja | — | `cnpj_emp`, `id_pergunta_venda` | 8105 | 8605 |
| `LinxRespostaVenda` | loja | — | `cnpj_emp`, `id_resposta_venda` | 8106 | 8606 |

#### `LinxMicrovixPosSalesService` (ERP, vendas POS temporárias, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxVendasPosTmp` | loja | — | `cnpj_emp`, `id_vendas_pos` | 8107 | 8607 |
| `LinxVendasPosProdutosTmp` | loja | — | `cnpj_emp`, `id_vendas_pos_produtos` | 8108 | 8608 |
| `LinxVendasPosAgrupamentoTmp` | portal (1ª loja) | — | `portal`, `id_vendas_pos_agrupamento_tmp` | 8109 | 8609 |

#### `LinxMicrovixReturnsService` (ERP, trocas e devoluções, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxMovimentoTrocas` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `identificador`, `num_vale` | 8122 | 8622 |
| `LinxMovimentoTrocafone` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `identificador`, `num_vale` | 8123 | 8623 |
| `LinxMovimentoDevolucoesItens` | loja | — | `cnpj_emp`, `id_movimento_devolucoes_itens` | 8124 | 8624 |
| `LinxMovimentoOrigemDevolucoes` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `identificador`, `nota_origem` | 8125 | 8625 |
| `LinxMotivoDevolucao` | portal (1ª loja) | — | `portal`, `cod_motivo` | 8126 | 8626 |
| `LinxTrocaUnificadaResumoVendas` | loja | — | `cnpj_emp`, `id_troca_unificada_resumo_vendas` | 8127 | 8627 |
| `LinxTrocaUnificadaResumoVendasItens` | loja | — | `cnpj_emp`, `id_troca_unificada_resumo_vendas_itens` | 8128 | 8628 |
| `LinxTrocaUnificadaResumoBaixa` | loja | — | `cnpj_emp`, `id_troca_baixa` | 8129 | 8629 |
| `LinxTradeinParceiro` | portal (1ª loja) | — | `portal`, `id_tradein_parceiro` | 8130 | 8630 |
| `LinxDevolucaoRemanejoFabrica` | loja | — | `cnpj_emp`, `id_devolucao_remanejo_fabrica` | 8131 | 8631 |
| `LinxDevolucaoRemanejoFabricaItem` | loja | — | `cnpj_emp`, `id_devolucao_remanejo_fabrica_item` | 8132 | 8632 |
| `LinxDevolucaoRemanejoFabricaStatus` | portal (1ª loja) | — | `portal`, `id_devolucao_remanejo_fabrica_status` | 8133 | 8633 |
| `LinxDevolucaoRemanejoFabricaTipo` | portal (1ª loja) | — | `portal`, `id_devolucao_remanejo_fabrica_tipo` | 8134 | 8634 |

#### `LinxMicrovixConsignmentsService` (ERP, remessas, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxMovimentoRemessas` | loja | — | `cnpj_emp`, `id_remessas` | 8135 | 8635 |
| `LinxMovimentoRemessasItens` | loja | — | `cnpj_emp`, `id_remessas_itens` | 8136 | 8636 |
| `LinxMovimentoRemessasAcertos` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `id_remessas_acertos` | 8137 | 8637 |
| `LinxMovimentoRemessasAcertosItens` | loja | — | `cnpj_emp`, `transacao` | 8138 | 8638 |
| `LinxRemessasIdentificadores` | loja | `identificador_venda` de `LinxMovimentoRemessas` (só os novos) | `cnpj_emp`, `identificador_venda`, `identificador_remessa`, `transacao_acerto` | 8139 | 8639 |
| `LinxRemessasOperacoes` | portal (1ª loja) | — | `portal`, `id_remessa_operacoes` | 8140 | 8640 |
| `LinxRemessasRetornoTipos` | portal (1ª loja) | — | `portal`, `id_remessa_retorno_tipos` | 8141 | 8641 |

#### `LinxMicrovixOrdersService` (ERP, pedidos de venda, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxPedidosVenda` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje; `data_alt_inicial` = 2000-01-01; `data_alt_fim` = hoje; `filtra_somente_timestamp` = 1 | `cnpj_emp`, `transacao` | 8142 | 8642 |
| `LinxPlanosPedidoVenda` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje; `dt_update_inicial` = 2000-01-01; `dt_update_fim` = hoje; `cod_pedido` de `LinxPedidosVenda` (só os novos) | `cnpj_emp`, `cod_pedido`, `plano` | 8143 | 8643 |
| `LinxPedidosVendaChecklistEntregaLocal` | loja | — | `cnpj_emp`, `id_checklist_entrega_local` | 8144 | 8644 |
| `LinxPedidosVendaChecklistEntregaArmazenamento` | loja | — | `cnpj_emp`, `id_checklist_entrega_armazenamento` | 8145 | 8645 |
| `LinxPedidosVendaChecklistEntregaDificuldade` | loja | — | `cnpj_emp`, `id_checklist_entrega_dificuldade` | 8146 | 8646 |
| `LinxOrcamentoPosicao` | portal (1ª loja) | — | `portal`, `id_posicao` | 8147 | 8647 |
| `LinxB2CPedidos` | loja | — | `cnpj_emp`, `id_pedido` | 8148 | 8648 |
| `LinxB2CPedidosItens` | portal (1ª loja) | — | `portal`, `id_pedido_item` | 8149 | 8649 |
| `LinxB2CPedidosStatus` | portal (1ª loja) | — | `portal`, `id` | 8150 | 8650 |
| `LinxB2CStatus` | portal (1ª loja) | — | `portal`, `id_status` | 8151 | 8651 |

#### `LinxMicrovixPurchaseOrdersService` (ERP, pedidos de compra, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxPedidosCompra` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje; `filtra_somente_timestamp` = 1 | `cnpj_emp`, `transacao` | 8152 | 8652 |
| `LinxPedidosCompraStatus` | portal (1ª loja) | — | `portal`, `id_compras_pedido_status` | 8153 | 8653 |
| `LinxPedidosCompraNumeroFornecedor` | portal (1ª loja) | — | `portal`, `id_pedido_fornecedor` | 8154 | 8654 |

#### `LinxMicrovixFiscalDocumentsService` (ERP, documentos fiscais, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxXMLDocumentos` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `identificador_microvix` | 8083 | 8583 |
| `LinxNFeEvento` | loja | — | `cnpj_emp`, `id_nfe_evento` | 8084 | 8584 |
| `LinxNfse` | loja | — | `cnpj_emp`, `id_nfse` | 8085 | 8585 |
| `LinxEntradaNFe` | loja | — | `cnpj_emp`, `id_entrada_nfe` | 8086 | 8586 |
| `LinxEntradaNFeProdutos` | portal (1ª loja) | — | `portal`, `id_entrada_nfe_produtos` | 8087 | 8587 |
| `LinxConhecimentoFretes` | loja | — | `cnpj_emp`, `id_conhecimento` | 8088 | 8588 |
| `LinxFreteTipos` | portal (1ª loja) | — | `portal`, `id_frete_tipos` | 8089 | 8589 |
| `LinxReducoesZ` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje; `cnpjPortal` = N | `cnpj_emp`, `numeroserie`, `numero_reducoes`, `aliquota` | 8090 | 8590 |
| `LinxECF` | loja | — | `cnpj_emp`, `id_ecf` | 8091 | 8591 |
| `LinxECFFormasPagamento` | loja | — | `cnpj_emp`, `id_empresa_ecf_formas_pgto` | 8092 | 8592 |
| `LinxNfceEstacao` | loja | — | `cnpj_emp`, `id_nfce_estacao` | 8093 | 8593 |
| `LinxSeries` | loja | — | `cnpj_emp`, `serie`, `id_modelo_nf` | 8094 | 8594 |

#### `LinxMicrovixFinancialService` (ERP, financeiro, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxFaturas` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje; `filtra_somente_timestamp` = 1 | `cnpj_emp`, `codigo_fatura`, `ordem_parcela` | 8176 | 8676 |
| `LinxFaturasOrigem` | loja | — | `cnpj_emp`, `codigo_fatura`, `codigo_fatura_origem` | 8177 | 8677 |
| `LinxPortadores` | portal (1ª loja) | — | `portal`, `cod_portador` | 8178 | 8678 |
| `LinxAntecipacoesFinanceiras` | loja | `data_lanc_ini` = 2000-01-01; `data_lanc_fim` = hoje | `cnpj_emp`, `numero_antecipacao`, `codigoproduto` | 8179 | 8679 |
| `LinxAntecipacoesFinanceirasPlanos` | loja | — | `cnpj_emp`, `id_antecipacoes_financeiras`, `plano`, `forma_pgto` | 8180 | 8680 |
| `LinxDesfazimentosAntecipacoes` | loja | — | `cnpj_emp`, `id_desfazimentos_antecipacoes` | 8181 | 8681 |
| `LinxDesfazimentosAntecipacoesDocumento` | loja | — | `cnpj_emp`, `id_documento_desfazimento_optico` | 8182 | 8682 |
| `LinxDesfazimentosAntecipacoesItens` | loja | — | `cnpj_emp`, `id_desfazimentos_antecipacoes_itens` | 8183 | 8683 |
| `LinxCategoriasFinanceiras` | portal (1ª loja) | — | `portal`, `id_categorias_financeiras` | 8184 | 8684 |
| `LinxCentroCusto` | loja | — | `cnpj_emp`, `empresa`, `id_centrocusto` | 8185 | 8685 |
| `LinxPlanoContas` | loja | — | `cnpj_emp`, `empresa`, `id_conta` | 8186 | 8686 |
| `LinxLancContabil` | loja | `data_lanc_ini` = 2000-01-01; `data_lanc_fim` = hoje; `filtra_somente_timestamp` = 1; `cod_cc` = 0; `cod_conta` = 0 | `cnpj_emp`, `id_lanc` | 8187 | 8687 |
| `LinxHistoricosContabeis` | portal (1ª loja) | — | `portal`, `id_historico` | 8188 | 8688 |
| `LinxExtratoBancarioLancamentos` | portal (1ª loja) | — | `portal`, `id_extrato_bancario_lancamentos` | 8189 | 8689 |
| `LinxExtratoBancarioConciliacoes` | portal (1ª loja) | — | `portal`, `id_extrato_bancario_conciliacoes` | 8190 | 8690 |

#### `LinxMicrovixServiceOrdersService` (ERP, ordens de serviço, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxOrdensServico` | loja | — | `cnpj_emp`, `numero_os` | 8155 | 8655 |
| `LinxOrdensServicoProdutos` | loja | — | `cnpj_emp`, `id_ordservprod` | 8156 | 8656 |
| `LinxServicos` | loja | `dt_update_inicio` = 2000-01-01; `dt_update_fim` = hoje | `cnpj_emp`, `cod_servico` | 8157 | 8657 |
| `LinxServicosDetalhes` | loja | — | `cnpj_emp`, `cod_servico` | 8158 | 8658 |
| `LinxServicoClientes` | loja | — | `cnpj_emp`, `id_cliente`, `id_servico`, `id_dav_item` | 8159 | 8659 |
| `LinxServicoAgendaComandas` | loja | — | `cnpj_emp`, `id_comanda` | 8160 | 8660 |
| `LinxServicoAgendaEventos` | loja | — | `cnpj_emp`, `id_evento` | 8161 | 8661 |
| `LinxOrdemServicoExternaDav` | loja | — | `cnpj_emp`, `id_vendas_pos_tmp_ordem_servico_externa` | 8162 | 8662 |
| `LinxMovimentoOrdemServicoExterna` | loja | — | `cnpj_emp`, `id_movimento_ordem_servico_externa` | 8163 | 8663 |
| `LinxValeOrdemServicoExterna` | loja | — | `cnpj_emp`, `id_vale_ordem_servico_externa` | 8164 | 8664 |
| `LinxPagamentoParcialDav` | portal (1ª loja) | — | `portal`, `id_pagamento_parcial_tmp` | 8165 | 8665 |

#### `LinxMicrovixOpticsService` (ERP, ótica, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `LinxProdutosOpticosTipo` | portal (1ª loja) | — | `portal`, `id_produtos_opticos_tipo` | 8166 | 8666 |
| `LinxProdutosOpticosTipoAro` | portal (1ª loja) | — | `portal`, `id_tipo_aro` | 8167 | 8667 |
| `LinxProdutosOpticosFormatoAro` | portal (1ª loja) | — | `portal`, `id_formato_aro` | 8168 | 8668 |
| `LinxDadosOpticosDav` | loja | — | `cnpj_emp`, `id_vendas_pos_produtos` | 8169 | 8669 |
| `LinxOticoPrismaDescricao` | portal (1ª loja) | — | `portal`, `id_otico_prisma_descricao` | 8170 | 8670 |
| `LinxOticoReceitas` | loja | — | `cnpj_emp`, `id_otico_receitas` | 8171 | 8671 |
| `LinxPosicaoOsRamoOptico` | portal (1ª loja) | — | `portal`, `id_posicao_os_ramo_optico` | 8172 | 8672 |
| `LinxOrdensServicoPosicaoOsRamoOpticoHistorico` | loja | — | `cnpj_emp`, `id_ordens_servico_posicao_os_ramo_optico_historico` | 8173 | 8673 |
| `LinxOrcamentoComponenteFormula` | loja | — | `cnpj_emp`, `id_orcamento_componente_formula` | 8174 | 8674 |
| `LinxComponenteFormulaDav` | loja | — | `cnpj_emp`, `id_componente_formula_dav` | 8175 | 8675 |

#### `LinxMicrovixB2CCustomersService` (B2C, clientes do B2C, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `B2CConsultaClientes` | portal (1ª loja) | — | `portal`, `cod_cliente_erp` | 8259 | 8700 |
| `B2CConsultaClientesContatos` | loja | — | `cnpj_emp`, `id_clientes_contatos` | 8260 | 8701 |
| `B2CConsultaClientesContatosParentesco` | portal (1ª loja) | — | `portal`, `id_parentesco` | 8261 | 8702 |
| `B2CConsultaClientesEnderecosEntrega` | portal (1ª loja) | — | `portal`, `id_endereco_entrega` | 8262 | 8703 |
| `B2CConsultaClientesEstadoCivil` | portal (1ª loja) | — | `portal`, `id_estado_civil` | 8263 | 8704 |
| `B2CConsultaClientesSaldo` | loja | — | `cnpj_emp`, `cod_cliente_erp`, `empresa` | 8264 | 8705 |
| `B2CConsultaClientesSaldoLinx` | loja | — | `cnpj_emp`, `cod_cliente_erp`, `empresa` | 8265 | 8706 |

#### `LinxMicrovixB2CProductAttributesService` (B2C, atributos de produto do B2C, a cada 1 dia)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `B2CConsultaClassificacao` | portal (1ª loja) | — | `portal`, `codigo_classificacao` | 8288 | 8729 |
| `B2CConsultaColecoes` | portal (1ª loja) | — | `portal`, `codigo_colecao` | 8289 | 8730 |
| `B2CConsultaEspessuras` | portal (1ª loja) | — | `portal`, `codigo_espessura` | 8290 | 8731 |
| `B2CConsultaGrade1` | portal (1ª loja) | — | `portal`, `codigo_grade1` | 8291 | 8732 |
| `B2CConsultaGrade2` | portal (1ª loja) | — | `portal`, `codigo_grade2` | 8292 | 8733 |
| `B2CConsultaLinhas` | portal (1ª loja) | — | `portal`, `codigo_linha` | 8293 | 8734 |
| `B2CConsultaMarcas` | portal (1ª loja) | — | `portal`, `codigo_marca` | 8294 | 8735 |
| `B2CConsultaSetores` | portal (1ª loja) | — | `portal`, `codigo_setor` | 8295 | 8736 |
| `B2CConsultaUnidade` | portal (1ª loja) | — | `portal`, `unidade` | 8296 | 8737 |
| `B2CConsultaFornecedores` | portal (1ª loja) | — | `portal`, `cod_fornecedor` | 8297 | 8738 |
| `B2CConsultaFlags` | loja | — | `cnpj_emp`, `id_b2c_flags` | 8298 | 8739 |
| `B2CConsultaTags` | loja | — | `cnpj_emp`, `id_b2c_tags` | 8299 | 8740 |
| `B2CConsultaPalavrasChavePesquisa` | loja | — | `cnpj_emp`, `id_b2c_palavras_chave_pesquisa` | 8300 | 8741 |
| `B2CConsultaLegendasCadastrosAuxiliares` | loja | — | `cnpj_emp`, `empresa` | 8301 | 8742 |

#### `LinxMicrovixB2CProductsService` (B2C, produtos do B2C, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `B2CConsultaProdutos` | portal (1ª loja) | — | `portal`, `codigoproduto` | 8266 | 8707 |
| `B2CConsultaProdutosAssociados` | loja | — | `cnpj_emp`, `id` | 8267 | 8708 |
| `B2CConsultaProdutosCampanhas` | loja | — | `cnpj_emp`, `codigo_campanha` | 8268 | 8709 |
| `B2CConsultaProdutosCodebar` | portal (1ª loja) | — | `portal`, `id_produtos_codebar` | 8269 | 8710 |
| `B2CConsultaProdutosDimensoes` | portal (1ª loja) | — | `portal`, `codigoproduto` | 8270 | 8711 |
| `B2CConsultaProdutosInformacoes` | portal (1ª loja) | — | `portal`, `id_produtos_informacoes` | 8271 | 8712 |
| `B2CConsultaProdutosStatus` | portal (1ª loja) | — | `portal`, `codigoproduto` | 8272 | 8713 |
| `B2CConsultaProdutosFlags` | loja | — | `cnpj_emp`, `id_b2c_flags_produtos` | 8273 | 8714 |
| `B2CConsultaProdutosTags` | loja | — | `cnpj_emp`, `id_b2c_tags_produtos` | 8274 | 8715 |
| `B2CConsultaProdutosPalavrasChavePesquisa` | loja | — | `cnpj_emp`, `id_b2c_palavras_chave_pesquisa_produtos` | 8275 | 8716 |

#### `LinxMicrovixB2CProductImagesService` (B2C, imagens de produto do B2C, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `B2CConsultaProdutosImagens` | portal (1ª loja) | — | `portal`, `id_imagem_produto` | 8276 | 8717 |
| `B2CConsultaProdutosImagensURL` | loja | — | `cnpj_emp`, `codigoproduto`, `url_imagem_blob` | 8277 | 8718 |
| `B2CConsultaImagens` | loja | — | `cnpj_emp`, `id_imagem` | 8278 | 8719 |
| `B2CConsultaImagensHD` | loja | `codigoproduto` de `B2CConsultaProdutos` (só os novos) | `cnpj_emp`, `codigoproduto`, `identificador_imagem` | 8279 | 8720 |

#### `LinxMicrovixB2CProductFieldsService` (B2C, campos adicionais de produto do B2C, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `B2CConsultaProdutosCamposAdicionaisDetalhes` | portal (1ª loja) | — | `portal`, `id_campo_detalhe` | 8280 | 8721 |
| `B2CConsultaProdutosCamposAdicionaisNomes` | portal (1ª loja) | — | `portal`, `id_campo` | 8281 | 8722 |
| `B2CConsultaProdutosCamposAdicionaisValores` | portal (1ª loja) | — | `portal`, `id_campo_detalhe`, `codigoproduto` | 8282 | 8723 |
| `B2CConsultaB2CProdutosCamposAdicionais` | loja | — | `cnpj_emp`, `id_b2c_produtos_campos_adicionais` | 8283 | 8724 |
| `B2CConsultaB2CProdutosCamposAdicionaisDados` | loja | — | `cnpj_emp`, `id_b2c_produtos_campos_adicionais_dados` | 8284 | 8725 |
| `B2CConsultaB2CProdutosCamposAdicionaisMedia` | loja | — | `cnpj_emp`, `id_b2c_produtos_campos_adicionais_media` | 8285 | 8726 |
| `B2CConsultaB2CProdutosCamposAdicionaisMediaDados` | loja | — | `cnpj_emp`, `id_b2c_produtos_campos_adicionais_media_dados` | 8286 | 8727 |
| `B2CConsultaB2CProdutosCamposAdicionaisOpcoes` | loja | — | `cnpj_emp`, `id_b2c_produtos_campos_adicionais_opcoes` | 8287 | 8728 |

#### `LinxMicrovixB2CStocksAndPricesService` (B2C, estoque, custos e preços do B2C, a cada 120 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `B2CConsultaProdutosCustos` | portal (1ª loja) | — | `portal`, `codigoproduto` | 8302 | 8743 |
| `B2CConsultaProdutosDepositos` | portal (1ª loja) | — | `portal`, `id_deposito` | 8303 | 8744 |
| `B2CConsultaProdutosDetalhes` | loja | — | `cnpj_emp`, `id_prod_det` | 8304 | 8745 |
| `B2CConsultaProdutosDetalhesDepositos` | loja | — | `cnpj_emp`, `codigoproduto` | 8305 | 8746 |
| `B2CConsultaProdutosPromocao` | loja | — | `cnpj_emp`, `codigo_promocao`, `codigoproduto` | 8306 | 8747 |
| `B2CConsultaProdutosTabelas` | portal (1ª loja) | — | `portal`, `id_tabela` | 8307 | 8748 |
| `B2CConsultaProdutosTabelasPrecos` | portal (1ª loja) | — | `portal`, `id_tabela`, `codigoproduto` | 8308 | 8749 |

#### `LinxMicrovixB2COrdersService` (B2C, pedidos e notas do B2C, a cada 3 min)

| Método | Escopo | Parâmetros | Chave | Job | Procedure |
|---|---|---|---|---|---|
| `B2CConsultaPedidos` | portal (1ª loja) | `data_origem_inicial` = 2000-01-01; `data_origem_fim` = hoje | `portal`, `id_pedido` | 8309 | 8750 |
| `B2CConsultaPedidosItens` | portal (1ª loja) | — | `portal`, `id_pedido_item` | 8310 | 8751 |
| `B2CConsultaPedidosPlanos` | portal (1ª loja) | — | `portal`, `id_pedido_planos` | 8311 | 8752 |
| `B2CConsultaPedidosStatus` | portal (1ª loja) | `data_inicial` = 2000-01-01; `data_fim` = hoje | `portal`, `id` | 8312 | 8753 |
| `B2CConsultaPedidosIdentificador` | portal (1ª loja) | `data_origem_inicial` = 2000-01-01; `data_origem_fim` = hoje | `portal`, `identificador` | 8313 | 8754 |
| `B2CConsultaPedidosTipos` | portal (1ª loja) | — | `portal`, `id_tipo_b2c` | 8314 | 8755 |
| `B2CConsultaStatus` | portal (1ª loja) | — | `portal`, `id_status` | 8315 | 8756 |
| `B2CConsultaCodigoRastreio` | portal (1ª loja) | — | `portal`, `id_pedido`, `codigo_rastreio` | 8316 | 8757 |
| `B2CConsultaNFe` | loja | `data_inicial` = 2000-01-01; `data_fim` = hoje | `cnpj_emp`, `id_nfe` | 8317 | 8758 |
| `B2CConsultaNFeSituacao` | portal (1ª loja) | — | `portal`, `id_nfe_situacao` | 8318 | 8759 |

### Ordem de dependência

* **`LinxGrupoLojas` roda antes de todo método de loja do ERP**, e **`B2CConsultaEmpresas` antes de todo método de
  loja do B2C**: as lojas vêm das tabelas deles. Sem lojas gravadas, o método registra a mensagem e passa para o
  próximo cliente.
* Métodos que exigem um valor de outro método por chamada (leem os valores já gravados com `GetSourceValues`):

  * `LinxConfiguracoesTributariasDetalhes` ← `id_config_tributaria` de `LinxConfiguracoesTributarias`
  * `LinxProdutosDetalhesDepositos` ← `cod_deposito` de `LinxProdutosDepositos`
  * `LinxProdutosTabelasPrecos` ← `id_tabela` de `LinxProdutosTabelas`
  * `LinxRemessasIdentificadores` ← `identificador_remessa` de `LinxMovimentoRemessas` (incremental: só os valores gravados depois da última leitura; na primeira execução, uma chamada por valor)
  * `LinxPlanosPedidoVenda` ← `cod_pedido` de `LinxPedidosVenda` (incremental: só os valores gravados depois da última leitura; na primeira execução, uma chamada por valor)
  * `B2CConsultaImagensHD` ← `codigoproduto` de `B2CConsultaProdutos` (incremental: só os valores gravados depois da última leitura; na primeira execução, uma chamada por valor)

### Fluxo das leituras recorrentes

1. abre um log em `auditing.Logs` com o job do método;
2. para cada cliente de `LinxMicrovixParameters` com a chave daquela API, e para cada loja (métodos de loja), lê o
   maior timestamp gravado;
3. monta o envelope XML (`Authentication`, `ResponseFormat`, `Command` com os parâmetros) e chama a API. Um
   502/503/504 espera 10 s e tenta de novo uma vez;
4. trata as respostas de erro (`ResponseSuccess = False`, `<C />` vazio com mensagem) como falha da leitura daquela
   loja: **leitura incompleta não é gravada**, porque a próxima execução sairia do `MAX(timestamp)` e pularia o que
   faltou;
5. lê `<C>` (colunas) e `<R>` (linhas), preenche o DTO e repete com o maior timestamp da página até a resposta vir
   vazia;
6. valida os DTOs (FluentValidation) e registra as inconsistências em `auditing.Messages`, sem barrar a gravação;
7. fica a linha de maior timestamp de cada chave, converte para entidades, faz o bulk insert em `untreated` e chama a
   procedure `linxmicrovix.sp_<Método>_Sync`;
8. grava cada registro integrado (com o XML da linha) em `auditing.Records` e fecha o log num `finally`.

### Leituras sob demanda (um registro por chave)

Ficam no service do método recorrente, com o nome `<Método>By<Parâmetro>`.

| Método | Rota (`GET LinxMicrovixJobs/LinxMicrovix/...`) | Parâmetro enviado à API | Job |
|---|---|---|---|
| `LinxClientesFornecByDocCliente` | `LinxClientesFornec/{docCompany}/{docCliente}` | `doc_cliente` | 8191 |
| `LinxMovimentoByDocumento` | `LinxMovimento/{docCompany}/{cnpjEmp}/{documento}` | `documento` | 8192 |
| `LinxMovimentoCartoesByIdentificador` | `LinxMovimentoCartoes/{docCompany}/{cnpjEmp}/{identificador}` | `identificador` | 8193 |
| `LinxMovimentoPlanosByIdentificador` | `LinxMovimentoPlanos/{docCompany}/{cnpjEmp}/{identificador}` | `identificador` | 8194 |
| `LinxMovimentoTrocasByDocOrigem` | `LinxMovimentoTrocas/{docCompany}/{cnpjEmp}/{docOrigem}` | `doc_origem` | 8195 |
| `LinxPlanosByPlano` | `LinxPlanos/{docCompany}/{plano}` | `plano` | 8196 |
| `LinxNaturezaOperacaoByCodNaturezaOperacao` | `LinxNaturezaOperacao/{docCompany}/{codNaturezaOperacao}` | `cod_natureza_operacao` | 8197 |
| `LinxProdutosCodBarByCodProduto` | `LinxProdutosCodBar/{docCompany}/{codProduto}` | `cod_produto` | 8198 |
| `LinxProdutosTabelasByIdTabela` | `LinxProdutosTabelas/{docCompany}/{cnpjEmp}/{idTabela}` | `id_tabela` | 8199 |
| `LinxProdutosTabelasPrecosByIdTabelaAndCodProduto` | `LinxProdutosTabelasPrecos/{docCompany}/{cnpjEmp}/{idTabela}/{codProduto}` | `id_tabela` e `cod_produto` | 8200 |
| `LinxProdutosDepositosByCodDeposito` | `LinxProdutosDepositos/{docCompany}/{codDeposito}` | `cod_deposito` | 8201 |
| `LinxProdutosInventarioByCodProduto` | `LinxProdutosInventario/{docCompany}/{cnpjEmp}/{codProduto}` | `cod_produto` | 8202 |
| `LinxProdutosDetalhesByCodProduto` | `LinxProdutosDetalhes/{docCompany}/{cnpjEmp}/{codProduto}` | `cod_produto` | 8203 |
| `LinxProdutosCamposAdicionaisByCodProduto` | `LinxProdutosCamposAdicionais/{docCompany}/{cnpjEmp}/{codProduto}` | `cod_produto` | 8204 |
| `LinxProdutosByCodProduto` | `LinxProdutos/{docCompany}/{codProduto}` | `cod_produto` | 8205 |
| `LinxPedidosVendaByCodPedido` | `LinxPedidosVenda/{docCompany}/{cnpjEmp}/{codPedido}` | `doc_origem` | 8206 |
| `LinxSetoresByIdSetor` | `LinxSetores/{docCompany}/{idSetor}` | `id_setor` | 8207 |
| `LinxXMLDocumentosByDocumento` | `LinxXMLDocumentos/{docCompany}/{cnpjEmp}/{documento}` | `documento` | 8208 |
| `LinxVendedoresByCodVendedor` | `LinxVendedores/{docCompany}/{cnpjEmp}/{codVendedor}` | `cod_vendedor` | 8209 |
| `B2CConsultaClientesByDocCliente` | `B2CConsultaClientes/{docCompany}/{docCliente}` | `doc_cliente` | 8319 |
| `B2CConsultaNFeByIdPedido` | `B2CConsultaNFe/{docCompany}/{cnpjEmp}/{idPedido}` | `id_pedido` | 8320 |
| `B2CConsultaPedidosByIdPedido` | `B2CConsultaPedidos/{docCompany}/{idPedido}` | `id_pedido` | 8321 |
| `B2CConsultaPedidosItensByIdPedido` | `B2CConsultaPedidosItens/{docCompany}/{idPedido}` | `id_pedido` | 8322 |
| `B2CConsultaPedidosStatusByIdPedido` | `B2CConsultaPedidosStatus/{docCompany}/{idPedido}` | `id_pedido` | 8323 |

Fluxo:

1. abre um log em `auditing.Logs` com o job do método;
2. busca o cliente pelo CNPJ recebido. Se ele não estiver cadastrado ou não tiver a chave daquela API, devolve `null`
   sem chamar a API;
3. nos métodos de loja, confere se o CNPJ da loja é do cliente (`GetStores`); nos de portal, usa a primeira loja;
4. chama o comando com `timestamp = 0`, as datas do recorrente e o parâmetro da busca, lendo as páginas até a
   resposta vir vazia, e grava pelo **mesmo método de gravação do job** (mesmas tabelas, merge, validação e
   `auditing.Records`);
5. grava a chamada em `linxmicrovix.LinxMicrovixRequests`, também quando ela falha antes de ter resposta;
6. devolve `true` (gravado), `false` (a API respondeu sem linhas) ou `null` (cliente não cadastrado, loja de outro
   cliente ou resposta de erro).

---

## Infrastructure.LinxMicrovix.Outbound

```
Infrastructure.LinxMicrovix.Outbound/
├── DependencyInjection.cs      (AddScopedLinxMicrovixServices)
├── Repositorys/                (LinxMicrovixRepository e um por service)
└── Scripts/
    ├── SQLServer/              (migrator.csx, Parameters/ com os scripts gerais e uma pasta por service em Erp/ e B2C/)
    ├── MySQL/                  (migrator.csx)
    └── PostgreSQL/             (migrator.csx)
```

### Injeção de dependência

`AddScopedLinxMicrovixServices(IServiceCollection, IConfiguration)` registra:

* `IApiClient` com a chave `"LinxMicrovixAPI"` e o `HttpClient` nomeado `"LinxMicrovixAPI"` (base `LinxMicrovix:BaseUrl`, timeout de 20 minutos);
* os 29 services, os repositórios e o `ILinxMicrovixCommandHandler` (`Scoped`);
* todos os validadores do assembly numa chamada só.

O host chama o método em `Hosts/Hangfire/Extensions/ServicesExtensions.cs`.

### Scripts de banco (SQL Server)

O DbUp executa cada script uma única vez. Alterar um script que já rodou não muda um banco existente: para isso é
preciso um script novo. Os scripts gerais do módulo ficam em `Parameters/` e cada service tem a sua pasta (`Erp/<Service>` ou `B2C/<Service>`) com um trio de scripts:
tabelas, procedures e jobs (os recorrentes e os sob demanda do service). O `migrator.csx` lê as subpastas e executa
na ordem do número do arquivo, não na da pasta.

| Pasta | Script | Conteúdo |
|---|---|---|
| `Parameters/` | `Script001` | Schema `linxmicrovix` e `linxmicrovix.LinxMicrovixParameters` |
| `Parameters/` | `Script002` | App `80 - LinxMicrovix` em `auditing.Apps` (um app para o ERP e o B2C) |
| `Parameters/` | `Script003` | Os clientes em `LinxMicrovixParameters`, com as chaves. **Fica fora do git** (`.gitignore`); o modelo é o `Script003_...sql.example` |
| `Parameters/` | `Script004` | `linxmicrovix.LinxMicrovixRequests` (log das chamadas sob demanda) |
| `Erp/Stores/` | `Script005`–`007` | Stores |
| `Erp/ProductAttributes/` | `Script008`–`010` | Product Attributes |
| `Erp/Taxes/` | `Script011`–`013` | Taxes |
| `Erp/TaxSettings/` | `Script014`–`016` | Tax Settings |
| `Erp/Products/` | `Script017`–`019` | Products |
| `Erp/Stocks/` | `Script020`–`022` | Stocks |
| `Erp/Prices/` | `Script023`–`025` | Prices |
| `Erp/Customers/` | `Script026`–`028` | Customers |
| `Erp/Sellers/` | `Script029`–`031` | Sellers |
| `Erp/Payments/` | `Script032`–`034` | Payments |
| `Erp/Loyalty/` | `Script035`–`037` | Loyalty |
| `Erp/Movements/` | `Script038`–`040` | Movements |
| `Erp/PosSales/` | `Script041`–`043` | Pos Sales |
| `Erp/Returns/` | `Script044`–`046` | Returns |
| `Erp/Consignments/` | `Script047`–`049` | Consignments |
| `Erp/Orders/` | `Script050`–`052` | Orders |
| `Erp/PurchaseOrders/` | `Script053`–`055` | Purchase Orders |
| `Erp/FiscalDocuments/` | `Script056`–`058` | Fiscal Documents |
| `Erp/Financial/` | `Script059`–`061` | Financial |
| `Erp/ServiceOrders/` | `Script062`–`064` | Service Orders |
| `Erp/Optics/` | `Script065`–`067` | Optics |
| `B2C/Stores/` | `Script068`–`070` | B2C Stores |
| `B2C/Customers/` | `Script071`–`073` | B2C Customers |
| `B2C/ProductAttributes/` | `Script074`–`076` | B2C Product Attributes |
| `B2C/Products/` | `Script077`–`079` | B2C Products |
| `B2C/ProductImages/` | `Script080`–`082` | B2C Product Images |
| `B2C/ProductFields/` | `Script083`–`085` | B2C Product Fields |
| `B2C/StocksAndPrices/` | `Script086`–`088` | B2C Stocks And Prices |
| `B2C/Orders/` | `Script089`–`091` | B2C Orders |

**Regras gerais das tabelas e dos merges:**
* Toda tabela de dados existe em dois schemas: `untreated` (recebe o bulk insert, sem chave) e `linxmicrovix` (tabela
  final, com PK). A exceção é `linxmicrovix.LinxMicrovixRequests`, que só recebe inserts.
* As procedures `linxmicrovix.sp_<Método>_Sync` são geradas por `dbo.sp_Create_Merge_Procedure`, do projeto Shared.
  Com a coluna `timestamp`, o merge só atualiza quando o timestamp recebido é maior ou igual ao gravado.
* As respostas são planas, sem filhas: os merges **não têm condição de exclusão** e nada é apagado do banco. Nos
  métodos que trazem uma coluna de exclusão (como `excluido`), ela é gravada como os outros campos.

---

## Endpoints expostos (`LinxMicrovixController`)

Rota base: `LinxMicrovixJobs/LinxMicrovix`. A classe é `partial`: um arquivo por service
(`LinxMicrovixController.<Service>.cs`), cada um injetando só o seu service na ação (`[FromServices]`).

### Leituras recorrentes (disparam a integração de todos os clientes)

`POST <Método>` para cada um dos 260 métodos (ex.: `POST LinxMicrovixJobs/LinxMicrovix/LinxProdutos`), agrupados no
Swagger pela tag do service.

### Leituras sob demanda (chamadas por sistema externo)

Todas exigem:
* o cabeçalho **`X-User-Name`** (até 100 caracteres);
* o CNPJ do cliente na rota (`{docCompany}`, 14 dígitos) e, nos métodos de loja, o da loja (`{cnpjEmp}`);
* o valor da busca (até 100 caracteres). As rotas estão na tabela das leituras sob demanda.

| Código | Quando |
|---|---|
| **200** | Registro gravado |
| **404** | A API respondeu sem linhas para a chave |
| **422** | Cliente não cadastrado ou sem a chave da API, loja de outro cliente ou resposta de erro da API (detalhes no log da execução) |

> O `[Authorize]` do controller está comentado, como no Movidesk, no AfterSale e no Totvs, porque o host ainda não tem
> um esquema de autenticação configurado.

---

## Auditoria (jobs)

Os jobs ficam em `auditing.Jobs`, com os mesmos ids em `EnumJob`. Os ids de cada método estão nas tabelas dos services.

| Faixa | Tipo (`IdType`) | Conteúdo |
|---|---|---|
| 8000–8249 | 2 (Integração) | Leituras do ERP, com `IdParent = 80` (app LinxMicrovix): recorrentes 8000–8190, sob demanda 8191–8209 |
| 8250–8499 | 2 (Integração) | Leituras do B2C: recorrentes 8250–8318, sob demanda 8319–8323 |
| 8500–8999 | 1 (Procedure) | Procedures `sp_<Método>_Sync`, com `IdParent` = job da leitura e o mesmo intervalo |

| Intervalo | Services |
|---|---|
| 3 min | `Payments`, `Loyalty`, `Movements`, `PosSales`, `Returns`, `Consignments`, `Orders`, `PurchaseOrders`, `FiscalDocuments`, `ServiceOrders`, `B2COrders` |
| 120 min | `Stores`, `B2CStores`, `Products`, `Stocks`, `Prices`, `Customers`, `Sellers`, `Financial`, `Optics`, `B2CCustomers`, `B2CProducts`, `B2CProductImages`, `B2CProductFields`, `B2CStocksAndPrices` |
| 1 dia | `ProductAttributes`, `Taxes`, `TaxSettings`, `B2CProductAttributes` |
| sob demanda (0) | as 24 leituras por chave |

Os intervalos são ajustáveis por um `update` em `auditing.Jobs`, sem mudar o código. A especificação avisa que o
consumo excessivo leva à desativação da chave; por isso o intervalo segue o quanto o dado muda.

Toda execução grava `auditing.Logs` e, conforme o caso:
* `auditing.Messages`, com mensagens e erros de validação;
* `auditing.Records`, com os registros integrados e o XML da linha recebida.

---

## Configuração

| Onde | Chave | Uso |
|---|---|---|
| `appsettings.json` do host | `LinxMicrovix:BaseUrl` | Endereço da API (`https://webapi.microvix.com.br/1.0/api/integracao/`) |
| `appsettings.json` do host | `ConnectionStrings` | Conexão com o SQL Server (dados e auditoria) |
| Banco | `linxmicrovix.LinxMicrovixParameters` | Por cliente: CNPJ (`doc_company`), `grupo` (opcional, filtro do `LinxGrupoLojas`) e, para cada API (`erp_*`, `b2c_*`), chave, usuário e senha. Um cliente só com uma das APIs deixa as colunas da outra nulas |

As chaves ficam só no banco (via `Script003`, fora do git) e nunca são gravadas em logs: o corpo das chamadas sob
demanda é registrado com a chave trocada por `(oculta)`.

---

## Pasta `docs/Responsed XMLs`

A primeira página real de cada método (`docs/Responsed XMLs/<Service>/<Método>/xml-returned-by-api-end-point.xml`), com
os dados trocados por **dados fictícios** para não expor informações no histórico do git:

* as linhas foram cortadas em 2; o envelope (`<C>`, `ResponseResult`) ficou como veio;
* textos viram `Exemplo <coluna>`; e-mails, `exemplo@exemplo.com.br`; URLs, `https://exemplo.com.br/<coluna>`; XMLs
  (notas), `Exemplo <coluna> (xml)`;
* CPF, CNPJ, CEP, telefones, códigos e valores viram dígitos fictícios do mesmo tamanho; GUIDs, GUIDs fictícios;
  datas de nascimento, `1990-01-01`;
* ficam como vieram só o `timestamp`, os booleanos, os números de um dígito (status e tipos), os textos de até 2
  caracteres (UF, `S`/`N`) e as datas de registro;
* o `LinxMetodos` ficou como veio: é o catálogo público de comandos e parâmetros da API.

Foram gravadas 260 respostas: 183 com linhas,
69 vazias (o cliente de teste não tem dados no método: o arquivo mostra só as colunas) e 8 de erro.

Respostas de erro (o arquivo é a mensagem da API):

* `B2CConsultaB2CProdutosCamposAdicionais`
* `B2CConsultaB2CProdutosCamposAdicionaisDados`
* `B2CConsultaB2CProdutosCamposAdicionaisMedia`
* `B2CConsultaB2CProdutosCamposAdicionaisMediaDados`
* `B2CConsultaB2CProdutosCamposAdicionaisOpcoes`
* `B2CConsultaClientesContatos`
* `B2CConsultaProdutosImagensURL`
* `LinxClientesRede`

As leituras sob demanda usam os mesmos comandos, com o parâmetro da busca; a resposta tem o mesmo formato.

---

## Pendências

* **Execução em banco:** os scripts, os merges e as rotas ainda não rodaram pelo host; a modelagem foi conferida com
  chamadas reais à API, sem banco.
* **8 métodos sem resposta real:** 7 do B2C que a chave de teste não tem liberados (`B2CConsultaClientesContatos`,
  `B2CConsultaProdutosImagensURL` e os 5 `B2CConsultaB2CProdutosCamposAdicionais*`) e o `LinxClientesRede`, que
  recusa os parâmetros da v274. As colunas vieram da documentação.
* **Memória na carga inicial** de `B2CConsultaNFe` e `B2CConsultaImagens` (respostas de centenas de MB, porque o B2C
  não tem limite de linhas): acompanhar a primeira execução pelo host.
* **13 métodos do `LinxMetodos` que a v274 não descreve** ficam fora até a Linx documentá-los.

---

## Documentação complementar

Na pasta `docs/` da raiz do repositório:

| Documento | Assunto |
|---|---|
| `PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md` | Proposta, decisões e plano em fases da padronização do módulo, com o que foi conferido na API |
| `PADRAO-CACHE-INTEGRACOES.md` | Padrão de cache das leituras |
| `ALTERACOES-MERGE-PROCEDURE-AUDITORIA.md` | Procedure de merge e auditoria |

# Proposta — padronização do módulo LinxMicrovix Outbound (padrão Movidesk / AfterSale)

Documento para revisão. **Nada foi alterado no código ainda.** As decisões tomadas em 26/10/2026 já estão aplicadas
no texto e resumidas na §10.1; não há decisões pendentes (§10.2). A implementação será feita **um service de cada
vez** (§9).

Objetivo: listar tudo o que precisa mudar em `src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service`
(e nos pontos do host que o referenciam) para que o módulo siga o mesmo padrão dos módulos **Movidesk** e
**AfterSale**, como foi feito no Totvs (`docs/PROPOSTA-PADRONIZACAO-TOTVS.md`), e para que ele passe a cobrir
**todos os métodos de leitura** dos dois documentos da Linx. O projeto **Inbound** fica fora do escopo, e a
integração do **Oto** não é tocada nesta refatoração.

Como o Totvs, o Outbound **lê** uma API (o Web Service de Saída Padrão do Linx Microvix e o WS B2C) e **grava** no
banco. A referência principal é a leitura recorrente do Movidesk (`GetTickets`, `GetPeople`, `GetServices`), o Totvs
já migrado (`TotvsService`, `TotvsPeopleService.GetBranchesList`) e o padrão de cache de
`docs/PADRAO-CACHE-INTEGRACOES.md`.

**Integração recorrente, para qualquer cliente (decidido).** O Linx foi descontinuado na Newbloomers: nenhuma loja
vende mais por Linx. Mesmo assim, as integrações são **recorrentes**, iguais às do Totvs e do Movidesk, para que o
módulo funcione sem mudança em outro cliente que use o Microvix. A primeira execução de cada método, com
`timestamp = 0`, já traz o histórico inteiro da loja; as seguintes leem só o que mudou (§4.2). Assim a carga do
histórico que o DW pede continua atendida (`docs/datawarehouse/01-visao-e-premissas.md` §1.2, item 7, e
`10-perguntas-em-aberto.md`, pergunta 13), e nada no módulo é específico da Newbloomers (§5.1).

Fontes conferidas em 26/10/2026:

* todo o código do módulo (572 arquivos, ~50 mil linhas), os controllers e as extensões de jobs do Hangfire, os hosts
  AzureWebJob e Quartz (sem nenhuma referência ao Linx) e o `appsettings.json`;
* *Especificação Web Service de Saída Padrão Linx Microvix* **v274**, de 01/09/2026: URLs, credenciais, regra de
  `timestamp`, limite de registros por retorno e parâmetros de cada método. Documenta **191 métodos**;
* *Documentação Integração WS B2C* **V64**, de 17/08/2026: URLs, credenciais e métodos do B2C. Documenta
  **69 métodos de consulta** e 11 de escrita;
* **chamadas reais às duas APIs**, com as chaves de produção da Newbloomers (ERP e B2C), em 26/10/2026: 13 chamadas
  pequenas, de leitura, a `LinxMetodos`, `LinxGrupoLojas`, `LinxLojas`, `LinxMovimento`, `LinxPedidosVenda`,
  `B2CConsultaEmpresas` e `B2CConsultaPedidos`. Os resultados estão na §4.2 e na §5.2. As chaves **não** estão neste
  documento: entram só no script de valores fora do git (§5.1).

---

## 1. Situação atual (inventário)

| Item | Hoje | Passa a ser |
|---|---|---|
| Projetos | `Domain.LinxMicrovix.Outbound`, `Application.LinxMicrovix.Outbound`, `Infrastructure.LinxMicrovix.Outbound` (no `.slnx`) | Os mesmos, nas mesmas pastas (§2) |
| Referências | `..\..\..\..\..\newbloomers-core-dlls\src\*.Core`, **que não existe no repositório**; `PackageReference` sem versão (`EPPlus`, `Caching.Memory`) e `VersionOverride` de Central Package Management sem `Directory.Packages.props` | `..\..\..\..\Shared\*.Shared` e versões explícitas (§2.1) |
| Host | `Hangfire.csproj` não referencia o módulo. `AddScopedLinxMicrovixServices`/`AddScopedB2CLinxMicrovixServices` comentados em `ServicesExtensions.cs`; os dois controllers (`ERP/LinxMicrovixController`, `B2C/B2CLinxMicrovixController`) estão comentados linha a linha, e os jobs recorrentes do Hangfire também | Ligado no host, um service por vez (§6.3, §9) |
| Métodos lidos | **36 de 260**: 27 do ERP e 9 do B2C, mais `LinxReshopSaldo`, que lê Excel do Azure Blob e não chama o Microvix | **Todos os 260** (§2.2). `LinxReshopSaldo` sai do Outbound (§10.1) |
| Services | Duas classes base **idênticas** (`LinxMicrovixBaseSyncService` e `B2CBaseSyncService`, 157 linhas cada). 24 dos 36 services sobrescrevem `GetRecords` com uma cópia do fluxo da base. Há um service por método (37 classes), mais 37 repositórios, 37 command handlers e 74 interfaces | 29 services por assunto com o prefixo do módulo, sem classe base, mais um `LinxMicrovixService` comum (§2.2, §4.1) |
| Credenciais | `LinxAPIParam` (chave, usuário, CNPJ principal) montado no controller a partir do `appsettings`, que só tem `"BaseUrl": ""`. A senha enviada é o **usuário** (`password="{UserAuthentication}"`) | `linxmicrovix.LinxMicrovixParameters`, uma linha por cliente, fora do git, com cache (§5.1) |
| Parâmetros da chamada | XML de parâmetros lido do banco (`[LINX_MICROVIX].[<ParametersTableName>]`, colunas `parameters_timestamp`/`parameters_dateinterval`/`parameters_individual`) com marcadores `[0]`, `[data_inicial]`... trocados por `Replace`. Nenhum script cria essa tabela | Parâmetros montados no código de cada método, conforme a especificação (§4.2) |
| Empresas | Três consultas diferentes: `B2CCONSULTAEMPRESAS`, `LINXLOJAS`, `LINXGRUPOLOJAS`, usadas sem critério (métodos ERP iteram empresas do B2C); um filtro fixo por nome de loja da Newbloomers (`"MISHA"`/`"OPEN"`) | Lojas de `linxmicrovix.LinxGrupoLojas` (ERP) e `B2CConsultaEmpresas` (B2C), gravadas pelo primeiro método de cada API (§4.2) |
| Janela de leitura | Cada método de um jeito: `timestamp = 0` (carga total a cada execução), `MIN(timestamp)` dos últimos 7 dias, datas de hoje−2/−3/−7 dias/−6 meses, ou o **texto** da query no lugar do número | `MAX(timestamp)` do método por loja, gravado no banco, como manda a especificação, com releitura enquanto vierem 5000 registros (§4.2) |
| Banco | Nenhum script de tabela ou procedure. Bulk em `untreated.<Método>` (sobrecarga `[Obsolete]`), merges em `linx_microvix_erp`/`linx_microvix_commerce` (`CallDbProcMerge(jobParameter.schema, ...)`) e consultas em `[linx_microvix]`, `[LINX_MICROVIX]`, `[linx_commerce]` | Schema `linxmicrovix` com scripts por service (§5) |
| Scripts | `Script001` (app 80) e `Script002` (jobs, **todo comentado**). O app 82 (B2C) não é inserido em lugar nenhum | Numeração refeita, tudo no app 80 (§5.4, §7) |
| Jobs | `EnumJob` 8000–8030 e 8250–8259, **deslocados em 1** em relação ao `Script002` a partir de `LinxClientesEnderecosEntrega` (§7) | Um job recorrente por método, nas faixas que o próprio `Script002` reservava (§7) |
| Código morto | `CustomValidations/**` (213 validadores e 6 helpers) fora do build por `<Compile Remove>`; DTO e entidade de `B2CConsultaClientesEnderecosEntrega` sem service; `InsertRecord` sem uso em 6 repositórios; `IntegrityLock*` que não grava nada | Validadores reaproveitados como ponto de partida; o resto removido (§2.4) |
| Documentação | Sem `README.md` e sem pasta de respostas de referência | README e `Responsed XMLs` (§9) |

Conclusão: o módulo **não compila** nem restaura. O `dotnet build` de `Infrastructure.LinxMicrovix.Outbound` para no
restore (`NU1015: EPPlus, Microsoft.Extensions.Caching.Memory` sem versão). Corrigido isso, faltam os três projetos
`*.Core`. Trocando por `*.Shared`, faltam tipos que o Shared não tem: `IIntegrationsCoreRepository` (hoje
`ISharedRepository`), `ParameterBase`, `APIException`, `GeneralException` e o `EnumJob` de `Domain.Core.Enums`, que
todos os services importam no lugar do `EnumJob` do próprio módulo.

Diferente do Totvs, as respostas do Microvix são **planas**: uma linha `<R>` por registro, com as colunas descritas em
`<C>`, sem filhos aninhados. Por isso a migração é sobretudo de **convenções, banco e fluxo**, e cada método novo é
uma tabela. Os DTOs, as entidades, o `CreateDto` e as colunas dos `INSERT` dos 36 métodos de hoje estão alinhados
entre si, com as exceções da §8, mas **não com a API de hoje**: o `LinxMovimento` real devolve 116 colunas, e o DTO não
tem 6 delas; o `LinxGrupoLojas` devolve 10, e a entidade tem 8 (§3.2).

---

## 2. Projetos, referências e nomes

### 2.1 `.csproj`

| Projeto | Hoje | Padrão (Movidesk) |
|---|---|---|
| `Domain.LinxMicrovix.Outbound` | `newbloomers-core-dlls\...\Domain.Core`; `<Compile Remove="CustomValidations\**">` | `..\..\..\..\Shared\Domain.Shared\Domain.Shared.csproj`; sem `Compile Remove` (a pasta sai, §2.4) |
| `Application.LinxMicrovix.Outbound` | `Application.Core`; `EPPlus` e `Caching.Memory` sem versão; `Compile Remove` e `None Remove="...ctwrzoso.apw~"` (arquivo que nem existe) | `Application.Shared`; `Microsoft.Extensions.Caching.Memory` **10.0.8**; sem `EPPlus` (sai com o `LinxReshopSaldo`) |
| `Infrastructure.LinxMicrovix.Outbound` | `Infrastructure.Core`; `Azure.Storage.Blobs` com `VersionOverride` | `Infrastructure.Shared`; sem pacote (o Blob sai com o `LinxReshopSaldo`) |

O Outbound fica no mesmo nível dos módulos de `General` (`Modules/LinxMicrovix/<pasta do módulo>/<projeto>`), então
o caminho do Shared tem 4 níveis, como no Movidesk. As pastas com espaço no nome (`LinxMicrovix - Outbound Web
Service`) **ficam** (decidido).

### 2.2 Services por assunto, com o prefixo do módulo (decidido)

No Movidesk e no Totvs um service agrupa os métodos de um assunto (`MovideskTicketsService`, `TotvsPeopleService`).
Hoje há uma classe por método da API. Passa a ser um service por assunto, com o nome em inglês como nos outros
módulos e o assunto tirado das seções da especificação. Os 260 métodos de leitura dos dois documentos ficam em
**29 services**: 21 do ERP e 8 do B2C. Um script conferiu, contra a v274 e a V64, que cada método está em exatamente
um service.

**`LinxMicrovixService` (comum, novo):** horário do lote, clientes, lojas e último timestamp (§4.1). Não tem método
da API.

**ERP — Web Service de Saída Padrão v274 (191 métodos):**

| Service | Qtd. | Métodos da API |
|---|---|---|
| `LinxMicrovixStoresService` | 6 | `LinxGrupoLojas` (¹), `LinxLojas` (¹), `LinxLojasParametros`, `LinxUsuarios` (¹), `LinxMetodos`, `LinxEntrantes` |
| `LinxMicrovixCustomersService` | 10 | `LinxClientesFornec` (¹), `LinxClientesFornecCamposAdicionais`, `LinxClientesFornecClasses`, `LinxClientesFornecContatos`, `LinxClientesFornecContatosParentesco`, `LinxClientesFornecCreditoAvulso`, `LinxClientesFornecVendedores`, `LinxClientesEnderecosEntrega` (¹), `LinxClientesRede`, `LinxRamosAtividade` |
| `LinxMicrovixSellersService` | 8 | `LinxVendedores` (¹), `LinxVendedoresFerias`, `LinxMetasVendedores`, `LinxMetasVendedoresDia`, `LinxComissoesVendedores`, `LinxListaDaVez`, `LinxFuncionariosFuncoes`, `LinxRegistroProfissional` |
| `LinxMicrovixProductsService` | 10 | `LinxProdutos` (¹), `LinxProdutosAssociados`, `LinxProdutosCamposAdicionais` (¹), `LinxProdutosCodBar` (¹), `LinxProdutosFornec`, `LinxProdutosInformacoes`, `LinxProdutosImagensURL`, `LinxProdutosLotes`, `LinxLotesProdutos`, `LinxProdutosSerial` |
| `LinxMicrovixProductAttributesService` | 10 | `LinxSetores` (¹), `LinxLinhas`, `LinxMarcas`, `LinxColecoes`, `LinxTamanhos`, `LinxCores`, `LinxEspessuras`, `LinxUnidades`, `LinxClassificacoes`, `LinxSubClasses` |
| `LinxMicrovixStocksService` | 9 | `LinxProdutosDetalhes` (¹), `LinxProdutosDetalhesSimplificado`, `LinxProdutosDetalhesDepositos`, `LinxProdutosDepositos` (¹), `LinxProdutosInventario` (¹), `LinxListagemBalanco`, `LinxBalancos`, `LinxBalancosArquivos`, `LinxBalancosSecoes` |
| `LinxMicrovixPricesService` | 10 | `LinxProdutosTabelas` (¹), `LinxProdutosTabelasPrecos` (¹), `LinxProdutosPromocoes` (¹), `LinxAcoesPromocionais`, `LinxAcoesPromocionaisCombinacaoProdutosItens`, `LinxAcoesPromocionaisProdutosCortesia`, `LinxMovimentoAcoesPromocionais`, `LinxCupomDesconto`, `LinxCupomDescontoVendas`, `LinxMotivoDesconto` |
| `LinxMicrovixTaxesService` | 16 | `LinxNaturezaOperacao` (¹), `LinxCfopFiscal` (¹), `LinxCstIcmsFiscal`, `LinxCstIpiFiscal`, `LinxCstPisFiscal`, `LinxCstCofinsFiscal`, `LinxCsosnFiscal`, `LinxClasseFiscal`, `LinxCest`, `LinxNcm`, `LinxEnquadramentoIpi`, `LinxMotivosDesoneracaoIcms`, `LinxSpedTipoItem`, `LinxSpedTipoBaseCredito`, `LinxRotinaOrigem` (¹), `LinxNfeIbgeMunicipio` |
| `LinxMicrovixTaxSettingsService` | 4 | `LinxConfiguracoesTributarias`, `LinxConfiguracoesTributariasDetalhes`, `LinxConfiguracoesTributariasDetalhesSimplificado`, `LinxConfiguracoesTributariasEmpresas` |
| `LinxMicrovixFiscalDocumentsService` | 12 | `LinxXMLDocumentos` (¹), `LinxNFeEvento`, `LinxNfse`, `LinxEntradaNFe`, `LinxEntradaNFeProdutos`, `LinxConhecimentoFretes`, `LinxFreteTipos`, `LinxReducoesZ`, `LinxECF`, `LinxECFFormasPagamento`, `LinxNfceEstacao`, `LinxSeries` |
| `LinxMicrovixMovementsService` | 12 | `LinxMovimento` (¹), `LinxMovimentoPrincipal`, `LinxMovimentoExtensao`, `LinxMovimentoCamposAdicionais`, `LinxMovimentoObservacoes`, `LinxMovimentoObservacaoCF`, `LinxMovimentoIndicacoes`, `LinxMovimentoSerial`, `LinxSerialVenda`, `LinxMovimentoVendaConjugada`, `LinxPerguntaVenda`, `LinxRespostaVenda` |
| `LinxMicrovixPosSalesService` | 3 | `LinxVendasPosTmp`, `LinxVendasPosProdutosTmp`, `LinxVendasPosAgrupamentoTmp` |
| `LinxMicrovixPaymentsService` | 9 | `LinxMovimentoCartoes` (¹), `LinxMovimentoPlanos` (¹), `LinxPlanos` (¹), `LinxPlanosParcelas`, `LinxPlanosBandeiras`, `LinxMovimentoGiftCard`, `LinxValesComprasEnviadosAPI`, `LinxSangriaSuprimentos`, `LinxFechamentoCaixa` |
| `LinxMicrovixLoyaltyService` | 3 | `LinxFidelidade`, `LinxMovimentoReshop`, `LinxMovimentoReshopItens` |
| `LinxMicrovixReturnsService` | 13 | `LinxMovimentoTrocas` (¹), `LinxMovimentoTrocafone`, `LinxMovimentoDevolucoesItens`, `LinxMovimentoOrigemDevolucoes`, `LinxMotivoDevolucao`, `LinxTrocaUnificadaResumoVendas`, `LinxTrocaUnificadaResumoVendasItens`, `LinxTrocaUnificadaResumoBaixa`, `LinxTradeinParceiro`, `LinxDevolucaoRemanejoFabrica`, `LinxDevolucaoRemanejoFabricaItem`, `LinxDevolucaoRemanejoFabricaStatus`, `LinxDevolucaoRemanejoFabricaTipo` |
| `LinxMicrovixConsignmentsService` | 7 | `LinxMovimentoRemessas`, `LinxMovimentoRemessasItens`, `LinxMovimentoRemessasAcertos`, `LinxMovimentoRemessasAcertosItens`, `LinxRemessasIdentificadores`, `LinxRemessasOperacoes`, `LinxRemessasRetornoTipos` |
| `LinxMicrovixOrdersService` | 10 | `LinxPedidosVenda` (¹), `LinxPlanosPedidoVenda`, `LinxPedidosVendaChecklistEntregaLocal`, `LinxPedidosVendaChecklistEntregaArmazenamento`, `LinxPedidosVendaChecklistEntregaDificuldade`, `LinxOrcamentoPosicao`, `LinxB2CPedidos`, `LinxB2CPedidosItens`, `LinxB2CPedidosStatus`, `LinxB2CStatus` |
| `LinxMicrovixPurchaseOrdersService` | 3 | `LinxPedidosCompra` (¹), `LinxPedidosCompraStatus`, `LinxPedidosCompraNumeroFornecedor` |
| `LinxMicrovixServiceOrdersService` | 11 | `LinxOrdensServico`, `LinxOrdensServicoProdutos`, `LinxServicos`, `LinxServicosDetalhes`, `LinxServicoClientes`, `LinxServicoAgendaComandas`, `LinxServicoAgendaEventos`, `LinxOrdemServicoExternaDav`, `LinxMovimentoOrdemServicoExterna`, `LinxValeOrdemServicoExterna`, `LinxPagamentoParcialDav` |
| `LinxMicrovixOpticsService` | 10 | `LinxProdutosOpticosTipo`, `LinxProdutosOpticosTipoAro`, `LinxProdutosOpticosFormatoAro`, `LinxDadosOpticosDav`, `LinxOticoPrismaDescricao`, `LinxOticoReceitas`, `LinxPosicaoOsRamoOptico`, `LinxOrdensServicoPosicaoOsRamoOpticoHistorico`, `LinxOrcamentoComponenteFormula`, `LinxComponenteFormulaDav` |
| `LinxMicrovixFinancialService` | 15 | `LinxFaturas`, `LinxFaturasOrigem`, `LinxPortadores`, `LinxAntecipacoesFinanceiras`, `LinxAntecipacoesFinanceirasPlanos`, `LinxDesfazimentosAntecipacoes`, `LinxDesfazimentosAntecipacoesDocumento`, `LinxDesfazimentosAntecipacoesItens`, `LinxCategoriasFinanceiras`, `LinxCentroCusto`, `LinxPlanoContas`, `LinxLancContabil`, `LinxHistoricosContabeis`, `LinxExtratoBancarioLancamentos`, `LinxExtratoBancarioConciliacoes` |

**B2C — WS B2C V64 (69 métodos de consulta):**

| Service | Qtd. | Métodos da API |
|---|---|---|
| `LinxMicrovixB2CStoresService` | 9 | `B2CConsultaEmpresas` (¹), `B2CConsultaCNPJsChave`, `B2CConsultaVendedores`, `B2CConsultaTransportadores`, `B2CConsultaFormasPagamento`, `B2CConsultaPlanos`, `B2CConsultaPlanosParcelas`, `B2CConsultaTiposCobrancaFrete`, `B2CConsultaTipoEncomenda` |
| `LinxMicrovixB2CCustomersService` | 7 | `B2CConsultaClientes` (¹), `B2CConsultaClientesContatos`, `B2CConsultaClientesContatosParentesco`, `B2CConsultaClientesEnderecosEntrega`, `B2CConsultaClientesEstadoCivil`, `B2CConsultaClientesSaldo`, `B2CConsultaClientesSaldoLinx` |
| `LinxMicrovixB2CProductsService` | 10 | `B2CConsultaProdutos`, `B2CConsultaProdutosAssociados`, `B2CConsultaProdutosCampanhas`, `B2CConsultaProdutosCodebar`, `B2CConsultaProdutosDimensoes`, `B2CConsultaProdutosInformacoes`, `B2CConsultaProdutosStatus`, `B2CConsultaProdutosFlags`, `B2CConsultaProdutosTags`, `B2CConsultaProdutosPalavrasChavePesquisa` |
| `LinxMicrovixB2CProductImagesService` | 4 | `B2CConsultaProdutosImagens`, `B2CConsultaProdutosImagensURL`, `B2CConsultaImagens`, `B2CConsultaImagensHD` |
| `LinxMicrovixB2CProductFieldsService` | 8 | `B2CConsultaProdutosCamposAdicionaisDetalhes`, `B2CConsultaProdutosCamposAdicionaisNomes`, `B2CConsultaProdutosCamposAdicionaisValores`, `B2CConsultaB2CProdutosCamposAdicionais`, `B2CConsultaB2CProdutosCamposAdicionaisDados`, `B2CConsultaB2CProdutosCamposAdicionaisMedia`, `B2CConsultaB2CProdutosCamposAdicionaisMediaDados`, `B2CConsultaB2CProdutosCamposAdicionaisOpcoes` |
| `LinxMicrovixB2CProductAttributesService` | 14 | `B2CConsultaClassificacao`, `B2CConsultaColecoes`, `B2CConsultaEspessuras`, `B2CConsultaGrade1`, `B2CConsultaGrade2`, `B2CConsultaLinhas`, `B2CConsultaMarcas`, `B2CConsultaSetores`, `B2CConsultaUnidade`, `B2CConsultaFornecedores`, `B2CConsultaFlags`, `B2CConsultaTags`, `B2CConsultaPalavrasChavePesquisa`, `B2CConsultaLegendasCadastrosAuxiliares` |
| `LinxMicrovixB2CStocksAndPricesService` | 7 | `B2CConsultaProdutosCustos`, `B2CConsultaProdutosDepositos`, `B2CConsultaProdutosDetalhes`, `B2CConsultaProdutosDetalhesDepositos`, `B2CConsultaProdutosPromocao`, `B2CConsultaProdutosTabelas`, `B2CConsultaProdutosTabelasPrecos` |
| `LinxMicrovixB2COrdersService` | 10 | `B2CConsultaPedidos` (¹), `B2CConsultaPedidosItens` (¹), `B2CConsultaPedidosPlanos`, `B2CConsultaPedidosStatus` (¹), `B2CConsultaPedidosIdentificador` (¹), `B2CConsultaPedidosTipos`, `B2CConsultaStatus` (¹), `B2CConsultaCodigoRastreio`, `B2CConsultaNFe` (¹), `B2CConsultaNFeSituacao` (¹) |

(¹) Os 36 métodos que o código de hoje já implementa. Os outros 224 são novos.

Notas sobre a lista:

* **Fora do Outbound:** os 11 métodos de escrita da V64 (`B2CCadastraClientes`, `B2CCadastraClientesContatos`,
  `B2CCadastraClientesEnderecosEntrega`, `B2CCadastraImagensHD`, `B2CCadastraProdutosImagensHD`, `B2CCadastraPedido`,
  `B2CCadastraPedidoItens`, `B2CCadastraPedidoPlanos`, `B2CCancelaPedido`, `B2CAtualizaCodigoRastreio`,
  `B2CAtualizaPedidoStatus`) usam outra URL (`importador.svc`) e pertencem ao projeto **Inbound**.
* `LinxCadastraClientesRede` estava na v169 como método de leitura, mas **saiu da v274** (só aparece na descrição de
  campos de outros métodos). Não entra.
* `LinxMetodos` **não recebe parâmetro nenhum**, nem a chave (com `chave` ou `cnpjEmp` responde "O parametro é
  inválido."), e devolve o **catálogo inteiro** da API: 204 métodos, cada um com a lista de parâmetros aceitos, numa
  coluna só e sem `<C>` (testado). Não diz quais métodos a chave do cliente tem liberados. Ele é gravado como os
  outros (uma linha por método, com nome e parâmetros) e serve de referência na implementação: os parâmetros que ele
  lista são os que a API aceita hoje, inclusive os que a v274 não cita (como `filtra_somente_timestamp`, §4.2). O
  catálogo tem os 191 métodos da v274 e mais 13 que a documentação não descreve, que ficam fora por enquanto (§10.1,
  decisão 18).
* Os métodos `LinxB2C*` do ERP (pedidos do e-commerce vistos pelo ERP) ficam no `LinxMicrovixOrdersService`; os
  métodos do WS B2C ficam nos services `LinxMicrovixB2C*`.
* O B2C de produtos foi dividido em três services (`B2CProducts`, `B2CProductImages`, `B2CProductFields`) e o pedido
  de compra saiu de `Orders` (`PurchaseOrders`) para nenhum service passar muito de 15 métodos.

Repositórios com o mesmo nome (`ILinxMicrovixProductsRepository`...), um `ILinxMicrovixRepository` comum
(parâmetros, lojas, último timestamp) e **um** `LinxMicrovixCommandHandler` com todas as queries,
como o `MovideskCommandHandler`. Somem as 37 interfaces `I<Método>CommandHandler` e as 37 classes de handler.

### 2.3 Nomes dos métodos e das tabelas (decidido)

**O nome do método é o nome do comando da API, sem prefixo** (decidido), exatamente como na documentação oficial e no
XML (`<Name>LinxClientesEnderecosEntrega</Name>`): o método do service é `LinxClientesEnderecosEntrega()`,
`B2CConsultaPedidos()`, e não `GetLinxClientesEnderecosEntrega()`. É a mesma regra do Totvs (§2.3 de lá: os nomes
seguem a documentação oficial). O nome do comando já leva o prefixo do sistema (`Linx...`, `B2CConsulta...`), então:

* a tabela tem o nome do comando (`linxmicrovix.LinxProdutos`, `linxmicrovix.B2CConsultaPedidos`), sem um segundo
  prefixo. O `untreated` é compartilhado, e esses nomes não colidem com nenhuma tabela de outro módulo;
* o job e a rota também têm o nome do comando (`LinxProdutos`), como já está no `EnumJob`; os ids mudam (§7).

### 2.4 Namespaces e pastas

| Item | Hoje | Padrão |
|---|---|---|
| Namespaces | Três famílias misturadas: `*.LinxMicrovix.Outbound.WebService.*`, `Application.LinxMicrovix.Outbound.Services.LinxMicrovix` (sem `WebService`) e `Infrastructure.Outbound.WebService.DependencyInjection`; `Infrastructure...Repository` (pasta `Repositorys`) | `Domain.LinxMicrovix.Outbound.*`, `Application.LinxMicrovix.Outbound.*`, `Infrastructure.LinxMicrovix.Outbound.Repositorys`, `Infrastructure.LinxMicrovix.Outbound.DependencyInjection` (o namespace segue a pasta) |
| `IAPICall` | Interface em `Domain/Abstractions/Api` com namespace `Application...`; implementação própria em `Infrastructure/Api/APICall.cs` | Removidos: `IApiClient` do Shared com a chave `"LinxMicrovixAPI"` (§6.1) |
| Pastas `LinxMicrovix/` e `LinxCommerce/` | Os métodos B2C ficam em pastas `LinxCommerce` (nome de outro módulo) | `Reads/Erp/<Assunto>` e `Reads/B2C/<Assunto>`, com o assunto do service (`Reads/Erp/Movements`, `Reads/B2C/Orders`) |
| `Entities/Base/Company.cs` | Herda `CompanyBase` vazia | `Entities/Parameters/Company.cs` com as colunas de `LinxMicrovixParameters` (§5.1) |
| `Entities/Parameters/LinxAPIParam.cs`, `LinxMethods.cs`, `IntegrityLockTablesRegister.cs` | Parâmetro de execução com `[Table("LinxAPIParam", Schema = "linx_microvix")]`, lista de métodos ativos do `appsettings`, trava de integridade com o Linx Commerce | Removidos. Um método que a chave não tenha liberado responde com `ResponseSuccess=False`, que vira mensagem no log (§4.2, item 6) |
| `CustomValidations/**` (Application) | 213 validadores fora do build: 35 repetem os de `Validations/` e 178 são de métodos que o código ainda não implementa | **Ponto de partida** dos validadores novos: as regras e os tamanhos são conferidos com a v274/V64, o validador vai para `Validations/<API>/<Assunto>` junto com o método e o arquivo antigo é apagado. A pasta some quando o último método entrar |
| `CustomValidations/**` (Domain) | 6 helpers estáticos no estilo `ValidationResult` | Apagados (o Shared já tem `FluentValidatorsExtension` e `NullableConverters`) |
| `B2CConsultaClientesEnderecosEntrega` (DTO e entidade) | Sem service, repositório nem job; a entidade grava `cidade_cliente = record.cep_cliente` | Implementado no `LinxMicrovixB2CCustomersService` |
| `LinxReshopSaldo` (service, repositório, entidade) | Importação de planilha do Azure Blob, registrada no DI do Outbound | Sai do Outbound (decidido); o destino é definido com quem usa o saldo Reshop, fora desta proposta |

Saem também os `using` sem uso (`System`, `System.Text` em `EnumJob.cs` e outros) e as atribuições repetidas nas
entidades (`portal` duas vezes em `LinxProdutos`, `LinxPlanos`, `LinxMovimentoCartoes`; `cnae_emp` duas vezes em
`LinxLojas`).

---

## 3. Domain.LinxMicrovix.Outbound

### 3.1 Estrutura proposta

```
Domain.LinxMicrovix.Outbound/
├── Abstractions/Repositorys/
│   ├── ILinxMicrovixRepository.cs            (parâmetros, lojas, último timestamp)
│   └── ILinxMicrovix<Assunto>Repository.cs   (um por service da §2.2)
├── Dtos/Reads/                                ← uma linha <R> da resposta (tudo string, { get; init; })
│   ├── Erp/<Assunto>/<Método>.cs              (Stores, Customers, Sellers, Products, ... Financial)
│   └── B2C/<Assunto>/<Método>.cs              (Stores, Customers, Products, ProductImages, ProductFields, ...)
├── Entities/
│   ├── Parameters/Company.cs                  (→ linxmicrovix.LinxMicrovixParameters)
│   └── Reads/<mesmas pastas dos DTOs>         (→ linxmicrovix.<Método>)
└── Enums/EnumJob.cs
```

### 3.2 Convenções que mudam

* **DTOs com `{ get; init; }` e sem construtor.** Hoje os 37 DTOs têm 237 propriedades `{ get; set; }` e um
  construtor com todos os campos, que o `CreateDto` de cada service chama com
  `record.Where(pair => pair.Key == "x").Select(pair => pair.Value).FirstOrDefault()` por campo (o `LinxMovimento`
  tem 113). Passa a ser como os DTOs do Movidesk: propriedades `init` e nenhum construtor. O método lê a linha `<R>`
  para um `Dictionary<string, string?>` indexado pelos nomes do `<C>` e preenche o DTO com um *object initializer*
  (`campo = row.GetValueOrDefault("campo")`), um acesso direto por campo no lugar de um `Where` sobre a linha inteira.
* **DTOs montados a partir da resposta real, e não do código de hoje.** O layout da API mudou desde que os DTOs foram
  escritos: o `LinxMovimento` devolve hoje 116 colunas, e o DTO não tem `conhecimento`, `custo_medio_epoca`,
  `dt_insert`, `ficha`, `numero_sequencial_pos` e `preco_produto_epoca`; o `LinxGrupoLojas` devolve 10, e a entidade
  não tem `lojas_proprias`, `classificacao_portal` e `emp_mod_linx_promo` (testado). Na fase de cada service, o DTO é
  conferido coluna a coluna com o `<C>` de uma resposta real e com a v274/V64. O nome da coluna é o da resposta, não o
  da documentação: no `B2CConsultaEmpresas` a coluna do CNPJ se chama `cnpjEmp` (testado), e o código de hoje lê
  esse nome certo. Também acaba com os erros de digitação da chave (`"complement_end_cli"` em
  `LinxClientesEnderecosEntrega`, `"CNPJ"`/`"cnpj"` em `LinxGrupoLojas`).
* **Datas no formato da resposta.** As datas vêm como `yyyy-MM-ddTHH:mm:ss` (`2024-01-29T00:00:00`, testado) e são
  convertidas com cultura invariante.
* **Entidades com `doc_company` e `integrated_or_updated_at`.** Nenhuma entidade tem as duas colunas; 38 têm
  `lastupdateon` (hora de Brasília calculada **por registro** no construtor). Passa a ser como no Movidesk e no
  Totvs: `doc_company` entra na chave primária e `integrated_or_updated_at` é o horário do lote (`GetBatchTime()`),
  igual para todo o lote. O construtor recebe `(doc_company, cnpj_emp, batchTime, dto)`.
* **`cnpj_emp` em toda tabela de loja.** A API é chamada por CNPJ da loja, mas alguns métodos devolvem dados do portal
  inteiro (vendedores, usuários, setores, planos, campos adicionais...) e outros não trazem o CNPJ na resposta. A
  entidade passa a receber o CNPJ consultado, e a chave fica `doc_company` + (`cnpj_emp`, quando o dado é da loja) +
  a chave natural (§5.2).
* **Datas e números anuláveis.** As entidades já declaram `DateTime?`, `Int32?`, `decimal?`, mas atribuem com
  `ConvertToDateTimeValidation`/`ConvertToInt32Validation`, que devolvem `1990-01-01` e `0` para nulo. O padrão é o
  da `docs/PROPOSTA-DATAS-NULAS-AFTERSALE.md`: `NullableConverters.ToNullableDateTime`, `ToNullableInt32`,
  `ToNullableDecimal`, `ToNullableBoolean` (e um `ToNullableInt64`, que o Shared ainda não tem). Chaves continuam
  não anuláveis. `LinxCfopFiscal` é a única entidade com tipos não anuláveis; `lastupdateon` não é anulável em
  `LinxUsuarios` e `LinxVendedores`.
* **`BIT` em dois formatos.** A especificação avisa (§4.3 de lá) que campos `BIT` podem vir como `0`/`1` ou
  `False`/`True`. O `ToNullableBoolean` aceita os dois.
* **`Guid` com conversão segura.** `Guid.Parse(record.identificador)` lança exceção com valor vazio em
  `LinxMovimento`, `LinxMovimentoCartoes`, `LinxMovimentoPlanos`, `LinxMovimentoTrocas`, `LinxXMLDocumentos` e
  `B2CConsultaNFe`. A exceção derruba o lote inteiro (o `DeserializeXMLToObjectCore` relança como
  `GeneralException`). Passa a usar `ConvertToGuidValidation`/um `ToNullableGuid`, e o registro sem o identificador
  que é chave é rejeitado na validação com mensagem.
* **Corte de texto com `StringLengthValidation`.** Hoje nada é cortado: o validador só registra a mensagem e o
  registro segue, e um texto maior que a coluna derruba o `SqlBulkCopy` **do lote inteiro**. O Movidesk usa
  `ConverterValidatorsExtensions.StringLengthValidation(dto.x, n)` com o tamanho do script.
* **Sem `recordKey`/`recordXml` na entidade.** Hoje cada entidade guarda a chave de cache em texto e o XML da linha,
  marcados `[NotMapped][SkipProperty]`, só para o cache e para o `AddRecord`. Como no Movidesk (`SaveServices`), o
  método monta um `Dictionary<chave, xml>` a partir da resposta e passa para a gravação; a entidade só tem colunas.
* **`EnumJob`** do próprio módulo, com os ids conferidos com o script (§7). Hoje nenhum service importa
  `Domain.LinxMicrovix.Outbound.Enums`: todos usam `Domain.Core.Enums`.

---

## 4. Application.LinxMicrovix.Outbound

### 4.1 Services

| Service | Observação |
|---|---|
| `LinxMicrovixService` (comum) | Mesmo papel do `MovideskService`/`TotvsService`: `GetBatchTime`, `GetCompanys` (cache), `GetStores(company, api)` e `GetLastTimestamp(company, método, cnpj)`. Substitui o `LinxMicrovixService` de hoje (três sobrecargas de `BuildBodyRequest` e o `DeserializeResponseToXML`) e o `APICall` |
| Os 29 services da §2.2 | Cada método escreve o próprio fluxo (§4.2), sem classe base |

**Sem classe base e tudo à mão em cada método (decidido, como no Totvs).** `LinxMicrovixBaseSyncService` e
`B2CBaseSyncService` saem. Hoje elas existem, mas 24 dos 36 services as contornam copiando o `GetRecordsCore`
inteiro só para mudar um detalhe (lista de lojas, um `ToUpper`, o timestamp), e as cópias divergiram umas das outras
(§8). Cada método implementa no próprio corpo, como o `GetTickets`, o `GetPeople` e o `GetServices` do Movidesk e as
buscas do Totvs:

* o envelope XML (`<Authentication>`, `<ResponseFormat>`, `<Command>`, `<Parameters>`) com os parâmetros do método;
* a chamada pelo `IApiClient` e o tratamento da resposta (`<ResponseSuccess>False`, que chega com HTTP 200);
* a repetição pelo timestamp enquanto a resposta vier no limite de registros (§4.2);
* a leitura de `<C>`/`<R>` e o preenchimento do DTO (§3.2);
* a validação, a conversão para entidade e a gravação.

O que é comum fica no `ILinxMicrovixService`, injetado nos services como o `IMovideskService`: horário do lote,
clientes, lojas e o último timestamp gravado. É o mesmo corte do Totvs, em que só o login
(`GetToken`) é centralizado. Partes de gravação podem virar métodos privados do próprio service, como o
`SaveServices` do Movidesk. O custo conhecido: cerca de 100 linhas a mais por método do que com o transporte
centralizado, em 260 métodos.

**Sob demanda.** Hoje 20 métodos ERP e 4 B2C têm uma versão "individual" (`parameters_individual`) chamada por rotas
comentadas, com `cod_cliente`, `id_pedido`, `cod_produto`... Elas não entram nas fases 1 e 2. Entram depois (fase 3,
§9; na implementação foram 24 métodos, 19 ERP e 5 B2C), no desenho do `GET Tickets/{docCompany}/{id}` do Movidesk e das rotas sob demanda do Totvs: cabeçalho
`X-User-Name`, `docCompany` por `^\d{14}$` e registro em `linxmicrovix.LinxMicrovixRequests`.

**`IntegrityLock*`** (todos os B2C, `LinxClientesFornec` e `LinxPedidosCompra`): comparam o B2C com o Linx Commerce
(`[linx_commerce]`) e com `[linx_microvix].IntegrityLockTablesRegisters`, mas montam a lista em memória e **nunca
gravam** (exceto `B2CConsultaPedidosStatus`, que quebra com `GetRecords("")`). Saem: dependem de outro módulo e de
uma tabela que nenhum script cria.

### 4.2 Fluxo de leitura (escrito em cada método)

Pela especificação (§4.1, *Timestamps*, e §7, *Restrições*): o `timestamp` do Microvix **não é data**, é o
`rowversion` do SQL Server. Ele é crescente e **próprio de cada método**: o timestamp de um método é o maior
timestamp das tabelas que ele junta. A forma correta de ler é "uma vez todo o passado da loja com timestamp zero,
depois o último timestamp da consulta anterior". A Linx avisa que ler a base inteira repetidamente "resultará na
desativação da chave de integração [...] sem prévio aviso". Hoje a maioria dos métodos manda `timestamp = 0` a cada
execução (§8, item 7).

A v274 acrescenta (§4.5 de lá, *Limite de Registros por Retorno*): **alguns métodos devolvem no máximo 5000
registros por chamada**, e o restante deve ser lido repetindo a chamada com o maior timestamp recebido. O código de
hoje faz uma chamada só e perde o que passa de 5000.

**O que as chamadas reais confirmaram (26/10/2026):**

| Ponto | Resultado |
|---|---|
| Chaves | As chaves de produção da Newbloomers, do ERP e do B2C, **estão ativas** |
| Credenciais por API | `linx_export` funciona só com a chave do ERP, e `linx_b2c` só com a do B2C. Trocadas, a API responde "Ocorreu um erro inesperado." |
| Erros | Todos chegam com **HTTP 200** e `<ResponseSuccess>False</ResponseSuccess>` + `<Message>`: "O parametro é inválido." (parâmetro que o método não aceita), "Ocorreu um erro inesperado." (credencial errada), "Ocorreu um erro na execução da integração, verifique o nome da consulta e os parâmetros informados." (parâmetro obrigatório faltando) |
| Formato da resposta | `<Microvix><ResponseData><C><D>coluna</D>...</C><R><D>valor</D>...</R>...</ResponseData><ResponseResult><ResponseSuccess>True</ResponseSuccess></ResponseResult></Microvix>`; valor nulo vem como `<D />` |
| Limite de 5000 | Confirmado no `LinxMovimento`: uma loja, um ano, veio **exatamente 5000** linhas, em ordem crescente de `timestamp`. A chamada seguinte, com o maior timestamp da primeira, trouxe mais 5000, **sem nenhuma repetida**. Cada página tem ~7–8 MB e levou 25–55 s, dentro do timeout de 20 min. **O limite muda por método**: o `LinxUsuarios` devolve no máximo **100** por chamada (188 usuários em duas páginas, testado em 27/10/2026) |
| `filtra_somente_timestamp` | Aceito por 8 métodos (`LinxFaturas`, `LinxLancContabil`, `LinxMovimento`, `LinxPedidosCompra`, `LinxPedidosVenda`, `LinxProdutosCamposAdicionais`, `LinxProdutosPromocoes`, `LinxSangriaSuprimentos`), segundo o `LinxMetodos`; a v274 não o descreve. As datas obrigatórias **continuam obrigatórias** (sem elas a API responde erro), mas com `filtra_somente_timestamp = 1` elas são **ignoradas**: com datas de 2000 e a flag ligada vieram 5000 movimentos de 2024; sem a flag, nenhum |
| `<IdPortal>` | **Não tem efeito**: o `LinxGrupoLojas` devolveu as mesmas 21 lojas com o portal certo (15133), com um portal errado (1) e sem o elemento. O filtro de portal do `LinxGrupoLojas` é o **parâmetro** `portal` (opcional, listado no `LinxMetodos`), não o elemento do envelope. O módulo **não envia** `<IdPortal>` |
| `LinxGrupoLojas` | Responde sem o parâmetro `grupo`. A chave da Newbloomers enxerga um portal (15133, "Misha") com 21 lojas; uma delas ("NOME DA EMPRESA", `empresa` 21) vem com **CNPJ vazio** |
| URLs | O ERP e o B2C responderam tanto em `https://webapi.microvix.com.br/1.0/api/integracao` quanto em `https://webapi.microvix.com.br/api/integracao`, com ou sem o nome do comando no fim da rota (`.../integracao/LinxPedidosVenda`) |

Cada método segue o desenho do `GetBranchesList` do Totvs:

1. abre o log em `auditing.Logs` com o job do método;
2. para cada cliente de `linxmicrovix.LinxMicrovixParameters` (§5.1), obtém as credenciais da API do método (ERP ou
   B2C) pelo `ILinxMicrovixService.GetCompanys`. Um cliente sem chave daquela API é pulado sem mensagem;
3. obtém as lojas. No ERP, o `LinxGrupoLojas` é o **primeiro método**: chama a API só com a `chave` (e `grupo`, se o
   cliente tiver) e grava todas as lojas do grupo. No B2C, o `B2CConsultaEmpresas` faz o mesmo papel. Os outros
   métodos usam essas lojas (`ILinxMicrovixService.GetStores`), e um cliente sem lojas gravadas registra a mensagem e
   é pulado. Loja sem CNPJ (como a "NOME DA EMPRESA" da Newbloomers) é gravada, mas não é consultada pelos outros
   métodos, porque `cnpjEmp` é obrigatório. Os métodos que devolvem dados do **portal** (e não da loja) chamam **uma**
   loja por portal: hoje `LinxVendedores` e `LinxProdutosCamposAdicionais` chamam todas e gravam o mesmo registro N
   vezes no mesmo lote. A classificação de cada método (loja ou portal) é feita pela especificação, na fase do
   service;
4. calcula o timestamp **a partir do banco**: `MAX(timestamp)` do método para o cliente e a loja (ou o portal). Com o
   banco vazio, `0`: a primeira execução traz o histórico inteiro. É o mesmo desenho do `GetTicketsLastUpdate` do
   Movidesk, sem folga (o timestamp é exclusivo: a API devolve só os maiores que o informado, testado);
5. monta os parâmetros **no código**, pela especificação e pela lista do `LinxMetodos`. Não há mais tabela de modelos
   de parâmetros nem `Replace("[0]", ...)`. Para os métodos com datas obrigatórias há dois casos:
   * **os 8 que aceitam `filtra_somente_timestamp`** mandam a flag ligada e um intervalo fixo qualquer (as datas são
     obrigatórias, mas ignoradas): a leitura fica só pelo timestamp, sem fatias de datas, e a primeira execução já
     percorre o histórico inteiro em páginas de 5000;
   * **os outros** mandam um intervalo largo (`2000-01-01` até hoje) com o timestamp. Implementado assim na fase 2,
     sem fatias de datas: a paginação pelo timestamp já limita cada resposta (5000 linhas no ERP), e as datas só
     restringem o conjunto.

   | Método | Parâmetros obrigatórios além de `chave`/`cnpjEmp` | Leitura proposta | Hoje |
   |---|---|---|---|
   | `LinxMovimento` | `timestamp`, `data_inicial`, `data_fim` | `filtra_somente_timestamp = 1` | hoje−3 dias, timestamp 0 |
   | `LinxPedidosVenda`, `LinxPedidosCompra` | `data_inicial`, `data_fim` | `filtra_somente_timestamp = 1` | hoje−7 dias / hoje−6 meses, timestamp 0 |
   | `LinxProdutosPromocoes`, `LinxProdutosCamposAdicionais` | datas do método | `filtra_somente_timestamp = 1` | a partir de hoje / hoje−7 |
   | `LinxProdutos` | `dt_update_inicio`, `dt_update_fim` | fatias | hoje−7, por setor (`id_setor` de `LinxSetores`; produto sem setor nunca vem) |
   | `LinxProdutosDetalhes` | `data_mov_ini`, `data_mov_fim` | fatias | de 2000-01-01 a hoje, timestamp 0, a cada execução |
   | `LinxMovimentoCartoes`, `LinxMovimentoPlanos`, `LinxMovimentoTrocas`, `LinxXMLDocumentos`, B2C de pedidos e notas | datas do método | fatias | janelas fixas de 2 dias a 6 meses |

   Os parâmetros dos 224 métodos novos são tirados da v274/V64 e conferidos no `LinxMetodos` na fase do service de
   cada um. O envelope **não** leva `<IdPortal>` (testado, sem efeito);
6. chama a API pelo `IApiClient` e trata a resposta **pelo corpo**, não pelo status HTTP: `<ResponseSuccess>False`
   (que chega com HTTP 200) ou erro HTTP viram mensagem em `auditing.Messages` com a `<Message>` da API, e a loja
   segue para a próxima. Hoje `APICall.PostAsync` só olha o status HTTP e lança `APIException`, e o job inteiro para;
7. **repete a chamada** com o maior timestamp da resposta **até ela vir vazia**, juntando as páginas. O limite de
   registros por chamada muda por método (5000 no `LinxMovimento`, 100 no `LinxUsuarios`, testado) e não está na v274
   para cada um, então o método não depende dele: a última chamada, vazia, custa uma requisição a mais por loja;
8. lê a resposta **sem `Parallel.For`**: hoje `DeserializeResponseToXML` percorre as linhas em paralelo gravando
   numa `List` comum (perde linhas ou lança exceção) e lendo um `XmlDocument`, que não aceita leitura simultânea;
9. deduplica a resposta pela chave primária (§5.2), ficando com o maior timestamp;
10. valida cada DTO (FluentValidation), registra as mensagens e converte para entidade com o `batchTime` do lote;
11. grava (bulk em `untreated` + merges em `linxmicrovix`) e registra cada registro em `auditing.Records` com o XML
    original da linha;
12. fecha o log (`SetLogEndDate` + `CommitAllChanges`) num `finally`. Hoje nenhuma das cópias de `GetRecords` tem
    `try/finally`, e os `GetRecord` sob demanda nem abrem o log (`_logger.Clear().AddLog()`): passam ao merge o GUID
    da execução anterior.

Uma leitura com erro em qualquer fatia ou página não grava nada daquela loja naquele método, para o
`MAX(timestamp)` não pular registros (como no `GetBranchesList`). Na carga inicial de métodos grandes (o
`LinxMovimento` de uma loja tem dezenas de páginas de 5000), a gravação pode ser feita **por página**, em ordem de
timestamp: como as páginas vêm em ordem crescente, uma falha no meio deixa o `MAX(timestamp)` no fim da última página
gravada, e a execução seguinte continua dali sem pular nada.

**Ordem de dependência.** Os métodos que fornecem lista para outros rodam antes:

* `LinxGrupoLojas` → `LinxLojas` (ERP) e `B2CConsultaEmpresas` (B2C);
* `LinxProdutosTabelas` → `LinxProdutosTabelasPrecos` (itera `id_tabela`);
* `LinxProdutosDepositos` → `LinxProdutosInventario` (itera depósitos).

Com a recorrência, o método dependente roda com a lista que já está no banco: a primeira execução dele, antes da
lista existir, só registra a mensagem e segue.

### 4.3 Validações (FluentValidation)

Os validadores de `Validations/` já seguem o padrão do Movidesk (`MustBeValidInt32`, `MustBeValidDateTime`,
`MaximumLength` com mensagem). Mudanças:

* acompanham a renomeação de pastas e DTOs (§2.4); os 178 validadores de `CustomValidations/` dos métodos novos são o
  ponto de partida de cada um (§2.4);
* os tamanhos passam a citar o script de tabela, que por sua vez cita a v274/V64. Hoje não há script, e há limites
  divergentes: `uf_cliente` 20 em `LinxClientesFornec` e 2 em `LinxClientesEnderecosEntrega`; `grupo_usuarios` 1 em
  `LinxUsuarios`;
* campos sem regra ganham regra (`obs` em `LinxClientesFornec`, `LinxMovimento`, `LinxPedidosVenda` e
  `LinxPedidosCompra`; `texto_comprovante`; `empresas`; `nf_gerada`; `nf_origem_ws`; `conta_central`;
  `mensagem_falha_faturamento`);
* o tipo segue a especificação (`timestamp` é `BIGINT` em todos; `B2CConsultaPedidosIdentificador` converte com
  `ConvertToInt32Validation`);
* registro com **uma** chamada `AddValidatorsFromAssemblyContaining<...>()` (§6.1).

### 4.4 Cache (padrão `docs/PADRAO-CACHE-INTEGRACOES.md`)

| Chave | Conteúdo | Expiração |
|---|---|---|
| `LinxMicrovix:Parameters` | Linhas de `linxmicrovix.LinxMicrovixParameters` | 2 h absoluto / 30 min deslizante |
| (sem cache) | Lojas do cliente | Sem cache, como `GetBranchCodes` no Totvs: uma loja nova vale na próxima execução |

**Sem cache de estado (proposto).** Hoje cada método guarda em cache um `HashSet` com a chave de **todas** as linhas
da tabela (sem filtro de empresa na maioria: `"LinxLojas:RecordKeys"`), com o timestamp dentro da chave, para não
regravar o que já está no banco. No padrão, o cache de estado existe porque a janela de leitura **relê** registros (o
Totvs relê a última hora). No Microvix isso não acontece: o timestamp é exclusivo e a API **nunca** devolve de novo o
que já foi lido, então todo registro que chega é novo ou alterado. O cache não evitaria nenhuma gravação, só ocuparia
memória com 260 tabelas, e o de hoje ainda erra:

* a chave em texto não confere com a montada no SQL em vários métodos (§8, itens 9 a 11): o registro nunca é
  encontrado e é regravado sempre;
* o cache é carregado com as chaves da **primeira** execução e alterado sem trava por execuções simultâneas;
* nenhuma chave leva a empresa.

### 4.5 Command handler (queries)

* Um `LinxMicrovixCommandHandler` com as queries de todos os métodos.
* As queries passam a ler `linxmicrovix.<Método>` filtrando por `DOC_COMPANY` (hoje `[linx_microvix_erp]` fixo no
  texto, mesmo quando o service usa `jobParameter.schema`).
* `CreateGetCompanysQuery`, `CreateGetStoresQuery(doc_company, api)` (lojas com CNPJ) e
  `CreateGetLastTimestampQuery(doc_company, método, cnpj_emp)` (`MAX(TIMESTAMP)`). A última recebe o nome da tabela
  de uma lista fechada (os nomes do `EnumJob`), nunca de texto livre.
* Somem `CreateGetParametersQuery`, as três consultas de empresas e `CreateGetLast7DaysMinTimestamp*`.
* Valores interpolados (`doc_company`, CNPJ, ids) seguem a regra do Movidesk: só valores já validados (CNPJ por
  `^\d{14}$`, números sem aspas). Hoje entram no `IN (...)` valores vindos da API (`doc_cliente`, `CUSTOMERCPF` do
  Commerce), protegidos só pelo `Replace("'", "''")` que o `DeserializeResponseToXML` aplica a **todos** os campos e
  que grava as aspas duplicadas no banco.

---

## 5. Banco de dados

### 5.0 Schema `linxmicrovix` (proposto)

Todas as tabelas do módulo ficam no schema próprio **`linxmicrovix`**, como `movidesk`, `aftersale` e `totvs` (sem
`_`, como os outros). Hoje o código aponta para três schemas legados (`linx_microvix`, `linx_microvix_erp`,
`linx_microvix_commerce`) que nenhum script cria.

* o bulk vai para `untreated.<Método>` (`BulkInsertIntoTableRaw(table, "untreated")`, sem a sobrecarga `[Obsolete]`);
* as merges ficam em `linxmicrovix.sp_<Método>_Sync`, geradas por `dbo.sp_Create_Merge_Procedure`;
* o primeiro script cria o schema (`if not exists (select 1 from sys.schemas where name = 'linxmicrovix') exec('create schema linxmicrovix')`).

A integração do Oto, que lê os schemas legados `LINX_MICROVIX_ERP` e `LINX_MICROVIX_COMMERCE`, **não é tocada**
nesta refatoração (decidido).

### 5.1 `linxmicrovix.LinxMicrovixParameters` (novo; tira as credenciais do `appsettings`)

**Uma linha por cliente** (decidido: o módulo não pode ter nada fixo da Newbloomers). O que identifica o cliente na
Linx é a **chave** (GUID) e o **grupo**, entregues na ativação. Usuário e senha são **diferentes por API**: pela
v274, `linx_export`/`linx_export` no WS de saída; pela V64, `linx_b2c`/`linx_b2c` no B2C. As chamadas reais
confirmaram que a chave do ERP e a do B2C são diferentes e que cada uma só funciona com o seu usuário (§4.2). O
código de hoje manda o usuário nos dois campos, o que só funciona porque cada par é igual. O `grupo` é opcional: o
`LinxGrupoLojas` respondeu sem ele (testado).

```sql
create table [linxmicrovix].LinxMicrovixParameters (
	integrated_or_updated_at datetime,
	id int not null identity primary key,
	[doc_company] nvarchar(14) not null,   --CNPJ do cliente (chave em todas as tabelas do módulo)
	[grupo] nvarchar(100),                 --nome do grupo na ativação (parâmetro grupo do LinxGrupoLojas, opcional)
	erp_chave uniqueidentifier,            --chave do WS de saída padrão (nula se o cliente não tiver)
	erp_user_name nvarchar(50),            --linx_export pela v274
	erp_password nvarchar(50),             --linx_export pela v274
	b2c_chave uniqueidentifier,            --chave do WS B2C (nula se o cliente não tiver)
	b2c_user_name nvarchar(50),            --linx_b2c pela V64
	b2c_password nvarchar(50)              --linx_b2c pela V64
);
```

* `doc_company` é o CNPJ que o cliente escolher para identificá-lo (a matriz ou a holding), e a loja de cada registro
  é o `cnpj_emp`. Na Newbloomers, o CNPJ da holding (`44065634000106`), como no Totvs.
* Um cliente só com ERP, ou só com B2C, deixa as colunas da outra API nulas, e os jobs dela pulam o cliente.
* O script de valores fica **fora do git** (`Script003_..._Table.sql.example` no repositório e o `.sql` no
  `.gitignore`), como no Totvs. As chaves de produção da Newbloomers entram só nesse `.sql`, nunca em documento,
  comentário ou `appsettings`.
* Somem as regras fixas da Newbloomers no código, como o filtro de lojas `"MISHA"`/`"OPEN"` de
  `LinxProdutosTabelasPrecos` (§8, item 26).

### 5.2 Tabelas de dados

Mesma convenção do Movidesk e do Totvs: par `untreated.<Método>` (sem chave, recebe o bulk) e
`linxmicrovix.<Método>` (com chave primária), com `integrated_or_updated_at`, `doc_company` e `timestamp` (`bigint`)
em todas. As respostas são planas: **uma tabela por método, sem filhas** (260 pares).

**Regra da chave primária:** `doc_company` + `cnpj_emp` (quando o dado é da loja, §4.2 item 3) + a chave natural do
método, tirada da v274/V64 e **conferida com uma resposta real** na fase do service, antes do script. Os tipos e
tamanhos das colunas são os da especificação (ex.: `LinxLojas.cnpj_emp varchar(14)`, `smalldatetime` → `datetime`,
`float` → `decimal`).

Chaves já levantadas para os 36 métodos de hoje. As marcadas com "?" são as que a resposta real tem de confirmar; as
marcadas com "✓" foram conferidas nas chamadas de 26/10/2026:

| Método | Chave primária (além de `doc_company`) | Hoje (chave do cache) |
|---|---|---|
| `LinxGrupoLojas` | `portal`, `empresa` ✓ (21 lojas, 21 `empresa` distintas; `CNPJ` não serve: uma loja vem com ele vazio) | as 8 colunas, sem timestamp |
| `LinxLojas` | `cnpj_emp` | `empresa`, `cnpj_emp`, `timestamp` |
| `LinxClientesFornec` | `cod_cliente` | `cod_cliente`, `doc_cliente`, `timestamp` |
| `LinxClientesEnderecosEntrega` | `id_endereco_entrega` | + `cod_cliente`, `timestamp` |
| `LinxSetores` | `id_setor` | + `timestamp` |
| `LinxPlanos` | `plano` | + `timestamp` |
| `LinxNaturezaOperacao` | `cod_natureza_operacao` | `TRIM(...)`, `timestamp` |
| `LinxUsuarios` | `portal`, `usuario_id` ✓ (188 usuários, todos distintos; método de portal, sem `cnpjEmp`) | + `usuario_doc`, `timestamp` |
| `LinxLojasParametros` | `cnpj_emp` ✓ (a resposta não traz o CNPJ: é a loja consultada; uma linha por loja) | — (método novo) |
| `LinxMetodos` | `metodo` ✓ (204 métodos distintos) | — (método novo) |
| `LinxEntrantes` | `portal`, `id_entrantes` (sem dados na Newbloomers: só devolve os últimos 2 anos) | — (método novo) |
| `LinxVendedores` | `cod_vendedor` | + `cpf`, `timestamp` |
| `LinxCfopFiscal` | `id_cfop_fiscal` | + `timestamp` |
| `LinxRotinaOrigem` | `codigo_rotina` | + `timestamp` |
| `LinxMovimento` | `cnpj_emp`, `transacao` ✓ (5000 linhas de uma loja com `transacao` distinta, e nenhuma repetida na página seguinte; `identificador` e `documento` se repetem por item) | 11 campos, sem conferir com o SQL (§8) |
| `LinxMovimentoCartoes` | `cnpj_emp`, `identificador`, `ordem_cartao`? | `cnpj`, `cupom`, `cod_aut`, `identificador`, `timestamp` |
| `LinxMovimentoPlanos` | `cnpj_emp`, `identificador`, `plano` | + `timestamp` |
| `LinxMovimentoTrocas` | `cnpj_emp`, `num_vale`? | 6 campos + `timestamp` |
| `LinxXMLDocumentos` | `cnpj_emp`, `chave_nfe` | + `documento`, `timestamp` |
| `LinxPedidosVenda` | `cnpj_emp`, `transacao` ✓ (5000 linhas, todas com `transacao` distinta; `cod_pedido` se repete por item) | + `codigo_cliente`, `cod_produto` (ordem trocada, §8) |
| `LinxPedidosCompra` | `cnpj_emp`, `cod_pedido`, `transacao`? | + `fornecedor`, `produto` |
| `LinxProdutos` | `cod_produto` | + `timestamp` |
| `LinxProdutosCodBar` | `cod_produto`, `cod_barra` | + `timestamp` |
| `LinxProdutosTabelas` | `cnpj_emp`, `id_tabela` | + `tipo_tabela`, `timestamp` |
| `LinxProdutosTabelasPrecos` | `cnpj_emp`, `id_tabela`, `cod_produto` | + `timestamp` |
| `LinxProdutosInventario` | `cnpj_emp`, `cod_deposito`, `cod_produto`, `data_inventario` (a entidade hoje não guarda a data) | sem timestamp |
| `LinxProdutosDetalhes` | `cnpj_emp`, `cod_produto` | + `timestamp` |
| `LinxProdutosDepositos` | `cod_deposito` | + `timestamp` |
| `LinxProdutosPromocoes` | `cnpj_emp`, `cod_produto`, `id_campanha`? | strings cruas da API (§8) |
| `LinxProdutosCamposAdicionais` | `cod_produto`, `campo` | + `timestamp` |
| `B2CConsultaEmpresas` | `empresa` | `cnpj_emp` (a coluna da resposta é `cnpjEmp`, testado) |
| `B2CConsultaClientes` | `cod_cliente_b2c` | + `cod_cliente_erp`, `doc_cliente`, `timestamp` |
| `B2CConsultaNFe` | `id_nfe` | |
| `B2CConsultaNFeSituacao` | `id_nfe_situacao` | |
| `B2CConsultaPedidos` | `id_pedido` ✓ (2.756 pedidos de um mês, todos distintos) | 6 campos |
| `B2CConsultaPedidosIdentificador` | `identificador` | |
| `B2CConsultaPedidosItens` | `id_pedido_item` | |
| `B2CConsultaPedidosStatus` | `id` | |
| `B2CConsultaStatus` | `id_status` | |

As chaves dos 224 métodos novos entram nesta tabela à medida que cada service é implementado (§9).

### 5.3 Procedures de sincronização

Uma por tabela, no schema `linxmicrovix`, sem `@delete_condition`: como não há filhas, nada é apagado pela merge. Um
registro excluído no Microvix fica no banco com a flag que a própria API devolve (`excluido`, `cancelado`,
`ativo`...), como as notas `Deleted` do Totvs.

```sql
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxGrupoLojas', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxLojas', @execute = 1;
```

### 5.4 Scripts DbUp (`Infrastructure.LinxMicrovix.Outbound/Scripts/SQLServer`)

Os scripts `001` e `002` atuais **não rodaram** com efeito em nenhum banco deste repositório (o `002` é só
comentário; o `001` insere o app 80). A numeração é refeita no formato do Totvs: cada service traz o seu **trio** de
scripts (tabelas, procedures, jobs), com os números seguintes, e nenhum script executado é editado depois. São 3
scripts de base + 29 trios = **90 scripts** ao fim da fase 2.

| Script | Conteúdo | Entra com |
|---|---|---|
| `Script001_Create_LinxMicrovix_Schema_And_Parameters_Table.sql` | Schema `linxmicrovix` + `LinxMicrovixParameters` | Base (fase 1) |
| `Script002_Insert_LinxMicrovix_Values_Into_Apps_Table.sql` | App `80 - LinxMicrovix` (o `Script001` atual), único app do módulo, ERP e B2C (§7) | Base |
| `Script003_Insert_Values_Into_LinxMicrovix_Parameters_Table.sql` | Clientes, chaves e usuários; fora do git, a partir do `.sql.example` | Base |
| `Script004_Create_LinxMicrovix_Stores_Tables.sql` | As 6 tabelas do `LinxMicrovixStoresService` | Service 1 |
| `Script005_Create_LinxMicrovix_Stores_Sync_Procedures.sql` | | Service 1 |
| `Script006_Insert_LinxMicrovix_Stores_Jobs_Into_Jobs_Table.sql` | Jobs e procedures (§7) | Service 1 |
| `Script007`… | Mesmo trio por service, na ordem da §9 | |

O `Script002_Insert_LinxMicrovix_Values_Into_Jobs_Table.sql` atual sai. Os três `migrator.csx` têm o mesmo erro do
Movidesk e do AfterSale (`PostgreSQL/migrator.csx` chama `MySqlDatabase`); fica anotado, fora do escopo.

### 5.5 Log de requisições (`linxmicrovix.LinxMicrovixRequests`)

Entra com as rotas sob demanda (fase 3, §9), igual à `MovideskRequests`/`TotvsRequests`: só inserção, com o corpo XML
enviado em `request_body` **sem a chave** (a chave no XML é o equivalente ao token na rota).

---

## 6. Infrastructure.LinxMicrovix.Outbound e host

### 6.1 `DependencyInjection.cs`

* Namespace `Infrastructure.LinxMicrovix.Outbound.DependencyInjection` (hoje `Infrastructure.Outbound.WebService...`).
* **Um** método `AddScopedLinxMicrovixServices(IConfiguration)`, no lugar de dois que registram de novo os mesmos
  `ILinxMicrovixCommandHandler`, `ILinxMicrovixService` e `IAPICall`.
* **Um** `HttpClient`/`IApiClient` com a chave `"LinxMicrovixAPI"` (`LinxMicrovix:BaseUrl`), como o `"MovideskAPI"`,
  timeout de 20 min, para as duas APIs. A v274 documenta `https://webapi.microvix.com.br/1.0/api/integracao` e a V64
  documenta `https://webapi.microvix.com.br/api/integracao`, mas as chamadas reais responderam nas duas URLs para o
  ERP e para o B2C (§4.2). Fica a da v274, que é a dos curls usados hoje. A rota de cada chamada é o nome do comando
  (`.../integracao/LinxPedidosVenda`), como nos curls e no `APICall` de hoje. Para o portal de teste,
  `appsettings.Development.json` usa `https://webapi-aceitacao.microvix.com.br/api/integracao`.
* O corpo vai em **UTF-8** com `Content-Type: application/xml`, como nos curls. Hoje o `APICall` monta o
  `StringContent` com `Encoding.ASCII` e `text/xml`: um acento num parâmetro vira `?`.
* Hoje a URL de produção está fixa no código, o `IApiClient` é registrado mas ninguém o usa (todos usam `IAPICall`),
  e o `AddScopedB2CLinxMicrovixServices` nem registra o cliente: o B2C falharia no `CreateClient`.
* `AddValidatorsFromAssemblyContaining` é chamado **36 vezes** sobre o mesmo assembly (cada validador fica
  registrado 36 vezes). Fica uma chamada.
* Registrar `ILinxMicrovixService`, os 29 services, os repositórios e o `ILinxMicrovixCommandHandler`.

### 6.2 Repositórios

* `ISharedRepository` no lugar de `IIntegrationsCoreRepository`.
* `BulkInsertIntoTableRaw(table, "untreated")` e `CallDbProcMerge("linxmicrovix", tabela, execução)`.
* Um método de gravação por método da API no repositório do service, com um bulk e uma merge (as tabelas são
  planas).
* Saem `GetRegistersExists` (cache de estado, §4.4), os `InsertRecord`/`CreateInsertRecordQuery` (usados só pelos
  `GetRecord`; vários gravam em `{tableName}` ou `{tableName}_raw` **sem schema**) e o `BulkInsert` posicional de
  `B2CConsultaPedidosItensRepository`.

### 6.3 Host Hangfire

| Arquivo | Alteração |
|---|---|
| `Hangfire.csproj` | Referenciar `Application.LinxMicrovix.Outbound` e `Infrastructure.LinxMicrovix.Outbound` |
| `Extensions/ServicesExtensions.cs` | `using Infrastructure.LinxMicrovix.Outbound.DependencyInjection;` + `AddScopedLinxMicrovixServices(builder.Configuration)`; sai a linha comentada `AddValidatorsFromAssembly(Assembly.Load("Application.LinxMicrovix.Outbound"))` |
| `Extensions/AppUseExtensions.cs`, `RecurringJobs/LinxMicrovixERPRecurringJobsExtensions.cs`, `LinxMicrovixB2CRecurringJobsExtensions.cs` | Os dois arquivos de jobs (comentados, com propriedades que não existem mais e restos da FlashCourier) e as chamadas a eles saem. A recorrência passa a ser a do Movidesk e do Totvs: o intervalo de cada job em `auditing.Jobs.IntervalTimeInMinutes` (§7); nenhum dos dois módulos registra job no Hangfire |
| `appsettings.json` | `LinxMicrovix:BaseUrl` de produção; sai `B2CLinxMicrovix` (as credenciais B2C vão para a tabela de parâmetros) |
| `appsettings.Development.json` | `LinxMicrovix:BaseUrl` do portal de teste (`webapi-aceitacao`) |
| `Controllers/LinxMicrovix/ERP/LinxMicrovixController.cs` e `B2C/B2CLinxMicrovixController.cs` | Um só `LinxMicrovixController`, dividido em `partial` (abaixo) |

### 6.4 `LinxMicrovixController`

Rota base `LinxMicrovixJobs/LinxMicrovix` (mantida), `[Authorize]` **comentado** e `IgnoreApi` em `RELEASE` no nível
da classe, como no Movidesk e no Totvs (hoje o `[Authorize]` está ativo e o `IgnoreApi` é por ação), `[Tags]` com o
nome do service (`LinxMicrovixMovements`, `LinxMicrovixB2COrders`...) e resposta `Ok("Records integrated successfully.")`.

Uma rota por método da API, `POST` (o WS é `POST`), com o nome do comando: `POST LinxMicrovixJobs/LinxMicrovix/LinxLojas`,
`.../B2CConsultaPedidos`. São 260 rotas, que entram junto com o service de cada uma.

**`partial` por service (decidido):** a classe é uma só, com a rota base e os atributos num arquivo
`LinxMicrovixController.cs`, e um arquivo por service com as rotas dele (`LinxMicrovixController.Stores.cs`,
`LinxMicrovixController.B2COrders.cs`...), cada um injetando só o seu service.

Saem as rotas `*Individual` (voltam na fase 3 no formato novo), `IntegrityLock*`, `LinxProdutosDetalhesWithZeroStock`
e `Import` (Reshop), e o campo `LinxAPIParam` montado no construtor a partir do `appsettings`.

---

## 7. Auditoria (jobs)

Hoje o `EnumJob` e o `Script002` (comentado) **não batem**:

| Método | `EnumJob` | `Script002` |
|---|---|---|
| `LinxClientesFornec` | 8004 | 8004 |
| `LinxClientesEnderecosEntrega` | 8005 | 8027 |
| `LinxGrupoLojas` | 8006 | 8005 |
| `LinxLojas` … `LinxXMLDocumentos` | 8007 … 8026 | 8006 … 8025 (deslocados em 1) |
| `LinxSetores`, `LinxUsuarios`, `LinxCfopFiscal`, `LinxRotinaOrigem` | 8027 … 8030 | 8026, 8028 … 8030 |

No script, as procedures do ERP usam `IdJob` 8000–8030, os **mesmos** ids das integrações, e o comentário da faixa
diz 8250–8499, que é a faixa das integrações B2C.

**Um app só, o 80 (decidido)**, para ERP e B2C. Os 260 jobs cabem nas faixas que o próprio `Script002` já
reservava (como o Movidesk usa 30/3000/3500):

| Faixa | Uso | Ocupação |
|---|---|---|
| 8000–8249 | Integrações ERP, um id por método, na ordem da §2.2 (`LinxGrupoLojas` = 8000, `LinxLojas` = 8001...) | 191 de 250 |
| 8250–8499 | Integrações B2C, na ordem da §2.2 (`B2CConsultaEmpresas` = 8250...) | 69 de 250 |
| 8500–8999 | Procedures (`sp_<Método>_Sync`, `IdType = 1`, `IdParent` = job de origem) | 260 de 500 |

Os ids de hoje saem do `EnumJob` (nenhum foi gravado em `auditing.Jobs`: o script nunca rodou descomentado). Os ids
são reservados agora, mas cada job só entra no banco junto com o seu service. O comentário do `EnumJob` passa a
apontar para os scripts de jobs de cada service.

**Intervalos (decidido).** Todos os jobs são recorrentes. A especificação avisa que consumo excessivo leva à
desativação da chave, e 260 jobs × N lojas é muito tráfego, então o intervalo segue o quanto o dado muda:

| Intervalo | Services |
|---|---|
| 3 min | `Movements`, `PosSales`, `Payments`, `Returns`, `Consignments`, `Orders`, `PurchaseOrders`, `FiscalDocuments`, `Loyalty`, `ServiceOrders`, `B2COrders` |
| 120 min | `Stores`, `Customers`, `Sellers`, `Products`, `Stocks`, `Prices`, `Financial`, `Optics`, `B2CStores`, `B2CCustomers`, `B2CProducts`, `B2CProductImages`, `B2CProductFields`, `B2CStocksAndPrices` |
| 1440 min (1 dia) | `ProductAttributes`, `Taxes`, `TaxSettings`, `B2CProductAttributes` |

As procedures de cada job usam o mesmo intervalo do job, como no Movidesk. Os intervalos são ajustáveis depois por um
`update` em `auditing.Jobs`, sem mudar o código.

---

## 8. Erros de comportamento encontrados (corrigir junto)

Os caminhos são relativos à pasta do módulo; `S` = service, `E` = entidade, `CH` = command handler.

| # | Onde | Problema |
|---|---|---|
| 1 | Projetos | Não compilam: `newbloomers-core-dlls` não existe; pacotes sem versão; tipos ausentes no Shared (§1) |
| 2 | `LinxMicrovixService.BuildBodyRequest` (3 sobrecargas) | `password="{UserAuthentication}"`: a senha enviada é o usuário. Funciona por coincidência, porque usuário e senha são iguais em cada API (§5.1) |
| 3 | `LinxMicrovixService.DeserializeResponseToXML` | `Parallel.For` com `List.Add` e `XmlDocument` compartilhados: linhas perdidas ou exceção. `Replace("'", "''")` em todos os valores grava aspas duplicadas no banco (o bulk não precisa de escape); `APICall` ainda remove todas as aspas da resposta (`Replace("'", "")`) |
| 4 | `LinxLojas`, `LinxMovimentoCartoes`, `LinxMovimentoPlanos`, `B2CConsultaStatus` (`GetRecords`) | Montam `sqlCnpjsEmp` e executam `GetRecords<Company?>(sql)`, a query de **parâmetros**: a lista de lojas vem vazia ou com `Doc` nulo. Os três primeiros (ERP) ainda usam as empresas do **B2C** |
| 5 | `LinxMovimentoCartoes` S:138, `LinxMovimentoPlanos` S:127, `B2CConsultaPedidosItens` S:66 | O timestamp recebe o **texto** da query (`SELECT ISNULL(MIN(TIMESTAMP), 0) ...`), que nunca é executada e vai literal no XML |
| 6 | `LinxMovimentoCartoes.GetRecord` S:96 | `CreateGetParametersQuery(jobName, identificador, cnpj_emp)` com os argumentos trocados: a query sempre falha |
| 7 | A maioria dos métodos (`LinxSetores`, `LinxPlanos`, `LinxNaturezaOperacao`, `LinxUsuarios`, `LinxProdutosTabelas`, `LinxProdutosDetalhes`, `LinxProdutosDepositos`, `LinxCfopFiscal`, `LinxRotinaOrigem`, `B2CConsultaEmpresas`, `B2CConsultaNFeSituacao`...) | `timestamp = 0` a cada execução: relê a base inteira, o que a Linx diz que leva à desativação da chave (§4.2). `LinxProdutosDetalhes` lê de 2000-01-01 até hoje sempre |
| 8 | `CreateGetLast7DaysMinTimestamp*` | Sem linha nos últimos 7 dias devolve 0 e dispara carga total. Usado com `lastupdateon` (data de gravação) em uns métodos e com campos do Linx (`dt_update`, `data_lancamento`) em outros |
| 9 | `LinxMovimento` CH:11 × E:355 | A chave do cache tem `chave_nf` só na entidade, e `data_documento` vem crua na entidade e formatada pelo `CONCAT` no SQL: a chave nunca confere e todo movimento é regravado a cada execução |
| 10 | `LinxPedidosVenda` CH:10 × E:114 | `cod_produto` e `codigo_cliente` em ordem trocada entre o SQL e a entidade: mesmo efeito do item 9 |
| 11 | `LinxProdutosPromocoes` E:48 | Chave com as strings cruas da API (preço, datas, bool) contra o `CONCAT` de colunas tipadas: nunca confere. A janela começa em **hoje** (`data_cad_inicial`), então promoções cadastradas antes nunca vêm |
| 12 | DTOs e entidades de hoje | Desatualizados em relação à API: o `LinxMovimento` não tem 6 das 116 colunas que a API devolve, e o `LinxGrupoLojas` não tem 3 das 10 (§3.2, testado). Os dados dessas colunas nunca são gravados. (A auditoria do código apontou aqui que o `B2CConsultaEmpresas` lia `"cnpjEmp"` no lugar de `cnpj_emp`; a chamada real mostrou que a coluna se chama mesmo `cnpjEmp`, então isso **não** é erro) |
| 13 | `B2CConsultaEmpresas` E:57 | `email_unidade = record.end_unidade` (cópia) |
| 14 | `B2CConsultaClientesEnderecosEntrega` E:56 | `cidade_cliente = record.cep_cliente` (entidade sem service) |
| 15 | `LinxClientesEnderecosEntrega` S:45 | Lê `"complement_end_cli"`; DTO, entidade e SQL usam `complemento_end_cli`: o complemento vem sempre nulo |
| 16 | `LinxProdutos` E:53 | `cod_comprador` nunca é atribuído; `LinxPlanos` não atribui `conta_central` |
| 17 | `LinxNaturezaOperacao` E:96 | `cod_natureza_operacao.Trim()` sem checar nulo: `NullReferenceException` derruba o lote |
| 18 | 6 entidades (§3.2) | `Guid.Parse` sem tratamento: um identificador vazio derruba o lote |
| 19 | Todas as entidades | Nulos gravados como `1990-01-01` e `0`; texto longo derruba o `SqlBulkCopy` do lote |
| 20 | `LinxLojas` S:77/83, `LinxGrupoLojas` S:41/55 | O texto de contexto do erro e o da entidade não conferem (`cnpj_emp` × `empresa`; `CNPJ` × `cnpj`): as mensagens de validação nunca são associadas ao registro |
| 21 | `LinxClientesFornec.GetRecord` S:145-179, `LinxProdutosDetalhes.GetZeroStockRecords` S:228 | Bulk em `untreated` sem `CallDbProcMerge`: os dados nunca chegam à tabela final |
| 22 | `LinxVendedores` CH:14, `LinxXMLDocumentos` CH:15, `B2CConsultaPedidosIdentificador` CH:16 | `INSERT INTO {tableName}_raw` / `{tableName}` sem schema |
| 23 | `LinxPedidosVenda.GetRecord` S:188 | Troca `[doc_origem]` (copiado de `LinxMovimentoTrocas`) e não troca `[data_alt_inicial]`/`[data_alt_fim]`, que vão literais |
| 24 | `LinxProdutosTabelas.GetRecord` S:144, `LinxProdutosInventario.GetRecord` S:108 | `[0]` não é trocado; a data informada no inventário é sobrescrita por `2000-01-01` |
| 25 | `LinxClientesFornec` CH:40 | A trava de integridade lê só `CUSTOMERCPF`; para PJ o marcador some e a consulta à API sai sem filtro, de 2000 até hoje |
| 26 | `LinxProdutosTabelasPrecos` S:92 | Filtro fixo por nome de loja da Newbloomers (`Contains("MISHA") \|\| Contains("OPEN")`): em outro cliente nenhuma tabela de preço é lida; e `NullReferenceException` se o nome vier nulo |
| 27 | `LinxProdutosInventario`, `LinxProdutosDetalhes` S:26 | `_errorMessagesList` **estática**: compartilhada entre execuções simultâneas e, no `GetZeroStockRecords`, nunca limpa |
| 28 | `LinxVendedores`, `LinxProdutosCamposAdicionais`, `B2CConsultaStatus` | Dados do portal lidos por loja e gravados repetidos no mesmo lote (sem deduplicação dentro da resposta) |
| 29 | `B2CConsultaPedidosStatus` S:122/144 | `GetRecords("")` falha sempre; testa a lista errada (`_listRegistrosConsultados` no lugar de `_listSomenteNovos`) |
| 30 | `B2CConsultaPedidosIdentificador` S:117/125 | `ToUpper()` na busca e não na inclusão no cache; `timestamp` convertido para `Int32` |
| 31 | `IntegrityLock*` (B2C, `LinxClientesFornec`, `LinxPedidosCompra`) | Montam a lista e não gravam nada; `B2CConsultaNFe` não exclui os já registrados e devolve os mesmos 500 sempre |
| 32 | Todas as cópias de `GetRecords` | Sem `finally`: uma exceção deixa o log aberto. Os `GetRecord` não abrem o log e passam ao merge o GUID de outra execução |
| 33 | `GetRegistersExists` de `LinxProdutos`, `LinxProdutosCodBar`, `LinxProdutosTabelasPrecos`, `LinxProdutosCamposAdicionais`, `LinxPlanos`, `LinxMovimento*`, `B2CConsultaEmpresas`... | `IN (...)` sem lotes, com todos os registros da resposta |
| 34 | `DependencyInjection` | Validadores registrados 36 vezes; o B2C não registra o `HttpClient` |
| 35 | `EnumJob` × `Script002` | Ids deslocados e repetidos (§7) |
| 36 | Todos os métodos | Uma chamada só por loja: os métodos com limite de 5000 registros por retorno (v274 §4.5) perdem o que passa disso (testado: o `LinxMovimento` de uma loja em um ano tem mais de 10.000 linhas) |
| 37 | `APICall.PostAsync` | Só olha o status HTTP, mas a API devolve erro com HTTP 200 e `ResponseSuccess=False` (testado); o erro só aparece depois, no `DeserializeResponseToXML`, como `APIException` que para o job. O corpo vai em `Encoding.ASCII` |
| 38 | `LinxReshopSaldo` | Licença não comercial do EPPlus (`SetNonCommercialOrganization`) em uso da empresa; valores em reais e datas lidos com `ToString()` dependente da cultura (`12.5` vira `125`; data numérica vira `1990-01-01`); sem log de auditoria nem `EnumJob`; um arquivo com erro interrompe os seguintes depois do bulk. Fica anotado para quem receber o service (§2.4) |
| 39 | `appsettings.json` | Senha do `sa` em texto na connection string (fora do módulo, mas no mesmo arquivo que o módulo vai usar) |

---

## 9. Plano em fases

Como no Totvs: **um service de cada vez**. A fase 1 deixa a base pronta; a fase 2 se repete para cada um dos 29
services, que só é ligado no host e no controller quando está pronto.

**Chaves para os testes:** as chaves de produção da Newbloomers, do ERP e do B2C, estão ativas (conferido em
26/10/2026, §4.2) e servem para conferir cada método contra a resposta real. Como a Linx desativa a chave por consumo
excessivo, os testes de cada service usam uma loja e janelas curtas, como os da §4.2, e a carga inicial completa só
roda pelo host, com os intervalos da §7.

**Fase 1 — base:**

* `.csproj` como na §2.1 (Shared, versões, sem `Compile Remove` do Domain, sem EPPlus e Blob);
* código antigo que não compila fica **fora do build** (`<Compile Remove>` com comentário, como no Totvs) e volta
  método a método; `IAPICall`/`APICall`, `LinxAPIParam`, `LinxMethods`, `IntegrityLockTablesRegister`, os helpers de
  `Domain/CustomValidations` e as duas classes base são apagados; `LinxReshopSaldo` sai do módulo;
* `ILinxMicrovixService`/`LinxMicrovixService` (§4.1: lote, clientes, lojas e timestamp),
  `Entities/Parameters/Company`, `ILinxMicrovixRepository`, `LinxMicrovixCommandHandler`;
* `DependencyInjection` com o cliente `"LinxMicrovixAPI"` e uma chamada de validadores;
* scripts `Script001` a `Script003` (§5.4); `EnumJob` só com `None` e o comentário novo;
* host: referências, `AddScopedLinxMicrovixServices`, `appsettings`, controller `partial` só com a rota base; jobs
  recorrentes do Hangfire e controllers antigos apagados;
* `dotnet build` do host Hangfire com 0 erros.

**Fase 1 implementada (26/10/2026):**

* `.csproj` como na §2.1: referências para `..\..\..\..\Shared\*.Shared`, `Microsoft.Extensions.Caching.Memory`
  **10.0.8** na Application, sem `EPPlus` nem `Azure.Storage.Blobs`;
* o código antigo que não compila ficou **fora do build** (`<Compile Remove>`, com comentário em cada `.csproj`): as
  pastas `LinxCommerce/` e `LinxMicrovix/` de `Abstractions`, `Dtos`, `Entities`, `Handlers`, `Services`, `Validations` e
  `Repositorys`, e o `CustomValidations/` da Application (ponto de partida dos validadores novos). Cada service que
  entrar na fase 2 apaga os arquivos dele e tira a linha do `.csproj`;
* apagados: `IAPICall`/`APICall`, `LinxAPIParam`, `LinxMethods`, `IntegrityLockTablesRegister`,
  `Entities/Base/Company`, os 6 helpers de `Domain/CustomValidations`, `LinxMicrovixBaseSyncService`,
  `B2CBaseSyncService`, o `LinxReshopSaldo` (service, repositório, interfaces e entidade; recuperável pelo histórico do
  git para o destino que for definido) e os scripts `001`/`002` antigos;
* novos: `ILinxMicrovixService`/`LinxMicrovixService` (`GetBatchTime`, `GetCompanys` com cache
  `LinxMicrovix:Parameters`, `GetStores(company, api)`, `GetLastTimestamp(company, método, cnpj_emp)`, com o CNPJ
  validado por `^\d{14}$` antes de entrar nas queries), `Entities/Parameters/Company`, `Enums/EnumApi` (`Erp`, `B2C`),
  `ILinxMicrovixRepository`/`LinxMicrovixRepository` e `ILinxMicrovixCommandHandler`/`LinxMicrovixCommandHandler`
  (`CreateGetCompanysQuery`, `CreateGetStoresQuery`, `CreateGetLastTimestampQuery`). As queries de lojas e de timestamp
  já apontam para `linxmicrovix.LinxGrupoLojas`, `linxmicrovix.B2CConsultaEmpresas` e `linxmicrovix.<Método>`, tabelas
  que entram com os services da fase 2;
* `EnumJob` só com `None` e o comentário novo (faixas da §7);
* `DependencyInjection` em `Infrastructure.LinxMicrovix.Outbound.DependencyInjection`, com o cliente `"LinxMicrovixAPI"`
  (`LinxMicrovix:BaseUrl`, timeout de 20 min), os registros acima e uma chamada só a `AddValidatorsFromAssemblyContaining`;
* scripts: `Script001` (schema `linxmicrovix` + `LinxMicrovixParameters`), `Script002` (app 80) e o modelo
  `Script003_..._Table.sql.example`; o `Script003` real, com as chaves da Newbloomers, fica só na máquina local
  (`.gitignore`);
* host: referências no `Hangfire.csproj`, `AddScopedLinxMicrovixServices(builder.Configuration)` ligado,
  `LinxMicrovix:BaseUrl` preenchido e `B2CLinxMicrovix` removido do `appsettings.json`, `LinxMicrovixController`
  `partial` só com a rota base (`LinxMicrovixJobs/LinxMicrovix`, `[Authorize]` comentado). Os dois controllers antigos e
  as duas extensões de jobs recorrentes do Hangfire foram apagados;
* `dotnet build` do host Hangfire: **0 erros**; nenhum aviso de código nos arquivos do módulo (os avisos NU1902/NU1903
  do `System.Security.Cryptography.Pkcs` chegam pelo `Infrastructure.Shared`, como no Totvs);
* **não verificado:** os scripts não rodaram em banco, e a leitura de `LinxMicrovixParameters` não rodou pelo host.
  O `appsettings.Development.json` com a URL do portal de teste não foi criado: o host ainda não tem esse arquivo, e
  criá-lo só com a URL de teste faria o ambiente de desenvolvimento chamar o portal de teste com as chaves de produção.

**Fase 2 — um service por vez.** A ordem segue as dependências da §4.2: primeiro o que fornece lista ou cadastro
para os outros, depois o volume:

| Ordem | Services | Por quê |
|---|---|---|
| 1 | `Stores` | As lojas, usadas por todos, e o catálogo do `LinxMetodos`, referência dos parâmetros |
| 2 | `B2CStores` | `B2CConsultaEmpresas`, as lojas do B2C |
| 3 | `ProductAttributes`, `Taxes`, `TaxSettings` | Cadastros pequenos, sem datas obrigatórias, referenciados pelos produtos e pelo movimento |
| 4 | `Products`, `Stocks`, `Prices` | `LinxProdutosDepositos` e `LinxProdutosTabelas` antes dos que dependem deles |
| 5 | `Customers`, `Sellers`, `Payments`, `Loyalty` | Cadastros usados pelo movimento |
| 6 | `Movements`, `PosSales`, `Returns`, `Consignments`, `Orders`, `PurchaseOrders` | O maior volume: páginas de 5000 pelo timestamp (`filtra_somente_timestamp`) ou fatias de datas, por loja |
| 7 | `FiscalDocuments`, `Financial`, `ServiceOrders`, `Optics` | Restante do ERP |
| 8 | `B2CCustomers`, `B2CProductAttributes`, `B2CProducts`, `B2CProductImages`, `B2CProductFields`, `B2CStocksAndPrices`, `B2COrders` | Restante do B2C |

Cada service traz: DTOs, entidades e validadores nas pastas novas (partindo de `CustomValidations/` quando houver);
repositório e queries; o trio de scripts (tabelas, procedures, jobs); o arquivo `partial` do controller com as rotas;
os registros no DI; o código antigo do método apagado (e as linhas `<Compile Remove>` dele); a classificação
loja/portal de cada método e as chaves conferidas na §5.2; a conferência com um console descartável contra a API
real (lojas, mensagens de validação, chaves repetidas, paginação pelo timestamp, colunas da `DataTable` contra o
script).

**Fase 2, service 1: `LinxMicrovixStoresService` implementado (27/10/2026):**

* 6 métodos, com o nome da API (`LinxGrupoLojas`, `LinxLojas`, `LinxLojasParametros`, `LinxUsuarios`, `LinxMetodos`,
  `LinxEntrantes`), cada um com o fluxo da §4.2 escrito no próprio método: log, clientes com chave do ERP, lojas de
  `LinxGrupoLojas` (os métodos de loja), timestamp do banco, envelope XML montado com `XElement` (valores escapados),
  `POST` na rota com o nome do método em UTF-8 / `application/xml`, uma nova tentativa em 502/503/504, resposta tratada
  **pelo corpo** (`ResponseSuccess`), páginas pelo maior timestamp **até a resposta vir vazia**, validação, gravação,
  `auditing.Records` com a linha reconstruída com o nome de cada coluna (`<R><portal>15133</portal>...</R>`, porque o
  `<R>` da API só tem `<D>` sem nome) e `finally` no log. Leitura com erro não grava nada daquela loja;
* sem timestamp: `LinxGrupoLojas` (relido inteiro, chave `portal` + `empresa`) e `LinxMetodos` (sem parâmetro nenhum;
  o retorno "Nome - descrição (parâmetros)" é separado em `metodo`, `descricao` e `parametros`). Métodos de portal
  (sem `cnpjEmp`, uma leitura por cliente): `LinxUsuarios` e `LinxEntrantes`. Métodos de loja (um por CNPJ de
  `LinxGrupoLojas`): `LinxLojas` e `LinxLojasParametros`;
* DTOs em `Dtos/Reads/Erp/Stores` com as colunas da resposta real (inclusive as que a v274 não cita:
  `emp_mod_linx_promo`, `subrede`, `produto_disponivel_empresa`, `id_grupo`, `entrantes`); entidades em
  `Entities/Reads/Erp/Stores` com `doc_company`, horário do lote e anuláveis; validadores em `Validations/Erp/Stores`;
* novos conversores do módulo (`Domain/Extensions/MicrovixConverters`: `BIT` como `True`/`False` ou `0`/`1`, `Int64`,
  decimal e data em cultura invariante, texto cortado no tamanho da coluna) e a regra `MustBeValidMicrovixBoolean`, porque o
  `ToNullableBoolean` e o `MustBeValidBoolean` do Shared não aceitam `0`/`1`;
* `ILinxMicrovixStoresRepository`/`LinxMicrovixStoresRepository` (um bulk em `untreated` e uma merge por método);
* scripts `Script004` (as 6 tabelas e PKs), `Script005` (procedures, sem condição de exclusão) e `Script006` (jobs
  8000–8005 e procedures 8500–8505, 120 min); `EnumJob` com os 6 jobs;
* rotas `POST LinxMicrovixJobs/LinxMicrovix/<Método>` (tag `LinxMicrovixStores`) em `LinxMicrovixController.Stores.cs`,
  com o service injetado na ação (`[FromServices]`), já que cada arquivo da classe `partial` injeta só o seu; registros
  no DI;
* o código antigo de `LinxGrupoLojas`, `LinxLojas` e `LinxUsuarios` (service, repositório, handler, interfaces, DTO,
  entidade e validador) e os modelos de `CustomValidations` dos 5 que tinham (o `LinxEntrantes` não tinha) foram
  **apagados**;
* conferido com um console descartável contra a API real (sem banco), com as 3 primeiras lojas de CNPJ:
  `LinxGrupoLojas` 21 lojas (1 sem CNPJ), `LinxMetodos` 204, `LinxUsuarios` 188 em duas páginas (limite de 100 por
  chamada), `LinxEntrantes` 0 (sem dados nos últimos 2 anos), `LinxLojas` e `LinxLojasParametros` 3 cada; 0 mensagens
  de validação, nenhuma chave repetida, e as colunas de cada entidade iguais às do `Script004`;
* **não verificado:** os scripts e as procedures não rodaram em banco, e os métodos não rodaram pelo host (bulk,
  merges, `auditing` e o `MAX(timestamp)` dependem do banco).

**Fase 2, services 2 a 29 implementados (27/10/2026):**

* os 28 services restantes (254 métodos: 185 do ERP e 69 do B2C), no padrão aprovado do service 1, com DTOs,
  entidades, validadores, repositórios, scripts `Script007` a `Script090` (um trio por service, na ordem da §9), jobs
  8006–8190 (ERP) e 8250–8318 (B2C), procedures 8506–8759, rotas em `LinxMicrovixController.<Service>.cs` e registros
  no DI. Os ids de job seguem a ordem da §2.2;
* o código foi gerado por script a partir de: colunas da resposta real (uma chamada por método com timestamp alto,
  que devolve o `<C>` sem linhas), tipos e tamanhos da v274/V64 conferidos com uma página de amostra (tipo alargado
  quando a amostra não cabe), parâmetros do `LinxMetodos`/V64 e chaves escolhidas pela amostra e revisadas método a
  método. O script ficou fora do repositório; o código gerado é igual ao escrito à mão no service 1;
* **escopo testado com duas lojas**: um método só lê a primeira loja (dado do portal, sem `cnpj_emp` na chave) quando
  duas lojas devolveram os mesmos registros (115 métodos). Os outros 138 leem todas as lojas, com `cnpj_emp` na chave
  (7 deles não trazem a loja na resposta, mas o resultado muda por loja, como `LinxMovimentoPrincipal`);
* **métodos que exigem um valor de outro por chamada** (`GetSourceValues` e timestamp por valor, novos no service
  comum): `id_config_tributaria` (`LinxConfiguracoesTributariasDetalhes`), `cod_deposito`
  (`LinxProdutosDetalhesDepositos`), `id_tabela` (`LinxProdutosTabelasPrecos`); e, incrementais (só os valores gravados
  depois da última leitura), `cod_pedido` (`LinxPlanosPedidoVenda`), `identificador_remessa`
  (`LinxRemessasIdentificadores`) e `codigoproduto` (`B2CConsultaImagensHD`). Na primeira execução estes três fazem
  uma chamada por pedido, remessa ou produto;
* parâmetros fixos: `promocao_ativa` lido com `S` e `N`, gravados juntos (`LinxProdutosPromocoes`); `data_inventario`
  = hoje (`LinxProdutosInventario`); `cnpjPortal = N` (`LinxReducoesZ`); `cod_cc = 0` e `cod_conta = 0`
  (`LinxLancContabil`, sem dados para confirmar que 0 traz todos); só o período de emissão no `LinxFaturas` (a API
  recusa os dois períodos);
* achados que entraram no código:
  * **erro de parâmetro com `ResponseSuccess = True`**: `<C />` vazio e a mensagem numa linha (`LinxFaturas`). Todos os
    métodos, inclusive o service 1, tratam isso como erro;
  * **o B2C não tem limite de linhas**: com timestamp 0 o `B2CConsultaPedidosStatus` trouxe 408.663 linhas numa
    chamada (99 s). `B2CConsultaNFe` (XML das notas) e `B2CConsultaImagens` (as imagens) têm respostas de centenas de
    MB na carga inicial: risco de memória a acompanhar na primeira execução pelo host;
  * **colunas repetidas na resposta** (`valor` no `LinxPagamentoParcialDav`, `portal` no `LinxDevolucaoRemanejoFabrica`,
    `codigo_gerencial` no `LinxDadosOpticosDav`): a segunda vira `<nome>_2`;
  * **parte da chave vazia** (ex.: `id_campanha` vazio em todas as promoções): vira o valor padrão (0, texto vazio ou
    1900-01-01); só o registro sem nenhuma parte da chave é descartado;
  * métodos sem identificador na API (`LinxComissoesVendedores`, `LinxListaDaVez`, `LinxMovimentoAcoesPromocionais`):
    chave pelas colunas do registro, e nos dois últimos o `timestamp` entra na chave (cada versão vira uma linha);
* o código antigo restante (pastas `LinxCommerce/`, `LinxMicrovix/` e `CustomValidations/`) foi apagado e os
  `<Compile Remove>` saíram dos `.csproj`; os validadores novos vieram da v274/V64, não de `CustomValidations`;
* `dotnet build` do host Hangfire: **0 erros**. No módulo, só dois avisos `CS0162` (código inacessível), no
  `LinxMicrovixB2CStoresService` e no `LinxMicrovixConsignmentsService`: métodos sem timestamp na resposta, em que o
  laço de páginas sempre sai na primeira volta e o `page++` nunca roda. Na primeira versão desta seção constava
  "nenhum aviso", porque o build tinha sido incremental;
* conferido com um console descartável contra a API real (sem banco), uma loja por API, a primeira página real de
  cada método: 252 métodos sem exceção, 371.686 linhas, nenhuma chave repetida, colunas de todas as entidades iguais
  às dos scripts. Mensagens só onde esperado: 7 métodos B2C sem acesso pela chave da Newbloomers e o `LinxClientesRede`
  (erro de parâmetro com todos os parâmetros da v274). `B2CConsultaImagens` e `B2CConsultaNFe` ficaram fora pelo
  tamanho da resposta;
* **não verificado:** scripts, procedures e execução pelo host (bulk, merges, `auditing`, `MAX(timestamp)` e
  `GetSourceValues` dependem do banco); os 10 métodos sem resposta real (colunas da documentação) e as chaves marcadas
  como "a confirmar" nos scripts.

**Fase 3 — sob demanda: feita (27/10/2026).** `linxmicrovix.LinxMicrovixRequests` e as rotas por chave, como no
Totvs (§4.1, §5.5):

* **24 métodos** (19 ERP e 5 B2C), os que tinham versão "individual" no código antigo (§4.1). Cada um fica no
  service do método recorrente, com o nome `<Método>By<Parâmetro>`; o `LinxProdutosTabelasPrecos` tem duas partes
  de chave:

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
  | `LinxProdutosDepositosByCodDeposito` | `LinxProdutosDepositos/{docCompany}/{codDeposito}` | `cod_deposito` (a rota antiga usava `cod_produto`, que o método não aceita) | 8201 |
  | `LinxProdutosInventarioByCodProduto` | `LinxProdutosInventario/{docCompany}/{cnpjEmp}/{codProduto}` | `cod_produto` | 8202 |
  | `LinxProdutosDetalhesByCodProduto` | `LinxProdutosDetalhes/{docCompany}/{cnpjEmp}/{codProduto}` | `cod_produto` | 8203 |
  | `LinxProdutosCamposAdicionaisByCodProduto` | `LinxProdutosCamposAdicionais/{docCompany}/{cnpjEmp}/{codProduto}` | `cod_produto` | 8204 |
  | `LinxProdutosByCodProduto` | `LinxProdutos/{docCompany}/{codProduto}` | `cod_produto` | 8205 |
  | `LinxPedidosVendaByCodPedido` | `LinxPedidosVenda/{docCompany}/{cnpjEmp}/{codPedido}` | `doc_origem` (= `cod_pedido`, testado) | 8206 |
  | `LinxSetoresByIdSetor` | `LinxSetores/{docCompany}/{idSetor}` | `id_setor` | 8207 |
  | `LinxXMLDocumentosByDocumento` | `LinxXMLDocumentos/{docCompany}/{cnpjEmp}/{documento}` | `documento` | 8208 |
  | `LinxVendedoresByCodVendedor` | `LinxVendedores/{docCompany}/{cnpjEmp}/{codVendedor}` | `cod_vendedor` | 8209 |
  | `B2CConsultaClientesByDocCliente` | `B2CConsultaClientes/{docCompany}/{docCliente}` | `doc_cliente` | 8319 |
  | `B2CConsultaNFeByIdPedido` | `B2CConsultaNFe/{docCompany}/{cnpjEmp}/{idPedido}` | `id_pedido` | 8320 |
  | `B2CConsultaPedidosByIdPedido` | `B2CConsultaPedidos/{docCompany}/{idPedido}` | `id_pedido` | 8321 |
  | `B2CConsultaPedidosItensByIdPedido` | `B2CConsultaPedidosItens/{docCompany}/{idPedido}` | `id_pedido` | 8322 |
  | `B2CConsultaPedidosStatusByIdPedido` | `B2CConsultaPedidosStatus/{docCompany}/{idPedido}` | `id_pedido` | 8323 |

* **rotas**: `{cnpjEmp}` só nos métodos de loja; nos de portal a leitura usa a primeira loja do cliente, como no
  recorrente. Cabeçalho `X-User-Name` obrigatório, `docCompany` e `cnpjEmp` por `^\d{14}$`. Respostas: 200
  (gravado), 404 (a API respondeu sem linhas), 422 (cliente fora de `LinxMicrovixParameters` ou sem a chave daquela
  API, loja que não é do cliente, erro da API);
* **fluxo** (escrito em cada método, como o `GetCustomerByCode` do Totvs): `GetCompany`, conferência da loja em
  `GetStores`, chamada com `timestamp = 0`, as datas do recorrente e o parâmetro da busca, páginas até a resposta vir
  vazia, gravação pelo mesmo `Save<Método>` do recorrente e registro em `LinxMicrovixRequests`. O corpo vai em
  `request_body` com a chave trocada por `(oculta)`. A exceção de rede também é registrada e depois relançada;
* **achado**: com `filtra_somente_timestamp = 1` a API ignora também o filtro da busca. Testado no `LinxMovimento`
  com `documento = 1`: com 1, 5.000 linhas de vários documentos em 112 s; com 0, as 47 linhas do documento 1 em 1 s.
  Os três métodos que enviam esse parâmetro (`LinxMovimento`, `LinxPedidosVenda`, `LinxProdutosCamposAdicionais`)
  mandam 0 na busca sob demanda, e as datas de 2000-01-01 até hoje cobrem o histórico. O recorrente continua com 1;
* novos no service comum: `GetCompany(doc_company)` e `SaveRequest(...)`. No repositório comum entrou
  `InsertIntoLinxMicrovixRequests`, com bulk direto no schema `linxmicrovix`. A entidade nova é
  `Entities/Writes/Requests/Request`, igual à do Totvs;
* scripts: `Script091_Create_LinxMicrovix_Requests_Table.sql`, com a tabela e dois índices no formato do
  `TotvsRequests`, e `Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql`, com 24 jobs `IdType = 2` e
  intervalo 0 em `EnumJob`;
* `dotnet build` do host Hangfire: **0 erros**, sem avisos novos;
* **conferido contra a API real** (console descartável, sem banco): para cada método, uma chave real veio da
  primeira página do recorrente e outra inexistente foi inventada. O `B2CConsultaNFe` usou um `id_pedido` do
  `B2CConsultaPedidos` e o `LinxProdutosTabelasPrecos` usou a tabela 1, porque a tabela 5 está vazia. Resultado nos
  24:
  * a chave real devolveu `true`, e todas as linhas eram dessa chave;
  * a chave inexistente devolveu `false`;
  * o registro da chamada saiu sem a chave.

  Os `identificador` (`LinxMovimentoCartoes`, `LinxMovimentoPlanos`) são GUID, e um valor que não é GUID volta como
  erro da API, ou seja, 422;
* **não verificado:** a gravação no banco (`LinxMicrovixRequests`, merges) e as rotas pelo host.

**Fase 4 — documentação: feita (27/10/2026).** No modelo do README e do `Responsed Jsons` do Totvs:

* **`README.md` do módulo** (`LinxMicrovix - Outbound Web Service/README.md`). Traz:
  * os conceitos (portal, chave, loja, timestamp) e os comportamentos da API conferidos (§4.2 e fases 2 e 3);
  * a arquitetura e as convenções;
  * o service comum;
  * uma tabela por service, com escopo, parâmetros, chave (PK do script), job e procedure de cada um dos 260
    métodos;
  * a ordem de dependência (lojas primeiro; os 6 métodos que leem valores de outro);
  * os fluxos recorrente e sob demanda, com as 24 rotas;
  * os scripts, os endpoints, a auditoria, os intervalos, a configuração e as pendências.

  As tabelas foram geradas do código (spec dos services, PKs e jobs lidos dos scripts), para não divergirem dele;
* **pasta `Responsed XMLs`**: `Responsed XMLs/<Service>/<Método>/xml-returned-by-api-end-point.xml`, com a primeira
  página real de cada um dos 260 métodos, cortada em 2 linhas e com os dados trocados por dados fictícios (regras no
  README):
  * 183 com linhas;
  * 69 vazias, porque o cliente de teste não tem dados no método e o arquivo mostra só as colunas;
  * 8 com a mensagem de erro da API: os 8 métodos da pendência 1 da §10.2.

  O `B2CConsultaNFe` veio da busca sob demanda (`id_pedido`). O `B2CConsultaImagens` veio de um `id_imagem` do
  `B2CConsultaProdutosImagens`, para não trazer as imagens inteiras. O `LinxMetodos` ficou como veio, porque é o
  catálogo público da API;
* **conferência dos dados**:
  * nenhuma chave nem CNPJ do cliente de teste nos arquivos;
  * nenhum valor trocado ficou na saída;
  * nenhum e-mail real;
  * só datas, booleanos e o `timestamp` mantêm o valor original.

  As respostas reais ficaram só fora do repositório e foram apagadas depois da troca.

---

## 10. Decisões

### 10.1 Tomadas (26/10/2026)

| # | Decisão |
|---|---|
| 1 | **Integração recorrente**, como no Totvs e no Movidesk, e não carga histórica sob demanda: o ERP está descontinuado na Newbloomers, mas o módulo tem de funcionar em outro cliente que use o Microvix. A primeira execução de cada método (`timestamp = 0`) traz o histórico (§4.2) |
| 2 | **Todos os métodos de leitura** das versões mais recentes dos documentos: v274 (191) e V64 (69), 260 no total (§2.2) |
| 3 | **Services agrupados por assunto**, com os 29 services e os nomes da §2.2 |
| 4 | **Nada fixo de cliente no código**: uma linha de `LinxMicrovixParameters` por cliente, com as credenciais de cada API (§5.1) |
| 5 | Os 11 métodos de escrita do B2C ficam para o projeto **Inbound** |
| 6 | **Tudo à mão em cada método**, como no Totvs: envelope, chamada, repetição pelo timestamp, leitura do XML, validação e gravação no próprio método; no comum, só lote, clientes, lojas e timestamp (§4.1) |
| 7 | **App 80**, um módulo só, para ERP e B2C (§7) |
| 8 | **`LinxReshopSaldo` sai do Outbound**; o destino é definido com quem usa o saldo Reshop, fora desta proposta (§2.4) |
| 9 | **A integração do Oto não é tocada** nesta refatoração; o foco é só o Microvix (§5.0) |
| 10 | **As pastas com espaço no nome ficam** (`LinxMicrovix - Outbound Web Service`) (§2.1) |
| 11 | **Controller `partial` por service** (§6.4) |
| 12 | **Nome do método = nome do comando da API**, sem `Get` (`LinxClientesEnderecosEntrega()`); tabela, job e rota com o mesmo nome (§2.3) |
| 13 | **Intervalos da tabela da §7**, ajustáveis em `auditing.Jobs` |
| 14 | **Docs do DW alinhados com a decisão 1**: o histórico continua vindo na primeira execução, e o módulo segue recorrente (`docs/datawarehouse/01-visao-e-premissas.md` §1.2, item 7, e `10-perguntas-em-aberto.md`, pergunta 13) |
| 15 | **Chaves de teste**: as de produção da Newbloomers (ERP e B2C), ativas em 26/10/2026; só no script de valores fora do git (§5.1, §9) |
| 16 | **`<IdPortal>` não é enviado**: sem efeito nas chamadas reais; o filtro de portal do `LinxGrupoLojas` é o parâmetro `portal` (§4.2) |
| 17 | **Agrupamento dos 52 métodos novos** da v274/V64 aprovado como está na §2.2 (`PosSales`, `PurchaseOrders`, `B2CProductImages`, `B2CProductFields` e os demais) |

| 18 | **Os 13 métodos do `LinxMetodos` que a v274 não descreve ficam fora por enquanto**: `LinxBalanco`, `LinxClientesFornecClasseFiscalRegra`, `LinxGestaoFiscalAliquotaMunicipio`, `LinxGestaoFiscalAliquotaUf`, `LinxGestaoFiscalAliquotaUniao`, `LinxGestaoFiscalClassificacaoTributaria`, `LinxGestaoFiscalConfigTributaria`, `LinxGestaoFiscalConfigTributariaCfop`, `LinxGestaoFiscalConfigTributariaEmpresa`, `LinxGestaoFiscalConfigTributariaNcm`, `LinxGestaoFiscalConfigTributariaProduto`, `LinxLogs`, `LinxParceiroHabilitaChave`. Entram quando a Linx os documentar (os 9 `LinxGestaoFiscal*` iriam para `TaxSettings`) |

### 10.2 Pendentes

| # | Pendência | Recomendação |
|---|---|---|
| 1 | 8 métodos sem resposta real: 7 do B2C que a chave da Newbloomers não tem liberados (`B2CConsultaClientesContatos`, `B2CConsultaProdutosImagensURL` e os 5 `B2CConsultaB2CProdutosCamposAdicionais*`) e o `LinxClientesRede`, que recusa os parâmetros da v274 | Conferir colunas e chaves quando houver uma chave com acesso ou a Linx esclarecer os parâmetros |
| 2 | Memória na carga inicial de `B2CConsultaNFe` e `B2CConsultaImagens` (respostas de centenas de MB) | Acompanhar a primeira execução pelo host; se necessário, ler o `B2CConsultaNFe` em fatias de `data_inicial`/`data_fim` |

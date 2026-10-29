# DW do NEWBLOOMERS · 08. Auditoria, dado pessoal e acesso

> Guia 8 de 12 do guia do DW, versão 7. Seções deste arquivo: 3.9 e 12. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 3. Decisões técnicas (continuação)

### 3.9 Auditoria e retenção

- **Auditoria:** cada execução grava início e fim com o contrato `auditing.sp_Log_*` (1.3), e uma mensagem por procedure (9.4).
- **Retenção:** `dbo.sp_Delete_Auditing_Logs` apaga mensagens, registros e logs com mais de 15 dias, e é chamado pela rotina de manutenção. Por isso o resultado das checagens do DW vai para `dw_control.CheckResult`, que não entra nessa limpeza. A tabela cresce pouco (uma linha por checagem por execução), então fica sem limpeza automática por enquanto.
- **Auditoria dos envios aos SaaS:** as tabelas `*Requests` dos cinco módulos (1.2, item 10) não entram nessa limpeza e, com `integrated_or_updated_at`, viram `FactApiRequest` na fase 4 (7.6): quem, quando, o quê e em qual SaaS.

---

## 12. Dado pessoal e acesso

### 12.1 Dados pessoais identificados

| Fonte | Dado pessoal | Tratamento no DW |
|---|---|---|
| `totvs.TotvsPeople`, `totvs.TotvsLegalEntities` | Nome, CPF/CNPJ, RG, filiação, data de nascimento | CPF/CNPJ como hash com pepper e mascarado em `DimCustomer`; data de nascimento vira `age_range`; em claro só em `dw_personal_data` |
| `totvs.TotvsPeopleAddresses`, `TotvsPeopleEmails`, `TotvsPeoplePhones` e as equivalentes de `TotvsLegalEntities` | Endereço, e-mail, telefone | Em claro só em `dw_personal_data` (registro padrão de cada uma); `DimCustomer` fica com a UF |
| `linxmicrovix.LinxClientesFornec` | Nome, CPF/CNPJ, data de nascimento, endereço, telefone, e-mail (54 colunas) | Mesmo tratamento: hash e máscara em `DimCustomer`, membro `linxmicrovix`; em claro só em `dw_personal_data` |
| `linxmicrovix.LinxVendedores` | `cpf_vendedor`, endereço, telefone, e-mail | Não entram na landing; `DimSeller` fica com nome, tipo e situação |
| `linxcommerce.LinxCommerceCustomers`; colunas `Customer*` de `LinxCommerceOrders` | Nome, CPF/CNPJ, e-mail, telefone, data de nascimento | Mesmo tratamento; as colunas de cliente do pedido não entram na landing do pedido |
| `totvs.TotvsCommissionsPaid`, `TotvsDuplicates`, `TotvsPurchaseOrders` (fase 4) | `commissionedCpfCnpj`, `supplierCpfCnpj`, `buyerCpfCnpj` | Hash com pepper nos fatos; em claro só em `dw_personal_data` |
| As cinco tabelas `*Requests` (fase 4) | `user_name` (quem chamou a API) e, no corpo, o dado que foi enviado | `FactApiRequest` leva `user_name`, que é identificação de funcionário, não de cliente; `request_body` e `response_body` não entram no DW |
| `movidesk.MovideskPeopleEmails` | E-mail | Fora do DW de vendas; na fase 2, só hash |
| `aftersale.AfterSaleReversesCustomers` | Nome e endereço | Hash e mascaramento |

Nenhuma dessas colunas entra em `dw_landing` nem em `dw_history` (5.2, regra 6): a landing de pessoas, empresas, clientes e vendedores recebe só a chave e os atributos não identificadores, e as tabelas de endereço, e-mail e telefone não têm landing. O dado identificado existe no DW num lugar só, carregado direto das tabelas de origem (10.1, passo 5). O `doc_hash` com pepper é igual para o mesmo documento em qualquer ERP, e é por ele que o cliente do legado Linx se liga ao cliente da Totvs (7.5), sem o documento em claro. As únicas saídas do dado em claro são `dw_outbound.OtoCustomers`, porque o Oto recebe nome, e-mail e telefone, e `dw_outbound.LogisticsShipments`, porque a transportadora recebe nome, endereço e telefone do destinatário (4.2).

**Premissa:** não haverá pedido de exclusão nem de anonimização de dado pessoal no DW. Por isso não existe política de retenção nem procedure de anonimização para `dw_personal_data`: a tabela guarda a identificação atual de todos os clientes carregados, enquanto o banco existir.

### 12.2 Acesso por schema

Os `GRANT` são por schema, criados por script do módulo. Grupos, não usuários no banco:

- `dw_leitura`: `SELECT` em `datamart` e `datawarehouse`, sem `dw_personal_data` e sem nenhum schema dos módulos de integração.
- `dw_pii`: leitura em `dw_personal_data`. Concedido só a quem precisa, com registro.
- `dw_engenharia`: escrita em `dw_landing`, `dw_history`, `dw_staging`, `dw_intermediate`, `datawarehouse`, `datamart`, `dw_reference`, `dw_control` e `dw_outbound`. Sem `dw_personal_data`: como esses schemas não têm coluna identificadora (5.2, regra 6), quem mantém o DW não vê dado pessoal em claro.
- `dw_admin`: DDL. Uso restrito.
- A identidade que roda a carga é a única com escrita em `dw_personal_data` e com leitura nos schemas de origem (`totvs`, `linxmicrovix`, `linxcommerce`, `movidesk`, `aftersale`).
- **Consumidores (4.2):** cada módulo de saída tem identidade própria, com `SELECT` só nas views de `dw_outbound` que ele usa e `EXECUTE` em `dw_control.sp_Watermark_Get` e `dw_control.sp_Watermark_Set`, que leem e gravam o watermark dele (nunca leitura ou escrita direta em `dw_control`). Só as identidades do Oto (`dw_outbound.OtoCustomers`) e dos módulos de logística (`dw_outbound.LogisticsShipments`) recebem `SELECT` nas views que leem `dw_personal_data`; elas entram no grupo `dw_pii` com registro.

Dynamic Data Masking como camada extra nas colunas sensíveis.

### 12.3 Pepper do hash

O `doc_hash` de `DimCustomer` é `SHA-256(pepper + CPF/CNPJ)` (8.3). O pepper não existe em tabela, script ou log: chega à procedure `datawarehouse.sp_DimCustomer_Load` como parâmetro, fornecido por quem a chama. Onde ele fica guardado é decisão fora deste guia.

**Onde calcular o hash (pergunta 32, decidida): na procedure `sp_DimCustomer_Load`, com o pepper como parâmetro.** Risco conhecido e aceito: como parâmetro de procedure, o pepper pode aparecer no plano em cache e no Query Store. A alternativa descartada era calcular o hash fora do banco e a procedure receber só o resultado, o que exigiria uma tabela de apoio. O mesmo pepper e a mesma procedure valem para os documentos do Linx e para os CPF/CNPJ da fase 4 (comissionados, fornecedores e compradores), para que o hash seja comparável em todo o DW.

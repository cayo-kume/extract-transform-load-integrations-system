# DW do NEWBLOOMERS · 08. Auditoria, dado pessoal e acesso

> Guia 8 de 11 do guia do DW, versão 5. Seções deste arquivo: 3.9 e 12. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 3. Decisões técnicas (continuação)

### 3.9 Auditoria e retenção

- **Auditoria:** cada execução grava início e fim com o contrato `auditing.sp_Log_*` (1.3), e uma mensagem por procedure (9.4).
- **Retenção:** `dbo.sp_Delete_Auditing_Logs` apaga mensagens, registros e logs com mais de 15 dias, e é chamado pela rotina de manutenção. Por isso o resultado das checagens do DW vai para `dw_control.check_result`, que não entra nessa limpeza. A tabela cresce pouco (uma linha por checagem por execução), então fica sem limpeza automática por enquanto.

---

## 12. Dado pessoal e acesso

### 12.1 Dados pessoais identificados

| Fonte | Dado pessoal | Tratamento no DW |
|---|---|---|
| `totvs.TotvsPeople`, `totvs.TotvsLegalEntities` | Nome, CPF/CNPJ, RG, filiação, data de nascimento | CPF/CNPJ como hash com pepper e mascarado em `dim_cliente`; data de nascimento vira `faixa_etaria`; em claro só em `dw_personal_data` |
| `totvs.TotvsPeopleAddresses`, `TotvsPeopleEmails`, `TotvsPeoplePhones` e as equivalentes de `TotvsLegalEntities` | Endereço, e-mail, telefone | Em claro só em `dw_personal_data` (registro padrão de cada uma); `dim_cliente` fica com a UF |
| `movidesk.MovideskPeopleEmails` | E-mail | Fora do DW de vendas; na fase 2, só hash |
| `aftersale.AfterSaleReversesCustomers` | Nome e endereço | Hash e mascaramento |

Nenhuma dessas colunas entra em `dw_landing` nem em `dw_history` (5.2, regra 6): a landing de pessoas e empresas recebe só a chave e os atributos não identificadores, e as tabelas de endereço, e-mail e telefone não têm landing. O dado identificado existe no DW num lugar só, carregado direto das tabelas `totvs` (10.1, passo 5).

**Premissa:** não haverá pedido de exclusão nem de anonimização de dado pessoal no DW. Por isso não existe política de retenção nem procedure de anonimização para `dw_personal_data`: a tabela guarda a identificação atual de todos os clientes carregados, enquanto o banco existir.

### 12.2 Acesso por schema

Os `GRANT` são por schema, criados por script do módulo. Grupos, não usuários no banco:

- `dw_leitura`: `SELECT` em `datamart` e `datawarehouse`, sem `dw_personal_data` e sem nenhum schema dos módulos de integração.
- `dw_pii`: leitura em `dw_personal_data`. Concedido só a quem precisa, com registro.
- `dw_engenharia`: escrita em `dw_landing`, `dw_history`, `dw_staging`, `dw_intermediate`, `datawarehouse`, `datamart`, `dw_reference` e `dw_control`. Sem `dw_personal_data`: como esses schemas não têm coluna identificadora (5.2, regra 6), quem mantém o DW não vê dado pessoal em claro.
- `dw_admin`: DDL. Uso restrito.
- A identidade que roda a carga é a única com escrita em `dw_personal_data` e com leitura nos schemas de origem (`totvs`, `movidesk`, `aftersale`).

Dynamic Data Masking como camada extra nas colunas sensíveis.

### 12.3 Pepper do hash

O `doc_hash` de `dim_cliente` é `SHA-256(pepper + CPF/CNPJ)` (8.3). O pepper não existe em tabela, script ou log: chega à procedure `datawarehouse.sp_dim_cliente_load` como parâmetro, fornecido por quem a chama. Onde ele fica guardado é decisão fora deste guia.

**Onde calcular o hash (pergunta 32, decidida): na procedure `sp_dim_cliente_load`, com o pepper como parâmetro.** Risco conhecido e aceito: como parâmetro de procedure, o pepper pode aparecer no plano em cache e no Query Store. A alternativa descartada era calcular o hash fora do banco e a procedure receber só o resultado, o que exigiria uma tabela de apoio.

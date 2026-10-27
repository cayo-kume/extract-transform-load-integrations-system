# DW do NEWBLOOMERS · 10. Perguntas em aberto

> Guia 10 de 11 do guia do DW, versão 5. Seções deste arquivo: 17. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 17. Perguntas da modelagem ⚠️

Só as perguntas que mudam tabelas, views ou procedures. As demais (operação, BI, ambientes, CI/CD, custos e credenciais) saíram do guia na versão 5; a numeração original foi mantida, por isso há lacunas. A seção 21 lista as removidas.

As decididas estão marcadas com ✅ e aplicadas no texto. Nenhuma em aberto. Adiadas: 10 e 14.

**A. Banco e carga**

1. ✅ **Decidido:** o DW começa do zero na primeira carga do banco novo. O histórico do DW atual não entra.
5. ✅ **Decidido:** banco zerado com os schemas `datawarehouse` e `datamart`, junto com os módulos de integração; os demais schemas do DW levam o prefixo `dw_` (3.1 e 4.1).
6. ✅ **Decidido:** transformação em T-SQL, sem dbt (3.4).
8. ✅ **Decidido:** recarga completa do fato de venda a cada execução (9.1); otimizar depois de medir.
9. ✅ **Decidido:** snapshot a cada hora para notas e pedidos, junto com a carga, e diário para cadastros (3.3). O snapshot é incremental (seção 6).

**B. Modelo**

10. *(Adiada: fora do escopo desta versão, 7.4.)* Estoque: "append" ou "substituir o dia", quando o módulo de estoque tiver tabelas aqui.
11. ✅ **Decidido:** `dim_cliente` é SCD1 (7.1 e 8.3). Com a regra 6 da 5.2, SCD2 exigiria versionar UF e endereço fora de `dw_personal_data`, que não guarda versões.
13. ✅ **Decidido:** nenhuma loja vende por Linx hoje (descontinuado). O mapa CNPJ × ERP × período fica em `dw_reference` para olhar o passado: quanto cada loja vendeu nos anos em que vendia pela Linx. O histórico de Linx Microvix e Linx Commerce entra quando os módulos tiverem tabelas aqui; só o Totvs gera linhas novas (7.2). O módulo Linx Microvix segue com integrações recorrentes, para servir a outros clientes: o histórico vem na primeira execução de cada método, e as seguintes só trazem o que mudou (`docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md`, §10.1).
14. *(Adiada: fora do escopo desta versão, 7.4.)* Quanto histórico de estoque diário é necessário?
15. ✅ **Decidido:** canal de e-commerce no Totvs é o pedido de `totvs.TotvsSaleOrders` com `integrationCode` não nulo; nulo é loja. O prefixo `MI-`/`OA-` não é o critério (7.1 e 7.2).
23. ✅ **Decidido:** `datamart`, sem prefixo, como `datawarehouse` (3.1).
26. ✅ **Decidido:** `dim_operacao` a partir dos campos de operação da nota do Totvs (`operationCode`, `operatioName`, `operationType`) (7.1).

**C. Dado pessoal**

32. ✅ **Decidido:** o `doc_hash` é calculado na procedure `sp_dim_cliente_load`, com o pepper como parâmetro (12.3). A alternativa, calcular fora do banco com a procedure lendo só o resultado, foi descartada.

A pergunta 19 (retenção e anonimização de dado pessoal) foi retirada: a premissa é que não haverá pedido de exclusão nem de anonimização no DW (12.1).

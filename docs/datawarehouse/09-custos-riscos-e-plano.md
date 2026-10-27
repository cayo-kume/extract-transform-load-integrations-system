# DW do NEWBLOOMERS · 09. Riscos e plano

> Guia 9 de 11 do guia do DW, versão 5. Seções deste arquivo: 15 e 16. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 15. Riscos da modelagem

| Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|
| Histórico começa tarde e a versão anterior se perde | Alta | Alto | Snapshots desde a primeira carga, nas tabelas que já estão no padrão. O DW começa do zero (pergunta 1) |
| Duas versões da mesma linha com o mesmo `origem_atualizado_em` colidem na PK de `dw_history` | Baixa | Alto | Confirmar na carga de teste que `maxChangeFilterDate` muda a cada alteração; se não mudar, incluir `carregado_em` na PK (seção 6) |
| Cadastros de produto e vendedor incompletos na primeira carga: vendas de 2025 apontam para membros ausentes | Baixa | Alto | Janelas desde 2000, como pessoas (5.2, regra 5); checagem de membro desconhecido (seção 11) |
| Passado das lojas que vendiam por Linx fica fora até o histórico Linx entrar | Média | Médio | Mapa CNPJ × ERP × período em `dw_reference` (pergunta 13); carga única do legado quando os módulos Linx tiverem tabelas (fase 3) |
| Ordem errada dos migrators quebra a criação das views | Média | Médio | Ordem fixa documentada (1.1); testar num banco local descartável antes de rodar em produção |
| Script DbUp editado depois de executado não se aplica onde já rodou | Média | Médio | Alteração sempre em script novo, com `CREATE OR ALTER` (1.3) |
| Procedures T-SQL sem framework de teste viram código difícil de manter | Média | Médio | Views de checagem (9.2); convenção de nomes (seção 9); tSQLt se o time quiser (a decidir) |
| Grão do fato duplicado por chave de origem repetida | Baixa | Alto | A PK do fato é a PK da origem (8.4); `vw_check_fato_venda_grao` bloqueia a carga (9.2) |
| Dado pessoal exposto | Baixa | Alto | Colunas identificadoras só em `dw_personal_data` (5.2, regra 6); hash com pepper, mascaramento e `GRANT` por schema (seção 12) |
| Retenção de 15 dias do `auditing` apaga o histórico de execuções | Alta | Médio | Resultados das checagens em `dw_control.check_result`, com retenção própria (3.9) |

---

## 16. Plano de construção do banco

1. Criar o módulo `DataWarehouse` com `migrator.csx` sem connection string padrão, e os scripts 001 a 009: schemas, `dw_control.watermark` e `dw_control.check_result` (seção 9). Fora da modelagem, mantido a pedido: remover a connection string padrão e a impressão da string dos 42 `migrator.csx` existentes (14 em cada pasta: `Scripts/SQLServer`, `Scripts/MySQL` e `Scripts/PostgreSQL`); no lugar, quando o argumento faltar ou vier incompleto, o script para com uma mensagem dizendo que a connection string é obrigatória e quais partes faltam (servidor, banco, usuário e senha ou autenticação integrada), sem imprimir valores.
2. Subir o banco pelos migrators na ordem da 1.1, primeiro num banco local descartável, depois em produção. Criar a landing (scripts 010 e 011) e o history (020) e **ligar os snapshots** nas tabelas Totvs: `TotvsInvoices`, `TotvsSaleOrders`, `TotvsBranches` (`dim_loja` é SCD2, 7.1), `TotvsPeople`, `TotvsLegalEntities` e `TotvsSkus`. O histórico começa aqui: é o único passo que não pode ser refeito depois.
3. **Fase 1:** `dw_personal_data` (030), dimensões (031), fatos de venda, pedido e pagamento (040), `datamart.vendas_diarias` (050), views de checagem (060) e seeds (070). Carga inicial pelas janelas da 5.2, regra 5. Validar os totais com quem usa os números (7.3).
4. **Fase 2:** Movidesk e AfterSale (`fato_atendimento` e `fato_reembolso`).
5. **Fase 3:** cada fonte entra quando o módulo criar as tabelas de dados neste repositório (1.1). Linx Microvix e Linx Commerce entram como carga única do histórico, ligada pelo mapa CNPJ × ERP × período (pergunta 13); a PK dos fatos ganha `fonte_erp` (8.4).

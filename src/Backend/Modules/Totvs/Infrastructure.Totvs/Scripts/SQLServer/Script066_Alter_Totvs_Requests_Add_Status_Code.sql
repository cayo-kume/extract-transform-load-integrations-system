/****** status da Totvs em TotvsRequests (docs/PLANO-TOTVS-METODOS-FALTANTES.md, T5) ******/
--0 quando a chamada lançou exceção (sem saber se a Totvs gravou); null sem chamada à rota (login que falhou) e nos registros anteriores.
--a trava de repetição das escritas separa por ele a recusa da Totvs (4xx, nada gravado) da dúvida (5xx ou exceção)
alter table [totvs].TotvsRequests add status_code int null;

INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas de cadastro da API Person - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:People:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.4.1 a §3.4.3, fase PE-6)*/
/*Range 13000 - 13499 (Person: 13030 - 13069)*/
(130, 13040, 'TotvsPostIndividualCustomers', 'Inclusão ou alteração de cliente pessoa física (POST IndividualCustomers/{docCompany})', 0, 2),
(130, 13041, 'TotvsPostLegalCustomers', 'Inclusão ou alteração de cliente pessoa jurídica (POST LegalCustomers/{docCompany})', 0, 2),
(130, 13042, 'TotvsPostCustomerRepresentatives', 'Vínculo de clientes a um representante (POST CustomerRepresentatives/{docCompany})', 0, 2),
(130, 13043, 'TotvsPostRemoveClassificationRelationship', 'Remoção de classificações de uma pessoa (POST ClassificationRelationship/{docCompany}/Remove)', 0, 2);

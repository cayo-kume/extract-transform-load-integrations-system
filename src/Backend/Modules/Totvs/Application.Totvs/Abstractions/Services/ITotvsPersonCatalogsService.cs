namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// catálogos da API Person: classificações, tipos de e-mail, de telefone e de contato (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.2)
    /// </summary>
    public interface ITotvsPersonCatalogsService
    {
        /// <summary>
        /// sincroniza o catálogo de classificações pelo GET person/v2/classifications (janela de alteração, como as filiais); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> GetClassifications();

        /// <summary>
        /// sincroniza o catálogo de tipos de e-mail pelo GET person/v2/email-types (janela de alteração, como as filiais); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> GetEmailTypes();

        /// <summary>
        /// sincroniza o catálogo de tipos de telefone pelo GET person/v2/phone-types (janela de alteração, como as filiais); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> GetPhoneTypes();

        /// <summary>
        /// sincroniza o catálogo de tipos de contato pelo GET person/v2/contact-types (janela de alteração, como as filiais); só grava os novos ou alterados
        /// </summary>
        public Task<bool?> GetContactTypes();
    }
}

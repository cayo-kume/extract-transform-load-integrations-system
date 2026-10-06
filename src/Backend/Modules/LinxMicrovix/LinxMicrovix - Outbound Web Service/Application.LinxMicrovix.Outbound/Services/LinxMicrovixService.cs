using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Domain.Core.Entities.Exceptions;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using System.Xml;


namespace Application.LinxMicrovix.Outbound.WebService.Services
{
    public class LinxMicrovixService : ILinxMicrovixService
    {
        public string? BuildBodyRequest(LinxAPIParam jobParameter, string? parametersList, string? cnpj_emp)
        {
            return @$"<?xml version=""1.0"" encoding=""utf-8""?>
                          <LinxMicrovix>
                              <Authentication user=""{jobParameter.UserAuthentication}"" password=""{jobParameter.UserAuthentication}"" />
                              <ResponseFormat>xml</ResponseFormat>
                              <Command>
                                  <Name>{jobParameter.jobName}</Name>
                                  <Parameters>
                                      <Parameter id=""chave"">{jobParameter.KeyAuthorization}</Parameter>
                                      <Parameter id=""cnpjEmp"">{cnpj_emp}</Parameter>
                                      {parametersList}
                                  </Parameters>
                              </Command>
                          </LinxMicrovix>";
        }

        public string? BuildBodyRequest(LinxAPIParam jobParameter, string? parametersList)
        {
            return @$"<?xml version=""1.0"" encoding=""utf-8""?>
                          <LinxMicrovix>
                              <Authentication user=""{jobParameter.UserAuthentication}"" password=""{jobParameter.UserAuthentication}"" />
                              <ResponseFormat>xml</ResponseFormat>
                              <Command>
                                  <Name>{jobParameter.jobName}</Name>
                                  <Parameters>
                                      <Parameter id=""chave"">{jobParameter.KeyAuthorization}</Parameter>
                                      {parametersList}
                                  </Parameters>
                              </Command>
                          </LinxMicrovix>";
        }

        public string? BuildBodyRequest(LinxAPIParam jobParameter)
        {
            return @$"<?xml version=""1.0"" encoding=""utf-8""?>
                          <LinxMicrovix>
                              <Authentication user=""{jobParameter.UserAuthentication}"" password=""{jobParameter.UserAuthentication}"" />
                              <ResponseFormat>xml</ResponseFormat>
                              <Command>
                                  <Name>{jobParameter.jobName}</Name>
                                  <Parameters>
                                      <Parameter id=""chave"">{jobParameter.KeyAuthorization}</Parameter>
                                  </Parameters>
                              </Command>
                          </LinxMicrovix>";
        }

        public List<Dictionary<string?, string?>> DeserializeResponseToXML(LinxAPIParam jobParameter, string? response)
        {
            try
            {
                var listRegistros = new List<Dictionary<string?, string?>>();
                var xml = new XmlDocument();

                if (response != string.Empty)
                {
                    xml.LoadXml(response);

                    if (xml.GetElementsByTagName("ResponseSuccess")[0].ChildNodes[0].InnerText == "False")
                        throw new APIException(
                            message: "Error when deserealizing response to xml list",
                            exceptionMessage: xml.GetElementsByTagName("Message")[0].ChildNodes[0].InnerText,
                            apiRequestResponse: response
                        );

                    if (xml.GetElementsByTagName("R").Count > 0)
                    {
                        Parallel.For(0, xml.GetElementsByTagName("R").Count, row =>
                        {
                            var registro = new Dictionary<string?, string?>();
                            var c0 = xml.GetElementsByTagName("C")[0];
                            var rrow = xml.GetElementsByTagName("R")[row];
                            registro.Add("recordXml", rrow.InnerXml);

                            for (int col = 0; col < c0.ChildNodes.Count; col++)
                            {
                                string? key = c0.ChildNodes[col].InnerText;
                                string? value = rrow.ChildNodes[col].InnerText.Replace("'", "''");

                                registro.Add(key, value);
                            }
                            listRegistros.Add(registro);
                        });

                        return listRegistros;
                    }
                    else
                        return listRegistros;
                }
                return listRegistros;
            }
            catch (Exception ex)
            {
                throw new GeneralException(
                    message: $"Error when deserealizing response to xml list - {ex.Message}",
                    exceptionMessage: ex.StackTrace
                );
            }
        }
    }
}

using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Extensions;
using Domain.LinxMicrovix.Outbound.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace Application.LinxMicrovix.Outbound.Services.LinxMicrovix
{
    public class LinxReshopSaldoService : ILinxReshopSaldoService
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxReshopSaldoRepository _blobReader;
        //refatorar aqui - adicionar padr�o logger dos demais jobs
        private readonly ILogger<LinxReshopSaldoService> _logger;
        private readonly string _epplusLicensee;
        private readonly string _timeZoneId;

        public LinxReshopSaldoService(
            IIntegrationsCoreRepository coreRepository,
            ILinxReshopSaldoRepository blobReader,
            ILogger<LinxReshopSaldoService> logger,
            IConfiguration configuration)
        {
            _coreRepository = coreRepository;
            _blobReader = blobReader;
            _logger = logger;
            _epplusLicensee = configuration["LinxReshopExcel:EPPlusLicensee"] ?? "Newbloomers";
            _timeZoneId = configuration["LinxReshopExcel:TimeZoneId"] ?? "E. South America Standard Time";
        }

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                var license = ExcelPackage.License;
                license.SetNonCommercialOrganization(_epplusLicensee);

                var blobPaths = await _blobReader.ListBlobsAsync();

                if (blobPaths.Count == 0)
                {
                    _logger.LogInformation("Nenhum arquivo Excel encontrado no Blob Storage para importacao LinxReshopExcel.");
                    return true;
                }

                foreach (var blobPath in blobPaths)
                {
                    _logger.LogInformation("Processando arquivo: {BlobPath}", blobPath);

                    var list = new List<LinxReshopSaldo>();

                    using var stream = await _blobReader.GetExcelStreamAsync(blobPath);
                    using var package = new ExcelPackage(stream);

                    var worksheet = package.Workbook.Worksheets[0];

                    if (worksheet.Dimension is null)
                        throw new InvalidOperationException($"Planilha vazia: {blobPath}");

                    int rowCount = worksheet.Dimension.End.Row;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        list.Add(FromExcelRow(
                            rede: worksheet.Cells[row, 1].Value?.ToString(),
                            loja: worksheet.Cells[row, 2].Value?.ToString(),
                            cpf: worksheet.Cells[row, 3].Value?.ToString(),
                            cliente: worksheet.Cells[row, 4].Value?.ToString(),
                            email: worksheet.Cells[row, 5].Value?.ToString(),
                            telefone: worksheet.Cells[row, 6].Value?.ToString(),
                            pontos: worksheet.Cells[row, 7].Value?.ToString(),
                            saldo: worksheet.Cells[row, 8].Value?.ToString(),
                            ticket: worksheet.Cells[row, 9].Value?.ToString(),
                            data: worksheet.Cells[row, 10].Value?.ToString(),
                            expiracaoPontos: worksheet.Cells[row, 11].Value?.ToString(),
                            expiracaoSaldo: worksheet.Cells[row, 12].Value?.ToString(),
                            timeZoneId: _timeZoneId
                        ));
                    }

                    _logger.LogInformation("Lidas {RowCount} linhas do arquivo {BlobPath}.", list.Count, blobPath);

                    var table = _coreRepository.CreateSystemDataTable("LinxReshopSaldo", new LinxReshopSaldo());
                    _coreRepository.FillSystemDataTable(table, list);
                    _coreRepository.BulkInsertIntoTableRaw(dataTable: table);

                    await _coreRepository.CallDbProcMerge("linx_microvix_erp", table.TableName);

                    await _blobReader.DeleteBlobAsync(blobPath);
                    _logger.LogInformation("Blob {BlobPath} deletado com sucesso apos importacao.", blobPath);
                }

                _logger.LogInformation("Importacao LinxReshopExcel concluida. {FileCount} arquivo(s) processado(s).", blobPaths.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar importacao LinxReshopExcel.");
                return false;
            }
        }

        private LinxReshopSaldo FromExcelRow(
            string? rede,
            string? loja,
            string? cpf,
            string? cliente,
            string? email,
            string? telefone,
            string? pontos,
            string? saldo,
            string? ticket,
            string? data,
            string? expiracaoPontos,
            string? expiracaoSaldo,
            string timeZoneId)
        {
            return new LinxReshopSaldo
            {
                Rede = rede ?? string.Empty,
                Loja = loja ?? string.Empty,
                Cpf = cpf ?? string.Empty,
                Cliente = cliente ?? string.Empty,
                Email = email ?? string.Empty,
                Telefone = telefone ?? string.Empty,
                Pontos = ConverterValidatorsExtensions.ConvertToInt64Validation(pontos),
                Saldo = ParseBrlCurrency(saldo),
                Ticket = ConverterValidatorsExtensions.ConvertToInt64Validation(ticket),
                Data = ConverterValidatorsExtensions.ConvertToDateTimeValidation(data),
                LastUpdateOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)),
                ExpiracaoPontos = ConverterValidatorsExtensions.ConvertToInt64Validation(expiracaoPontos),
                ExpiracaoSaldo = ParseBrlCurrency(expiracaoSaldo)
            };
        }

        private decimal ParseBrlCurrency(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            string cleaned = Regex.Replace(value, @"[R$ ]", string.Empty)
                .Replace(".", "")
                .Replace(",", ".")
                .Trim();

            return ConverterValidatorsExtensions.ConvertToDecimalValidation(cleaned);
        }
    }
}

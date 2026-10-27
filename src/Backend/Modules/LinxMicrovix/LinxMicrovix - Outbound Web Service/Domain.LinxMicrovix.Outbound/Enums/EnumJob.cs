namespace Domain.LinxMicrovix.Outbound.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir no script de jobs do service correspondente
    /// (um por service, Script0NN_Insert_LinxMicrovix_*_Jobs_Into_Jobs_Table.sql). O nome de cada job é o nome do método da
    /// API, que também é o nome da tabela. Faixas (app 80): integrações ERP 8000 - 8249, integrações B2C 8250 - 8499,
    /// procedures 8500 - 8999 (ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        /*ERP - LinxMicrovixStoresService*/
        LinxGrupoLojas = 8000,        //Script006_Insert_LinxMicrovix_Stores_Jobs_Into_Jobs_Table.sql
        LinxLojas = 8001,             //Script006_Insert_LinxMicrovix_Stores_Jobs_Into_Jobs_Table.sql
        LinxLojasParametros = 8002,   //Script006_Insert_LinxMicrovix_Stores_Jobs_Into_Jobs_Table.sql
        LinxUsuarios = 8003,          //Script006_Insert_LinxMicrovix_Stores_Jobs_Into_Jobs_Table.sql
        LinxMetodos = 8004,           //Script006_Insert_LinxMicrovix_Stores_Jobs_Into_Jobs_Table.sql
        LinxEntrantes = 8005,         //Script006_Insert_LinxMicrovix_Stores_Jobs_Into_Jobs_Table.sql

        /*B2C - LinxMicrovixB2CStoresService*/
        B2CConsultaEmpresas = 8250,                                   //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql
        B2CConsultaCNPJsChave = 8251,                                 //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql
        B2CConsultaVendedores = 8252,                                 //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql
        B2CConsultaTransportadores = 8253,                            //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql
        B2CConsultaFormasPagamento = 8254,                            //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql
        B2CConsultaPlanos = 8255,                                     //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql
        B2CConsultaPlanosParcelas = 8256,                             //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql
        B2CConsultaTiposCobrancaFrete = 8257,                         //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql
        B2CConsultaTipoEncomenda = 8258,                              //Script009_Insert_LinxMicrovix_B2C_Stores_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixProductAttributesService*/
        LinxSetores = 8034,                                           //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxLinhas = 8035,                                            //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxMarcas = 8036,                                            //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxColecoes = 8037,                                          //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxTamanhos = 8038,                                          //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxCores = 8039,                                             //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxEspessuras = 8040,                                        //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxUnidades = 8041,                                          //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxClassificacoes = 8042,                                    //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql
        LinxSubClasses = 8043,                                        //Script012_Insert_LinxMicrovix_Product_Attributes_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixTaxesService*/
        LinxNaturezaOperacao = 8063,                                  //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxCfopFiscal = 8064,                                        //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxCstIcmsFiscal = 8065,                                     //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxCstIpiFiscal = 8066,                                      //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxCstPisFiscal = 8067,                                      //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxCstCofinsFiscal = 8068,                                   //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxCsosnFiscal = 8069,                                       //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxClasseFiscal = 8070,                                      //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxCest = 8071,                                              //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxNcm = 8072,                                               //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxEnquadramentoIpi = 8073,                                  //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxMotivosDesoneracaoIcms = 8074,                            //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxSpedTipoItem = 8075,                                      //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxSpedTipoBaseCredito = 8076,                               //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxRotinaOrigem = 8077,                                      //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql
        LinxNfeIbgeMunicipio = 8078,                                  //Script015_Insert_LinxMicrovix_Taxes_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixTaxSettingsService*/
        LinxConfiguracoesTributarias = 8079,                          //Script018_Insert_LinxMicrovix_Tax_Settings_Jobs_Into_Jobs_Table.sql
        LinxConfiguracoesTributariasDetalhes = 8080,                  //Script018_Insert_LinxMicrovix_Tax_Settings_Jobs_Into_Jobs_Table.sql
        LinxConfiguracoesTributariasDetalhesSimplificado = 8081,      //Script018_Insert_LinxMicrovix_Tax_Settings_Jobs_Into_Jobs_Table.sql
        LinxConfiguracoesTributariasEmpresas = 8082,                  //Script018_Insert_LinxMicrovix_Tax_Settings_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixProductsService*/
        LinxProdutos = 8024,                                          //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxProdutosAssociados = 8025,                                //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxProdutosCamposAdicionais = 8026,                          //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxProdutosCodBar = 8027,                                    //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxProdutosFornec = 8028,                                    //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxProdutosInformacoes = 8029,                               //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxProdutosImagensURL = 8030,                                //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxProdutosLotes = 8031,                                     //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxLotesProdutos = 8032,                                     //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql
        LinxProdutosSerial = 8033,                                    //Script021_Insert_LinxMicrovix_Products_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixStocksService*/
        LinxProdutosDetalhes = 8044,                                  //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql
        LinxProdutosDetalhesSimplificado = 8045,                      //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql
        LinxProdutosDetalhesDepositos = 8046,                         //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql
        LinxProdutosDepositos = 8047,                                 //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql
        LinxProdutosInventario = 8048,                                //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql
        LinxListagemBalanco = 8049,                                   //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql
        LinxBalancos = 8050,                                          //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql
        LinxBalancosArquivos = 8051,                                  //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql
        LinxBalancosSecoes = 8052,                                    //Script024_Insert_LinxMicrovix_Stocks_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixPricesService*/
        LinxProdutosTabelas = 8053,                                   //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxProdutosTabelasPrecos = 8054,                             //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxProdutosPromocoes = 8055,                                 //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxAcoesPromocionais = 8056,                                 //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxAcoesPromocionaisCombinacaoProdutosItens = 8057,          //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxAcoesPromocionaisProdutosCortesia = 8058,                 //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxMovimentoAcoesPromocionais = 8059,                        //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxCupomDesconto = 8060,                                     //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxCupomDescontoVendas = 8061,                               //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql
        LinxMotivoDesconto = 8062,                                    //Script027_Insert_LinxMicrovix_Prices_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixCustomersService*/
        LinxClientesFornec = 8006,                                    //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxClientesFornecCamposAdicionais = 8007,                    //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxClientesFornecClasses = 8008,                             //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxClientesFornecContatos = 8009,                            //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxClientesFornecContatosParentesco = 8010,                  //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxClientesFornecCreditoAvulso = 8011,                       //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxClientesFornecVendedores = 8012,                          //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxClientesEnderecosEntrega = 8013,                          //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxClientesRede = 8014,                                      //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql
        LinxRamosAtividade = 8015,                                    //Script030_Insert_LinxMicrovix_Customers_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixSellersService*/
        LinxVendedores = 8016,                                        //Script033_Insert_LinxMicrovix_Sellers_Jobs_Into_Jobs_Table.sql
        LinxVendedoresFerias = 8017,                                  //Script033_Insert_LinxMicrovix_Sellers_Jobs_Into_Jobs_Table.sql
        LinxMetasVendedores = 8018,                                   //Script033_Insert_LinxMicrovix_Sellers_Jobs_Into_Jobs_Table.sql
        LinxMetasVendedoresDia = 8019,                                //Script033_Insert_LinxMicrovix_Sellers_Jobs_Into_Jobs_Table.sql
        LinxComissoesVendedores = 8020,                               //Script033_Insert_LinxMicrovix_Sellers_Jobs_Into_Jobs_Table.sql
        LinxListaDaVez = 8021,                                        //Script033_Insert_LinxMicrovix_Sellers_Jobs_Into_Jobs_Table.sql
        LinxFuncionariosFuncoes = 8022,                               //Script033_Insert_LinxMicrovix_Sellers_Jobs_Into_Jobs_Table.sql
        LinxRegistroProfissional = 8023,                              //Script033_Insert_LinxMicrovix_Sellers_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixPaymentsService*/
        LinxMovimentoCartoes = 8110,                                  //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql
        LinxMovimentoPlanos = 8111,                                   //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql
        LinxPlanos = 8112,                                            //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql
        LinxPlanosParcelas = 8113,                                    //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql
        LinxPlanosBandeiras = 8114,                                   //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql
        LinxMovimentoGiftCard = 8115,                                 //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql
        LinxValesComprasEnviadosAPI = 8116,                           //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql
        LinxSangriaSuprimentos = 8117,                                //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql
        LinxFechamentoCaixa = 8118,                                   //Script036_Insert_LinxMicrovix_Payments_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixLoyaltyService*/
        LinxFidelidade = 8119,                                        //Script039_Insert_LinxMicrovix_Loyalty_Jobs_Into_Jobs_Table.sql
        LinxMovimentoReshop = 8120,                                   //Script039_Insert_LinxMicrovix_Loyalty_Jobs_Into_Jobs_Table.sql
        LinxMovimentoReshopItens = 8121,                              //Script039_Insert_LinxMicrovix_Loyalty_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixMovementsService*/
        LinxMovimento = 8095,                                         //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxMovimentoPrincipal = 8096,                                //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxMovimentoExtensao = 8097,                                 //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxMovimentoCamposAdicionais = 8098,                         //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxMovimentoObservacoes = 8099,                              //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxMovimentoObservacaoCF = 8100,                             //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxMovimentoIndicacoes = 8101,                               //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxMovimentoSerial = 8102,                                   //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxSerialVenda = 8103,                                       //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxMovimentoVendaConjugada = 8104,                           //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxPerguntaVenda = 8105,                                     //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql
        LinxRespostaVenda = 8106,                                     //Script042_Insert_LinxMicrovix_Movements_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixPosSalesService*/
        LinxVendasPosTmp = 8107,                                      //Script045_Insert_LinxMicrovix_Pos_Sales_Jobs_Into_Jobs_Table.sql
        LinxVendasPosProdutosTmp = 8108,                              //Script045_Insert_LinxMicrovix_Pos_Sales_Jobs_Into_Jobs_Table.sql
        LinxVendasPosAgrupamentoTmp = 8109,                           //Script045_Insert_LinxMicrovix_Pos_Sales_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixReturnsService*/
        LinxMovimentoTrocas = 8122,                                   //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxMovimentoTrocafone = 8123,                                //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxMovimentoDevolucoesItens = 8124,                          //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxMovimentoOrigemDevolucoes = 8125,                         //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxMotivoDevolucao = 8126,                                   //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxTrocaUnificadaResumoVendas = 8127,                        //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxTrocaUnificadaResumoVendasItens = 8128,                   //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxTrocaUnificadaResumoBaixa = 8129,                         //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxTradeinParceiro = 8130,                                   //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxDevolucaoRemanejoFabrica = 8131,                          //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxDevolucaoRemanejoFabricaItem = 8132,                      //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxDevolucaoRemanejoFabricaStatus = 8133,                    //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql
        LinxDevolucaoRemanejoFabricaTipo = 8134,                      //Script048_Insert_LinxMicrovix_Returns_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixConsignmentsService*/
        LinxMovimentoRemessas = 8135,                                 //Script051_Insert_LinxMicrovix_Consignments_Jobs_Into_Jobs_Table.sql
        LinxMovimentoRemessasItens = 8136,                            //Script051_Insert_LinxMicrovix_Consignments_Jobs_Into_Jobs_Table.sql
        LinxMovimentoRemessasAcertos = 8137,                          //Script051_Insert_LinxMicrovix_Consignments_Jobs_Into_Jobs_Table.sql
        LinxMovimentoRemessasAcertosItens = 8138,                     //Script051_Insert_LinxMicrovix_Consignments_Jobs_Into_Jobs_Table.sql
        LinxRemessasIdentificadores = 8139,                           //Script051_Insert_LinxMicrovix_Consignments_Jobs_Into_Jobs_Table.sql
        LinxRemessasOperacoes = 8140,                                 //Script051_Insert_LinxMicrovix_Consignments_Jobs_Into_Jobs_Table.sql
        LinxRemessasRetornoTipos = 8141,                              //Script051_Insert_LinxMicrovix_Consignments_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixOrdersService*/
        LinxPedidosVenda = 8142,                                      //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxPlanosPedidoVenda = 8143,                                 //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxPedidosVendaChecklistEntregaLocal = 8144,                 //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxPedidosVendaChecklistEntregaArmazenamento = 8145,         //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxPedidosVendaChecklistEntregaDificuldade = 8146,           //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxOrcamentoPosicao = 8147,                                  //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxB2CPedidos = 8148,                                        //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxB2CPedidosItens = 8149,                                   //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxB2CPedidosStatus = 8150,                                  //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql
        LinxB2CStatus = 8151,                                         //Script054_Insert_LinxMicrovix_Orders_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixPurchaseOrdersService*/
        LinxPedidosCompra = 8152,                                     //Script057_Insert_LinxMicrovix_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        LinxPedidosCompraStatus = 8153,                               //Script057_Insert_LinxMicrovix_Purchase_Orders_Jobs_Into_Jobs_Table.sql
        LinxPedidosCompraNumeroFornecedor = 8154,                     //Script057_Insert_LinxMicrovix_Purchase_Orders_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixFiscalDocumentsService*/
        LinxXMLDocumentos = 8083,                                     //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxNFeEvento = 8084,                                         //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxNfse = 8085,                                              //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxEntradaNFe = 8086,                                        //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxEntradaNFeProdutos = 8087,                                //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxConhecimentoFretes = 8088,                                //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxFreteTipos = 8089,                                        //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxReducoesZ = 8090,                                         //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxECF = 8091,                                               //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxECFFormasPagamento = 8092,                                //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxNfceEstacao = 8093,                                       //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql
        LinxSeries = 8094,                                            //Script060_Insert_LinxMicrovix_Fiscal_Documents_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixFinancialService*/
        LinxFaturas = 8176,                                           //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxFaturasOrigem = 8177,                                     //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxPortadores = 8178,                                        //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxAntecipacoesFinanceiras = 8179,                           //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxAntecipacoesFinanceirasPlanos = 8180,                     //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxDesfazimentosAntecipacoes = 8181,                         //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxDesfazimentosAntecipacoesDocumento = 8182,                //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxDesfazimentosAntecipacoesItens = 8183,                    //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxCategoriasFinanceiras = 8184,                             //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxCentroCusto = 8185,                                       //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxPlanoContas = 8186,                                       //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxLancContabil = 8187,                                      //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxHistoricosContabeis = 8188,                               //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxExtratoBancarioLancamentos = 8189,                        //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql
        LinxExtratoBancarioConciliacoes = 8190,                       //Script063_Insert_LinxMicrovix_Financial_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixServiceOrdersService*/
        LinxOrdensServico = 8155,                                     //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxOrdensServicoProdutos = 8156,                             //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxServicos = 8157,                                          //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxServicosDetalhes = 8158,                                  //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxServicoClientes = 8159,                                   //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxServicoAgendaComandas = 8160,                             //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxServicoAgendaEventos = 8161,                              //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxOrdemServicoExternaDav = 8162,                            //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxMovimentoOrdemServicoExterna = 8163,                      //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxValeOrdemServicoExterna = 8164,                           //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql
        LinxPagamentoParcialDav = 8165,                               //Script066_Insert_LinxMicrovix_Service_Orders_Jobs_Into_Jobs_Table.sql

        /*ERP - LinxMicrovixOpticsService*/
        LinxProdutosOpticosTipo = 8166,                               //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxProdutosOpticosTipoAro = 8167,                            //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxProdutosOpticosFormatoAro = 8168,                         //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxDadosOpticosDav = 8169,                                   //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxOticoPrismaDescricao = 8170,                              //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxOticoReceitas = 8171,                                     //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxPosicaoOsRamoOptico = 8172,                               //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxOrdensServicoPosicaoOsRamoOpticoHistorico = 8173,         //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxOrcamentoComponenteFormula = 8174,                        //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql
        LinxComponenteFormulaDav = 8175,                              //Script069_Insert_LinxMicrovix_Optics_Jobs_Into_Jobs_Table.sql

        /*B2C - LinxMicrovixB2CCustomersService*/
        B2CConsultaClientes = 8259,                                   //Script072_Insert_LinxMicrovix_B2C_Customers_Jobs_Into_Jobs_Table.sql
        B2CConsultaClientesContatos = 8260,                           //Script072_Insert_LinxMicrovix_B2C_Customers_Jobs_Into_Jobs_Table.sql
        B2CConsultaClientesContatosParentesco = 8261,                 //Script072_Insert_LinxMicrovix_B2C_Customers_Jobs_Into_Jobs_Table.sql
        B2CConsultaClientesEnderecosEntrega = 8262,                   //Script072_Insert_LinxMicrovix_B2C_Customers_Jobs_Into_Jobs_Table.sql
        B2CConsultaClientesEstadoCivil = 8263,                        //Script072_Insert_LinxMicrovix_B2C_Customers_Jobs_Into_Jobs_Table.sql
        B2CConsultaClientesSaldo = 8264,                              //Script072_Insert_LinxMicrovix_B2C_Customers_Jobs_Into_Jobs_Table.sql
        B2CConsultaClientesSaldoLinx = 8265,                          //Script072_Insert_LinxMicrovix_B2C_Customers_Jobs_Into_Jobs_Table.sql

        /*B2C - LinxMicrovixB2CProductAttributesService*/
        B2CConsultaClassificacao = 8288,                              //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaColecoes = 8289,                                   //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaEspessuras = 8290,                                 //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaGrade1 = 8291,                                     //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaGrade2 = 8292,                                     //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaLinhas = 8293,                                     //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaMarcas = 8294,                                     //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaSetores = 8295,                                    //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaUnidade = 8296,                                    //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaFornecedores = 8297,                               //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaFlags = 8298,                                      //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaTags = 8299,                                       //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaPalavrasChavePesquisa = 8300,                      //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql
        B2CConsultaLegendasCadastrosAuxiliares = 8301,                //Script075_Insert_LinxMicrovix_B2C_Product_Attributes_Jobs_Into_Jobs_Table.sql

        /*B2C - LinxMicrovixB2CProductsService*/
        B2CConsultaProdutos = 8266,                                   //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosAssociados = 8267,                         //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosCampanhas = 8268,                          //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosCodebar = 8269,                            //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosDimensoes = 8270,                          //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosInformacoes = 8271,                        //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosStatus = 8272,                             //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosFlags = 8273,                              //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosTags = 8274,                               //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosPalavrasChavePesquisa = 8275,              //Script078_Insert_LinxMicrovix_B2C_Products_Jobs_Into_Jobs_Table.sql

        /*B2C - LinxMicrovixB2CProductImagesService*/
        B2CConsultaProdutosImagens = 8276,                            //Script081_Insert_LinxMicrovix_B2C_Product_Images_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosImagensURL = 8277,                         //Script081_Insert_LinxMicrovix_B2C_Product_Images_Jobs_Into_Jobs_Table.sql
        B2CConsultaImagens = 8278,                                    //Script081_Insert_LinxMicrovix_B2C_Product_Images_Jobs_Into_Jobs_Table.sql
        B2CConsultaImagensHD = 8279,                                  //Script081_Insert_LinxMicrovix_B2C_Product_Images_Jobs_Into_Jobs_Table.sql

        /*B2C - LinxMicrovixB2CProductFieldsService*/
        B2CConsultaProdutosCamposAdicionaisDetalhes = 8280,           //Script084_Insert_LinxMicrovix_B2C_Product_Fields_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosCamposAdicionaisNomes = 8281,              //Script084_Insert_LinxMicrovix_B2C_Product_Fields_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosCamposAdicionaisValores = 8282,            //Script084_Insert_LinxMicrovix_B2C_Product_Fields_Jobs_Into_Jobs_Table.sql
        B2CConsultaB2CProdutosCamposAdicionais = 8283,                //Script084_Insert_LinxMicrovix_B2C_Product_Fields_Jobs_Into_Jobs_Table.sql
        B2CConsultaB2CProdutosCamposAdicionaisDados = 8284,           //Script084_Insert_LinxMicrovix_B2C_Product_Fields_Jobs_Into_Jobs_Table.sql
        B2CConsultaB2CProdutosCamposAdicionaisMedia = 8285,           //Script084_Insert_LinxMicrovix_B2C_Product_Fields_Jobs_Into_Jobs_Table.sql
        B2CConsultaB2CProdutosCamposAdicionaisMediaDados = 8286,      //Script084_Insert_LinxMicrovix_B2C_Product_Fields_Jobs_Into_Jobs_Table.sql
        B2CConsultaB2CProdutosCamposAdicionaisOpcoes = 8287,          //Script084_Insert_LinxMicrovix_B2C_Product_Fields_Jobs_Into_Jobs_Table.sql

        /*B2C - LinxMicrovixB2CStocksAndPricesService*/
        B2CConsultaProdutosCustos = 8302,                             //Script087_Insert_LinxMicrovix_B2C_Stocks_And_Prices_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosDepositos = 8303,                          //Script087_Insert_LinxMicrovix_B2C_Stocks_And_Prices_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosDetalhes = 8304,                           //Script087_Insert_LinxMicrovix_B2C_Stocks_And_Prices_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosDetalhesDepositos = 8305,                  //Script087_Insert_LinxMicrovix_B2C_Stocks_And_Prices_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosPromocao = 8306,                           //Script087_Insert_LinxMicrovix_B2C_Stocks_And_Prices_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosTabelas = 8307,                            //Script087_Insert_LinxMicrovix_B2C_Stocks_And_Prices_Jobs_Into_Jobs_Table.sql
        B2CConsultaProdutosTabelasPrecos = 8308,                      //Script087_Insert_LinxMicrovix_B2C_Stocks_And_Prices_Jobs_Into_Jobs_Table.sql

        /*B2C - LinxMicrovixB2COrdersService*/
        B2CConsultaPedidos = 8309,                                    //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaPedidosItens = 8310,                               //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaPedidosPlanos = 8311,                              //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaPedidosStatus = 8312,                              //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaPedidosIdentificador = 8313,                       //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaPedidosTipos = 8314,                               //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaStatus = 8315,                                     //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaCodigoRastreio = 8316,                             //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaNFe = 8317,                                        //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql
        B2CConsultaNFeSituacao = 8318,                                //Script090_Insert_LinxMicrovix_B2C_Orders_Jobs_Into_Jobs_Table.sql

        /*Sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
        LinxClientesFornecByDocCliente = 8191,                        //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxMovimentoByDocumento = 8192,                              //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxMovimentoCartoesByIdentificador = 8193,                   //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxMovimentoPlanosByIdentificador = 8194,                    //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxMovimentoTrocasByDocOrigem = 8195,                        //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxPlanosByPlano = 8196,                                     //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxNaturezaOperacaoByCodNaturezaOperacao = 8197,             //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxProdutosCodBarByCodProduto = 8198,                        //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxProdutosTabelasByIdTabela = 8199,                         //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxProdutosTabelasPrecosByIdTabelaAndCodProduto = 8200,      //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxProdutosDepositosByCodDeposito = 8201,                    //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxProdutosInventarioByCodProduto = 8202,                    //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxProdutosDetalhesByCodProduto = 8203,                      //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxProdutosCamposAdicionaisByCodProduto = 8204,              //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxProdutosByCodProduto = 8205,                              //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxPedidosVendaByCodPedido = 8206,                           //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxSetoresByIdSetor = 8207,                                  //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxXMLDocumentosByDocumento = 8208,                          //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        LinxVendedoresByCodVendedor = 8209,                           //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        B2CConsultaClientesByDocCliente = 8319,                       //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        B2CConsultaNFeByIdPedido = 8320,                              //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        B2CConsultaPedidosByIdPedido = 8321,                          //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        B2CConsultaPedidosItensByIdPedido = 8322,                     //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
        B2CConsultaPedidosStatusByIdPedido = 8323                     //Script092_Insert_LinxMicrovix_On_Demand_Jobs_Into_Jobs_Table.sql
    }
}

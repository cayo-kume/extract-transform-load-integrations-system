using Domain.Core.Abstractions.Repositorys;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Product;
using Domain.LinxCommerce.Entities.Sku;
using Domain.LinxCommerce.Interfaces.Repositorys;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class SKURepository : ISKURepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;

        public SKURepository(IIntegrationsCoreRepository coreRepository) =>
            _coreRepository = coreRepository;

        public bool BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<Sku> records, Guid? parentExecutionGUID)
        {
            try
            {
                var skuTable = _coreRepository.CreateSystemDataTable("SKU", new Sku());
                var metadataValuesTable = _coreRepository.CreateSystemDataTable("SKUMetadataValue", new Domain.LinxCommerce.Entities.Sku.MetadataValue());
                var parentRelationsTable = _coreRepository.CreateSystemDataTable("SKUParentRelation", new ParentRelation());

                for (int i = 0; i < records.Count(); i++)
                {
                    skuTable.Rows.Add(records[i].lastupdateon, records[i].UPC, records[i].DisplayCondition, records[i].DefinitionID, records[i].SuppliersIDs, records[i].ParentsIDs, records[i].ConditionID, records[i].UnitOfMeasureID, records[i].ManageStock, records[i].MinimumQtyAllowed,
                        records[i].MaximumQtyAllowed, records[i].Preorderable, records[i].PreorderDate, records[i].PreorderLimit, records[i].Backorderable, records[i].BackorderLimit, records[i].PurchasingPolicyID,
                        records[i].WrappingQty, records[i].Weight, records[i].Height, records[i].Width, records[i].Depth, records[i].Cost, records[i].Tax, records[i].IsVisible, records[i].VisibleFrom, records[i].VisibleTo,
                        records[i].IsDeleted, records[i].ProductID, records[i].Name, records[i].SKU, records[i].CreatedDate, records[i].CreatedBy, records[i].ModifiedDate, records[i].ModifiedBy, records[i].IntegrationID);

                    for (int j = 0; j < records[i].MetadataValues.Count(); j++)
                    {
                        metadataValuesTable.Rows.Add(records[i].MetadataValues[j].lastupdateon, records[i].MetadataValues[j].ProductID, records[i].MetadataValues[j].PropertyMetadataID, records[i].MetadataValues[j].PropertyName, records[i].MetadataValues[j].PropertyGroup,
                            records[i].MetadataValues[j].InputType, records[i].MetadataValues[j].Value, records[i].MetadataValues[j].SerializedValue, records[i].MetadataValues[j].SerializedBlobValue, records[i].MetadataValues[j].IntegrationID, records[i].MetadataValues[j].FormattedValue,
                            records[i].MetadataValues[j].DisplayName
                        );
                    }

                    for (int h = 0; h < records[i].ParentRelations.Count(); h++)
                    {
                        parentRelationsTable.Rows.Add(records[i].ParentRelations[h].lastupdateon, records[i].ParentRelations[h].ParentID, records[i].ParentRelations[h].ParentSKU, records[i].ParentRelations[h].ParentIntegrationID, records[i].ParentRelations[h].ProductID);
                    }
                };

                _coreRepository.BulkInsertIntoTableRaw(dataTable: skuTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SKU", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: metadataValuesTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SKUMetadataValue", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: parentRelationsTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SKUParentRelation", parentExecutionGUID);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<Product> records, Guid? parentExecutionGUID)
        {
            try
            {
                var productTable = _coreRepository.CreateSystemDataTable("Product", new Product());
                var imageTable = _coreRepository.CreateSystemDataTable("ProductMediaImage", new Image());
                var mediaAssociationTable = _coreRepository.CreateSystemDataTable("ProductMediaAssociations", new MediaAssociation());
                var metadataValueTable = _coreRepository.CreateSystemDataTable("ProductMetadataValues", new Domain.LinxCommerce.Entities.Product.MetadataValue());
                var midiaTable = _coreRepository.CreateSystemDataTable("ProductMedia", new Midia());
                var videoTable = _coreRepository.CreateSystemDataTable("ProductMediaVideo", new Video());

                for (int i = 0; i < records.Count(); i++)
                {
                    productTable.Rows.Add(records[i].lastupdateon, records[i].BrandID, records[i].SkusIDs, records[i].FlagsIDs, records[i].RatingSetID, records[i].MediasIDs, records[i].CategoriesIDs, records[i].CatalogItemType, records[i].PurchasingPolicyID, records[i].IsFreeShipping,
                        records[i].ShippingRegionsIDs, records[i].SearchKeywords, records[i].PageTitle, records[i].UrlFriendly, records[i].MetaDescription, records[i].MetaKeywords, records[i].ShortDescription,
                        records[i].LongDescription, records[i].WarrantyDescription, records[i].TagsIDs, records[i].IsVisible, records[i].VisibleFrom, records[i].VisibleTo, records[i].IsSearchable, records[i].IsUponRequest, records[i].DisplayAvailability, records[i].DisplayStockQty,
                        records[i].DisplayPrice, records[i].IsNew, records[i].NewFrom, records[i].NewTo, records[i].IsGift, records[i].UseAcceptanceTerm, records[i].AcceptanceTermID, records[i].DefinitionID, records[i].IsDeleted, records[i].SendToMarketplace, records[i].ProductID,
                        records[i].Name, records[i].SKU, records[i].CreatedDate, records[i].CreatedBy, records[i].ModifiedDate, records[i].ModifiedBy, records[i].IntegrationID);

                    for (int j = 0; j < records[i].MetadataValues.Count(); j++)
                    {
                        metadataValueTable.Rows.Add(records[i].MetadataValues[j].lastupdateon, records[i].MetadataValues[j].PropertyMetadataID, records[i].MetadataValues[j].ProductID, records[i].MetadataValues[j].PropertyName, records[i].MetadataValues[j].DisplayName,
                            records[i].MetadataValues[j].FormattedValue, records[i].MetadataValues[j].PropertyGroup
                        );
                    }

                    for (int h = 0; h < records[i].Medias.Count(); h++)
                    {
                        midiaTable.Rows.Add(records[i].Medias[h].lastupdateon, records[i].Medias[h].MediaID, records[i].Medias[h].IsDeleted, records[i].Medias[h].Index, records[i].Medias[h].CreatedDate, records[i].Medias[h].Type, records[i].Medias[h].ParentMediaID, records[i].Medias[h].OriginalFileName);
                        imageTable.Rows.Add(records[i].Medias[h].Image.lastupdateon, records[i].Medias[h].Image.RelativePath, records[i].Medias[h].Image.Extension, records[i].Medias[h].Image.MaxWidth, records[i].Medias[h].Image.MaxHeight, records[i].Medias[h].Image.Width, records[i].Medias[h].Image.Height, records[i].Medias[h].Image.MediaSizeType, records[i].Medias[h].Image.AbsolutePath, records[i].Medias[h].Image.MediaID);
                        videoTable.Rows.Add(records[i].Medias[h].Video.lastupdateon, records[i].Medias[h].Video.Title, records[i].Medias[h].Video.Url, records[i].Medias[h].Video.MediaID);
                    }

                    for (int z = 0; z < records[i].MediaAssociations.Count(); z++)
                    {
                        mediaAssociationTable.Rows.Add(records[i].MediaAssociations[z].lastupdateon, records[i].MediaAssociations[z].ID, records[i].MediaAssociations[z].ProductID, records[i].MediaAssociations[z].MediaID, records[i].MediaAssociations[z].Path, records[i].MediaAssociations[z].Order);
                    }
                };

                _coreRepository.BulkInsertIntoTableRaw(dataTable: productTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "Product", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: metadataValueTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "ProductMetadataValues", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: mediaAssociationTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "ProductMediaAssociations", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: midiaTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "ProductMedia", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: videoTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "ProductMediaVideo", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: imageTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "ProductMediaImage", parentExecutionGUID);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task<bool> InsertRecord(LinxCommerceJobParameter jobParameter, Sku? record)
        {
            string? sql = $"INSERT INTO {jobParameter.tableName}_raw " +
                          "([UPC], [DisplayCondition], [DefinitionID], [ConditionID], [UnitOfMeasureID], [ManageStock], [MinimumQtyAllowed], [MaximumQtyAllowed], [Preorderable], [PreorderDate], [PreorderLimit], [Backorderable], " +
                          "[BackorderLimit], [PurchasingPolicyID], [WrappingQty], [Weight], [Height], [Width], [Depth], [Cost], [Tax], [IsVisible], [VisibleFrom], [VisibleTo], [IsDeleted], [ProductID], [Name], [SKU], [CreatedDate], [CreatedBy], " +
                          "[ModifiedDate], [ModifiedBy], [IntegrationID], [lastupdateon]) " +
                          "Values " +
                          "(@UPC, @DisplayCondition, @DefinitionID, @ConditionID, @UnitOfMeasureID, @ManageStock, @MinimumQtyAllowed, @MaximumQtyAllowed, @Preorderable, @PreorderDate, @PreorderLimit, @Backorderable, @BackorderLimit, @PurchasingPolicyID, " +
                          "@WrappingQty, @Weight, @Height, @Width, @Depth, @Cost, @Tax, @IsVisible, @VisibleFrom, @VisibleTo, @IsDeleted, @ProductID, @Name, @SKU, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy, @IntegrationID, GETDATE())";

            try
            {
                return await _coreRepository.InsertRecord(sql: sql, entity: record);
            }
            catch
            {
                throw;
            }
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RPFO.Data.Entities;

#nullable disable

namespace RPFO.Data.Context
{
    public partial class JumpContext : DbContext
    {
        public JumpContext()
        {
        }

        public JumpContext(DbContextOptions<JumpContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MArea> MAreas { get; set; }
        public virtual DbSet<MBomgroupOption> MBomgroupOptions { get; set; }
        public virtual DbSet<MBomheader> MBomheaders { get; set; }
        public virtual DbSet<MBomline> MBomlines { get; set; }
        public virtual DbSet<MCompany> MCompanies { get; set; }
        public virtual DbSet<MControl> MControls { get; set; }
        public virtual DbSet<MCountry> MCountries { get; set; }
        public virtual DbSet<MCurrency> MCurrencies { get; set; }
        public virtual DbSet<MCustomer> MCustomers { get; set; }
        public virtual DbSet<MCustomerGroup> MCustomerGroups { get; set; }
        public virtual DbSet<MEmployee> MEmployees { get; set; }
        public virtual DbSet<MEmployeeStore> MEmployeeStores { get; set; }
        public virtual DbSet<MFunction> MFunctions { get; set; }
        public virtual DbSet<MItem> MItems { get; set; }
        public virtual DbSet<MItemGroup> MItemGroups { get; set; }
        public virtual DbSet<MItemSerial> MItemSerials { get; set; }
        public virtual DbSet<MItemSerialStock> MItemSerialStocks { get; set; }
        public virtual DbSet<MItemUom> MItemUoms { get; set; }
        public virtual DbSet<MMerchandiseCategory> MMerchandiseCategories { get; set; }
        public virtual DbSet<MPaymentMethod> MPaymentMethods { get; set; }
        public virtual DbSet<MPermission> MPermissions { get; set; }
        public virtual DbSet<MPriceList> MPriceLists { get; set; }
        public virtual DbSet<MProduct> MProducts { get; set; }
        public virtual DbSet<MProductOption1> MProductOption1s { get; set; }
        public virtual DbSet<MProductOption10> MProductOption10s { get; set; }
        public virtual DbSet<MProductOption2> MProductOption2s { get; set; }
        public virtual DbSet<MProductOption3> MProductOption3s { get; set; }
        public virtual DbSet<MProductOption4> MProductOption4s { get; set; }
        public virtual DbSet<MProductOption5> MProductOption5s { get; set; }
        public virtual DbSet<MProductOption6> MProductOption6s { get; set; }
        public virtual DbSet<MProductOption7> MProductOption7s { get; set; }
        public virtual DbSet<MProductOption8> MProductOption8s { get; set; }
        public virtual DbSet<MProductOption9> MProductOption9s { get; set; }
        public virtual DbSet<MProductVariant> MProductVariants { get; set; }
        public virtual DbSet<MPromoType> MPromoTypes { get; set; }
        public virtual DbSet<MProvince> MProvinces { get; set; }
        public virtual DbSet<MRegion> MRegions { get; set; }
        public virtual DbSet<MRole> MRoles { get; set; }
        public virtual DbSet<MStorage> MStorages { get; set; }
        public virtual DbSet<MStore> MStores { get; set; }
        public virtual DbSet<MStoreArea> MStoreAreas { get; set; }
        public virtual DbSet<MStoreCapacity> MStoreCapacities { get; set; }
        public virtual DbSet<MStoreGroup> MStoreGroups { get; set; }
        public virtual DbSet<MStorePayment> MStorePayments { get; set; }
        public virtual DbSet<MTax> MTaxes { get; set; }
        public virtual DbSet<MTimeFrame> MTimeFrames { get; set; }
        public virtual DbSet<MTransactionType> MTransactionTypes { get; set; }
        public virtual DbSet<MUom> MUoms { get; set; }
        public virtual DbSet<MUser> MUsers { get; set; }
        public virtual DbSet<MUserLicense> MUserLicenses { get; set; }
        public virtual DbSet<MUserRole> MUserRoles { get; set; }
        public virtual DbSet<MUserStore> MUserStores { get; set; }
        public virtual DbSet<MWarehouse> MWarehouses { get; set; }
        public virtual DbSet<SConfigType> SConfigTypes { get; set; }
        public virtual DbSet<SFormatConfig> SFormatConfigs { get; set; }
        public virtual DbSet<SGeneralSetting> SGeneralSettings { get; set; }
        public virtual DbSet<SLicense> SLicenses { get; set; }
        public virtual DbSet<SLicenseType> SLicenseTypes { get; set; }
        public virtual DbSet<SPermission> SPermissions { get; set; }
        public virtual DbSet<SPersonalSetting> SPersonalSettings { get; set; }
        public virtual DbSet<SPromoBuy> SPromoBuys { get; set; }
        public virtual DbSet<SPromoCustomer> SPromoCustomers { get; set; }
        public virtual DbSet<SPromoGet> SPromoGets { get; set; }
        public virtual DbSet<SPromoHeader> SPromoHeaders { get; set; }
        public virtual DbSet<SPromoSchema> SPromoSchemas { get; set; }
        public virtual DbSet<SSchemaLine> SSchemaLines { get; set; }
        public virtual DbSet<SStatus> SStatuses { get; set; }
        public virtual DbSet<TCapacityRemain> TCapacityRemains { get; set; }
        public virtual DbSet<TCapacityTransaction> TCapacityTransactions { get; set; }
        public virtual DbSet<TGoodsIssueHeader> TGoodsIssueHeaders { get; set; }
        public virtual DbSet<TGoodsIssueLine> TGoodsIssueLines { get; set; }
        public virtual DbSet<TGoodsIssueLineSerial> TGoodsIssueLineSerials { get; set; }
        public virtual DbSet<TGoodsReceiptHeader> TGoodsReceiptHeaders { get; set; }
        public virtual DbSet<TGoodsReceiptLine> TGoodsReceiptLines { get; set; }
        public virtual DbSet<TGoodsReceiptLineSerial> TGoodsReceiptLineSerials { get; set; }
        public virtual DbSet<TGoodsReceiptPoheader> TGoodsReceiptPoheaders { get; set; }
        public virtual DbSet<TGoodsReceiptPoline> TGoodsReceiptPolines { get; set; }
        public virtual DbSet<TInventoryCountingHeader> TInventoryCountingHeaders { get; set; }
        public virtual DbSet<TInventoryCountingLine> TInventoryCountingLines { get; set; }
        public virtual DbSet<TInventoryCountingLineSerial> TInventoryCountingLineSerials { get; set; }
        public virtual DbSet<TInventoryHeader> TInventoryHeaders { get; set; }
        public virtual DbSet<TInventoryLine> TInventoryLines { get; set; }
        public virtual DbSet<TInventoryLineSerial> TInventoryLineSerials { get; set; }
        public virtual DbSet<TInventoryPostingHeader> TInventoryPostingHeaders { get; set; }
        public virtual DbSet<TInventoryPostingLine> TInventoryPostingLines { get; set; }
        public virtual DbSet<TInventoryPostingLineSerial> TInventoryPostingLineSerials { get; set; }
        public virtual DbSet<TItemSerial> TItemSerials { get; set; }
        public virtual DbSet<TItemStorage> TItemStorages { get; set; }
        public virtual DbSet<TOrderHeader> TOrderHeaders { get; set; }
        public virtual DbSet<TOrderLine> TOrderLines { get; set; }
        public virtual DbSet<TPurchaseOrderHeader> TPurchaseOrderHeaders { get; set; }
        public virtual DbSet<TPurchaseOrderLine> TPurchaseOrderLines { get; set; }
        public virtual DbSet<TPurchaseOrderLineSerial> TPurchaseOrderLineSerials { get; set; }
        public virtual DbSet<TSalesHeader> TSalesHeaders { get; set; }
        public virtual DbSet<TSalesLine> TSalesLines { get; set; }
        public virtual DbSet<TSalesLineSerial> TSalesLineSerials { get; set; }
        public virtual DbSet<TSalesPayment> TSalesPayments { get; set; }
        public virtual DbSet<TSalesPromo> TSalesPromos { get; set; }
        public virtual DbSet<TShiftHeader> TShiftHeaders { get; set; }
        public virtual DbSet<TShiftLine> TShiftLines { get; set; }
        public virtual DbSet<TStoreDaily> TStoreDailies { get; set; }
        public virtual DbSet<TTransactionLog> TTransactionLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=192.168.1.83;Database=RTFO_POS_NEW;user id=sa;password=sa@787&*&;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MArea>(entity =>
            {
                entity.HasKey(e => e.AreaCode);

                entity.ToTable("M_Area");

                entity.Property(e => e.AreaCode).HasMaxLength(50);

                entity.Property(e => e.AreaName).HasMaxLength(250);

                entity.Property(e => e.ForeignName).HasMaxLength(250);

                entity.Property(e => e.RegionCode).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MBomgroupOption>(entity =>
            {
                entity.ToTable("M_BOMGroupOption");

                entity.Property(e => e.Id).HasMaxLength(10);

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Status).HasMaxLength(1);
            });

            modelBuilder.Entity<MBomheader>(entity =>
            {
                entity.HasKey(e => new { e.ItemCode, e.CompanyCode });

                entity.ToTable("M_BOMHeader");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ItemName).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<MBomline>(entity =>
            {
                entity.HasKey(e => new { e.Bomid, e.ItemCode, e.CompanyCode });

                entity.ToTable("M_BOMLine");

                entity.Property(e => e.Bomid)
                    .HasMaxLength(50)
                    .HasColumnName("BOMId");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ItemName).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionGroup).HasMaxLength(10);

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<MCompany>(entity =>
            {
                entity.HasKey(e => e.CompanyCode);

                entity.ToTable("M_Company");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CompanyName).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MControl>(entity =>
            {
                entity.HasKey(e => new { e.ControlId, e.CompanyCode, e.FunctionId });

                entity.ToTable("M_Control");

                entity.Property(e => e.ControlId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.FunctionId).HasMaxLength(50);

                entity.Property(e => e.Action).HasMaxLength(50);

                entity.Property(e => e.ControlName).HasMaxLength(250);

                entity.Property(e => e.ControlType).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Custom1).HasMaxLength(150);

                entity.Property(e => e.Custom2).HasMaxLength(150);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionKey).HasMaxLength(50);

                entity.Property(e => e.OptionName).HasMaxLength(50);

                entity.Property(e => e.OptionValue).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MCountry>(entity =>
            {
                entity.HasKey(e => e.CountryCode);

                entity.ToTable("M_Country");

                entity.Property(e => e.CountryCode).HasMaxLength(50);

                entity.Property(e => e.AreaCode).HasMaxLength(50);

                entity.Property(e => e.CountryName).HasMaxLength(250);

                entity.Property(e => e.ForeignName).HasMaxLength(250);
            });

            modelBuilder.Entity<MCurrency>(entity =>
            {
                entity.HasKey(e => e.CurrencyCode)
                    .HasName("PK_M_Currency_1");

                entity.ToTable("M_Currency");

                entity.Property(e => e.CurrencyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrencyName).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Rounding).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MCustomer>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.CompanyCode });

                entity.ToTable("M_Customer");

                entity.Property(e => e.CustomerId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerGrpId).HasMaxLength(50);

                entity.Property(e => e.CustomerName).HasMaxLength(250);

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.JoinedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MCustomerGroup>(entity =>
            {
                entity.HasKey(e => new { e.CusGrpId, e.CompanyCode });

                entity.ToTable("M_CustomerGroup");

                entity.Property(e => e.CusGrpId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CusGrpDesc).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MEmployee>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.CompanyCode });

                entity.ToTable("M_Employee");

                entity.Property(e => e.EmployeeId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmployeeName).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Position).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MEmployeeStore>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.StoreId });

                entity.ToTable("M_EmployeeStore");

                entity.Property(e => e.EmployeeId).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.ToDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<MFunction>(entity =>
            {
                entity.HasKey(e => new { e.FunctionId, e.CompanyCode });

                entity.ToTable("M_Function");

                entity.Property(e => e.FunctionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Icon).HasMaxLength(150);

                entity.Property(e => e.LicenseType).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.ParentId).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Url).HasMaxLength(250);
            });

            modelBuilder.Entity<MItem>(entity =>
            {
                entity.HasKey(e => new { e.ItemCode, e.CompanyCode, e.ProductId, e.VariantId });

                entity.ToTable("M_Item");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ProductId).HasMaxLength(50);

                entity.Property(e => e.VariantId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomField1).HasMaxLength(250);

                entity.Property(e => e.CustomField10).HasMaxLength(250);

                entity.Property(e => e.CustomField2).HasMaxLength(250);

                entity.Property(e => e.CustomField3).HasMaxLength(250);

                entity.Property(e => e.CustomField4).HasMaxLength(250);

                entity.Property(e => e.CustomField5).HasMaxLength(250);

                entity.Property(e => e.CustomField6).HasMaxLength(250);

                entity.Property(e => e.CustomField7).HasMaxLength(250);

                entity.Property(e => e.CustomField8).HasMaxLength(250);

                entity.Property(e => e.CustomField9).HasMaxLength(250);

                entity.Property(e => e.DefaultPrice).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ForeignName).HasMaxLength(250);

                entity.Property(e => e.ImageLink).HasMaxLength(250);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(250)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.InventoryUom)
                    .HasMaxLength(50)
                    .HasColumnName("InventoryUOM");

                entity.Property(e => e.IsBom).HasColumnName("IsBOM");

                entity.Property(e => e.ItemCategory1)
                    .HasMaxLength(50)
                    .HasColumnName("ItemCategory_1");

                entity.Property(e => e.ItemCategory2)
                    .HasMaxLength(50)
                    .HasColumnName("ItemCategory_2");

                entity.Property(e => e.ItemCategory3)
                    .HasMaxLength(50)
                    .HasColumnName("ItemCategory_3");

                entity.Property(e => e.ItemDescription).HasMaxLength(250);

                entity.Property(e => e.ItemGroupId).HasMaxLength(50);

                entity.Property(e => e.ItemName).HasMaxLength(250);

                entity.Property(e => e.Mcid)
                    .HasMaxLength(50)
                    .HasColumnName("MCId");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PurchaseTaxCode).HasMaxLength(50);

                entity.Property(e => e.SalesTaxCode).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ValidFrom).HasColumnType("date");

                entity.Property(e => e.ValidTo).HasColumnType("date");
            });

            modelBuilder.Entity<MItemGroup>(entity =>
            {
                entity.HasKey(e => new { e.Igid, e.CompanyCode });

                entity.ToTable("M_ItemGroup");

                entity.Property(e => e.Igid)
                    .HasMaxLength(50)
                    .HasColumnName("IGId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Igdescription)
                    .HasMaxLength(250)
                    .HasColumnName("IGDescription");

                entity.Property(e => e.Igname)
                    .HasMaxLength(250)
                    .HasColumnName("IGName");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MItemSerial>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.ItemCode, e.SerialNum });

                entity.ToTable("M_ItemSerial");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpDate).HasColumnType("date");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoredDate).HasColumnType("date");
            });

            modelBuilder.Entity<MItemSerialStock>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.ItemCode, e.SlocId, e.SerialNum });

                entity.ToTable("M_ItemSerialStock");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StockQty).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<MItemUom>(entity =>
            {
                entity.HasKey(e => new { e.ItemCode, e.Uomcode })
                    .HasName("PK_M_ItemUOM_1");

                entity.ToTable("M_ItemUOM");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Factor).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Qrcode)
                    .HasMaxLength(250)
                    .HasColumnName("QRCode");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MMerchandiseCategory>(entity =>
            {
                entity.HasKey(e => new { e.Mcid, e.CompanyCode });

                entity.ToTable("M_MerchandiseCategory");

                entity.Property(e => e.Mcid)
                    .HasMaxLength(50)
                    .HasColumnName("MCId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Mchier)
                    .HasMaxLength(50)
                    .HasColumnName("MCHier");

                entity.Property(e => e.Mcname)
                    .HasMaxLength(250)
                    .HasColumnName("MCName");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.MMerchandiseCategoryNavigation)
                    .WithMany(p => p.InverseMMerchandiseCategoryNavigation)
                    .HasForeignKey(d => new { d.Mchier, d.CompanyCode })
                    .HasConstraintName("FK_M_MerchandiseCategory_M_MerchandiseCategory");
            });

            modelBuilder.Entity<MPaymentMethod>(entity =>
            {
                entity.HasKey(e => new { e.PaymentCode, e.CompanyCode });

                entity.ToTable("M_PaymentMethod");

                entity.Property(e => e.PaymentCode).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.AccountCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PaymentDesc).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MPermission>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.PermissionId });

                entity.ToTable("M_Permission");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ControlId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FunctionId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Permissions).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MPriceList>(entity =>
            {
                entity.HasKey(e => new { e.PriceListId, e.CompanyCode, e.StoreId });

                entity.ToTable("M_PriceList");

                entity.Property(e => e.PriceListId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.BarCode)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ItemCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PriceAfterTax).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.PriceBeforeTax).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.ValidFrom).HasColumnType("datetime");

                entity.Property(e => e.ValidTo).HasColumnType("datetime");
            });

            modelBuilder.Entity<MProduct>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.ProductId });

                entity.ToTable("M_Product");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ProductId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.ProductName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption1>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption1");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption10>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption10");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption2>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption2");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption3>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption3");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption4>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption4");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption5>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption5");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption6>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption6");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption7>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption7");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption8>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption8");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductOption9>(entity =>
            {
                entity.HasKey(e => new { e.OptionId, e.CompanyCode });

                entity.ToTable("M_ProductOption9");

                entity.Property(e => e.OptionId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OptionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProductVariant>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.VariantId, e.CompanyCode })
                    .HasName("PK_M_ProductOption");

                entity.ToTable("M_ProductVariant");

                entity.Property(e => e.ProductId).HasMaxLength(50);

                entity.Property(e => e.VariantId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Option1).HasMaxLength(50);

                entity.Property(e => e.Option10).HasMaxLength(50);

                entity.Property(e => e.Option2).HasMaxLength(50);

                entity.Property(e => e.Option3).HasMaxLength(50);

                entity.Property(e => e.Option4).HasMaxLength(50);

                entity.Property(e => e.Option5).HasMaxLength(50);

                entity.Property(e => e.Option6).HasMaxLength(50);

                entity.Property(e => e.Option7).HasMaxLength(50);

                entity.Property(e => e.Option8).HasMaxLength(50);

                entity.Property(e => e.Option9).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MPromoType>(entity =>
            {
                entity.HasKey(e => e.PromoType);

                entity.ToTable("M_PromoType");

                entity.Property(e => e.PromoType).ValueGeneratedNever();

                entity.Property(e => e.PromoName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MProvince>(entity =>
            {
                entity.HasKey(e => new { e.ProvinceId, e.DistrictId, e.WardId })
                    .HasName("PK_M_Provinces");

                entity.ToTable("M_Province");

                entity.Property(e => e.ProvinceId).HasMaxLength(50);

                entity.Property(e => e.DistrictId).HasMaxLength(50);

                entity.Property(e => e.WardId).HasMaxLength(50);

                entity.Property(e => e.AreaCode).HasMaxLength(50);

                entity.Property(e => e.CountryCode).HasMaxLength(50);

                entity.Property(e => e.DistrictName).HasMaxLength(250);

                entity.Property(e => e.ForeignName).HasMaxLength(250);

                entity.Property(e => e.ProvinceName).HasMaxLength(250);

                entity.Property(e => e.RegionCode).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.WardLevel).HasMaxLength(50);

                entity.Property(e => e.WardName).HasMaxLength(250);
            });

            modelBuilder.Entity<MRegion>(entity =>
            {
                entity.HasKey(e => e.RegionCode);

                entity.ToTable("M_Region");

                entity.Property(e => e.RegionCode).HasMaxLength(50);

                entity.Property(e => e.CountryCode).HasMaxLength(50);

                entity.Property(e => e.ForeignName).HasMaxLength(250);

                entity.Property(e => e.RegionName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MRole>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.CompanyCode })
                    .HasName("PK_M_Roles");

                entity.ToTable("M_Role");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.RoleName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MStorage>(entity =>
            {
                entity.HasKey(e => new { e.SlocId, e.CompanyCode, e.WhsCode });

                entity.ToTable("M_Storage");

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.WhsCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MStore>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.CompanyCode })
                    .HasName("PK_M_Stores");

                entity.ToTable("M_Store");

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.AreaCode).HasMaxLength(50);

                entity.Property(e => e.CountryCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrencyCode).HasMaxLength(50);

                entity.Property(e => e.CustomField1).HasMaxLength(250);

                entity.Property(e => e.CustomField2).HasMaxLength(250);

                entity.Property(e => e.CustomField3).HasMaxLength(250);

                entity.Property(e => e.CustomField4).HasMaxLength(250);

                entity.Property(e => e.CustomField5).HasMaxLength(250);

                entity.Property(e => e.DefaultCusId).HasMaxLength(50);

                entity.Property(e => e.DistrictId).HasMaxLength(50);

                entity.Property(e => e.ForeignName).HasMaxLength(250);

                entity.Property(e => e.FormatConfigId).HasMaxLength(50);

                entity.Property(e => e.ListType).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.ProvinceId).HasMaxLength(50);

                entity.Property(e => e.RegionCode).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreDescription).HasMaxLength(250);

                entity.Property(e => e.StoreGroupId).HasMaxLength(50);

                entity.Property(e => e.StoreName).HasMaxLength(250);

                entity.Property(e => e.StoreType).HasMaxLength(50);

                entity.Property(e => e.WardId).HasMaxLength(50);

                entity.Property(e => e.WhsCode).HasMaxLength(50);
            });

            modelBuilder.Entity<MStoreArea>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.StoreAreaId })
                    .HasName("PK_M_StoreArea_1");

                entity.ToTable("M_StoreArea");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreAreaId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreAreaName).HasMaxLength(250);

                entity.Property(e => e.StoreAreaType).HasMaxLength(50);
            });

            modelBuilder.Entity<MStoreCapacity>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.StoreId, e.StoreAreaId, e.TimeFrameId });

                entity.ToTable("M_StoreCapacity");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.StoreAreaId).HasMaxLength(50);

                entity.Property(e => e.TimeFrameId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MStoreGroup>(entity =>
            {
                entity.HasKey(e => new { e.StoreGroupId, e.CompanyCode });

                entity.ToTable("M_StoreGroup");

                entity.Property(e => e.StoreGroupId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreGroupName).HasMaxLength(250);
            });

            modelBuilder.Entity<MStorePayment>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.PaymentCode });

                entity.ToTable("M_StorePayment");

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.PaymentCode).HasMaxLength(50);

                entity.Property(e => e.IsShow).HasColumnName("isShow");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MTax>(entity =>
            {
                entity.HasKey(e => new { e.TaxCode, e.CompanyCode });

                entity.ToTable("M_Tax");

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TaxName).HasMaxLength(50);

                entity.Property(e => e.TaxPercent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TaxType).HasMaxLength(50);
            });

            modelBuilder.Entity<MTimeFrame>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.TimeFrameId });

                entity.ToTable("M_TimeFrame");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.TimeFrameId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Duration).HasComputedColumnSql("(datediff(minute,[StartTime],[EndTime]))", true);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MTransactionType>(entity =>
            {
                entity.HasKey(e => e.TransType);

                entity.ToTable("M_TransactionType");

                entity.Property(e => e.TransType).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MUom>(entity =>
            {
                entity.HasKey(e => new { e.Uomcode, e.CompanyCode });

                entity.ToTable("M_UOM");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomname)
                    .HasMaxLength(250)
                    .HasColumnName("UOMName");
            });

            modelBuilder.Entity<MUser>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CompanyCode })
                    .HasName("PK_M_Users");

                entity.ToTable("M_User");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastLoginStoreId).HasMaxLength(50);

                entity.Property(e => e.LastLoginStoreLang).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<MUserLicense>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LicenseId });

                entity.ToTable("M_UserLicense");

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.Property(e => e.LicenseId).HasMaxLength(50);
            });

            modelBuilder.Entity<MUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK_M_UserRoles");

                entity.ToTable("M_UserRole");

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.Property(e => e.RoleId).HasMaxLength(50);
            });

            modelBuilder.Entity<MUserStore>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.StoreId })
                    .HasName("PK_M_UserStores");

                entity.ToTable("M_UserStore");

                entity.Property(e => e.StoreId).HasMaxLength(50);
            });

            modelBuilder.Entity<MWarehouse>(entity =>
            {
                entity.HasKey(e => new { e.WhsCode, e.CompanyCode })
                    .HasName("PK_M_Warehouses");

                entity.ToTable("M_Warehouse");

                entity.Property(e => e.WhsCode).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.WhsName).HasMaxLength(250);

                entity.Property(e => e.WhsType).HasMaxLength(50);
            });

            modelBuilder.Entity<SConfigType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("S_ConfigType");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.Status).HasMaxLength(1);
            });

            modelBuilder.Entity<SFormatConfig>(entity =>
            {
                entity.HasKey(e => new { e.FormatId, e.CompanyCode })
                    .HasName("PK_M_FormatConfig");

                entity.ToTable("S_FormatConfig");

                entity.Property(e => e.FormatId).ValueGeneratedOnAdd();

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateFormat).HasMaxLength(50);

                entity.Property(e => e.DecimalFormat).HasMaxLength(50);

                entity.Property(e => e.DecimalPlacesFormat).HasMaxLength(50);

                entity.Property(e => e.FormatName).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.SetupCode).HasMaxLength(50);

                entity.Property(e => e.SetupType).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ThousandFormat).HasMaxLength(50);
            });

            modelBuilder.Entity<SGeneralSetting>(entity =>
            {
                entity.HasKey(e => new { e.SettingId, e.CompanyCode })
                    .HasName("PK_M_GeneralSetting");

                entity.ToTable("S_GeneralSetting");

                entity.Property(e => e.SettingId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.SettingDescription).HasMaxLength(250);

                entity.Property(e => e.SettingName).HasMaxLength(250);

                entity.Property(e => e.SettingValue).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.TokenExpired).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ValueType).HasMaxLength(50);
            });

            modelBuilder.Entity<SLicense>(entity =>
            {
                entity.HasKey(e => new { e.LicenseId, e.CompanyCode, e.LicenseType })
                    .HasName("PK_S_License_1");

                entity.ToTable("S_License");

                entity.Property(e => e.LicenseId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.LicenseType).HasMaxLength(50);

                entity.Property(e => e.LicenseAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.LicenseCode).HasMaxLength(250);

                entity.Property(e => e.LicenseDesc).HasMaxLength(250);

                entity.Property(e => e.LicenseRemain).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ValidFrom).HasColumnType("datetime");

                entity.Property(e => e.ValidTo).HasColumnType("datetime");
            });

            modelBuilder.Entity<SLicenseType>(entity =>
            {
                entity.HasKey(e => new { e.LicenseType, e.CompanyCode });

                entity.ToTable("S_LicenseType");

                entity.Property(e => e.LicenseType).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(250);
            });

            modelBuilder.Entity<SPermission>(entity =>
            {
                entity.ToTable("S_Permission");

                entity.Property(e => e.Id).HasMaxLength(10);

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<SPersonalSetting>(entity =>
            {
                entity.HasKey(e => new { e.SettingId, e.CompanyCode })
                    .HasName("PK_PersonalSetting");

                entity.ToTable("S_PersonalSetting");

                entity.Property(e => e.SettingId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FunctionId).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.SettingName).HasMaxLength(50);

                entity.Property(e => e.SettingType).HasMaxLength(50);

                entity.Property(e => e.SettingValue).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<SPromoBuy>(entity =>
            {
                entity.HasKey(e => new { e.PromoId, e.CompanyCode, e.LineNum });

                entity.ToTable("S_PromoBuy");

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.Condition1)
                    .HasMaxLength(10)
                    .HasColumnName("Condition_1");

                entity.Property(e => e.Condition2)
                    .HasMaxLength(10)
                    .HasColumnName("Condition_2");

                entity.Property(e => e.LineCode).HasMaxLength(50);

                entity.Property(e => e.LineName).HasMaxLength(250);

                entity.Property(e => e.LineType).HasMaxLength(50);

                entity.Property(e => e.LineUom).HasMaxLength(50);

                entity.Property(e => e.Value1)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("Value_1");

                entity.Property(e => e.Value2)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("Value_2");

                entity.Property(e => e.ValueType).HasMaxLength(50);
            });

            modelBuilder.Entity<SPromoCustomer>(entity =>
            {
                entity.HasKey(e => new { e.PromoId, e.CompanyCode, e.LineNum });

                entity.ToTable("S_PromoCustomer");

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CustomerType).HasMaxLength(50);

                entity.Property(e => e.CustomerValue).HasMaxLength(50);
            });

            modelBuilder.Entity<SPromoGet>(entity =>
            {
                entity.HasKey(e => new { e.PromoId, e.CompanyCode, e.LineNum });

                entity.ToTable("S_PromoGet");

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.Condition1)
                    .HasMaxLength(10)
                    .HasColumnName("Condition_1");

                entity.Property(e => e.Condition2)
                    .HasMaxLength(50)
                    .HasColumnName("Condition_2");

                entity.Property(e => e.ConditionType).HasMaxLength(50);

                entity.Property(e => e.GetValue).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.LineCode).HasMaxLength(50);

                entity.Property(e => e.LineName).HasMaxLength(250);

                entity.Property(e => e.LineType).HasMaxLength(50);

                entity.Property(e => e.LineUom).HasMaxLength(50);

                entity.Property(e => e.Value1)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("Value_1");

                entity.Property(e => e.Value2)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("Value_2");

                entity.Property(e => e.ValueType).HasMaxLength(50);
            });

            modelBuilder.Entity<SPromoHeader>(entity =>
            {
                entity.HasKey(e => new { e.PromoId, e.CompanyCode });

                entity.ToTable("S_PromoHeader");

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.CustomerType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsCombine)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsFri)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsMon)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsSat)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsSun)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsThu)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsTue)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsWed)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PromoName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TotalBuyFrom).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalBuyTo).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalGetType).HasMaxLength(50);

                entity.Property(e => e.ValidDateFrom).HasColumnType("date");

                entity.Property(e => e.ValidDateTo).HasColumnType("date");
            });

            modelBuilder.Entity<SPromoSchema>(entity =>
            {
                entity.HasKey(e => new { e.SchemaId, e.CompanyCode });

                entity.ToTable("S_PromoSchema");

                entity.Property(e => e.SchemaId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.SchemaName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<SSchemaLine>(entity =>
            {
                entity.HasKey(e => new { e.SchemaId, e.CompanyCode, e.LineNum });

                entity.ToTable("S_SchemaLine");

                entity.Property(e => e.SchemaId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.IsApply)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.PromoId).HasMaxLength(50);
            });

            modelBuilder.Entity<SStatus>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("S_Status");

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.Name).HasMaxLength(150);
            });

            modelBuilder.Entity<TCapacityRemain>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.CompanyCode, e.TimeFrameId, e.StoreAreaId, e.TransDate });

                entity.ToTable("T_CapacityRemain");

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.TimeFrameId).HasMaxLength(50);

                entity.Property(e => e.StoreAreaId).HasMaxLength(50);

                entity.Property(e => e.TransDate).HasColumnType("date");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<TCapacityTransaction>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.TransId, e.StoreId, e.StoreAreaId, e.TimeFrameId, e.TransDate });

                entity.ToTable("T_CapacityTransaction");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.StoreAreaId).HasMaxLength(50);

                entity.Property(e => e.TimeFrameId).HasMaxLength(50);

                entity.Property(e => e.TransDate).HasColumnType("date");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModififedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<TGoodsIssueHeader>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.CompanyCode, e.StoreId })
                    .HasName("PK_T_GoodsIssue");

                entity.ToTable("T_GoodsIssueHeader");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Remark).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TotalDiscountAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalPayable).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalReceipt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalTax).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<TGoodsIssueLine>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.CompanyCode, e.LineId, e.ItemCode, e.SlocId });

                entity.ToTable("T_GoodsIssueLine");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrencyCode).HasMaxLength(50);

                entity.Property(e => e.CurrencyRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Remark).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TaxAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.TaxRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TGoodsIssueLineSerial>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.LineId, e.CompanyCode, e.ItemCode, e.SerialNum, e.SlocId });

                entity.ToTable("T_GoodsIssueLineSerial");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TGoodsReceiptHeader>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.CompanyCode, e.StoreId });

                entity.ToTable("T_GoodsReceiptHeader");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Remark).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TotalDiscountAmt)
                    .HasColumnType("decimal(19, 6)")
                    .HasComment("Không dùng");

                entity.Property(e => e.TotalPayable)
                    .HasColumnType("decimal(19, 6)")
                    .HasComment("Không dùng");

                entity.Property(e => e.TotalReceipt)
                    .HasColumnType("decimal(19, 6)")
                    .HasComment("");

                entity.Property(e => e.TotalTax).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<TGoodsReceiptLine>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.CompanyCode, e.LineId, e.ItemCode, e.SlocId });

                entity.ToTable("T_GoodsReceiptLine");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrencyCode).HasMaxLength(50);

                entity.Property(e => e.CurrencyRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Remark).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TaxAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.TaxRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TGoodsReceiptLineSerial>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.LineId, e.CompanyCode, e.ItemCode, e.SerialNum, e.SlocId });

                entity.ToTable("T_GoodsReceiptLineSerial");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TGoodsReceiptPoheader>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseId, e.CompanyCode, e.StoreId });

                entity.ToTable("T_GoodsReceiptPOHeader");

                entity.Property(e => e.PurchaseId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CardCode).HasMaxLength(50);

                entity.Property(e => e.CardName).HasMaxLength(250);

                entity.Property(e => e.Comment).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DocDate).HasColumnType("datetime");

                entity.Property(e => e.DocDueDate).HasColumnType("datetime");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DocTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.InvoiceAddress).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreName).HasMaxLength(250);

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.Vatpercent)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("VATPercent");

                entity.Property(e => e.Vattotal)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("VATTotal");
            });

            modelBuilder.Entity<TGoodsReceiptPoline>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseId, e.CompanyCode, e.LineId, e.ItemCode, e.SlocId });

                entity.ToTable("T_GoodsReceiptPOLine");

                entity.Property(e => e.PurchaseId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.BaseEntry).HasMaxLength(50);

                entity.Property(e => e.BaseRef).HasMaxLength(50);

                entity.Property(e => e.BaseType).HasMaxLength(50);

                entity.Property(e => e.Comment).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.DiscPercent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.LineStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OpenQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.Vatpercent)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("VATPercent");
            });

            modelBuilder.Entity<TInventoryCountingHeader>(entity =>
            {
                entity.HasKey(e => new { e.Icid, e.CompanyCode, e.StoreId });

                entity.ToTable("T_InventoryCountingHeader");

                entity.Property(e => e.Icid)
                    .HasMaxLength(50)
                    .HasColumnName("ICId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.Comment).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DocDate).HasColumnType("datetime");

                entity.Property(e => e.DocDueDate).HasColumnType("datetime");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DocTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreName).HasMaxLength(250);
            });

            modelBuilder.Entity<TInventoryCountingLine>(entity =>
            {
                entity.HasKey(e => new { e.Icid, e.CompanyCode, e.LineId, e.ItemCode, e.SlocId });

                entity.ToTable("T_InventoryCountingLine");

                entity.Property(e => e.Icid)
                    .HasMaxLength(50)
                    .HasColumnName("ICId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.BaseEntry).HasMaxLength(50);

                entity.Property(e => e.BaseRef).HasMaxLength(50);

                entity.Property(e => e.BaseType).HasMaxLength(50);

                entity.Property(e => e.Comment).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.LineStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OpenQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TotalCount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalDifferent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalStock).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TInventoryCountingLineSerial>(entity =>
            {
                entity.HasKey(e => new { e.Icid, e.LineId, e.CompanyCode, e.ItemCode, e.SerialNum, e.SlocId });

                entity.ToTable("T_InventoryCountingLineSerial");

                entity.Property(e => e.Icid)
                    .HasMaxLength(50)
                    .HasColumnName("ICId");

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalCount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalDifferent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalStock).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TInventoryHeader>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.CompanyCode });

                entity.ToTable("T_InventoryHeader");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DocDate).HasColumnType("datetime");

                entity.Property(e => e.DocDueDate).HasColumnType("datetime");

                entity.Property(e => e.DocType).HasMaxLength(50);

                entity.Property(e => e.FromStore).HasMaxLength(50);

                entity.Property(e => e.FromStoreName).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.RefInvtid)
                    .HasMaxLength(50)
                    .HasColumnName("RefINVTId");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ToStore).HasMaxLength(50);

                entity.Property(e => e.ToStoreName).HasMaxLength(250);
            });

            modelBuilder.Entity<TInventoryLine>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.CompanyCode, e.LineId, e.ItemCode, e.SlocId });

                entity.ToTable("T_InventoryLine");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.DocType).HasMaxLength(50);

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OpenQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ShipDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TInventoryLineSerial>(entity =>
            {
                entity.HasKey(e => new { e.Invtid, e.LineId, e.CompanyCode, e.ItemCode, e.SerialNum, e.SlocId });

                entity.ToTable("T_InventoryLineSerial");

                entity.Property(e => e.Invtid)
                    .HasMaxLength(50)
                    .HasColumnName("INVTId");

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TInventoryPostingHeader>(entity =>
            {
                entity.HasKey(e => new { e.Ipid, e.CompanyCode, e.StoreId });

                entity.ToTable("T_InventoryPostingHeader");

                entity.Property(e => e.Ipid)
                    .HasMaxLength(50)
                    .HasColumnName("IPId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.Comment).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DocDate).HasColumnType("datetime");

                entity.Property(e => e.DocDueDate).HasColumnType("datetime");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DocTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreName).HasMaxLength(250);
            });

            modelBuilder.Entity<TInventoryPostingLine>(entity =>
            {
                entity.HasKey(e => new { e.Ipid, e.CompanyCode, e.LineId, e.ItemCode, e.SlocId });

                entity.ToTable("T_InventoryPostingLine");

                entity.Property(e => e.Ipid)
                    .HasMaxLength(50)
                    .HasColumnName("IPId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.BaseEntry).HasMaxLength(50);

                entity.Property(e => e.BaseRef).HasMaxLength(50);

                entity.Property(e => e.BaseType).HasMaxLength(50);

                entity.Property(e => e.Comment).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.LineStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OpenQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TotalCount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalDifferent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalStock).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TInventoryPostingLineSerial>(entity =>
            {
                entity.HasKey(e => new { e.Ipid, e.LineId, e.CompanyCode, e.ItemCode, e.SerialNum, e.SlocId });

                entity.ToTable("T_InventoryPostingLineSerial");

                entity.Property(e => e.Ipid)
                    .HasMaxLength(50)
                    .HasColumnName("IPId");

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalCount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalDifferent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalStock).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TItemSerial>(entity =>
            {
                entity.HasKey(e => new { e.SlocId, e.CompanyCode, e.TransId, e.ItemCode, e.SerialNum });

                entity.ToTable("T_ItemSerial");

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OutQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.TransDate).HasColumnType("date");

                entity.Property(e => e.TransType).HasMaxLength(50);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TItemStorage>(entity =>
            {
                entity.HasKey(e => new { e.SlocId, e.CompanyCode, e.ItemCode, e.Uomcode, e.StoreId });

                entity.ToTable("T_ItemStorage");

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<TOrderHeader>(entity =>
            {
                entity.HasKey(e => new { e.TransId, e.CompanyCode, e.StoreId, e.ContractNo });

                entity.ToTable("T_OrderHeader");

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.ContractNo).HasMaxLength(50);

                entity.Property(e => e.AmountChange).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CusId).HasMaxLength(50);

                entity.Property(e => e.CusIdentifier).HasMaxLength(50);

                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.DiscountRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.DiscountType).HasMaxLength(15);

                entity.Property(e => e.ManualDiscount)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PaymentDiscount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.RefTransId).HasMaxLength(50);

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.SalesMode).HasMaxLength(50);

                entity.Property(e => e.SalesPerson).HasMaxLength(50);

                entity.Property(e => e.ShiftId).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreName).HasMaxLength(250);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalDiscountAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalPayable).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalReceipt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalTax).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<TOrderLine>(entity =>
            {
                entity.HasKey(e => new { e.TransId, e.LineId, e.CompanyCode, e.ItemCode, e.SlocId });

                entity.ToTable("T_OrderLine");

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveryType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DiscountAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.DiscountRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.DiscountType).HasMaxLength(15);

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.MinDepositAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.MinDepositPercent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Posservice)
                    .HasMaxLength(50)
                    .HasColumnName("POSService");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.PromoBaseItem).HasMaxLength(50);

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.PromoPercent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.PromoType).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Remark).HasMaxLength(250);

                entity.Property(e => e.SalesMode).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TaxAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.TaxRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TPurchaseOrderHeader>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseId, e.CompanyCode, e.StoreId });

                entity.ToTable("T_PurchaseOrderHeader");

                entity.Property(e => e.PurchaseId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CardCode).HasMaxLength(50);

                entity.Property(e => e.CardName).HasMaxLength(250);

                entity.Property(e => e.Comment).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DocDate).HasColumnType("datetime");

                entity.Property(e => e.DocDueDate).HasColumnType("datetime");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DocTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.InvoiceAddress).HasMaxLength(250);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreName).HasMaxLength(250);

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.Vatpercent)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("VATPercent");

                entity.Property(e => e.Vattotal)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("VATTotal");
            });

            modelBuilder.Entity<TPurchaseOrderLine>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseId, e.CompanyCode, e.LineId, e.ItemCode, e.SlocId });

                entity.ToTable("T_PurchaseOrderLine");

                entity.Property(e => e.PurchaseId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.BaseEntry).HasMaxLength(50);

                entity.Property(e => e.BaseRef).HasMaxLength(50);

                entity.Property(e => e.BaseType).HasMaxLength(50);

                entity.Property(e => e.Comment).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.DiscPercent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.LineStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OpenQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.Vatpercent)
                    .HasColumnType("decimal(19, 6)")
                    .HasColumnName("VATPercent");
            });

            modelBuilder.Entity<TPurchaseOrderLineSerial>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseId, e.LineId, e.CompanyCode, e.ItemCode, e.SerialNum, e.SlocId });

                entity.ToTable("T_PurchaseOrderLineSerial");

                entity.Property(e => e.PurchaseId).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TSalesHeader>(entity =>
            {
                entity.HasKey(e => new { e.TransId, e.CompanyCode, e.StoreId });

                entity.ToTable("T_SalesHeader");

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.AmountChange).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ContractNo).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CusId).HasMaxLength(50);

                entity.Property(e => e.CusIdentifier).HasMaxLength(50);

                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.DiscountRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.DiscountType).HasMaxLength(15);

                entity.Property(e => e.ManualDiscount)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PaymentDiscount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.RefTransId).HasMaxLength(50);

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.SalesMode).HasMaxLength(50);

                entity.Property(e => e.SalesPerson).HasMaxLength(50);

                entity.Property(e => e.ShiftId).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreName).HasMaxLength(250);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalDiscountAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalPayable).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalReceipt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalTax).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<TSalesLine>(entity =>
            {
                entity.HasKey(e => new { e.TransId, e.LineId, e.CompanyCode, e.ItemCode, e.SlocId });

                entity.ToTable("T_SalesLine");

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.AppointmentDate).HasColumnType("date");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveryType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DiscountAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.DiscountRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.DiscountType).HasMaxLength(15);

                entity.Property(e => e.LineTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.MinDepositAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.MinDepositPercent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Posservice)
                    .HasMaxLength(50)
                    .HasColumnName("POSService");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.PromoBaseItem).HasMaxLength(50);

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.PromoPercent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.PromoType).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Remark).HasMaxLength(250);

                entity.Property(e => e.SalesMode).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreAreaId).HasMaxLength(50);

                entity.Property(e => e.TaxAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.TaxRate).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TimeFrameId).HasMaxLength(50);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TSalesLineSerial>(entity =>
            {
                entity.HasKey(e => new { e.TransId, e.LineId, e.CompanyCode, e.ItemCode, e.SerialNum, e.SlocId })
                    .HasName("PK_SalesLineSerial");

                entity.ToTable("T_SalesLineSerial");

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.SerialNum).HasMaxLength(100);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            modelBuilder.Entity<TSalesPayment>(entity =>
            {
                entity.HasKey(e => new { e.PaymentCode, e.CompanyCode, e.TransId, e.LineId });

                entity.ToTable("T_SalesPayment");

                entity.Property(e => e.PaymentCode).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.LineId).HasMaxLength(50);

                entity.Property(e => e.CardHolderName).HasMaxLength(250);

                entity.Property(e => e.CardNo).HasMaxLength(50);

                entity.Property(e => e.CardType).HasMaxLength(50);

                entity.Property(e => e.ChangeAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ChargableAmount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.CollectedAmount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PaidAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.PaymentDiscount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.PaymentMode).HasMaxLength(50);

                entity.Property(e => e.ReceivedAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.RefNumber).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TotalAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.VoucherBarCode).HasMaxLength(250);

                entity.Property(e => e.VoucherSerial).HasMaxLength(50);
            });

            modelBuilder.Entity<TSalesPromo>(entity =>
            {
                entity.HasKey(e => new { e.TransId, e.CompanyCode, e.ItemCode })
                    .HasName("PK_S_SalesPromo");

                entity.ToTable("T_SalesPromo");

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.ApplyType).HasMaxLength(50);

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ItemGroupId).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.PromoType).HasMaxLength(50);

                entity.Property(e => e.PromoTypeLine).HasMaxLength(50);

                entity.Property(e => e.RefTransId).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.Value).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<TShiftHeader>(entity =>
            {
                entity.HasKey(e => new { e.ShiftId, e.CompanyCode, e.StoreId, e.DailyId, e.DeviceId });

                entity.ToTable("T_ShiftHeader");

                entity.Property(e => e.ShiftId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.DailyId).HasMaxLength(50);

                entity.Property(e => e.DeviceId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OpenAmt).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ShiftTotal).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<TShiftLine>(entity =>
            {
                entity.HasKey(e => new { e.ShiftId, e.CompanyCode, e.PaymentCode })
                    .HasName("PK_T_ShiftLines");

                entity.ToTable("T_ShiftLine");

                entity.Property(e => e.ShiftId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.PaymentCode).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Value).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<TStoreDaily>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.DailyId, e.CompanyCode, e.DeviceId });

                entity.ToTable("T_StoreDaily");

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.DailyId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.DeviceId).HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TotalCount).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TotalSales).HasColumnType("decimal(19, 6)");
            });

            modelBuilder.Entity<TTransactionLog>(entity =>
            {
                entity.HasKey(e => new { e.TransId, e.CompanyCode, e.SlocId, e.ItemCode, e.StoreId });

                entity.ToTable("T_TransactionLog");

                entity.Property(e => e.TransId).HasMaxLength(50);

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.SlocId)
                    .HasMaxLength(50)
                    .HasColumnName("SLocId");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.OutQty).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransType).HasMaxLength(50);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

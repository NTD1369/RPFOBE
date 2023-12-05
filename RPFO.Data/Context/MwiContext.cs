using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RPFO.Data.EntitiesMWI;

#nullable disable

namespace RPFO.Data.Context
{
    public partial class MwiContext : DbContext
    {
        public MwiContext()
        {
        }

        public MwiContext(DbContextOptions<MwiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MItem> MItems { get; set; }
        public virtual DbSet<MItemUom> MItemUoms { get; set; }
        public virtual DbSet<MPriceList> MPriceLists { get; set; }
        public virtual DbSet<MStore> MStores { get; set; }
        public virtual DbSet<MTax> MTaxes { get; set; }
        public virtual DbSet<MUom> MUoms { get; set; }
        public virtual DbSet<MUser> MUsers { get; set; }
        public virtual DbSet<MWarehouse> MWarehouses { get; set; }
        public virtual DbSet<SConnectSetting> SConnectSettings { get; set; }
        public virtual DbSet<TInvoiceHeader> TInvoiceHeaders { get; set; }
        public virtual DbSet<TInvoiceLine> TInvoiceLines { get; set; }
        public virtual DbSet<TSalesHeader> TSalesHeaders { get; set; }
        public virtual DbSet<TSalesLine> TSalesLines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=sapvn.com,56231;Database=ABEO_MWI;user id=sa;password=sa@787&*&;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MItem>(entity =>
            {
                entity.HasKey(e => e.ItemCode);

                entity.ToTable("M_Item");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.CustomField1).HasMaxLength(50);

                entity.Property(e => e.CustomField10).HasMaxLength(50);

                entity.Property(e => e.CustomField2).HasMaxLength(50);

                entity.Property(e => e.CustomField3).HasMaxLength(50);

                entity.Property(e => e.CustomField4).HasMaxLength(50);

                entity.Property(e => e.CustomField5).HasMaxLength(50);

                entity.Property(e => e.CustomField6).HasMaxLength(50);

                entity.Property(e => e.CustomField7).HasMaxLength(50);

                entity.Property(e => e.CustomField8).HasMaxLength(50);

                entity.Property(e => e.CustomField9).HasMaxLength(50);

                entity.Property(e => e.DefaultPrice).HasColumnType("numeric(16, 6)");

                entity.Property(e => e.InventoryUom)
                    .HasMaxLength(50)
                    .HasColumnName("InventoryUOM");

                entity.Property(e => e.IsBom)
                    .HasMaxLength(50)
                    .HasColumnName("IsBOM");

                entity.Property(e => e.IsSerial).HasMaxLength(50);

                entity.Property(e => e.ItemForeignName).HasMaxLength(250);

                entity.Property(e => e.ItemName).HasMaxLength(250);

                entity.Property(e => e.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<MItemUom>(entity =>
            {
                entity.HasKey(e => new { e.ItemCode, e.Uomcode });

                entity.ToTable("M_ItemUOM");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.BarCode).HasMaxLength(250);

                entity.Property(e => e.Factor).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.Qrcode)
                    .HasMaxLength(250)
                    .HasColumnName("QRCode");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MPriceList>(entity =>
            {
                entity.HasKey(e => new { e.PriceListId, e.StoreId });

                entity.ToTable("M_PriceList");

                entity.Property(e => e.PriceListId).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.BarCode)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ItemCode)
                    .IsRequired()
                    .HasMaxLength(50);

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
            });

            modelBuilder.Entity<MStore>(entity =>
            {
                entity.HasKey(e => e.StoreId);

                entity.ToTable("M_Store");

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreDescription).HasMaxLength(250);

                entity.Property(e => e.StoreName).HasMaxLength(250);

                entity.Property(e => e.WhsCode).HasMaxLength(50);
            });

            modelBuilder.Entity<MTax>(entity =>
            {
                entity.HasKey(e => e.TaxCode);

                entity.ToTable("M_Tax");

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TaxName).HasMaxLength(50);

                entity.Property(e => e.TaxPercent).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.TaxType).HasMaxLength(50);
            });

            modelBuilder.Entity<MUom>(entity =>
            {
                entity.HasKey(e => e.Uomcode);

                entity.ToTable("M_UOM");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

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
                entity.HasKey(e => new { e.CompanyCode, e.Username });

                entity.ToTable("M_User");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.LastLoginStoreId).HasMaxLength(50);

                entity.Property(e => e.LastLoginStoreLang).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MWarehouse>(entity =>
            {
                entity.HasKey(e => e.WhsCode);

                entity.ToTable("M_Warehouse");

                entity.Property(e => e.WhsCode).HasMaxLength(50);

                entity.Property(e => e.WhsName).HasMaxLength(250);

                entity.Property(e => e.WhsType).HasMaxLength(50);
            });

            modelBuilder.Entity<SConnectSetting>(entity =>
            {
                entity.HasKey(e => new { e.SettingId, e.CompanyCode });

                entity.ToTable("S_ConnectSetting");

                entity.Property(e => e.SettingId)
                    .HasMaxLength(50)
                    .HasColumnName("SettingID");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.SettingDescription).HasMaxLength(250);

                entity.Property(e => e.SettingName).HasMaxLength(250);

                entity.Property(e => e.SettingValue).HasMaxLength(50);
            });

            modelBuilder.Entity<TInvoiceHeader>(entity =>
            {
                entity.HasKey(e => new { e.PostransId, e.CompanyCode, e.StoreId });

                entity.ToTable("T_InvoiceHeader");

                entity.Property(e => e.PostransId)
                    .HasMaxLength(50)
                    .HasColumnName("POSTransId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CardCode).HasMaxLength(50);

                entity.Property(e => e.CardName).HasMaxLength(250);

                entity.Property(e => e.ContractNo).HasMaxLength(50);

                entity.Property(e => e.DiscPrcnt).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.DiscSum).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.DocCur).HasMaxLength(50);

                entity.Property(e => e.DocEntry).HasMaxLength(50);

                entity.Property(e => e.DocRate).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.DocStatus).HasMaxLength(50);

                entity.Property(e => e.DocTotal).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.VatSum).HasColumnType("numeric(19, 6)");
            });

            modelBuilder.Entity<TInvoiceLine>(entity =>
            {
                entity.HasKey(e => new { e.PostransId, e.LineId, e.ItemCode });

                entity.ToTable("T_InvoiceLines");

                entity.Property(e => e.PostransId)
                    .HasMaxLength(50)
                    .HasColumnName("POSTransId");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.BarCode).HasMaxLength(50);

                entity.Property(e => e.DiscPrcnt).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.DiscSum).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.LineStatus).HasMaxLength(50);

                entity.Property(e => e.LineTotal).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.Price).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.PromoType).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.Remark).HasMaxLength(250);

                entity.Property(e => e.TaxAmt).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.TaxRate).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.WhsCode).HasMaxLength(50);
            });

            modelBuilder.Entity<TSalesHeader>(entity =>
            {
                entity.HasKey(e => new { e.PostransId, e.CompanyCode, e.StoreId });

                entity.ToTable("T_SalesHeader");

                entity.Property(e => e.PostransId)
                    .HasMaxLength(50)
                    .HasColumnName("POSTransId");

                entity.Property(e => e.CompanyCode).HasMaxLength(50);

                entity.Property(e => e.StoreId).HasMaxLength(50);

                entity.Property(e => e.CardCode).HasMaxLength(50);

                entity.Property(e => e.CardName).HasMaxLength(250);

                entity.Property(e => e.ContractNo).HasMaxLength(50);

                entity.Property(e => e.DiscPrcnt).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.DiscSum).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.DocCur).HasMaxLength(50);

                entity.Property(e => e.DocEntry).HasMaxLength(50);

                entity.Property(e => e.DocRate).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.DocStatus).HasMaxLength(50);

                entity.Property(e => e.DocTotal).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.VatSum).HasColumnType("numeric(19, 6)");
            });

            modelBuilder.Entity<TSalesLine>(entity =>
            {
                entity.HasKey(e => new { e.PostransId, e.LineId, e.ItemCode });

                entity.ToTable("T_SalesLines");

                entity.Property(e => e.PostransId)
                    .HasMaxLength(50)
                    .HasColumnName("POSTransId");

                entity.Property(e => e.ItemCode).HasMaxLength(50);

                entity.Property(e => e.BarCode).HasMaxLength(50);

                entity.Property(e => e.DiscPrcnt).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.DiscSum).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.LineStatus).HasMaxLength(50);

                entity.Property(e => e.LineTotal).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.OpenQty).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.Price).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.PromoId).HasMaxLength(50);

                entity.Property(e => e.PromoType).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.Remark).HasMaxLength(250);

                entity.Property(e => e.TaxAmt).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.TaxRate).HasColumnType("numeric(19, 6)");

                entity.Property(e => e.Uomcode)
                    .HasMaxLength(50)
                    .HasColumnName("UOMCode");

                entity.Property(e => e.WhsCode).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;

namespace Spectra.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountUser>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<AccountAdmin>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Banner>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<CategoryNew>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<ImageProduct>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<NewsDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Policy>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<PolicyDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<UserLanding>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Application>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Characteristic>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<CharacterList>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<ServiceDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<QuestionService>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Welcome>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<WelcomeDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Recrutement>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Home>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<ProductSeo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<NewSeo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<RecruitmentSeo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<ContactSeo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Warranty>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<County>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<WarrantySeo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Gift>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<OrderCus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<OrderDetailCus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Attribute>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<ValueAttribute>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Agency>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<PaymentInformationModel>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<SeriProduct>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<GiftUser>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<QuestionPrize>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<WarrantyGold>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<WarrantyGoldLog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<QualityAssessment>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<ExperienceDay>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Routesr>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            //modelBuilder.Entity<ProductCombo>(entity =>
            //{
            //    entity.Property(e => e.Id).ValueGeneratedOnAdd();
            //});
            //modelBuilder.Entity<ComboDetail>(entity =>
            //{
            //    entity.Property(e => e.Id).ValueGeneratedOnAdd();
            //});
            modelBuilder.Entity<VoucherUsage>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            // Set Unique Constraint
            modelBuilder.Entity<AccountUser>().HasIndex(accountuser => accountuser.Email).IsUnique();
            modelBuilder.Entity<AccountUser>().HasIndex(accountuser => accountuser.Code).IsUnique();
            modelBuilder.Entity<AccountAdmin>().HasIndex(accountadmin => accountadmin.Email).IsUnique();
            modelBuilder.Entity<AccountAdmin>().HasIndex(accountadmin => accountadmin.Code).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(product => product.Code).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(category => category.Code).IsUnique();
            modelBuilder.Entity<PaymentInformationModel>().HasIndex(pay => pay.PaymentCode).IsUnique();
            modelBuilder.Entity<Order>().HasIndex(pay => pay.Code).IsUnique();

        }

        public DbSet<AccountUser> AccountUsers { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryNew> CategoryNews { get; set; }
        public DbSet<AccountAdmin> AccountAdmins { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<ImageProduct> ImageProducts { get; set; }
        public DbSet<NewsDetail> NewsDetails { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<PolicyDetail> PolicyDetails { get; set; }
        public DbSet<UserLanding> UserLandings { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Characteristic> Characteristics { get; set; }
        public DbSet<CharacterList> CharacterLists { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceDetail> ServiceDetails { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionService> QuestionServices { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Welcome> Welcomes { get; set; }
        public DbSet<WelcomeDetail> WelcomeDetails { get; set; }
        public DbSet<Recrutement> Recrutements { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<ProductSeo> ProductSeos { get; set; }
        public DbSet<ContactSeo> ContactSeos { get; set; }
        public DbSet<RecruitmentSeo> RecruitmentSeos { get; set; }
        public DbSet<NewSeo> NewSeos { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<WarrantySeo> WarrantySeos { get; set; }
        public DbSet<Gift> Gift { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<OrderCus> OrderCus { get; set; }
        public DbSet<OrderDetailCus> OrderDetailCus { get; set; }
        public DbSet<ValueAttribute> ValueAttributes { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<PaymentInformationModel> PaymentInformationModels { get; set; }
        public DbSet<SeriProduct> SeriProducts { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<GiftUser> GiftUsers { get; set; }
        public DbSet<QuestionPrize> QuestionPrizes { get; set; }
        public DbSet<WarrantyGold> WarrantyGolds { get; set; }
        public DbSet<WarrantyGoldLog> WarrantyGoldLogs { get; set; }
        public DbSet<QualityAssessment> QualityAssessments { get; set; }
        public DbSet<ExperienceDay> ExperienceDays { get; set; }
        public DbSet<Routesr> Routesrs { get; set; }
        //public DbSet<ProductCombo> ProductCombos { get; set; }
        //public DbSet<ComboDetail> ComboDetails { get; set; }
        public DbSet<VoucherUsage> VoucherUsages { get; set; }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Spectra.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spectra_AccountAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Role = table.Column<byte>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_AccountAdmin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_AccountUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Gender = table.Column<bool>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_AccountUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_AgencySeo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_AgencySeo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Attribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Attribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Banner",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Banner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Option = table.Column<bool>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_CategoryNew",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_CategoryNew", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_City",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_City", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Contact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_ContactSeo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_ContactSeo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_ExperienceDay",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Phone = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Old = table.Column<string>(nullable: false),
                    Mom = table.Column<string>(nullable: false),
                    Breastpump = table.Column<string>(nullable: true),
                    Private = table.Column<string>(nullable: false),
                    Time = table.Column<string>(nullable: false),
                    Website = table.Column<byte>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_ExperienceDay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Gift",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Price = table.Column<float>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Gift", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_GiftUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NameFacebook = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    LinkArticle = table.Column<string>(nullable: false),
                    Prize = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    BirthDay = table.Column<DateTime>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_GiftUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Home",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Home", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_NewSeo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_NewSeo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_OrderCus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 250, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true),
                    Email = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    TotalQuantity = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<int>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    AccountCusId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_OrderCus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Payment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true),
                    PaymentCode = table.Column<string>(nullable: true),
                    PaymentInfor = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Payment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Policy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Options = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Policy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_ProductSeo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_ProductSeo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Quality_Assessment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Experience = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Advise = table.Column<string>(nullable: false),
                    Pack = table.Column<string>(nullable: false),
                    Expectation = table.Column<string>(nullable: false),
                    Desire = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Quality_Assessment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_QuestionPrize",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_QuestionPrize", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_RecruitmentSeo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_RecruitmentSeo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Recrutement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Recrutement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Routes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(nullable: false),
                    Component = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Service",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Service", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_UserLanding",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    InforProduct = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_UserLanding", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Warranty",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Phone = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Email = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductSeri = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    StoreCode = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Warranty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_WarrantyGold",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Phone = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductSeri = table.Column<string>(nullable: true),
                    DateBuy = table.Column<DateTime>(nullable: false),
                    GtriHĐ = table.Column<float>(nullable: false),
                    PhiDVBH = table.Column<float>(nullable: false),
                    File = table.Column<string>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_WarrantyGold", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_WarrantySeo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_WarrantySeo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Welcome",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Welcome", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 250, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true),
                    Email = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    PaymentMethod = table.Column<string>(nullable: true),
                    TotalQuantity = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<int>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Website = table.Column<byte>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    AccountUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_Order_Spectra_AccountUser_AccountUserId",
                        column: x => x.AccountUserId,
                        principalTable: "Spectra_AccountUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_ValueAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    AttributeId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_ValueAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_ValueAttribute_Spectra_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Spectra_Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Application",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CategoryId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Application", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_Application_Spectra_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Spectra_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Characteristic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CategoryId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Characteristic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_Characteristic_Spectra_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Spectra_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_NewsDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CategoryNewId = table.Column<int>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_NewsDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_NewsDetail_Spectra_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Spectra_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Spectra_NewsDetail_Spectra_CategoryNew_CategoryNewId",
                        column: x => x.CategoryNewId,
                        principalTable: "Spectra_CategoryNew",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_County",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CityId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_County", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_County_Spectra_City_CityId",
                        column: x => x.CityId,
                        principalTable: "Spectra_City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Location",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CityId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_Location_Spectra_City_CityId",
                        column: x => x.CityId,
                        principalTable: "Spectra_City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    TitleDescription = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    SalePrice = table.Column<float>(nullable: false),
                    Images = table.Column<string>(nullable: true),
                    JobId = table.Column<string>(nullable: true),
                    WarrantyMonth = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: true),
                    GiftId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Option = table.Column<bool>(nullable: false),
                    ScheduleStatus = table.Column<bool>(nullable: false),
                    Information = table.Column<string>(nullable: true),
                    Instruct = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    AccountEdit = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    Ends = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_Product_Spectra_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Spectra_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Spectra_Product_Spectra_Gift_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Spectra_Gift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_PolicyDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    PolicyId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_PolicyDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_PolicyDetail_Spectra_Policy_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Spectra_Policy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_ServiceDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    ServiceId = table.Column<int>(nullable: true),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_ServiceDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_ServiceDetail_Spectra_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Spectra_Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_WarrantyGold_Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarrantyGoldId = table.Column<int>(nullable: true),
                    ChangedBy = table.Column<string>(nullable: true),
                    WarrantyContent = table.Column<string>(nullable: true),
                    OldValue = table.Column<float>(nullable: false),
                    NewValue = table.Column<float>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_WarrantyGold_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_WarrantyGold_Log_Spectra_WarrantyGold_WarrantyGoldId",
                        column: x => x.WarrantyGoldId,
                        principalTable: "Spectra_WarrantyGold",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_WelcomeDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    WelcomeId = table.Column<int>(nullable: true),
                    TitleSeo = table.Column<string>(nullable: true),
                    MetaKeyWords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_WelcomeDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_WelcomeDetail_Spectra_Welcome_WelcomeId",
                        column: x => x.WelcomeId,
                        principalTable: "Spectra_Welcome",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_CharacterList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CharacteristicId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_CharacterList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_CharacterList_Spectra_Characteristic_CharacteristicId",
                        column: x => x.CharacteristicId,
                        principalTable: "Spectra_Characteristic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Feedback",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 250, nullable: true),
                    Rating = table.Column<float>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    ProductId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_Feedback_Spectra_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Spectra_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_ImageProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImageName = table.Column<string>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    ProductId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_ImageProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_ImageProduct_Spectra_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Spectra_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Item",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: true),
                    ValueAttributeId = table.Column<int>(nullable: true),
                    AttributeId = table.Column<int>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    PriceAgain = table.Column<float>(nullable: false),
                    GiftId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    JobId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_Item_Spectra_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Spectra_Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Spectra_Item_Spectra_Gift_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Spectra_Gift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Spectra_Item_Spectra_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Spectra_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Spectra_Item_Spectra_ValueAttribute_ValueAttributeId",
                        column: x => x.ValueAttributeId,
                        principalTable: "Spectra_ValueAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_OrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Gift = table.Column<string>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    DiscountVoucher = table.Column<float>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_OrderDetail_Spectra_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Spectra_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Spectra_OrderDetail_Spectra_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Spectra_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_OrderDetailCus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Gift = table.Column<string>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    OrderCusId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_OrderDetailCus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_OrderDetailCus_Spectra_OrderCus_OrderCusId",
                        column: x => x.OrderCusId,
                        principalTable: "Spectra_OrderCus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Spectra_OrderDetailCus_Spectra_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Spectra_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_QuestionPro",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_QuestionPro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_QuestionPro_Spectra_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Spectra_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_SeriProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductSeri = table.Column<string>(nullable: true),
                    CityId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    DealerSaleDate = table.Column<DateTime>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_SeriProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_SeriProduct_Spectra_City_CityId",
                        column: x => x.CityId,
                        principalTable: "Spectra_City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Spectra_SeriProduct_Spectra_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Spectra_Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Spectra_SeriProduct_Spectra_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Spectra_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_Voucher",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VoucherCode = table.Column<string>(nullable: true),
                    Discount = table.Column<float>(nullable: false),
                    DiscountType = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    JobId = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    ScheduleStatus = table.Column<bool>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_Voucher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_Voucher_Spectra_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Spectra_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_QuestionServi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ServiceDetailId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_QuestionServi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_QuestionServi_Spectra_ServiceDetail_ServiceDetailId",
                        column: x => x.ServiceDetailId,
                        principalTable: "Spectra_ServiceDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spectra_VoucherUsage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VoucherId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    UsedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectra_VoucherUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spectra_VoucherUsage_Spectra_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Spectra_Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_AccountAdmin_Code",
                table: "Spectra_AccountAdmin",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_AccountAdmin_Email",
                table: "Spectra_AccountAdmin",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_AccountUser_Code",
                table: "Spectra_AccountUser",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_AccountUser_Email",
                table: "Spectra_AccountUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Application_CategoryId",
                table: "Spectra_Application",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Category_Code",
                table: "Spectra_Category",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Characteristic_CategoryId",
                table: "Spectra_Characteristic",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_CharacterList_CharacteristicId",
                table: "Spectra_CharacterList",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_County_CityId",
                table: "Spectra_County",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Feedback_ProductId",
                table: "Spectra_Feedback",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_ImageProduct_ProductId",
                table: "Spectra_ImageProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Item_AttributeId",
                table: "Spectra_Item",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Item_GiftId",
                table: "Spectra_Item",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Item_ProductId",
                table: "Spectra_Item",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Item_ValueAttributeId",
                table: "Spectra_Item",
                column: "ValueAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Location_CityId",
                table: "Spectra_Location",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_NewsDetail_CategoryId",
                table: "Spectra_NewsDetail",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_NewsDetail_CategoryNewId",
                table: "Spectra_NewsDetail",
                column: "CategoryNewId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Order_AccountUserId",
                table: "Spectra_Order",
                column: "AccountUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Order_Code",
                table: "Spectra_Order",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_OrderDetail_OrderId",
                table: "Spectra_OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_OrderDetail_ProductId",
                table: "Spectra_OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_OrderDetailCus_OrderCusId",
                table: "Spectra_OrderDetailCus",
                column: "OrderCusId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_OrderDetailCus_ProductId",
                table: "Spectra_OrderDetailCus",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Payment_PaymentCode",
                table: "Spectra_Payment",
                column: "PaymentCode",
                unique: true,
                filter: "[PaymentCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_PolicyDetail_PolicyId",
                table: "Spectra_PolicyDetail",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Product_CategoryId",
                table: "Spectra_Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Product_Code",
                table: "Spectra_Product",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Product_GiftId",
                table: "Spectra_Product",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_QuestionPro_ProductId",
                table: "Spectra_QuestionPro",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_QuestionServi_ServiceDetailId",
                table: "Spectra_QuestionServi",
                column: "ServiceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_SeriProduct_CityId",
                table: "Spectra_SeriProduct",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_SeriProduct_LocationId",
                table: "Spectra_SeriProduct",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_SeriProduct_ProductId",
                table: "Spectra_SeriProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_ServiceDetail_ServiceId",
                table: "Spectra_ServiceDetail",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_ValueAttribute_AttributeId",
                table: "Spectra_ValueAttribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_Voucher_ProductId",
                table: "Spectra_Voucher",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_VoucherUsage_VoucherId",
                table: "Spectra_VoucherUsage",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_WarrantyGold_Log_WarrantyGoldId",
                table: "Spectra_WarrantyGold_Log",
                column: "WarrantyGoldId");

            migrationBuilder.CreateIndex(
                name: "IX_Spectra_WelcomeDetail_WelcomeId",
                table: "Spectra_WelcomeDetail",
                column: "WelcomeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spectra_AccountAdmin");

            migrationBuilder.DropTable(
                name: "Spectra_AgencySeo");

            migrationBuilder.DropTable(
                name: "Spectra_Application");

            migrationBuilder.DropTable(
                name: "Spectra_Banner");

            migrationBuilder.DropTable(
                name: "Spectra_CharacterList");

            migrationBuilder.DropTable(
                name: "Spectra_Contact");

            migrationBuilder.DropTable(
                name: "Spectra_ContactSeo");

            migrationBuilder.DropTable(
                name: "Spectra_County");

            migrationBuilder.DropTable(
                name: "Spectra_ExperienceDay");

            migrationBuilder.DropTable(
                name: "Spectra_Feedback");

            migrationBuilder.DropTable(
                name: "Spectra_GiftUser");

            migrationBuilder.DropTable(
                name: "Spectra_Home");

            migrationBuilder.DropTable(
                name: "Spectra_ImageProduct");

            migrationBuilder.DropTable(
                name: "Spectra_Item");

            migrationBuilder.DropTable(
                name: "Spectra_NewsDetail");

            migrationBuilder.DropTable(
                name: "Spectra_NewSeo");

            migrationBuilder.DropTable(
                name: "Spectra_OrderDetail");

            migrationBuilder.DropTable(
                name: "Spectra_OrderDetailCus");

            migrationBuilder.DropTable(
                name: "Spectra_Payment");

            migrationBuilder.DropTable(
                name: "Spectra_PolicyDetail");

            migrationBuilder.DropTable(
                name: "Spectra_ProductSeo");

            migrationBuilder.DropTable(
                name: "Spectra_Quality_Assessment");

            migrationBuilder.DropTable(
                name: "Spectra_QuestionPrize");

            migrationBuilder.DropTable(
                name: "Spectra_QuestionPro");

            migrationBuilder.DropTable(
                name: "Spectra_QuestionServi");

            migrationBuilder.DropTable(
                name: "Spectra_RecruitmentSeo");

            migrationBuilder.DropTable(
                name: "Spectra_Recrutement");

            migrationBuilder.DropTable(
                name: "Spectra_Routes");

            migrationBuilder.DropTable(
                name: "Spectra_SeriProduct");

            migrationBuilder.DropTable(
                name: "Spectra_UserLanding");

            migrationBuilder.DropTable(
                name: "Spectra_VoucherUsage");

            migrationBuilder.DropTable(
                name: "Spectra_Warranty");

            migrationBuilder.DropTable(
                name: "Spectra_WarrantyGold_Log");

            migrationBuilder.DropTable(
                name: "Spectra_WarrantySeo");

            migrationBuilder.DropTable(
                name: "Spectra_WelcomeDetail");

            migrationBuilder.DropTable(
                name: "Spectra_Characteristic");

            migrationBuilder.DropTable(
                name: "Spectra_ValueAttribute");

            migrationBuilder.DropTable(
                name: "Spectra_CategoryNew");

            migrationBuilder.DropTable(
                name: "Spectra_Order");

            migrationBuilder.DropTable(
                name: "Spectra_OrderCus");

            migrationBuilder.DropTable(
                name: "Spectra_Policy");

            migrationBuilder.DropTable(
                name: "Spectra_ServiceDetail");

            migrationBuilder.DropTable(
                name: "Spectra_Location");

            migrationBuilder.DropTable(
                name: "Spectra_Voucher");

            migrationBuilder.DropTable(
                name: "Spectra_WarrantyGold");

            migrationBuilder.DropTable(
                name: "Spectra_Welcome");

            migrationBuilder.DropTable(
                name: "Spectra_Attribute");

            migrationBuilder.DropTable(
                name: "Spectra_AccountUser");

            migrationBuilder.DropTable(
                name: "Spectra_Service");

            migrationBuilder.DropTable(
                name: "Spectra_City");

            migrationBuilder.DropTable(
                name: "Spectra_Product");

            migrationBuilder.DropTable(
                name: "Spectra_Category");

            migrationBuilder.DropTable(
                name: "Spectra_Gift");
        }
    }
}

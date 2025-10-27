using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsernameUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Alias = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrewMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrindingMechanisms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrindingMechanisms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Aliases = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grinders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    ModelAlias = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseDate = table.Column<int>(type: "INTEGER", nullable: true),
                    BrandId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grinders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grinders_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Brewers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    ModelAlias = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseDate = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    WaterCapacity = table.Column<decimal>(type: "TEXT", nullable: true),
                    BrewMethodId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrandId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brewers_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Brewers_BrewMethods_BrewMethodId",
                        column: x => x.BrewMethodId,
                        principalTable: "BrewMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GrindingElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Diameter = table.Column<decimal>(type: "TEXT", nullable: false),
                    GrindingMechanismId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrindingElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrindingElements_GrindingMechanisms_GrindingMechanismId",
                        column: x => x.GrindingMechanismId,
                        principalTable: "GrindingMechanisms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Beans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Aliases = table.Column<string>(type: "TEXT", nullable: true),
                    Decaf = table.Column<bool>(type: "INTEGER", nullable: false),
                    Elevation = table.Column<int>(type: "INTEGER", nullable: true),
                    Roast = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Region = table.Column<string>(type: "TEXT", nullable: true),
                    Farm = table.Column<string>(type: "TEXT", nullable: true),
                    WetMill = table.Column<string>(type: "TEXT", nullable: true),
                    Varietal = table.Column<string>(type: "TEXT", nullable: true),
                    Producer = table.Column<string>(type: "TEXT", nullable: true),
                    Importer = table.Column<string>(type: "TEXT", nullable: true),
                    Process = table.Column<string>(type: "TEXT", nullable: true),
                    FlavorProfile = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseDate = table.Column<int>(type: "INTEGER", nullable: true),
                    RoasterId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beans_Roasters_RoasterId",
                        column: x => x.RoasterId,
                        principalTable: "Roasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GrinderDials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    Step = table.Column<decimal>(type: "TEXT", nullable: true),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    GrinderId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrinderDials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrinderDials_Grinders_GrinderId",
                        column: x => x.GrinderId,
                        principalTable: "Grinders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrewerStockSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    WaterTemperature = table.Column<decimal>(type: "TEXT", nullable: true),
                    WaterVolume = table.Column<decimal>(type: "TEXT", nullable: true),
                    BrewTime = table.Column<decimal>(type: "TEXT", nullable: true),
                    BrewerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewerStockSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrewerStockSettings_Brewers_BrewerId",
                        column: x => x.BrewerId,
                        principalTable: "Brewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrewerUserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    WaterTemperature = table.Column<decimal>(type: "TEXT", nullable: true),
                    WaterVolume = table.Column<decimal>(type: "TEXT", nullable: true),
                    BrewTime = table.Column<decimal>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrewerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewerUserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrewerUserSettings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BrewerUserSettings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BrewerUserSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrewerUserSettings_Brewers_BrewerId",
                        column: x => x.BrewerId,
                        principalTable: "Brewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrinderElementCompatibilities",
                columns: table => new
                {
                    GrinderId = table.Column<int>(type: "INTEGER", nullable: false),
                    GrindingElementId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrinderElementCompatibilities", x => new { x.GrinderId, x.GrindingElementId });
                    table.ForeignKey(
                        name: "FK_GrinderElementCompatibilities_Grinders_GrinderId",
                        column: x => x.GrinderId,
                        principalTable: "Grinders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrinderElementCompatibilities_GrindingElements_GrindingElementId",
                        column: x => x.GrindingElementId,
                        principalTable: "GrindingElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeanBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoastDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BeanId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeanBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeanBatches_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BeanBatches_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BeanBatches_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BeanBatches_Beans_BeanId",
                        column: x => x.BeanId,
                        principalTable: "Beans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrewSetups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GrinderId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrewerId = table.Column<int>(type: "INTEGER", nullable: false),
                    BeanId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewSetups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrewSetups_Beans_BeanId",
                        column: x => x.BeanId,
                        principalTable: "Beans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrewSetups_Brewers_BrewerId",
                        column: x => x.BrewerId,
                        principalTable: "Brewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrewSetups_Grinders_GrinderId",
                        column: x => x.GrinderId,
                        principalTable: "Grinders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrewSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Dose = table.Column<decimal>(type: "TEXT", nullable: false),
                    Sour = table.Column<int>(type: "INTEGER", nullable: false),
                    Bitter = table.Column<int>(type: "INTEGER", nullable: false),
                    Recommended = table.Column<bool>(type: "INTEGER", nullable: false),
                    WaterTemperature = table.Column<decimal>(type: "TEXT", nullable: false),
                    WaterVolume = table.Column<decimal>(type: "TEXT", nullable: false),
                    BrewTime = table.Column<decimal>(type: "TEXT", nullable: false),
                    Comments = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrewSetupId = table.Column<int>(type: "INTEGER", nullable: false),
                    BeanBatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrewSettings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BrewSettings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BrewSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrewSettings_BeanBatches_BeanBatchId",
                        column: x => x.BeanBatchId,
                        principalTable: "BeanBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrewSettings_BrewSetups_BrewSetupId",
                        column: x => x.BrewSetupId,
                        principalTable: "BrewSetups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserBrewSetups",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrewSetupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBrewSetups", x => new { x.UserId, x.BrewSetupId });
                    table.ForeignKey(
                        name: "FK_UserBrewSetups_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserBrewSetups_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserBrewSetups_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBrewSetups_BrewSetups_BrewSetupId",
                        column: x => x.BrewSetupId,
                        principalTable: "BrewSetups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrewGrinderDialSettings",
                columns: table => new
                {
                    BrewSettingId = table.Column<int>(type: "INTEGER", nullable: false),
                    GrinderDialId = table.Column<int>(type: "INTEGER", nullable: false),
                    DialValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewGrinderDialSettings", x => new { x.BrewSettingId, x.GrinderDialId });
                    table.ForeignKey(
                        name: "FK_BrewGrinderDialSettings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BrewGrinderDialSettings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BrewGrinderDialSettings_BrewSettings_BrewSettingId",
                        column: x => x.BrewSettingId,
                        principalTable: "BrewSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrewGrinderDialSettings_GrinderDials_GrinderDialId",
                        column: x => x.GrinderDialId,
                        principalTable: "GrinderDials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UpdatedById",
                table: "AspNetUsers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BeanBatches_BeanId",
                table: "BeanBatches",
                column: "BeanId");

            migrationBuilder.CreateIndex(
                name: "IX_BeanBatches_CreatedById",
                table: "BeanBatches",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BeanBatches_UpdatedById",
                table: "BeanBatches",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BeanBatches_UserId",
                table: "BeanBatches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Beans_RoasterId_Name",
                table: "Beans",
                columns: new[] { "RoasterId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brewers_BrandId",
                table: "Brewers",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Brewers_BrewMethodId",
                table: "Brewers",
                column: "BrewMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewerStockSettings_BrewerId",
                table: "BrewerStockSettings",
                column: "BrewerId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewerUserSettings_BrewerId",
                table: "BrewerUserSettings",
                column: "BrewerId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewerUserSettings_CreatedById",
                table: "BrewerUserSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BrewerUserSettings_UpdatedById",
                table: "BrewerUserSettings",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BrewerUserSettings_UserId",
                table: "BrewerUserSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewGrinderDialSettings_CreatedById",
                table: "BrewGrinderDialSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BrewGrinderDialSettings_GrinderDialId",
                table: "BrewGrinderDialSettings",
                column: "GrinderDialId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewGrinderDialSettings_UpdatedById",
                table: "BrewGrinderDialSettings",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BrewMethods_Name",
                table: "BrewMethods",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrewSettings_BeanBatchId",
                table: "BrewSettings",
                column: "BeanBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSettings_BrewSetupId",
                table: "BrewSettings",
                column: "BrewSetupId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSettings_CreatedById",
                table: "BrewSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSettings_UpdatedById",
                table: "BrewSettings",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSettings_UserId_BrewSetupId",
                table: "BrewSettings",
                columns: new[] { "UserId", "BrewSetupId" },
                unique: true,
                filter: "Recommended = 1");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSetups_BeanId",
                table: "BrewSetups",
                column: "BeanId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSetups_BrewerId",
                table: "BrewSetups",
                column: "BrewerId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewSetups_GrinderId_BrewerId_BeanId",
                table: "BrewSetups",
                columns: new[] { "GrinderId", "BrewerId", "BeanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrinderDials_GrinderId",
                table: "GrinderDials",
                column: "GrinderId");

            migrationBuilder.CreateIndex(
                name: "IX_GrinderElementCompatibilities_GrindingElementId",
                table: "GrinderElementCompatibilities",
                column: "GrindingElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Grinders_BrandId",
                table: "Grinders",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_GrindingElements_GrindingMechanismId",
                table: "GrindingElements",
                column: "GrindingMechanismId");

            migrationBuilder.CreateIndex(
                name: "IX_Roasters_Name_Location",
                table: "Roasters",
                columns: new[] { "Name", "Location" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBrewSetups_BrewSetupId",
                table: "UserBrewSetups",
                column: "BrewSetupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBrewSetups_CreatedById",
                table: "UserBrewSetups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserBrewSetups_UpdatedById",
                table: "UserBrewSetups",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BrewerStockSettings");

            migrationBuilder.DropTable(
                name: "BrewerUserSettings");

            migrationBuilder.DropTable(
                name: "BrewGrinderDialSettings");

            migrationBuilder.DropTable(
                name: "GrinderElementCompatibilities");

            migrationBuilder.DropTable(
                name: "UserBrewSetups");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BrewSettings");

            migrationBuilder.DropTable(
                name: "GrinderDials");

            migrationBuilder.DropTable(
                name: "GrindingElements");

            migrationBuilder.DropTable(
                name: "BeanBatches");

            migrationBuilder.DropTable(
                name: "BrewSetups");

            migrationBuilder.DropTable(
                name: "GrindingMechanisms");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Beans");

            migrationBuilder.DropTable(
                name: "Brewers");

            migrationBuilder.DropTable(
                name: "Grinders");

            migrationBuilder.DropTable(
                name: "Roasters");

            migrationBuilder.DropTable(
                name: "BrewMethods");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}

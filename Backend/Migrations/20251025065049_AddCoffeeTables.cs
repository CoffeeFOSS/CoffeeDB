using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCoffeeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Burrs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Diameter = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Burrs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grinders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ModelAlias = table.Column<string>(type: "TEXT", nullable: true),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    BrandAlias = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseDate = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grinders", x => x.Id);
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
                name: "Brewers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    ModelAlias = table.Column<string>(type: "TEXT", nullable: true),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    BrandAlias = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    WaterCapacity = table.Column<decimal>(type: "TEXT", nullable: true),
                    BrewMethodId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brewers_BrewMethods_BrewMethodId",
                        column: x => x.BrewMethodId,
                        principalTable: "BrewMethods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GrinderBurrs",
                columns: table => new
                {
                    GrinderId = table.Column<int>(type: "INTEGER", nullable: false),
                    BurrId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrinderBurrs", x => new { x.GrinderId, x.BurrId });
                    table.ForeignKey(
                        name: "FK_GrinderBurrs_Burrs_BurrId",
                        column: x => x.BurrId,
                        principalTable: "Burrs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrinderBurrs_Grinders_GrinderId",
                        column: x => x.GrinderId,
                        principalTable: "Grinders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Beans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Decaf = table.Column<bool>(type: "INTEGER", nullable: false),
                    Elevation = table.Column<int>(type: "INTEGER", nullable: true),
                    Roast = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Region = table.Column<string>(type: "TEXT", nullable: true),
                    Farm = table.Column<string>(type: "TEXT", nullable: true),
                    Varietal = table.Column<string>(type: "TEXT", nullable: true),
                    Producer = table.Column<string>(type: "TEXT", nullable: true),
                    Importer = table.Column<string>(type: "TEXT", nullable: true),
                    Process = table.Column<string>(type: "TEXT", nullable: true),
                    FlavorProfile = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
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
                    WaterTemperature = table.Column<decimal>(type: "TEXT", nullable: false),
                    WaterVolume = table.Column<decimal>(type: "TEXT", nullable: false),
                    BrewTime = table.Column<decimal>(type: "TEXT", nullable: false),
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
                name: "IX_BrewSettings_UserId_BrewSetupId_BeanBatchId",
                table: "BrewSettings",
                columns: new[] { "UserId", "BrewSetupId", "BeanBatchId" },
                unique: true);

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
                name: "IX_GrinderBurrs_BurrId",
                table: "GrinderBurrs",
                column: "BurrId");

            migrationBuilder.CreateIndex(
                name: "IX_GrinderDials_GrinderId",
                table: "GrinderDials",
                column: "GrinderId");

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
                name: "BrewerStockSettings");

            migrationBuilder.DropTable(
                name: "BrewerUserSettings");

            migrationBuilder.DropTable(
                name: "BrewGrinderDialSettings");

            migrationBuilder.DropTable(
                name: "GrinderBurrs");

            migrationBuilder.DropTable(
                name: "UserBrewSetups");

            migrationBuilder.DropTable(
                name: "BrewSettings");

            migrationBuilder.DropTable(
                name: "GrinderDials");

            migrationBuilder.DropTable(
                name: "Burrs");

            migrationBuilder.DropTable(
                name: "BeanBatches");

            migrationBuilder.DropTable(
                name: "BrewSetups");

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
        }
    }
}

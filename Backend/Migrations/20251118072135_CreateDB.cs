using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_users_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    alias = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "brew_methods",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brew_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "grinding_mechanisms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grinding_mechanisms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roasters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    alias = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    location_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    location_coordinates = table.Column<Point>(type: "geography (point, 4326)", nullable: true),
                    website_url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roasters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "revision_metadatas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<int>(type: "integer", nullable: false),
                    parent_revision_id = table.Column<int>(type: "integer", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_revision_metadatas", x => x.id);
                    table.ForeignKey(
                        name: "fk_revision_metadatas_revision_metadatas_parent_revision_id",
                        column: x => x.parent_revision_id,
                        principalTable: "revision_metadatas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_revision_metadatas_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_revision_metadatas_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "grinders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    model_alias = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    release_date = table.Column<int>(type: "integer", nullable: true),
                    brand_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grinders", x => x.id);
                    table.CheckConstraint("CK_Grinder_ReleaseDate_8Digits", "(\"release_date\" IS NULL) OR (\"release_date\" >= 10000000 AND \"release_date\" <= 99999999)");
                    table.ForeignKey(
                        name: "fk_grinders_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "brewers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    model_alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    release_date = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    water_capacity = table.Column<int>(type: "integer", nullable: true),
                    brew_method_id = table.Column<int>(type: "integer", nullable: false),
                    brand_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brewers", x => x.id);
                    table.CheckConstraint("CK_Brewer_ReleaseDate_8Digits", "(\"release_date\" IS NULL) OR (\"release_date\" >= 10000000 AND \"release_date\" <= 99999999)");
                    table.CheckConstraint("CK_Brewer_WaterCapacity_Positive", "(\"water_capacity\" IS NULL) OR (\"water_capacity\" > 0)");
                    table.ForeignKey(
                        name: "fk_brewers_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_brewers_brew_methods_brew_method_id",
                        column: x => x.brew_method_id,
                        principalTable: "brew_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "grinding_elements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    diameter = table.Column<float>(type: "real", nullable: false),
                    grinding_mechanism_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grinding_elements", x => x.id);
                    table.CheckConstraint("CK_GrindingElement_Diameter_Positive", "\"diameter\" >= 0");
                    table.ForeignKey(
                        name: "fk_grinding_elements_grinding_mechanisms_grinding_mechanism_id",
                        column: x => x.grinding_mechanism_id,
                        principalTable: "grinding_mechanisms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roaster_revisions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    alias = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    location_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    location_coordinates = table.Column<Point>(type: "geography (point, 4326)", nullable: true),
                    website_url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    roaster_id = table.Column<int>(type: "integer", nullable: true),
                    revision_metadata_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roaster_revisions", x => x.id);
                    table.ForeignKey(
                        name: "fk_roaster_revisions_revision_metadatas_revision_metadata_id",
                        column: x => x.revision_metadata_id,
                        principalTable: "revision_metadatas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_roaster_revisions_roasters_roaster_id",
                        column: x => x.roaster_id,
                        principalTable: "roasters",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "grinder_dials",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    min = table.Column<float>(type: "real", nullable: true),
                    max = table.Column<float>(type: "real", nullable: true),
                    step = table.Column<float>(type: "real", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    grinder_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grinder_dials", x => x.id);
                    table.ForeignKey(
                        name: "fk_grinder_dials_grinders_grinder_id",
                        column: x => x.grinder_id,
                        principalTable: "grinders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brewer_stock_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    water_temperature = table.Column<float>(type: "real", nullable: true),
                    water_volume = table.Column<float>(type: "real", nullable: true),
                    brew_time = table.Column<float>(type: "real", nullable: true),
                    brewer_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brewer_stock_settings", x => x.id);
                    table.CheckConstraint("CK_BrewerStockSetting_BrewTime_Positive", "(\"brew_time\" IS NULL) OR (\"brew_time\" >= 0)");
                    table.CheckConstraint("CK_BrewerStockSetting_WaterTemperature_Positive", "(\"water_temperature\" IS NULL) OR (\"water_temperature\" >= 0)");
                    table.CheckConstraint("CK_BrewerStockSetting_WaterVolume_Positive", "(\"water_volume\" IS NULL) OR (\"water_volume\" >= 0)");
                    table.ForeignKey(
                        name: "fk_brewer_stock_settings_brewers_brewer_id",
                        column: x => x.brewer_id,
                        principalTable: "brewers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brewer_user_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    water_temperature = table.Column<float>(type: "real", nullable: true),
                    water_volume = table.Column<float>(type: "real", nullable: true),
                    brew_time = table.Column<float>(type: "real", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    brewer_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brewer_user_settings", x => x.id);
                    table.CheckConstraint("CK_BrewerUserSetting_BrewTime_Positive", "(\"brew_time\" IS NULL) OR (\"brew_time\" >= 0)");
                    table.CheckConstraint("CK_BrewerUserSetting_WaterTemperature_Positive", "(\"water_temperature\" IS NULL) OR (\"water_temperature\" >= 0)");
                    table.CheckConstraint("CK_BrewerUserSetting_WaterVolume_Positive", "(\"water_volume\" IS NULL) OR (\"water_volume\" >= 0)");
                    table.ForeignKey(
                        name: "fk_brewer_user_settings_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_brewer_user_settings_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_brewer_user_settings_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brewer_user_settings_brewers_brewer_id",
                        column: x => x.brewer_id,
                        principalTable: "brewers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "grinder_element_compatibilities",
                columns: table => new
                {
                    grinder_id = table.Column<int>(type: "integer", nullable: false),
                    grinding_element_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grinder_element_compatibilities", x => new { x.grinder_id, x.grinding_element_id });
                    table.ForeignKey(
                        name: "fk_grinder_element_compatibilities_grinders_grinder_id",
                        column: x => x.grinder_id,
                        principalTable: "grinders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_grinder_element_compatibilities_grinding_elements_grinding_",
                        column: x => x.grinding_element_id,
                        principalTable: "grinding_elements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beans",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    alias = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    decaf = table.Column<bool>(type: "boolean", nullable: false),
                    elevation_min = table.Column<int>(type: "integer", nullable: true),
                    elevation_max = table.Column<int>(type: "integer", nullable: true),
                    roast = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    farm = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    wet_mill = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    varietal = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    producer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    importer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    process = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    flavor_profile = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    release_date = table.Column<int>(type: "integer", nullable: true),
                    roaster_id = table.Column<int>(type: "integer", nullable: true),
                    roaster_revision_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_beans", x => x.id);
                    table.CheckConstraint("CK_Bean_ReleaseDate_8Digits", "(\"release_date\" IS NULL) OR (\"release_date\" >= 10000000 AND \"release_date\" <= 99999999)");
                    table.ForeignKey(
                        name: "fk_beans_roaster_revisions_roaster_revision_id",
                        column: x => x.roaster_revision_id,
                        principalTable: "roaster_revisions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_beans_roasters_roaster_id",
                        column: x => x.roaster_id,
                        principalTable: "roasters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "bean_batches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roast_date = table.Column<DateOnly>(type: "date", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    bean_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bean_batches", x => x.id);
                    table.ForeignKey(
                        name: "fk_bean_batches_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_bean_batches_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_bean_batches_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bean_batches_beans_bean_id",
                        column: x => x.bean_id,
                        principalTable: "beans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brew_setups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    grinder_id = table.Column<int>(type: "integer", nullable: false),
                    brewer_id = table.Column<int>(type: "integer", nullable: false),
                    bean_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brew_setups", x => x.id);
                    table.ForeignKey(
                        name: "fk_brew_setups_beans_bean_id",
                        column: x => x.bean_id,
                        principalTable: "beans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brew_setups_brewers_brewer_id",
                        column: x => x.brewer_id,
                        principalTable: "brewers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brew_setups_grinders_grinder_id",
                        column: x => x.grinder_id,
                        principalTable: "grinders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brew_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sour = table.Column<int>(type: "integer", nullable: false),
                    bitter = table.Column<int>(type: "integer", nullable: false),
                    recommended = table.Column<bool>(type: "boolean", nullable: false),
                    water_temperature = table.Column<float>(type: "real", nullable: true),
                    water_volume = table.Column<float>(type: "real", nullable: true),
                    brew_time = table.Column<float>(type: "real", nullable: true),
                    dose = table.Column<float>(type: "real", nullable: true),
                    grind_time = table.Column<float>(type: "real", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    brew_setup_id = table.Column<int>(type: "integer", nullable: false),
                    bean_batch_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brew_settings", x => x.id);
                    table.CheckConstraint("CHK_BrewSetting_DoseOrGrindTime", "(\"dose\" IS NOT NULL) OR (\"grind_time\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_brew_settings_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_brew_settings_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_brew_settings_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brew_settings_bean_batches_bean_batch_id",
                        column: x => x.bean_batch_id,
                        principalTable: "bean_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brew_settings_brew_setups_brew_setup_id",
                        column: x => x.brew_setup_id,
                        principalTable: "brew_setups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_brew_setups",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    brew_setup_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_brew_setups", x => new { x.user_id, x.brew_setup_id });
                    table.ForeignKey(
                        name: "fk_user_brew_setups_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_brew_setups_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_brew_setups_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_brew_setups_brew_setups_brew_setup_id",
                        column: x => x.brew_setup_id,
                        principalTable: "brew_setups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brew_grinder_dial_settings",
                columns: table => new
                {
                    brew_setting_id = table.Column<int>(type: "integer", nullable: false),
                    grinder_dial_id = table.Column<int>(type: "integer", nullable: false),
                    dial_value = table.Column<float>(type: "real", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brew_grinder_dial_settings", x => new { x.brew_setting_id, x.grinder_dial_id });
                    table.ForeignKey(
                        name: "fk_brew_grinder_dial_settings_brew_settings_brew_setting_id",
                        column: x => x.brew_setting_id,
                        principalTable: "brew_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brew_grinder_dial_settings_grinder_dials_grinder_dial_id",
                        column: x => x.grinder_dial_id,
                        principalTable: "grinder_dials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brew_grinder_dial_settings_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_brew_grinder_dial_settings_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_updated_by_id",
                table: "AspNetUsers",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bean_batches_bean_id",
                table: "bean_batches",
                column: "bean_id");

            migrationBuilder.CreateIndex(
                name: "ix_bean_batches_created_by_id",
                table: "bean_batches",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_bean_batches_updated_by_id",
                table: "bean_batches",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_bean_batches_user_id",
                table: "bean_batches",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_beans_roaster_id_name",
                table: "beans",
                columns: new[] { "roaster_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_beans_roaster_revision_id",
                table: "beans",
                column: "roaster_revision_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_grinder_dial_settings_created_by_id",
                table: "brew_grinder_dial_settings",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_grinder_dial_settings_grinder_dial_id",
                table: "brew_grinder_dial_settings",
                column: "grinder_dial_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_grinder_dial_settings_updated_by_id",
                table: "brew_grinder_dial_settings",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_methods_name",
                table: "brew_methods",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_brew_settings_bean_batch_id",
                table: "brew_settings",
                column: "bean_batch_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_settings_brew_setup_id",
                table: "brew_settings",
                column: "brew_setup_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_settings_created_by_id",
                table: "brew_settings",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_settings_updated_by_id",
                table: "brew_settings",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_settings_user_id_brew_setup_id",
                table: "brew_settings",
                columns: new[] { "user_id", "brew_setup_id" },
                unique: true,
                filter: "\"recommended\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "ix_brew_setups_bean_id",
                table: "brew_setups",
                column: "bean_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_setups_brewer_id",
                table: "brew_setups",
                column: "brewer_id");

            migrationBuilder.CreateIndex(
                name: "ix_brew_setups_grinder_id_brewer_id_bean_id",
                table: "brew_setups",
                columns: new[] { "grinder_id", "brewer_id", "bean_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_brewer_stock_settings_brewer_id",
                table: "brewer_stock_settings",
                column: "brewer_id");

            migrationBuilder.CreateIndex(
                name: "ix_brewer_user_settings_brewer_id",
                table: "brewer_user_settings",
                column: "brewer_id");

            migrationBuilder.CreateIndex(
                name: "ix_brewer_user_settings_created_by_id",
                table: "brewer_user_settings",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_brewer_user_settings_updated_by_id",
                table: "brewer_user_settings",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_brewer_user_settings_user_id",
                table: "brewer_user_settings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_brewers_brand_id",
                table: "brewers",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_brewers_brew_method_id",
                table: "brewers",
                column: "brew_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_grinder_dials_grinder_id",
                table: "grinder_dials",
                column: "grinder_id");

            migrationBuilder.CreateIndex(
                name: "ix_grinder_element_compatibilities_grinding_element_id",
                table: "grinder_element_compatibilities",
                column: "grinding_element_id");

            migrationBuilder.CreateIndex(
                name: "ix_grinders_brand_id",
                table: "grinders",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_grinding_elements_grinding_mechanism_id",
                table: "grinding_elements",
                column: "grinding_mechanism_id");

            migrationBuilder.CreateIndex(
                name: "ix_revision_metadatas_created_by_id",
                table: "revision_metadatas",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_revision_metadatas_parent_revision_id",
                table: "revision_metadatas",
                column: "parent_revision_id");

            migrationBuilder.CreateIndex(
                name: "ix_revision_metadatas_updated_by_id",
                table: "revision_metadatas",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_roaster_revisions_revision_metadata_id",
                table: "roaster_revisions",
                column: "revision_metadata_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roaster_revisions_roaster_id",
                table: "roaster_revisions",
                column: "roaster_id");

            migrationBuilder.CreateIndex(
                name: "ix_roasters_name_location_address",
                table: "roasters",
                columns: new[] { "name", "location_address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_brew_setups_brew_setup_id",
                table: "user_brew_setups",
                column: "brew_setup_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_brew_setups_created_by_id",
                table: "user_brew_setups",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_brew_setups_updated_by_id",
                table: "user_brew_setups",
                column: "updated_by_id");
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
                name: "brew_grinder_dial_settings");

            migrationBuilder.DropTable(
                name: "brewer_stock_settings");

            migrationBuilder.DropTable(
                name: "brewer_user_settings");

            migrationBuilder.DropTable(
                name: "grinder_element_compatibilities");

            migrationBuilder.DropTable(
                name: "user_brew_setups");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "brew_settings");

            migrationBuilder.DropTable(
                name: "grinder_dials");

            migrationBuilder.DropTable(
                name: "grinding_elements");

            migrationBuilder.DropTable(
                name: "bean_batches");

            migrationBuilder.DropTable(
                name: "brew_setups");

            migrationBuilder.DropTable(
                name: "grinding_mechanisms");

            migrationBuilder.DropTable(
                name: "beans");

            migrationBuilder.DropTable(
                name: "brewers");

            migrationBuilder.DropTable(
                name: "grinders");

            migrationBuilder.DropTable(
                name: "roaster_revisions");

            migrationBuilder.DropTable(
                name: "brew_methods");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "revision_metadatas");

            migrationBuilder.DropTable(
                name: "roasters");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}

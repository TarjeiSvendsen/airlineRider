using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace airlineRider.Migrations
{
    /// <inheritdoc />
    public partial class expandedTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "AircraftTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false),
                    Model = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Iata = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Icao = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BodyType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AircraftTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    Icao = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AType = table.Column<string>(type: "text", nullable: false),
                    Iata = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Alpha2CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Elevation = table.Column<double>(type: "double precision", nullable: false),
                    Location = table.Column<Point>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.Icao);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: false),
                    Alpha2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Alpha3 = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IcaoPrefix = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Lobbies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    AccessCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Details_CurrentTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Settings_EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Settings_StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Settings_StartingMoney = table.Column<int>(type: "integer", nullable: false),
                    StarterAircraftDetails_StarterAircraftAmount = table.Column<int>(type: "integer", nullable: false),
                    StarterAircraftDetails_StarterAircraftTypeIcao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobbies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiveryInfos",
                columns: table => new
                {
                    LiveryInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkDir = table.Column<string>(type: "text", nullable: false),
                    AircraftTypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveryInfos", x => x.LiveryInfoId);
                    table.ForeignKey(
                        name: "FK_LiveryInfos_AircraftTypes_LiveryInfoId",
                        column: x => x.LiveryInfoId,
                        principalTable: "AircraftTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Runways",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AirportId = table.Column<string>(type: "character varying(4)", nullable: false),
                    LeIdent = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    LeHeading = table.Column<int>(type: "integer", nullable: false),
                    HeIdent = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    HeHeading = table.Column<int>(type: "integer", nullable: false),
                    Length = table.Column<double>(type: "double precision", nullable: false),
                    Width = table.Column<double>(type: "double precision", nullable: false),
                    Closed = table.Column<bool>(type: "boolean", nullable: false),
                    Lighted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runways", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Runways_Airports_AirportId",
                        column: x => x.AirportId,
                        principalTable: "Airports",
                        principalColumn: "Icao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Airlines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LobbyId = table.Column<int>(type: "integer", nullable: false),
                    SystemUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airlines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Airlines_Lobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Airlines_Users_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    SystemUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_LobbyId",
                table: "Airlines",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_Name",
                table: "Airlines",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_SystemUserId",
                table: "Airlines",
                column: "SystemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Runways_AirportId",
                table: "Runways",
                column: "AirportId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_SystemUserId",
                table: "UserRoles",
                column: "SystemUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Airlines");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "LiveryInfos");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Runways");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Lobbies");

            migrationBuilder.DropTable(
                name: "AircraftTypes");

            migrationBuilder.DropTable(
                name: "Airports");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

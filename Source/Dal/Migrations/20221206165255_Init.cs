using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dal.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GpParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GpParametersRecommendationsBenchmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpParametersRecommendationsBenchmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrustedHardware",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    HardwareId = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustedHardware", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrustedHardwareBenchmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustedHardwareBenchmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrustedSoftware",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustedSoftware", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrustedSoftwareBenchmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustedSoftwareBenchmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GpParameterRegistryParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    GpParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyName = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpParameterRegistryParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GpParameterRegistryParameters_GpParameters",
                        column: x => x.GpParameterId,
                        principalTable: "GpParameters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GpParametersRecommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    GpParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Direction = table.Column<bool>(type: "bit", nullable: true),
                    Rationale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Impact = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpParametersRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GpParametersRecommendations_GpParameters",
                        column: x => x.GpParameterId,
                        principalTable: "GpParameters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BenchmarksTrustedHardwareRelation",
                columns: table => new
                {
                    TrustedHardwareBenchmarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrustedHardwareId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarksTrustedHardwareRelation", x => new { x.TrustedHardwareBenchmarkId, x.TrustedHardwareId });
                    table.ForeignKey(
                        name: "FK_BenchmarksTrustedHardwareRelations_TrustedHardware",
                        column: x => x.TrustedHardwareId,
                        principalTable: "TrustedHardware",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BenchmarksTrustedHardwareRelations_TrustedHardwareBenchmarks",
                        column: x => x.TrustedHardwareBenchmarkId,
                        principalTable: "TrustedHardwareBenchmarks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Benchmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GpParametersRecommendationsBenchmarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrustedSoftwareBenchmarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrustedHardwareBenchmarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Benchmarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Benchmarks_GpParametersRecommendationsBenchmarks",
                        column: x => x.GpParametersRecommendationsBenchmarkId,
                        principalTable: "GpParametersRecommendationsBenchmarks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Benchmarks_TrustedHardwareBenchmarks",
                        column: x => x.TrustedHardwareBenchmarkId,
                        principalTable: "TrustedHardwareBenchmarks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Benchmarks_TrustedSoftwareBenchmarks",
                        column: x => x.TrustedSoftwareBenchmarkId,
                        principalTable: "TrustedSoftwareBenchmarks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BenchmarksTrustedSoftwareRelation",
                columns: table => new
                {
                    TrustedSoftwareBenchmarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrustedSoftwareId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarksTrustedSoftwareRelation", x => new { x.TrustedSoftwareBenchmarkId, x.TrustedSoftwareId });
                    table.ForeignKey(
                        name: "FK_BenchmarksTrustedSoftwareRelations_TrustedSoftware",
                        column: x => x.TrustedSoftwareId,
                        principalTable: "TrustedSoftware",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BenchmarksTrustedSoftwareRelations_TrustedSoftwareBenchmarks",
                        column: x => x.TrustedSoftwareBenchmarkId,
                        principalTable: "TrustedSoftwareBenchmarks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BenchmarksGpParametersRecommendationsRelation",
                columns: table => new
                {
                    GpParametersRecommendationsBenchmarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GpParametersRecommendationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarksGpParametersRecommendationsRelation", x => new { x.GpParametersRecommendationsBenchmarkId, x.GpParametersRecommendationId });
                    table.ForeignKey(
                        name: "FK_BenchmarksGpParametersRecommendationsRelations_GpParametersRecommendations",
                        column: x => x.GpParametersRecommendationId,
                        principalTable: "GpParametersRecommendations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BenchmarksGpParametersRecommendationsRelations_GpParametersRecommendationsBenchmarks",
                        column: x => x.GpParametersRecommendationsBenchmarkId,
                        principalTable: "GpParametersRecommendationsBenchmarks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "AK_Benchmarks_Name",
                table: "Benchmarks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Benchmarks_GpParametersRecommendationsBenchmarkId",
                table: "Benchmarks",
                column: "GpParametersRecommendationsBenchmarkId");

            migrationBuilder.CreateIndex(
                name: "IX_Benchmarks_TrustedHardwareBenchmarkId",
                table: "Benchmarks",
                column: "TrustedHardwareBenchmarkId");

            migrationBuilder.CreateIndex(
                name: "IX_Benchmarks_TrustedSoftwareBenchmarkId",
                table: "Benchmarks",
                column: "TrustedSoftwareBenchmarkId");

            migrationBuilder.CreateIndex(
                name: "IX_BenchmarksGpParametersRecommendationsRelation_GpParametersRecommendationId",
                table: "BenchmarksGpParametersRecommendationsRelation",
                column: "GpParametersRecommendationId");

            migrationBuilder.CreateIndex(
                name: "IX_BenchmarksTrustedHardwareRelation_TrustedHardwareId",
                table: "BenchmarksTrustedHardwareRelation",
                column: "TrustedHardwareId");

            migrationBuilder.CreateIndex(
                name: "IX_BenchmarksTrustedSoftwareRelation_TrustedSoftwareId",
                table: "BenchmarksTrustedSoftwareRelation",
                column: "TrustedSoftwareId");

            migrationBuilder.CreateIndex(
                name: "AK_GpParameterRegistryParameters_KeyName_ParameterName",
                table: "GpParameterRegistryParameters",
                columns: new[] { "KeyName", "ParameterName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GpParameterRegistryParameters_GpParameterId",
                table: "GpParameterRegistryParameters",
                column: "GpParameterId");

            migrationBuilder.CreateIndex(
                name: "AK_GpParameters_Name",
                table: "GpParameters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GpParametersRecommendations_GpParameterId",
                table: "GpParametersRecommendations",
                column: "GpParameterId");

            migrationBuilder.CreateIndex(
                name: "AK_GpParametersRecommendationsBenchmarks_Name",
                table: "GpParametersRecommendationsBenchmarks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "AK_TrustedHardware_HardwareId",
                table: "TrustedHardware",
                column: "HardwareId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "AK_TrustedHardwareBenchmarks_Name",
                table: "TrustedHardwareBenchmarks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "AK_TrustedSoftware_Name_Version",
                table: "TrustedSoftware",
                columns: new[] { "Name", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "AK_TrustedSoftwareBenchmarks_Name",
                table: "TrustedSoftwareBenchmarks",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Benchmarks");

            migrationBuilder.DropTable(
                name: "BenchmarksGpParametersRecommendationsRelation");

            migrationBuilder.DropTable(
                name: "BenchmarksTrustedHardwareRelation");

            migrationBuilder.DropTable(
                name: "BenchmarksTrustedSoftwareRelation");

            migrationBuilder.DropTable(
                name: "GpParameterRegistryParameters");

            migrationBuilder.DropTable(
                name: "GpParametersRecommendations");

            migrationBuilder.DropTable(
                name: "GpParametersRecommendationsBenchmarks");

            migrationBuilder.DropTable(
                name: "TrustedHardware");

            migrationBuilder.DropTable(
                name: "TrustedHardwareBenchmarks");

            migrationBuilder.DropTable(
                name: "TrustedSoftware");

            migrationBuilder.DropTable(
                name: "TrustedSoftwareBenchmarks");

            migrationBuilder.DropTable(
                name: "GpParameters");
        }
    }
}

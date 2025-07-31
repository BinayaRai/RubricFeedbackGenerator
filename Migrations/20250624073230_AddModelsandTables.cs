using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIS_RubricFeedbackGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsandTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rubrics",
                columns: table => new
                {
                    RubricId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rubrics", x => x.RubricId);
                    table.ForeignKey(
                        name: "FK_Rubric_User",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_Students_Users",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Criteria",
                columns: table => new
                {
                    CriterionId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RubricId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criteria", x => x.CriterionId);
                    table.ForeignKey(
                        name: "FK_Criteria_Rubrics_RubricId",
                        column: x => x.RubricId,
                        principalTable: "Rubrics",
                        principalColumn: "RubricId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreDefinitions",
                columns: table => new
                {
                    ScoreDefinitionId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RubricID = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ScoreValue = table.Column<int>(type: "int", nullable: false),
                    ScoreName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreDefinitions", x => x.ScoreDefinitionId);
                    table.ForeignKey(
                        name: "FK_ScoreDefinition_Rubric",
                        column: x => x.RubricID,
                        principalTable: "Rubrics",
                        principalColumn: "RubricId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreLevels",
                columns: table => new
                {
                    ScoreLevelId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CriterionId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ScoreValue = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreLevels", x => x.ScoreLevelId);
                    table.ForeignKey(
                        name: "FK_ScoreLevel_Criterion",
                        column: x => x.CriterionId,
                        principalTable: "Criteria",
                        principalColumn: "CriterionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_RubricId",
                table: "Criteria",
                column: "RubricId");

            migrationBuilder.CreateIndex(
                name: "IX_Rubrics_CreatedBy",
                table: "Rubrics",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreDefinitions_RubricID_ScoreValue",
                table: "ScoreDefinitions",
                columns: new[] { "RubricID", "ScoreValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreLevels_CriterionId",
                table: "ScoreLevels",
                column: "CriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CreatedBy",
                table: "Students",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreDefinitions");

            migrationBuilder.DropTable(
                name: "ScoreLevels");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Criteria");

            migrationBuilder.DropTable(
                name: "Rubrics");
        }
    }
}

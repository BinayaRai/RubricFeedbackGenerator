using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIS_RubricFeedbackGenerator.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRubricIdName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RubricID",
                table: "ScoreDefinitions",
                newName: "RubricId");

            migrationBuilder.RenameIndex(
                name: "IX_ScoreDefinitions_RubricID_ScoreValue",
                table: "ScoreDefinitions",
                newName: "IX_ScoreDefinitions_RubricId_ScoreValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RubricId",
                table: "ScoreDefinitions",
                newName: "RubricID");

            migrationBuilder.RenameIndex(
                name: "IX_ScoreDefinitions_RubricId_ScoreValue",
                table: "ScoreDefinitions",
                newName: "IX_ScoreDefinitions_RubricID_ScoreValue");
        }
    }
}

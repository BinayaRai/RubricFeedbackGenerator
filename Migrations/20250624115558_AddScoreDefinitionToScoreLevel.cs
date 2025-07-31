using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIS_RubricFeedbackGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreDefinitionToScoreLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreValue",
                table: "ScoreLevels");

            migrationBuilder.AddColumn<string>(
                name: "ScoreDefinitionId",
                table: "ScoreLevels",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "ScoreValue",
                table: "ScoreDefinitions",
                type: "float(4)",
                precision: 4,
                scale: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ScoreName",
                table: "ScoreDefinitions",
                type: "varchar(25)",
                unicode: false,
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreLevels_ScoreDefinitionId",
                table: "ScoreLevels",
                column: "ScoreDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreLevel_ScoreDefinition",
                table: "ScoreLevels",
                column: "ScoreDefinitionId",
                principalTable: "ScoreDefinitions",
                principalColumn: "ScoreDefinitionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreLevel_ScoreDefinition",
                table: "ScoreLevels");

            migrationBuilder.DropIndex(
                name: "IX_ScoreLevels_ScoreDefinitionId",
                table: "ScoreLevels");

            migrationBuilder.DropColumn(
                name: "ScoreDefinitionId",
                table: "ScoreLevels");

            migrationBuilder.AddColumn<double>(
                name: "ScoreValue",
                table: "ScoreLevels",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "ScoreValue",
                table: "ScoreDefinitions",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(4)",
                oldPrecision: 4,
                oldScale: 1);

            migrationBuilder.AlterColumn<string>(
                name: "ScoreName",
                table: "ScoreDefinitions",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldUnicode: false,
                oldMaxLength: 25);
        }
    }
}

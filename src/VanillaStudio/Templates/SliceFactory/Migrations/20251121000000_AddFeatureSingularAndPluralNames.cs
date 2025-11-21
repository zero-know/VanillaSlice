using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace {{RootNamespace}}.SliceFactory.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureSingularAndPluralNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeatureSingularName",
                table: "Features",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FeaturePluralName",
                table: "Features",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureSingularName",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "FeaturePluralName",
                table: "Features");
        }
    }
}

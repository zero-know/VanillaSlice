using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace {{RootNamespace}}.SliceFactory.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyFeatureModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the Name column
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Features");

            // Add FeaturePluralName column
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
            // Re-add the Name column
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Features",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Remove FeaturePluralName column
            migrationBuilder.DropColumn(
                name: "FeaturePluralName",
                table: "Features");
        }
    }
}

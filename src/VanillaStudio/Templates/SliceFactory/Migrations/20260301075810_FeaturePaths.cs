using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace {{RootNamespace}}.SliceFactory.Migrations
{
    /// <inheritdoc />
    public partial class FeaturePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Features",
                newName: "FeaturePluralName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeaturePluralName",
                table: "Features",
                newName: "Name");
        }
    }
}

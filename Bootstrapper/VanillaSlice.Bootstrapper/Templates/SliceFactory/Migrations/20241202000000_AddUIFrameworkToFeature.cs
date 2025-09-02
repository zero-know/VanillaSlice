using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace {{RootNamespace}}.SliceFactory.Migrations
{
    /// <inheritdoc />
    public partial class AddUIFrameworkToFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UIFramework",
                table: "Features",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "Bootstrap");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UIFramework",
                table: "Features");
        }
    }
}

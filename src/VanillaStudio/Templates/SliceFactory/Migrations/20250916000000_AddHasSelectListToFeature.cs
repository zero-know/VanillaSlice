using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace {{RootNamespace}}.SliceFactory.Migrations
{
    /// <inheritdoc />
    public partial class AddHasSelectListToFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSelectList",
                table: "Features",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SelectListModelType",
                table: "Features",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "SelectOption");

            migrationBuilder.AddColumn<string>(
                name: "SelectListDataType",
                table: "Features",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "string");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSelectList",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "SelectListModelType",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "SelectListDataType",
                table: "Features");
        }
    }
}

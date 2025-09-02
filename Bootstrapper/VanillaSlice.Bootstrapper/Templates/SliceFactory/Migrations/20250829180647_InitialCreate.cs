using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace {{RootNamespace}}.SliceFactory.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Features",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                ComponentPrefix = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                ModuleNamespace = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                ProjectNamespace = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                PrimaryKeyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                BasePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                DirectoryName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                HasForm = table.Column<bool>(type: "INTEGER", nullable: false),
                HasListing = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                ProfileConfiguration = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Features", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FeatureFiles",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                FeatureId = table.Column<int>(type: "INTEGER", nullable: false),
                FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                FileName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                ProjectType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                SliceType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                Exists = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FeatureFiles", x => x.Id);
                table.ForeignKey(
                    name: "FK_FeatureFiles_Features_FeatureId",
                    column: x => x.FeatureId,
                    principalTable: "Features",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "FeatureProjects",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                FeatureId = table.Column<int>(type: "INTEGER", nullable: false),
                ProjectType = table.Column<string>(type: "TEXT", nullable: false),
                ProjectPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                ProjectNamespace = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FeatureProjects", x => x.Id);
                table.ForeignKey(
                    name: "FK_FeatureProjects_Features_FeatureId",
                    column: x => x.FeatureId,
                    principalTable: "Features",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_FeatureFiles_FeatureId",
            table: "FeatureFiles",
            column: "FeatureId");

        migrationBuilder.CreateIndex(
            name: "IX_FeatureFiles_ProjectType",
            table: "FeatureFiles",
            column: "ProjectType");

        migrationBuilder.CreateIndex(
            name: "IX_FeatureFiles_SliceType",
            table: "FeatureFiles",
            column: "SliceType");

        migrationBuilder.CreateIndex(
            name: "IX_FeatureProjects_FeatureId",
            table: "FeatureProjects",
            column: "FeatureId");

        migrationBuilder.CreateIndex(
            name: "IX_FeatureProjects_FeatureId_ProjectType",
            table: "FeatureProjects",
            columns: new[] { "FeatureId", "ProjectType" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_FeatureProjects_ProjectType",
            table: "FeatureProjects",
            column: "ProjectType");

        migrationBuilder.CreateIndex(
            name: "IX_Features_ComponentPrefix",
            table: "Features",
            column: "ComponentPrefix");

        migrationBuilder.CreateIndex(
            name: "IX_Features_ModuleNamespace",
            table: "Features",
            column: "ModuleNamespace");

        migrationBuilder.CreateIndex(
            name: "IX_Features_ModuleNamespace_ComponentPrefix",
            table: "Features",
            columns: new[] { "ModuleNamespace", "ComponentPrefix" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "FeatureFiles");

        migrationBuilder.DropTable(
            name: "FeatureProjects");

        migrationBuilder.DropTable(
            name: "Features");
    }
}
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicWhereBuilder.IntegrationTests.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestClasses",
                columns: table => new
                {
                    IQueryableTestClassId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestClasses", x => x.IQueryableTestClassId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestClasses");
        }
    }
}

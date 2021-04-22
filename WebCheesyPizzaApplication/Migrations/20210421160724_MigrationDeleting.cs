using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCheesyPizzaApplication.Migrations
{
    public partial class MigrationDeleting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Removed",
                table: "BasketProducts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "BasketProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

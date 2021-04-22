using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCheesyPizzaApplication.Migrations
{
    public partial class removedMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "BasketProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Removed",
                table: "BasketProducts");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCheesyPizzaApplication.Migrations
{
    public partial class changeinStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Оплачено");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "В ожидании");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Отменено");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "New");

            migrationBuilder.UpdateData(
                table: "OrderStates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Rejected");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebShopMVC.Data.Migrations
{
    public partial class AddAppoinmentAndProdSlectedForApp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentDate = table.Column<DateTime>(nullable: false),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerPhoneNumber = table.Column<string>(nullable: true),
                    CustomerEmail = table.Column<string>(nullable: true),
                    isConfirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSelectedForAppoinment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppoinmentId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSelectedForAppoinment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSelectedForAppoinment_Appointments_AppoinmentId",
                        column: x => x.AppoinmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSelectedForAppoinment_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSelectedForAppoinment_AppoinmentId",
                table: "ProductSelectedForAppoinment",
                column: "AppoinmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSelectedForAppoinment_ProductId",
                table: "ProductSelectedForAppoinment",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSelectedForAppoinment");

            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}

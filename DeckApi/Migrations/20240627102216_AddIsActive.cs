using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeckApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Carts_CartEntityId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CartEntityId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CartEntityId",
                table: "Items");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Carts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CartId",
                table: "Items",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId_IsActive",
                table: "Carts",
                columns: new[] { "UserId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1 AND [IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Carts_CartId",
                table: "Items",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Carts_CartId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CartId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId_IsActive",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Carts");

            migrationBuilder.AddColumn<int>(
                name: "CartEntityId",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CartEntityId",
                table: "Items",
                column: "CartEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Carts_CartEntityId",
                table: "Items",
                column: "CartEntityId",
                principalTable: "Carts",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Migrations
{
    /// <inheritdoc />
    public partial class new_typeId_forgin_key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TypeId",
                table: "Departments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_TypeId",
                table: "Departments",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Lookups_TypeId",
                table: "Departments",
                column: "TypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Lookups_TypeId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_TypeId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Departments");
        }
    }
}

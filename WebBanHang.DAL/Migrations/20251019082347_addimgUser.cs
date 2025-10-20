using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addimgUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imgUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imgUrl",
                table: "AspNetUsers");
        }
    }
}

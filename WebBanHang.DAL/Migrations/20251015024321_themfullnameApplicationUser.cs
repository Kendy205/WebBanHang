using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.DAL.Migrations
{
    /// <inheritdoc />
    public partial class themfullnameApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FulName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FulName",
                table: "AspNetUsers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace FilmsCatalog.Migrations
{
    public partial class newPager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Movie",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movie_CreatorId",
                table: "Movie",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_AspNetUsers_CreatorId",
                table: "Movie",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_AspNetUsers_CreatorId",
                table: "Movie");

            migrationBuilder.DropIndex(
                name: "IX_Movie_CreatorId",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Movie");
        }
    }
}

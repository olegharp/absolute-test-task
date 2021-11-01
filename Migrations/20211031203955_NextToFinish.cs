using Microsoft.EntityFrameworkCore.Migrations;

namespace FilmsCatalog.Migrations
{
    public partial class NextToFinish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_AspNetUsers_CreatorId",
                table: "Movie");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Movie",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Movie_CreatorId",
                table: "Movie",
                newName: "IX_Movie_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_AspNetUsers_UserId",
                table: "Movie",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_AspNetUsers_UserId",
                table: "Movie");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Movie",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Movie_UserId",
                table: "Movie",
                newName: "IX_Movie_CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_AspNetUsers_CreatorId",
                table: "Movie",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

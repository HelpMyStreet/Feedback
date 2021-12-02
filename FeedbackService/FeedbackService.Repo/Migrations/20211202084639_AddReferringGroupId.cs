using Microsoft.EntityFrameworkCore.Migrations;

namespace FeedbackService.Repo.Migrations
{
    public partial class AddReferringGroupId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferringGroupId",
                schema: "Feedback",
                table: "Feedback",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferringGroupId",
                schema: "Feedback",
                table: "Feedback");
        }
    }
}

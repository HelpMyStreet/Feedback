using Microsoft.EntityFrameworkCore.Migrations;

namespace FeedbackService.Repo.Migrations
{
    public partial class AddUpdateChangeTrackingProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

CREATE PROCEDURE [dbo].[Update_ChangeTracking_Version] @CurrentTrackingVersion BIGINT, @TableName varchar(50)
AS

BEGIN

    UPDATE table_store_ChangeTracking_version
    SET [SYS_CHANGE_VERSION] = @CurrentTrackingVersion
WHERE [TableName] = @TableName

END   
GO
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP PROCEDURE [dbo].[Update_ChangeTracking_Version]
                ");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LastKey_Infrastructure.Migrations
{
    public partial class LockState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_locked",
                table: "Locks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_locked",
                table: "Locks");
        }
    }
}

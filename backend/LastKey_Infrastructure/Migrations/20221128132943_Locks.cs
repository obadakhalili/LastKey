using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LastKey_Infrastructure.Migrations
{
    public partial class Locks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    mac_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    admin_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Locks_Users_admin_id",
                        column: x => x.admin_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locks_admin_id_name",
                table: "Locks",
                columns: new[] { "admin_id", "name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locks");
        }
    }
}

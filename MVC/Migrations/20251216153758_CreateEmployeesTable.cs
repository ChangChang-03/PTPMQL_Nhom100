using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    public partial class CreateEmployeesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ❌ TUYỆT ĐỐI KHÔNG ĐỘNG TỚI EMPLOYEES
            // DB ĐÃ CÓ ĐẦY ĐỦ CỘT

            migrationBuilder.CreateTable(
                name: "MemberUnits",
                columns: table => new
                {
                    MemberUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberUnits", x => x.MemberUnitId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberUnits");
        }
    }
}

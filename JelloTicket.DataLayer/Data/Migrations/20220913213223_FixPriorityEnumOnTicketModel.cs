using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JelloTicket.DataLayer.Data.Migrations
{
    public partial class FixPriorityEnumOnTicketModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketPriority",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketPriority",
                table: "Tickets");
        }
    }
}

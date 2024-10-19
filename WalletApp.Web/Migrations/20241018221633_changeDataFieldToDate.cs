using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class changeDataFieldToDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Transactions",
                newName: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Transactions",
                newName: "Data");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoSafe.Write.Data.Migrations
{
    /// <inheritdoc />
    public partial class Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Main");

            migrationBuilder.RenameTable(
                name: "PhoneNumbers",
                newName: "PhoneNumbers",
                newSchema: "Main");

            migrationBuilder.RenameTable(
                name: "EmailAddresses",
                newName: "EmailAddresses",
                newSchema: "Main");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "Contacts",
                newSchema: "Main");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Addresses",
                newSchema: "Main");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PhoneNumbers",
                schema: "Main",
                newName: "PhoneNumbers");

            migrationBuilder.RenameTable(
                name: "EmailAddresses",
                schema: "Main",
                newName: "EmailAddresses");

            migrationBuilder.RenameTable(
                name: "Contacts",
                schema: "Main",
                newName: "Contacts");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "Main",
                newName: "Addresses");
        }
    }
}
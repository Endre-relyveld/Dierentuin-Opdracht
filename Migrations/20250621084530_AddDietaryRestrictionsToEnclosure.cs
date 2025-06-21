using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DierenTuin_opdracht.Migrations
{
    /// <inheritdoc />
    public partial class AddDietaryRestrictionsToEnclosure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DietaryRestrictions",
                table: "Enclosures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietaryRestrictions",
                table: "Enclosures");
        }
    }
}

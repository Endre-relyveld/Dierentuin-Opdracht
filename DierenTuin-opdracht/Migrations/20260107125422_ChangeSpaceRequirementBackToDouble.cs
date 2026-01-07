using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DierenTuin_opdracht.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSpaceRequirementBackToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SpaceRequirement",
                table: "Animals",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SpaceRequirement",
                table: "Animals",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}

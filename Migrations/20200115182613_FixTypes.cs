using Microsoft.EntityFrameworkCore.Migrations;

namespace ContosoUniversity2020.Migrations
{
    public partial class FixTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Person",
                maxLength: 65,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 65);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Person",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Person",
                maxLength: 85,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 85);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Person",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Person",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Department",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 60);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LastName",
                table: "Person",
                type: "int",
                maxLength: 65,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 65);

            migrationBuilder.AlterColumn<int>(
                name: "FirstName",
                table: "Person",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Email",
                table: "Person",
                type: "int",
                maxLength: 85,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 85);

            migrationBuilder.AlterColumn<int>(
                name: "City",
                table: "Person",
                type: "int",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<int>(
                name: "Address",
                table: "Person",
                type: "int",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "Department",
                type: "int",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 60);
        }
    }
}

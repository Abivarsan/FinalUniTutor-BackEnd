using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniTutor.Migrations
{
    /// <inheritdoc />
    public partial class varsha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Students_studentId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Tutors_tutorId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tutors_tutorId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "StripeSessionId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Students_studentId",
                table: "Reports",
                column: "studentId",
                principalTable: "Students",
                principalColumn: "_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Tutors_tutorId",
                table: "Reports",
                column: "tutorId",
                principalTable: "Tutors",
                principalColumn: "_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tutors_tutorId",
                table: "Transactions",
                column: "tutorId",
                principalTable: "Tutors",
                principalColumn: "_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Students_studentId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Tutors_tutorId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tutors_tutorId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "StripeSessionId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Students_studentId",
                table: "Reports",
                column: "studentId",
                principalTable: "Students",
                principalColumn: "_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Tutors_tutorId",
                table: "Reports",
                column: "tutorId",
                principalTable: "Tutors",
                principalColumn: "_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tutors_tutorId",
                table: "Transactions",
                column: "tutorId",
                principalTable: "Tutors",
                principalColumn: "_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

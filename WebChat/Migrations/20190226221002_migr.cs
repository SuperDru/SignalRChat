using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChat.Migrations
{
    public partial class migr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_rooms_room_id",
                table: "messages");

            migrationBuilder.AlterColumn<int>(
                name: "room_id",
                table: "messages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "room_credentials",
                keyColumn: "room_id",
                keyValue: 1,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "cIoBrtd+h++b9YDUUI70U9lLtPqZVkgEBQGhEsCgApI=", new byte[] { 92, 190, 212, 211, 31, 252, 31, 131, 119, 115, 202, 30, 248, 16, 33, 199 } });

            migrationBuilder.UpdateData(
                table: "room_credentials",
                keyColumn: "room_id",
                keyValue: 2,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "R+B+7Wog3dF5v4HR5TEfw7ePF7aSbhYp5jVf3c2qWDM=", new byte[] { 11, 204, 221, 177, 8, 199, 167, 39, 45, 144, 135, 165, 5, 156, 48, 214 } });

            migrationBuilder.UpdateData(
                table: "user_credentials",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "EpbX3suZ7GVJ6gALNOLlMCZvZnajvSEcPSfA6eOguCE=", new byte[] { 32, 46, 151, 129, 184, 45, 169, 11, 45, 209, 210, 50, 2, 185, 48, 33 } });

            migrationBuilder.UpdateData(
                table: "user_credentials",
                keyColumn: "user_id",
                keyValue: 2,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "7MgGZN2qI0t9xKEbpQ51DrCpfF+2gYOBLEH50VGX/o4=", new byte[] { 227, 156, 82, 234, 222, 150, 232, 35, 18, 231, 10, 105, 154, 119, 152, 255 } });

            migrationBuilder.UpdateData(
                table: "user_credentials",
                keyColumn: "user_id",
                keyValue: 3,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "x5NOObMlygjfoYNWBaNPSYt7BoiomAaDWqLL9JIs7vM=", new byte[] { 138, 159, 214, 35, 114, 199, 250, 30, 176, 40, 32, 244, 61, 8, 120, 60 } });

            migrationBuilder.CreateIndex(
                name: "ix_rooms_user_id",
                table: "rooms",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_rooms_room_id",
                table: "messages",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rooms_users_user_id",
                table: "rooms",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_rooms_room_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_rooms_users_user_id",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_rooms_user_id",
                table: "rooms");

            migrationBuilder.AlterColumn<int>(
                name: "room_id",
                table: "messages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.UpdateData(
                table: "room_credentials",
                keyColumn: "room_id",
                keyValue: 1,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "Yd1P1uBMBaie1UfzXOc7U5STpkCH1ZlTbVraHzz8wao=", new byte[] { 168, 56, 167, 119, 234, 43, 127, 156, 135, 63, 41, 12, 252, 24, 114, 225 } });

            migrationBuilder.UpdateData(
                table: "room_credentials",
                keyColumn: "room_id",
                keyValue: 2,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "FBzBQr4AuYLEZrZUheYXbMpxfoDwcLRm8w3s6rviwoY=", new byte[] { 126, 102, 242, 88, 56, 152, 20, 127, 12, 26, 80, 219, 74, 166, 193, 135 } });

            migrationBuilder.UpdateData(
                table: "user_credentials",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "koN6PsQVv1TPUdk9JUxp7vMtTfMaBskBWFHXdfbgkpw=", new byte[] { 138, 37, 136, 66, 236, 1, 5, 239, 142, 195, 47, 41, 62, 140, 2, 138 } });

            migrationBuilder.UpdateData(
                table: "user_credentials",
                keyColumn: "user_id",
                keyValue: 2,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "BQAd2cvEAnZ+Q+mS1/9AAVeL3qGO7LX6K9Q6SMT+SyQ=", new byte[] { 107, 44, 79, 242, 223, 79, 53, 175, 36, 239, 46, 88, 40, 69, 9, 9 } });

            migrationBuilder.UpdateData(
                table: "user_credentials",
                keyColumn: "user_id",
                keyValue: 3,
                columns: new[] { "hashed_password", "salt" },
                values: new object[] { "m2CpuyG7OLyTgYkelSJ0MgiQEp+NthUTmFXXZrCKtO0=", new byte[] { 165, 204, 114, 243, 14, 184, 94, 232, 188, 127, 7, 236, 211, 98, 123, 93 } });

            migrationBuilder.AddForeignKey(
                name: "fk_messages_rooms_room_id",
                table: "messages",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

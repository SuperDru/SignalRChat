using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebChat.Migrations
{
    public partial class ChatMigr1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rooms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "room_credentials",
                columns: table => new
                {
                    room_id = table.Column<int>(nullable: false),
                    salt = table.Column<byte[]>(nullable: true),
                    hashed_password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_credentials", x => x.room_id);
                    table.ForeignKey(
                        name: "fk_room_credentials_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    nickname = table.Column<string>(nullable: true),
                    current_room_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_rooms_current_room_id",
                        column: x => x.current_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    sending_time = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    message_value = table.Column<string>(nullable: true),
                    room_id = table.Column<int>(nullable: true),
                    type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => new { x.sending_time, x.user_id });
                    table.ForeignKey(
                        name: "fk_messages_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_messages_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_credentials",
                columns: table => new
                {
                    user_id = table.Column<int>(nullable: false),
                    salt = table.Column<byte[]>(nullable: true),
                    hashed_password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_credentials", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_credentials_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "rooms",
                columns: new[] { "id", "created_at", "name", "user_id" },
                values: new object[,]
                {
                    { 1, new DateTime(2017, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Developers", 1 },
                    { 2, new DateTime(2016, 5, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Managers", 3 }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "current_room_id", "nickname" },
                values: new object[,]
                {
                    { 1, null, "JFoster" },
                    { 2, null, "AShishkin" },
                    { 3, null, "AShurikov" }
                });

            migrationBuilder.InsertData(
                table: "room_credentials",
                columns: new[] { "room_id", "hashed_password", "salt" },
                values: new object[,]
                {
                    { 1, "Yd1P1uBMBaie1UfzXOc7U5STpkCH1ZlTbVraHzz8wao=", new byte[] { 168, 56, 167, 119, 234, 43, 127, 156, 135, 63, 41, 12, 252, 24, 114, 225 } },
                    { 2, "FBzBQr4AuYLEZrZUheYXbMpxfoDwcLRm8w3s6rviwoY=", new byte[] { 126, 102, 242, 88, 56, 152, 20, 127, 12, 26, 80, 219, 74, 166, 193, 135 } }
                });

            migrationBuilder.InsertData(
                table: "user_credentials",
                columns: new[] { "user_id", "hashed_password", "salt" },
                values: new object[,]
                {
                    { 1, "koN6PsQVv1TPUdk9JUxp7vMtTfMaBskBWFHXdfbgkpw=", new byte[] { 138, 37, 136, 66, 236, 1, 5, 239, 142, 195, 47, 41, 62, 140, 2, 138 } },
                    { 2, "BQAd2cvEAnZ+Q+mS1/9AAVeL3qGO7LX6K9Q6SMT+SyQ=", new byte[] { 107, 44, 79, 242, 223, 79, 53, 175, 36, 239, 46, 88, 40, 69, 9, 9 } },
                    { 3, "m2CpuyG7OLyTgYkelSJ0MgiQEp+NthUTmFXXZrCKtO0=", new byte[] { 165, 204, 114, 243, 14, 184, 94, 232, 188, 127, 7, 236, 211, 98, 123, 93 } }
                });

            migrationBuilder.CreateIndex(
                name: "ix_messages_room_id",
                table: "messages",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_user_id",
                table: "messages",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_name",
                table: "rooms",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_current_room_id",
                table: "users",
                column: "current_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_nickname",
                table: "users",
                column: "nickname",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "room_credentials");

            migrationBuilder.DropTable(
                name: "user_credentials");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "rooms");
        }
    }
}

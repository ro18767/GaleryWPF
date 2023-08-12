using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Galery.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewFullPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PixelWidth = table.Column<long>(type: "bigint", nullable: false),
                    PixelHeight = table.Column<long>(type: "bigint", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageInfo");
        }
    }
}

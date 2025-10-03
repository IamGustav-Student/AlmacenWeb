using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlmacenWeb.Migrations
{
    /// <inheritdoc />
    public partial class addmigrationv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.ProveedorId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoDescripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsApellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsTelefono = table.Column<int>(type: "int", nullable: false),
                    UsDireccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsActivo = table.Column<bool>(type: "bit", nullable: false),
                    UsFechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    EmpNLegajo = table.Column<int>(type: "int", nullable: true),
                    EmpSueldo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmpComisiona = table.Column<bool>(type: "bit", nullable: true),
                    EmpComision = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}

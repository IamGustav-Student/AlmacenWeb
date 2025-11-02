using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlmacenWeb.Migrations
{
    /// <inheritdoc />
    public partial class VentasYDetalles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClDniCuit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClDireccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ClTelefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClId);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    PrId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoBarra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadDisponible = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.PrId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoDescripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsApellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UsPassword = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UsActivo = table.Column<bool>(type: "bit", nullable: false),
                    UsFechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsId);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RoId",
                        column: x => x.RoId,
                        principalTable: "Roles",
                        principalColumn: "RoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    VeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeFecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClId = table.Column<int>(type: "int", nullable: false),
                    UsId = table.Column<int>(type: "int", nullable: false),
                    VeTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.VeId);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_ClId",
                        column: x => x.ClId,
                        principalTable: "Clientes",
                        principalColumn: "ClId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ventas_Usuarios_UsId",
                        column: x => x.UsId,
                        principalTable: "Usuarios",
                        principalColumn: "UsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleVenta",
                columns: table => new
                {
                    DeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeId = table.Column<int>(type: "int", nullable: false),
                    PrId = table.Column<int>(type: "int", nullable: false),
                    DeCantidad = table.Column<int>(type: "int", nullable: false),
                    DePrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeSubtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleVenta", x => x.DeId);
                    table.ForeignKey(
                        name: "FK_DetalleVenta_Productos_PrId",
                        column: x => x.PrId,
                        principalTable: "Productos",
                        principalColumn: "PrId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleVenta_Ventas_VeId",
                        column: x => x.VeId,
                        principalTable: "Ventas",
                        principalColumn: "VeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVenta_PrId",
                table: "DetalleVenta",
                column: "PrId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVenta_VeId",
                table: "DetalleVenta",
                column: "VeId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RoId",
                table: "Usuarios",
                column: "RoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClId",
                table: "Ventas",
                column: "ClId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UsId",
                table: "Ventas",
                column: "UsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleVenta");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}

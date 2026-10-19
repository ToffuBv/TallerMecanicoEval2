using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallerMecanico.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    cliente_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__clientes__47E34D64AFF9958A", x => x.cliente_id);
                });

            migrationBuilder.CreateTable(
                name: "especialidades",
                columns: table => new
                {
                    especialidad_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__especial__A9F2CD7135B200A1", x => x.especialidad_id);
                });

            migrationBuilder.CreateTable(
                name: "marcas",
                columns: table => new
                {
                    marca_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__marcas__BBC4319133CBDFBB", x => x.marca_id);
                });

            migrationBuilder.CreateTable(
                name: "repuestos",
                columns: table => new
                {
                    repuesto_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__repuesto__E0EEC4DC105D5C3A", x => x.repuesto_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    rol_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__roles__CF32E443E948AEF1", x => x.rol_id);
                });

            migrationBuilder.CreateTable(
                name: "servicios",
                columns: table => new
                {
                    servicio_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    costo = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__servicio__AF3A090C70C3AA04", x => x.servicio_id);
                });

            migrationBuilder.CreateTable(
                name: "mecanicos",
                columns: table => new
                {
                    mecanico_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    especialidad_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__mecanico__B88634A1AECCA46F", x => x.mecanico_id);
                    table.ForeignKey(
                        name: "FK__mecanicos__espec__4D94879B",
                        column: x => x.especialidad_id,
                        principalTable: "especialidades",
                        principalColumn: "especialidad_id");
                });

            migrationBuilder.CreateTable(
                name: "modelos",
                columns: table => new
                {
                    modelo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    marca_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__modelos__DBED97613272D5B1", x => x.modelo_id);
                    table.ForeignKey(
                        name: "FK__modelos__marca_i__4316F928",
                        column: x => x.marca_id,
                        principalTable: "marcas",
                        principalColumn: "marca_id");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rol_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__usuarios__2ED7D2AFF8314A19", x => x.usuario_id);
                    table.ForeignKey(
                        name: "FK__usuarios__rol_id__3B75D760",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "rol_id");
                });

            migrationBuilder.CreateTable(
                name: "vehiculos",
                columns: table => new
                {
                    vehiculo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    anio = table.Column<int>(type: "int", nullable: true),
                    color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    cliente_id = table.Column<int>(type: "int", nullable: false),
                    modelo_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vehiculo__1AD380790EBBF3B2", x => x.vehiculo_id);
                    table.ForeignKey(
                        name: "FK__vehiculos__clien__46E78A0C",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "cliente_id");
                    table.ForeignKey(
                        name: "FK__vehiculos__model__47DBAE45",
                        column: x => x.modelo_id,
                        principalTable: "modelos",
                        principalColumn: "modelo_id");
                });

            migrationBuilder.CreateTable(
                name: "ordenes_trabajo",
                columns: table => new
                {
                    orden_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha_ingreso = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cliente_id = table.Column<int>(type: "int", nullable: false),
                    vehiculo_id = table.Column<int>(type: "int", nullable: false),
                    mecanico_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ordenes___F983C4DAC78B094D", x => x.orden_id);
                    table.ForeignKey(
                        name: "FK__ordenes_t__clien__571DF1D5",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "cliente_id");
                    table.ForeignKey(
                        name: "FK__ordenes_t__mecan__59063A47",
                        column: x => x.mecanico_id,
                        principalTable: "mecanicos",
                        principalColumn: "mecanico_id");
                    table.ForeignKey(
                        name: "FK__ordenes_t__vehic__5812160E",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "vehiculo_id");
                });

            migrationBuilder.CreateTable(
                name: "detalle_repuestos",
                columns: table => new
                {
                    detalle_repuesto_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orden_id = table.Column<int>(type: "int", nullable: false),
                    repuesto_id = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__detalle___4D44998C41409EB4", x => x.detalle_repuesto_id);
                    table.ForeignKey(
                        name: "FK__detalle_r__orden__619B8048",
                        column: x => x.orden_id,
                        principalTable: "ordenes_trabajo",
                        principalColumn: "orden_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__detalle_r__repue__628FA481",
                        column: x => x.repuesto_id,
                        principalTable: "repuestos",
                        principalColumn: "repuesto_id");
                });

            migrationBuilder.CreateTable(
                name: "detalle_servicios",
                columns: table => new
                {
                    detalle_servicio_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orden_id = table.Column<int>(type: "int", nullable: false),
                    servicio_id = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__detalle___DAEBCE12F53ACC35", x => x.detalle_servicio_id);
                    table.ForeignKey(
                        name: "FK__detalle_s__orden__5CD6CB2B",
                        column: x => x.orden_id,
                        principalTable: "ordenes_trabajo",
                        principalColumn: "orden_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__detalle_s__servi__5DCAEF64",
                        column: x => x.servicio_id,
                        principalTable: "servicios",
                        principalColumn: "servicio_id");
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    pago_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orden_id = table.Column<int>(type: "int", nullable: false),
                    monto_total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    metodo_pago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    fecha_pago = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Completado")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__pagos__FFF0A58EA830DA4B", x => x.pago_id);
                    table.ForeignKey(
                        name: "FK__pagos__orden_id__68487DD7",
                        column: x => x.orden_id,
                        principalTable: "ordenes_trabajo",
                        principalColumn: "orden_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_detalle_repuestos_orden_id",
                table: "detalle_repuestos",
                column: "orden_id");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_repuestos_repuesto_id",
                table: "detalle_repuestos",
                column: "repuesto_id");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_servicios_orden_id",
                table: "detalle_servicios",
                column: "orden_id");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_servicios_servicio_id",
                table: "detalle_servicios",
                column: "servicio_id");

            migrationBuilder.CreateIndex(
                name: "UQ__especial__72AFBCC6CDB9ACDA",
                table: "especialidades",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__marcas__72AFBCC60CBA77B8",
                table: "marcas",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mecanicos_especialidad_id",
                table: "mecanicos",
                column: "especialidad_id");

            migrationBuilder.CreateIndex(
                name: "IX_modelos_marca_id",
                table: "modelos",
                column: "marca_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_trabajo_cliente_id",
                table: "ordenes_trabajo",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_trabajo_mecanico_id",
                table: "ordenes_trabajo",
                column: "mecanico_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_trabajo_vehiculo_id",
                table: "ordenes_trabajo",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "UQ__pagos__F983C4DBC00D825F",
                table: "pagos",
                column: "orden_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__roles__72AFBCC6B80A53A5",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_rol_id",
                table: "usuarios",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "UQ__usuarios__2A586E0B922D9B88",
                table: "usuarios",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_cliente_id",
                table: "vehiculos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_modelo_id",
                table: "vehiculos",
                column: "modelo_id");

            migrationBuilder.CreateIndex(
                name: "UQ__vehiculo__40228D081BD1E9D3",
                table: "vehiculos",
                column: "patente",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalle_repuestos");

            migrationBuilder.DropTable(
                name: "detalle_servicios");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "repuestos");

            migrationBuilder.DropTable(
                name: "servicios");

            migrationBuilder.DropTable(
                name: "ordenes_trabajo");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "mecanicos");

            migrationBuilder.DropTable(
                name: "vehiculos");

            migrationBuilder.DropTable(
                name: "especialidades");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "modelos");

            migrationBuilder.DropTable(
                name: "marcas");
        }
    }
}

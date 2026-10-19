using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TallerMecanico.Models;

public partial class TallerMecanicoDbContext : DbContext
{
    public TallerMecanicoDbContext()
    {
    }

    public TallerMecanicoDbContext(DbContextOptions<TallerMecanicoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetalleRepuesto> DetalleRepuestos { get; set; }

    public virtual DbSet<DetalleServicio> DetalleServicios { get; set; }

    public virtual DbSet<Especialidade> Especialidades { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Mecanico> Mecanicos { get; set; }

    public virtual DbSet<Modelo> Modelos { get; set; }

    public virtual DbSet<OrdenesTrabajo> OrdenesTrabajos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Repuesto> Repuestos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__clientes__47E34D64AFF9958A");

            entity.ToTable("clientes");

            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<DetalleRepuesto>(entity =>
        {
            entity.HasKey(e => e.DetalleRepuestoId).HasName("PK__detalle___4D44998C41409EB4");

            entity.ToTable("detalle_repuestos");

            entity.Property(e => e.DetalleRepuestoId).HasColumnName("detalle_repuesto_id");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue(1)
                .HasColumnName("cantidad");
            entity.Property(e => e.OrdenId).HasColumnName("orden_id");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.RepuestoId).HasColumnName("repuesto_id");

            entity.HasOne(d => d.Orden).WithMany(p => p.DetalleRepuestos)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK__detalle_r__orden__619B8048");

            entity.HasOne(d => d.Repuesto).WithMany(p => p.DetalleRepuestos)
                .HasForeignKey(d => d.RepuestoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__detalle_r__repue__628FA481");
        });

        modelBuilder.Entity<DetalleServicio>(entity =>
        {
            entity.HasKey(e => e.DetalleServicioId).HasName("PK__detalle___DAEBCE12F53ACC35");

            entity.ToTable("detalle_servicios");

            entity.Property(e => e.DetalleServicioId).HasColumnName("detalle_servicio_id");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue(1)
                .HasColumnName("cantidad");
            entity.Property(e => e.OrdenId).HasColumnName("orden_id");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.ServicioId).HasColumnName("servicio_id");

            entity.HasOne(d => d.Orden).WithMany(p => p.DetalleServicios)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK__detalle_s__orden__5CD6CB2B");

            entity.HasOne(d => d.Servicio).WithMany(p => p.DetalleServicios)
                .HasForeignKey(d => d.ServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__detalle_s__servi__5DCAEF64");
        });

        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.HasKey(e => e.EspecialidadId).HasName("PK__especial__A9F2CD7135B200A1");

            entity.ToTable("especialidades");

            entity.HasIndex(e => e.Nombre, "UQ__especial__72AFBCC6CDB9ACDA").IsUnique();

            entity.Property(e => e.EspecialidadId).HasColumnName("especialidad_id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.MarcaId).HasName("PK__marcas__BBC4319133CBDFBB");

            entity.ToTable("marcas");

            entity.HasIndex(e => e.Nombre, "UQ__marcas__72AFBCC60CBA77B8").IsUnique();

            entity.Property(e => e.MarcaId).HasColumnName("marca_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Mecanico>(entity =>
        {
            entity.HasKey(e => e.MecanicoId).HasName("PK__mecanico__B88634A1AECCA46F");

            entity.ToTable("mecanicos");

            entity.Property(e => e.MecanicoId).HasColumnName("mecanico_id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.EspecialidadId).HasColumnName("especialidad_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");

            entity.HasOne(d => d.Especialidad).WithMany(p => p.Mecanicos)
                .HasForeignKey(d => d.EspecialidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__mecanicos__espec__4D94879B");
        });

        modelBuilder.Entity<Modelo>(entity =>
        {
            entity.HasKey(e => e.ModeloId).HasName("PK__modelos__DBED97613272D5B1");

            entity.ToTable("modelos");

            entity.Property(e => e.ModeloId).HasColumnName("modelo_id");
            entity.Property(e => e.MarcaId).HasColumnName("marca_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.Marca).WithMany(p => p.Modelos)
                .HasForeignKey(d => d.MarcaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__modelos__marca_i__4316F928");
        });

        modelBuilder.Entity<OrdenesTrabajo>(entity =>
        {
            entity.HasKey(e => e.OrdenId).HasName("PK__ordenes___F983C4DAC78B094D");

            entity.ToTable("ordenes_trabajo");

            entity.Property(e => e.OrdenId).HasColumnName("orden_id");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasDefaultValue("Pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_ingreso");
            entity.Property(e => e.MecanicoId).HasColumnName("mecanico_id");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.VehiculoId).HasColumnName("vehiculo_id");

            entity.HasOne(d => d.Cliente).WithMany(p => p.OrdenesTrabajos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ordenes_t__clien__571DF1D5");

            entity.HasOne(d => d.Mecanico).WithMany(p => p.OrdenesTrabajos)
                .HasForeignKey(d => d.MecanicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ordenes_t__mecan__59063A47");

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.OrdenesTrabajos)
                .HasForeignKey(d => d.VehiculoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ordenes_t__vehic__5812160E");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.PagoId).HasName("PK__pagos__FFF0A58EA830DA4B");

            entity.ToTable("pagos");

            entity.HasIndex(e => e.OrdenId, "UQ__pagos__F983C4DBC00D825F").IsUnique();

            entity.Property(e => e.PagoId).HasColumnName("pago_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasDefaultValue("Completado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .HasColumnName("metodo_pago");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_total");
            entity.Property(e => e.OrdenId).HasColumnName("orden_id");

            entity.HasOne(d => d.Orden).WithOne(p => p.Pago)
                .HasForeignKey<Pago>(d => d.OrdenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pagos__orden_id__68487DD7");
        });

        modelBuilder.Entity<Repuesto>(entity =>
        {
            entity.HasKey(e => e.RepuestoId).HasName("PK__repuesto__E0EEC4DC105D5C3A");

            entity.ToTable("repuestos");

            entity.Property(e => e.RepuestoId).HasColumnName("repuesto_id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__roles__CF32E443E948AEF1");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Nombre, "UQ__roles__72AFBCC6B80A53A5").IsUnique();

            entity.Property(e => e.RolId).HasColumnName("rol_id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.ServicioId).HasName("PK__servicio__AF3A090C70C3AA04");

            entity.ToTable("servicios");

            entity.Property(e => e.ServicioId).HasColumnName("servicio_id");
            entity.Property(e => e.Costo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("costo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__usuarios__2ED7D2AFF8314A19");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Correo, "UQ__usuarios__2A586E0B922D9B88").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.RolId).HasColumnName("rol_id");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__usuarios__rol_id__3B75D760");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.VehiculoId).HasName("PK__vehiculo__1AD380790EBBF3B2");

            entity.ToTable("vehiculos");

            entity.HasIndex(e => e.Patente, "UQ__vehiculo__40228D081BD1E9D3").IsUnique();

            entity.Property(e => e.VehiculoId).HasColumnName("vehiculo_id");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.ModeloId).HasColumnName("modelo_id");
            entity.Property(e => e.Patente)
                .HasMaxLength(20)
                .HasColumnName("patente");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vehiculos__clien__46E78A0C");

            entity.HasOne(d => d.Modelo).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.ModeloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vehiculos__model__47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

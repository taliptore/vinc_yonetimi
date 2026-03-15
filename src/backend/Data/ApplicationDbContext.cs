using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentTenant? _currentTenant;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenant? currentTenant = null)
        : base(options)
    {
        _currentTenant = currentTenant;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Firm> Firms => Set<Firm>();
    public DbSet<Crane> Cranes => Set<Crane>();
    public DbSet<Operator> Operators => Set<Operator>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<SiteManager> SiteManagers => Set<SiteManager>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<OperatorDailyWork> OperatorDailyWorks => Set<OperatorDailyWork>();
    public DbSet<Hakedis> HakedisList => Set<Hakedis>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Advance> Advances => Set<Advance>();
    public DbSet<FuelLog> FuelLogs => Set<FuelLog>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<BreakdownReport> BreakdownReports => Set<BreakdownReport>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<JobPhoto> JobPhotos => Set<JobPhoto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Soft delete global filter
        modelBuilder.Entity<Firm>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Crane>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Operator>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Site>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Job>().HasQueryFilter(e => !e.IsDeleted);

        modelBuilder.Entity<Tenant>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200);
            e.HasOne(x => x.OwnerAdmin).WithMany().HasForeignKey(x => x.OwnerAdminId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Role>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.PasswordHash).HasMaxLength(500);
            e.Property(x => x.PasswordResetToken).HasMaxLength(200);
            e.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Role).WithMany(r => r.Users).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).HasMaxLength(20);
            e.Property(x => x.EntityType).HasMaxLength(100);
            e.Property(x => x.EntityId).HasMaxLength(50);
        });

        modelBuilder.Entity<Firm>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Crane>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Plate).HasMaxLength(20);
            e.Property(x => x.Status).HasMaxLength(20);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Operator>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Crane).WithMany().HasForeignKey(x => x.CraneId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<Site>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Firm).WithMany().HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<SiteManager>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Site).WithMany().HasForeignKey(x => x.SiteId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Job>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Firm).WithMany().HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Site).WithMany().HasForeignKey(x => x.SiteId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Crane).WithMany().HasForeignKey(x => x.CraneId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Operator).WithMany().HasForeignKey(x => x.OperatorId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<OperatorDailyWork>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Operator).WithMany().HasForeignKey(x => x.OperatorId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Hakedis>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Hakedis).WithMany().HasForeignKey(x => x.HakedisId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<Advance>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Operator).WithMany().HasForeignKey(x => x.OperatorId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<FuelLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Crane).WithMany().HasForeignKey(x => x.CraneId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<MaintenanceRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Crane).WithMany().HasForeignKey(x => x.CraneId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<BreakdownReport>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Crane).WithMany().HasForeignKey(x => x.CraneId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Operator).WithMany().HasForeignKey(x => x.OperatorId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<Notification>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Contract>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Firm).WithMany().HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Hakedis).WithMany().HasForeignKey(x => x.HakedisId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Firm).WithMany().HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<JobPhoto>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId).OnDelete(DeleteBehavior.Cascade);
        });

        // Seed roller: Admin=1, Muhasebe=2, Operatör=3, Firma=4
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Muhasebe" },
            new Role { Id = 3, Name = "Operatör" },
            new Role { Id = 4, Name = "Firma" });
    }
}

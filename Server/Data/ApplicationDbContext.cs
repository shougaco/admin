using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using Application.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Application.Shared.Models.Org;
using Application.Shared.Models.ProductManagement;
using Application.Shared.Models.Procurement;

namespace Application.Server.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("ApplicationUser");
            });
            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaim");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogin");
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("Token");
            });

            builder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Role");
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaim");
            });

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRole");
            });
    }

    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<Country> Country { get; set; }
    public DbSet<Company> Company { get; set; }
    public DbSet<Member> Member { get; set; }
    public DbSet<Store> Store { get; set; }
    public DbSet<TransLog> TransLog { get; set; }
    public DbSet<Barcode> Barcode { get; set; }
    public DbSet<Collection> Collection { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductCollection> ProductCollection { get; set; }
    public DbSet<ProductTag> ProductTag { get; set; }
    public DbSet<ProductVariant> ProductVariant { get; set; }
    public DbSet<VariantOption> VariantOption { get; set; }
    public DbSet<Supplier> Supplier { get; set; }
    public DbSet<SupplierUser> SupplierUser { get; set; }
    public DbSet<ApiKey> ApiKey { get; set; }
    
}

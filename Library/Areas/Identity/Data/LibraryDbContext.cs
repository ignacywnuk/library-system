using Library.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.Models;

namespace Library.Areas.Identity.Data;

public class LibraryDbContext : IdentityDbContext<LibraryUser>
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<LibraryUser>().Ignore(c => c.AccessFailedCount)
                                           .Ignore(c => c.Email)
                                           .Ignore(c => c.EmailConfirmed)
                                           .Ignore(c => c.LockoutEnabled)
                                           .Ignore(c => c.LockoutEnd)
                                           .Ignore(c => c.NormalizedEmail)//and so on...
                                           .Ignore(c => c.PhoneNumber)
                                           .Ignore(c => c.PhoneNumberConfirmed)
                                           .Ignore(c => c.TwoFactorEnabled);//and so on...

        builder.ApplyConfiguration(new LibraryUserEntityConfiguration());
    }

    public class LibraryUserEntityConfiguration : IEntityTypeConfiguration<LibraryUser>
    {
        public void Configure(EntityTypeBuilder<LibraryUser> builder)
        {
        }
    }


}

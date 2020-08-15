using Microsoft.EntityFrameworkCore;
using PhoneBook.DataAccess.Models;

namespace PhoneBook.DataAccess
{
    public class PhoneBookContext : DbContext
    {
        public PhoneBookContext(DbContextOptions<PhoneBookContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Contact>(b =>
            {
                b.Property(c => c.Name).IsRequired().HasMaxLength(25);
                b.Property(c => c.LastName).IsRequired().HasMaxLength(25);
                b.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(15);
            });
        }
    }
}

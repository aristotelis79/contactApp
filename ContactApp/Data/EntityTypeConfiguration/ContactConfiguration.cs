using ContactApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactApp.Data.EntityTypeConfiguration
{
    /// <summary>
    /// Contact Database entity mapping configuration
    /// </summary>
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder
                .ToTable(nameof(Contact))
                .HasKey(k => k.Id);

            builder
                .Property(p => p.FullName).IsRequired();
            
            builder
                .Property(p => p.Email).IsRequired();
            builder
                .HasIndex(i => i.Email).IsUnique();


            builder
                .HasMany(x => x.Phones)
                .WithOne(x=>x.Contact)
                .HasForeignKey(f=>f.ContactId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
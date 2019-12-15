using ContactApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactApp.Data.EntityTypeConfiguration
{
    /// <summary>
    /// Phone Database entity mapping configuration
    /// </summary>
    public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
    {
        public void Configure(EntityTypeBuilder<Phone> builder)
        {
            builder
                .ToTable(nameof(Phone))
                .HasKey(x => x.Id);

            builder
                .Property(p => p.PhoneNumber)
                .HasMaxLength(14);

            builder
                .HasIndex(x => x.PhoneNumber)
                .IsUnique();
        }
    }
}
using ContactApp.Data.Entities;
using ContactApp.Data.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Data
{
    /// <summary>
    /// Represents base object context
    /// </summary>
    public class ContactDbContext: DbContext, IDbContext
    {
        #region Fields

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Phone> Phones { get; set; }

        public readonly string _connectionString;

        #endregion

        #region ctor

        public ContactDbContext(string connectionString) : base()
        {
            _connectionString = connectionString;
        }

        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
        {
        }

        #endregion


        #region Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_connectionString);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new ContactConfiguration());
            modelBuilder.ApplyConfiguration(new PhoneConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<TEntity> Set<TEntity,T>() where TEntity : BaseEntity<T> where T : struct
        {
            return base.Set<TEntity>();
        }

        #endregion
    }
}
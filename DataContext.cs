using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Web_StoreAPI.DataModels
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){   }

        public DbSet<Category> Categories { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<VendorCompany> VendorCompanys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
    }
}

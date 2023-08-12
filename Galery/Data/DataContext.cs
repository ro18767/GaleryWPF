using Microsoft.EntityFrameworkCore;
using Galery.Data.Entities;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Windows.Shapes;
using System;

namespace Galery.Data
{
    internal class DataContext : DbContext
    {
        public DbSet<ImageInfo> ImageInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Directory.GetCurrentDirectory();

            string connectionString = @$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}\Database1.mdf;Integrated Security=True";

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}

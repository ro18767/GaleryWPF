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

            SqlConnection sqlConnection = GetSqlConnection();
            connection.Open();
            using SqlCommand cmd = new("SELECT Id, FullPath, PreviewFullPath, PixelWidth, PixelHeight, FileSize, CreationTime FROM ImageInfos", connection);
            {

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}

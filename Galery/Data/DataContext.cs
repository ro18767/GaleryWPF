using Microsoft.EntityFrameworkCore;
using Galery.Data.Entities;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Windows;
using System.Diagnostics;

namespace Galery.Data
{
    internal class DataContext
    {
        public List<ImageInfo> ImageInfos { get; set; } = new List<ImageInfo>();


        public DataContext()
        {
            InitDB();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using SqlCommand cmd = new("SELECT Id, FullPath, PreviewFullPath, PixelWidth, PixelHeight, FileSize, CreationTime FROM ImageInfos", connection);
                {

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ImageInfos.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            FullPath = reader.GetString(1),
                            PreviewFullPath = reader.GetString(2),
                            PixelWidth = reader.GetInt64(3),
                            PixelHeight = reader.GetInt64(4),
                            FileSize = reader.GetInt64(5),
                            CreationTime = reader.GetDateTime(6),
                        });
                    }
                    reader.Close();
                }
            }
        }
        public void Add(ImageInfo imageInfo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = $"INSERT INTO ImageInfos (Id, FullPath, PreviewFullPath, PixelWidth, PixelHeight, FileSize, CreationTime) VALUES (@id, @fullPath, @previewFullPath, @pixelWidth, @pixelHeight, @fileSize, @creationTime)";
                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", imageInfo.Id);
                cmd.Parameters.AddWithValue("@fullPath", imageInfo.FullPath);
                cmd.Parameters.AddWithValue("@previewFullPath", imageInfo.PreviewFullPath);
                cmd.Parameters.AddWithValue("@pixelWidth", imageInfo.PixelWidth);
                cmd.Parameters.AddWithValue("@pixelHeight", imageInfo.PixelHeight);
                cmd.Parameters.AddWithValue("@fileSize", imageInfo.FileSize);
                cmd.Parameters.AddWithValue("@creationTime", imageInfo.CreationTime);
                cmd.ExecuteNonQuery();

                ImageInfos.Add(imageInfo);
            }
        }

        public void Remove(ImageInfo imageInfo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = $"DELETE FROM ImageInfos WHERE Id = @id";
                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", imageInfo.Id);
                cmd.ExecuteNonQuery();

                ImageInfos.Remove(imageInfo);
            }
        }

        private string pwd { get => Directory.GetCurrentDirectory(); }
        private string mdfFilePath { get => $@"{pwd}\MyGalery.mdf"; }
        private string logFilePath { get => $@"{pwd}\MyGalery_log.ldf"; }
        private string connectionStringStart { get => @"Server=(LocalDB)\MSSQLLocalDB;Integrated Security=true;"; }
        private string connectionString { get => $"{connectionStringStart}AttachDbFilename={mdfFilePath};"; }
        private string databaseName { get => "ro18767_Galery"; }

        private void InitDB()
        {
            if (!File.Exists(mdfFilePath))
            {
                string checkAndCreateDatabaseQuery = $@"
                IF DB_ID('{databaseName}') IS NOT NULL
                BEGIN
                    PRINT 'Database already exists.'
                END
                ELSE
                BEGIN
                    CREATE DATABASE {databaseName} 
                    ON PRIMARY (NAME = {databaseName}, 
                    FILENAME = '{mdfFilePath}', 
                    SIZE = 10MB, 
                    MAXSIZE = 100MB, 
                    FILEGROWTH = 10%) 
                    LOG ON (NAME = {databaseName}_log, 
                    FILENAME = '{logFilePath}', 
                    SIZE = 5MB, 
                    MAXSIZE = 25MB, 
                    FILEGROWTH = 5%)
                    PRINT 'Database created successfully.'
                END";

                using (SqlConnection connection = new SqlConnection(connectionStringStart))
                {
                    SqlCommand command = new SqlCommand(checkAndCreateDatabaseQuery, connection);
                    try
                    {
                        connection.Open();
                        command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Database created Exception: {ex.Message}");
                    }
                }
            }
            if (!File.Exists(mdfFilePath))
            {
                throw new Exception("test");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ImageInfo' and xtype='U')
                CREATE TABLE ImageInfos (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    FullPath NVARCHAR(MAX) NOT NULL,
                    PreviewFullPath NVARCHAR(MAX) NOT NULL,
                    PixelWidth BIGINT NOT NULL,
                    PixelHeight BIGINT NOT NULL,
                    FileSize BIGINT NOT NULL,
                    CreationTime DATETIME NOT NULL
                )";
                SqlCommand command = new SqlCommand(createTableQuery, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Table creation exception: {ex.Message}");
                }
            }



        }

    }
}

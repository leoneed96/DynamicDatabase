using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class DataContext : DbContext
    {
        private readonly string _connection_string;
        public DataContext(string connectionString)
        {
            _connection_string = connectionString;

            Database.EnsureCreated();
        }
        public DbSet<DemoObject> DemoObjects { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connection_string);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DemoObject>(a =>
            {
                a.ToTable(nameof(DemoObject), "Data").HasKey(x => x.Number);
                a.HasData(InitialData);
            });

            base.OnModelCreating(modelBuilder);
        }

        public List<DemoObject> InitialData = Enumerable.Range(0, 1000).Select((x, i) => new DemoObject
        {
            Number = i + 1,
            Title = $"DemoObject {i + 1}"
        }).ToList();

    }

    public class DemoObject
    {
        public int Number { get; set; }
        public string Title { get; set; }
    }
}

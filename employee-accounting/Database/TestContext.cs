using System;
using System.Collections.Generic;
using employee_accounting.Objects;
using Microsoft.EntityFrameworkCore;

namespace employee_accounting.Database;

public partial class TestContext : DbContext
{
    public TestContext()
    {
        this.Database.EnsureCreated();
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Employee>()
    //        .HasIndex(e => new { e.Sex, e.Name })  // Создание составного индекса
    //        .HasDatabaseName("idx_employees_gender_name");  // Указание имени индекса
    //}

    public virtual DbSet<Employee> Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=test.db");
}

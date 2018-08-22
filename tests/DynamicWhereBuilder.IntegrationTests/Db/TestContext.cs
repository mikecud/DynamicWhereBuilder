using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DynamicWhereBuilder.IntegrationTests.Db
{
    public class TestContext : DbContext
    {
        public DbSet<IQueryableTestClass> TestClasses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=../../../test.db");
        }
    }

    public class IQueryableTestClass
    {
        [Key]
        public int IQueryableTestClassId { get; set; }

        public int Id { get; set; }

        public string Value { get; set; }
    }   
}

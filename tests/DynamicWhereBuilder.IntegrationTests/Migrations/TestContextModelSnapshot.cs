﻿// <auto-generated />
using DynamicWhereBuilder.IntegrationTests.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DynamicWhereBuilder.IntegrationTests.Migrations
{
    [DbContext(typeof(TestContext))]
    partial class TestContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("DynamicWhereBuilder.IntegrationTests.Db.IQueryableTestClass", b =>
                {
                    b.Property<int>("IQueryableTestClassId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Id");

                    b.Property<string>("Value");

                    b.HasKey("IQueryableTestClassId");

                    b.ToTable("TestClasses");
                });
#pragma warning restore 612, 618
        }
    }
}
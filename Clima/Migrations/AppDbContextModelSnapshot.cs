﻿// <auto-generated />
using System;
using Clima.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Clima.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Clima.Models.Tb_registos", b =>
                {
                    b.Property<int>("ID_registo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID_registo"));

                    b.Property<DateTime>("data_registo")
                        .HasColumnType("datetime2");

                    b.Property<int>("humidade")
                        .HasColumnType("int");

                    b.Property<int>("risco_humidade")
                        .HasColumnType("int");

                    b.Property<int>("risco_temperatura")
                        .HasColumnType("int");

                    b.Property<int>("temperatura")
                        .HasColumnType("int");

                    b.HasKey("ID_registo");

                    b.ToTable("Tb_Registos");
                });
#pragma warning restore 612, 618
        }
    }
}

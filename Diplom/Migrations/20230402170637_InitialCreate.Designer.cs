﻿// <auto-generated />
using Diplom.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Diplom.Migrations
{
    [DbContext(typeof(DiplomContext))]
    [Migration("20230402170637_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Diplom.Models.Device", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Description")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("DocumentPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("Diplom.Models.DevicePlacement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceID")
                        .HasColumnType("int");

                    b.Property<int>("PlacementID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeviceID");

                    b.HasIndex("PlacementID");

                    b.ToTable("DevicePlacement");
                });

            modelBuilder.Entity("Diplom.Models.Placement", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Placement");
                });

            modelBuilder.Entity("Diplom.Models.DevicePlacement", b =>
                {
                    b.HasOne("Diplom.Models.Device", "Device")
                        .WithMany("DevicePlacements")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Diplom.Models.Placement", "Placement")
                        .WithMany("DevicePlacements")
                        .HasForeignKey("PlacementID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");

                    b.Navigation("Placement");
                });

            modelBuilder.Entity("Diplom.Models.Device", b =>
                {
                    b.Navigation("DevicePlacements");
                });

            modelBuilder.Entity("Diplom.Models.Placement", b =>
                {
                    b.Navigation("DevicePlacements");
                });
#pragma warning restore 612, 618
        }
    }
}

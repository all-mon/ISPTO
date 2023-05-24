using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Diplom.Models;

namespace Diplom.Data
{
    public class DiplomContext : DbContext
    {
        public DiplomContext (DbContextOptions<DiplomContext> options)
            : base(options)
        {
        }

        public DbSet<Device> Device { get; set; }
        public DbSet<AnalogDevice> AnalogDevice { get; set; }
        public DbSet<Placement> Placement { get; set; } 
        public DbSet<DevicePlacement> DevicePlacement { get; set; }


        public DbSet<Goal> Task { get; set; }
        public DbSet<Instruction> Instruction { get; set; }
        public DbSet<LogEntry> LogEntry { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<DevicePlacement>()
                .HasKey(c => new { c.DeviceID, c.PlacementID });

            modelBuilder.Entity<AnalogDevice>()
                .HasKey(ad => new { ad.DeviceId, ad.AnalogId });

            modelBuilder.Entity<AnalogDevice>()
            .HasOne(ad => ad.Device)
            .WithMany(d => d.AnalogDevice)
            .HasForeignKey(ad => ad.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AnalogDevice>()
                .HasOne(ad => ad.Analog)
                .WithMany()
                .HasForeignKey(ad => ad.AnalogId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Device>()
                .HasMany(d => d.AnalogDevice)
                .WithOne(a => a.Device)
                .HasForeignKey(a => a.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

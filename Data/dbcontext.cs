using Microsoft.EntityFrameworkCore;
using RoofSafety.Models;
using System.Collections.Generic;
using Version = RoofSafety.Models.Version;

namespace RoofSafety.Data
{
    public class dbcontext : DbContext
    {
        public dbcontext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Client> Client { get; set; }
        public DbSet<Building> Building { get; set; }
        public DbSet<Inspection> Inspection { get; set; }
        public DbSet<Version> Version { get; set; }
        public DbSet<EquipType> EquipType { get; set; }
        public DbSet<InspEquip> InspEquip { get; set; }
        public DbSet<InspEquipTypeTest> InspEquipTypeTest { get; set; }
        public DbSet<InspEquipTypeTestFail> InspEquipTypeTestFail { get; set; }
        public DbSet<EquipTypeTest> EquipTypeTest { get; set; }
        public DbSet<EquipTypeTestFail> EquipTypeTestFail { get; set; }
        public DbSet<EquipTypeTestHazards> EquipTypeTestHazards { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Hazard> Hazard { get; set; }
        public DbSet<InspPhoto> InspPhoto { get; set; }
    }
}

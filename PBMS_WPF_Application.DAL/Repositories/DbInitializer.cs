using Microsoft.EntityFrameworkCore;
using PBMS_WPF_Application.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PBMS_WPF_Application.DAL.Repositories;

public static class DbInitializer
{
    public static void Initialize(PbmsDbContext context)
    {
        // 1. Ensure database is created
        context.Database.EnsureCreated();

        // 2. Seed Roles
        if (!context.Roles.Any())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Roles ON");
                    context.Roles.Add(new Role { RoleId = 1, RoleName = "Registered_Driver", IsDeleted = false });
                    context.Roles.Add(new Role { RoleId = 2, RoleName = "Staff", IsDeleted = false });
                    context.Roles.Add(new Role { RoleId = 3, RoleName = "Manager", IsDeleted = false });
                    context.Roles.Add(new Role { RoleId = 4, RoleName = "Admin", IsDeleted = false });
                    context.SaveChanges();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Roles OFF");
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    // Fallback to inserting without explicit RoleId
                    var r1 = new Role { RoleName = "Registered_Driver", IsDeleted = false };
                    var r2 = new Role { RoleName = "Staff", IsDeleted = false };
                    var r3 = new Role { RoleName = "Manager", IsDeleted = false };
                    var r4 = new Role { RoleName = "Admin", IsDeleted = false };
                    context.Roles.AddRange(r1, r2, r3, r4);
                    context.SaveChanges();
                }
            }
        }

        // 3. Seed VehiclesTypes
        if (!context.VehiclesTypes.Any())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT VehiclesType ON");
                    context.VehiclesTypes.Add(new VehiclesType { TypeId = 1, TypeName = "Motorbike", Price = 5000, IsDeleted = false });
                    context.VehiclesTypes.Add(new VehiclesType { TypeId = 2, TypeName = "Car", Price = 20000, IsDeleted = false });
                    context.SaveChanges();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT VehiclesType OFF");
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    var t1 = new VehiclesType { TypeName = "Motorbike", Price = 5000, IsDeleted = false };
                    var t2 = new VehiclesType { TypeName = "Car", Price = 20000, IsDeleted = false };
                    context.VehiclesTypes.AddRange(t1, t2);
                    context.SaveChanges();
                }
            }
        }

        // 4. Seed Floors
        if (!context.Floors.Any())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Floors ON");
                    context.Floors.Add(new Floor { FloorId = 1, FloorName = "Basement 1", Capacity = 10, IsDeleted = false });
                    context.Floors.Add(new Floor { FloorId = 2, FloorName = "Floor 1", Capacity = 50, IsDeleted = false });
                    context.SaveChanges();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Floors OFF");
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    var f1 = new Floor { FloorName = "Basement 1", Capacity = 10, IsDeleted = false };
                    var f2 = new Floor { FloorName = "Floor 1", Capacity = 50, IsDeleted = false };
                    context.Floors.AddRange(f1, f2);
                    context.SaveChanges();
                }
            }
        }

        // 5. Seed ParkingSlots
        if (!context.ParkingSlots.Any())
        {
            var floor1 = context.Floors.FirstOrDefault(f => f.FloorName.Contains("Basement") || f.FloorId == 1) ?? context.Floors.First();
            var floor2 = context.Floors.FirstOrDefault(f => f.FloorName.Contains("Floor") || f.FloorId == 2) ?? context.Floors.Skip(1).FirstOrDefault() ?? context.Floors.First();

            var motorbikeType = context.VehiclesTypes.FirstOrDefault(t => t.TypeName.Contains("Motor") || t.TypeId == 1) ?? context.VehiclesTypes.First();
            var carType = context.VehiclesTypes.FirstOrDefault(t => t.TypeName.Contains("Car") || t.TypeId == 2) ?? context.VehiclesTypes.Skip(1).FirstOrDefault() ?? context.VehiclesTypes.First();

            var statuses = new[] { "available", "reserved", "occupied" };
            var rand = new Random(42); // Seeded random for deterministic statuses on seeding

            // Basement 1: 10 car slots
            for (int i = 1; i <= 10; i++)
            {
                string status = statuses[i % 3]; // Deterministic status mix
                context.ParkingSlots.Add(new ParkingSlot
                {
                    SlotName = $"B1-{i:D2}",
                    FloorId = floor1.FloorId,
                    TypeId = carType.TypeId,
                    SlotStatus = status,
                    IsDeleted = false
                });
            }

            // Floor 1: 50 motorbike slots
            for (int i = 1; i <= 50; i++)
            {
                string status = statuses[i % 3]; // Deterministic status mix
                context.ParkingSlots.Add(new ParkingSlot
                {
                    SlotName = $"F1-{i:D2}",
                    FloorId = floor2.FloorId,
                    TypeId = motorbikeType.TypeId,
                    SlotStatus = status,
                    IsDeleted = false
                });
            }

            context.SaveChanges();
        }
    }
}

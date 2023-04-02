using Diplom.Data;
using Diplom.Models;
using Microsoft.EntityFrameworkCore;

namespace Diplom.SeedData
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new DiplomContext(serviceProvider.GetRequiredService<DbContextOptions<DiplomContext>>()))
            {

                if (context.Device.Any())
                {
                    return;   // DB has been seeded
                }

                var devices = new Device[]
                {
                    new Device {Name="Датчик температуры УЕ-100", Description="Измеряет температуру"},
                    new Device {Name="Датчик влажности ВВ-11", Description="Измеряет влажность в помещении"},
                    new Device {Name="Пульт управления ТТ-55", Description="Пульт управления фанкойлами"},
                    new Device {Name="Датчик температуры Т-700", Description="Измеряет температуру"},
                };

                foreach (Device d in devices)
                {
                    context.Device.Add(d);
                }
                context.SaveChanges();

                var placements = new Placement[]
                {
                    new Placement {Name="П-1", Description="Вент система"},
                    new Placement {Name="П-2", Description="Вент система"},
                    new Placement {Name="ЦТП-1", Description="Тепловой пункт"},
                    new Placement {Name="ЦТП-2", Description="Тепловой пункт"},
                };

                foreach (Placement p in placements)
                {
                    context.Placement.Add(p);
                }
                context.SaveChanges();

                var deviceIDs = from d in context.Device select d.ID;

                var placeIDs = from p in context.Placement select p.ID;

                var devicePlacements = new DevicePlacement[]
                {
                    new DevicePlacement {DeviceID=deviceIDs.First(), PlacementID = placeIDs.First()},
                    new DevicePlacement {DeviceID=deviceIDs.First(), PlacementID = placeIDs.Skip(1).First()},
                    new DevicePlacement {DeviceID=deviceIDs.First(), PlacementID = placeIDs.Skip(2).First()},
                    new DevicePlacement {DeviceID=deviceIDs.First(), PlacementID = placeIDs.Skip(3).First()},

                    new DevicePlacement {DeviceID=deviceIDs.Skip(1).First(), PlacementID = placeIDs.Skip(2).First()},
                    new DevicePlacement {DeviceID=deviceIDs.Skip(1).First(), PlacementID = placeIDs.Skip(3).First()},
                    new DevicePlacement {DeviceID=deviceIDs.Skip(1).First(), PlacementID = placeIDs.First()},

                    new DevicePlacement {DeviceID=deviceIDs.Skip(2).First(), PlacementID = placeIDs.First()},
                    new DevicePlacement {DeviceID=deviceIDs.Skip(2).First(), PlacementID = placeIDs.Skip(3).First()},

                    new DevicePlacement {DeviceID=deviceIDs.Skip(3).First(), PlacementID = placeIDs.Skip(2).First()},
                    new DevicePlacement {DeviceID=deviceIDs.Skip(3).First(), PlacementID = placeIDs.Skip(3).First()},

                };

                foreach (DevicePlacement dp in devicePlacements)
                {
                    context.DevicePlacement.Add(dp);
                }
                context.SaveChanges();
            }
        }
    }
}

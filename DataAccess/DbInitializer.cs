using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _db;

        public DbInitializer(AppDbContext db)
        {
            _db = db;
        }

        public void Initialize()
        {
            _db.Database.EnsureCreated();

            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            if (_db.Availabilities.Any())
            {
                return; //DB has been seeded
            }

            if (_db.Bookings.Any())
            {
                return; //DB has been seeded
            }

            if (_db.Locations.Any())
            {
                return; //DB has been seeded
            }

            if (_db.ProviderProfiles.Any())
            {
                return; //DB has been seeded
            }

            if (_db.CustomerProfiles.Any())
            {
                return; //DB has been seeded
            }

            if (_db.RecurringTypes.Any())
            {
                return; //DB has been seeded
            }



            //_roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
            //_roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
            //_roleManager.CreateAsync(new IdentityRole(SD.ShipperRole)).GetAwaiter().GetResult();



            var ApplicationUsers = new List<ApplicationUser>
            {
                new ApplicationUser { FirstName = "John",
                    LastName = "Doe",
                    PhoneNum = "801-555-5555",
                    ProfilePicture = ""},
                new ApplicationUser { FirstName = "Jane",
                    LastName = "Doe",
                    PhoneNum = "801-555-5556",
                    ProfilePicture = ""},
                new ApplicationUser { FirstName = "Richard",
                    LastName = "Fry",
                    PhoneNum = "801-555-5557",
                    ProfilePicture = ""},
                new ApplicationUser { FirstName = "Pat",
                    LastName = "DeJong",
                    PhoneNum = "801-555-5558",
                    ProfilePicture = ""},
                new ApplicationUser { FirstName = "Julie",
                    LastName = "Christenson",
                    PhoneNum = "801-555-5559",
                    ProfilePicture = ""},
            };

            foreach (var c in ApplicationUsers)
            {
                _db.ApplicationUsers.Add(c);
            }
            _db.SaveChanges();

            var ProviderProfiles = new List<ProviderProfile>
            {
                //Pat Dejong
                new ProviderProfile {
                    UserId = "4",
                    RemoteLink = "",
                    BookingPrompt = "",
                    DepartmentString = ""},
                //Rich Fry
                new ProviderProfile {
                    UserId = "3",
                    RemoteLink = "",
                    BookingPrompt = "",
                    DepartmentString = ""},
                //Julie Christenson
                new ProviderProfile {
                    UserId = "5",
                    RemoteLink = "",
                    BookingPrompt = "",
                    DepartmentString = ""}
            };


            foreach (var c in ProviderProfiles)
            {
                _db.ProviderProfiles.Add(c);
            }
            _db.SaveChanges();


            var CustomerProfiles = new List<CustomerProfile>
            {
                //John Doe
                new CustomerProfile {
                    UserId = "1",
                    WNumber = 01333732 },
                //Jane Doe
                new CustomerProfile {
                    UserId = "2",
                    WNumber = 01333733 }
            };

            foreach (var c in CustomerProfiles)
            {
                _db.CustomerProfiles.Add(c);
            }
            _db.SaveChanges();

            var Locations = new List<Location>
            {
                new Location {
                    LocationName = "Ogden Campus",
                    Address1 = "3848 Harrison Blvd",
                    Address2 = "",
                    AddressCity = "Ogden",
                    BuildingName = "Norda",
                    RoomNumber = 101
                },
                new Location {
                    LocationName = "Davis Campus",
                    Address1 = "2750 University Park Blvd",
                    Address2 = "",
                    AddressCity = "Layton",
                    BuildingName = "D2",
                    RoomNumber = 102
                },
                new Location {
                    LocationName = "Farmington Campus",
                    Address1 = "240 N East Promontory",
                    Address2 = "",
                    AddressCity = "Farmington",
                    BuildingName = "Main",
                    RoomNumber = 103
                }
            };

            foreach (var c in Locations)
            {
                _db.Locations.Add(c);
            }
            _db.SaveChanges();

            var RecurringTypes = new List<RecurringType>
            {
                new RecurringType {
                Name = "Weekly",
                DaysBetween = 7},
                new RecurringType {
                Name = "Bi-Weekly",
                DaysBetween = 14}
            };

            foreach (var c in RecurringTypes)
            {
                _db.RecurringTypes.Add(c);
            }
            _db.SaveChanges();

            var Bookings = new List<Booking>
            {
                new Booking {
                    Id = 1,
                    WNumber = 01333732,
                    Subject = "Comp Sci",
                    Note = "CBTD Help",
                    StartTime = DateTime.Now,
                    Duration = 15,
                    Attatchment = "",
                    ProviderId = 1,

                }     
            };

            foreach (var c in Bookings)
            {
                _db.Bookings.Add(c);
            }
            _db.SaveChanges();

            var Availabilitys = new List<Availability>
            {
                new Availability {

                }
            };
        }
    }
}

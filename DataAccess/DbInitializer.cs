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
        private readonly UnitOfWork _unitOfWork;

        public DbInitializer(AppDbContext db, UnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
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

            //if (_db.Availabilities.Any())
            //{
            //    return; //DB has been seeded
            //}

            //if (_db.Bookings.Any())
            //{
            //    return; //DB has been seeded
            //}

            //if (_db.Locations.Any())
            //{
            //    return; //DB has been seeded
            //}

            //if (_db.ProviderProfiles.Any())
            //{
            //    return; //DB has been seeded
            //}

            //if (_db.CustomerProfiles.Any())
            //{
            //    return; //DB has been seeded
            //}

            //if (_db.RecurringTypes.Any())
            //{
            //    return; //DB has been seeded
            //}



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
                    //UserId = "4",
                    User =  _unitOfWork.ApplicationUser.Get(p=> p.FirstName == "Pat"),
                    RemoteLink = "",
                    BookingPrompt = "",
                    DepartmentString = ""},
                //Rich Fry
                new ProviderProfile {
                    //UserId = "3",
                    User =  _unitOfWork.ApplicationUser.Get(p=> p.FirstName == "Richard"),
                    RemoteLink = "",
                    BookingPrompt = "",
                    DepartmentString = ""},
                //Julie Christenson
                new ProviderProfile {
                    //UserId = "5",
                    User =  _unitOfWork.ApplicationUser.Get(p=> p.FirstName == "Julie"),
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
                    //Id = 1,
                    User = _unitOfWork.ApplicationUser.Get(p=> p.FirstName == "John"),
                    WNumber = 01333732 },
                //Jane Doe
                new CustomerProfile {
                    //Id = 2,
                    User = _unitOfWork.ApplicationUser.Get(p=> p.FirstName == "Jane"),
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
                    //LocationId = 1,
                    LocationName = "Ogden Campus",
                    Address1 = "3848 Harrison Blvd",
                    Address2 = "",
                    AddressCity = "Ogden",
                    BuildingName = "Norda",
                    RoomNumber = 101
                },
                new Location {
                    //LocationId = 2,
                    LocationName = "Davis Campus",
                    Address1 = "2750 University Park Blvd",
                    Address2 = "",
                    AddressCity = "Layton",
                    BuildingName = "D2",
                    RoomNumber = 102
                },
                new Location {
                    //LocationId = 3,
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
                //Id  = 1,
                Name = "Weekly",
                DaysBetween = 7},
                new RecurringType {
                //Id = 2,
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
                    //Id = 1,
                    WNumber = 01333732,
                    Subject = "4760",
                    Note = "CBTD Help",
                    StartTime = new DateTime(2024, 3, 26, 13, 0, 0),
                    Duration = 60,
                    Attachment = "",
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p=> p.ProviderProfileID == 3),
                    User = _unitOfWork.ApplicationUser.Get(p=> p.FirstName == "John"),

                }
            };

            foreach (var c in Bookings)
            {
                _db.Bookings.Add(c);
            }
            _db.SaveChanges();

            var Availabilites = new List<Availability>
            {
                new Availability {
                    //Rich Fry availabile tuesdays 1-2
                    //Id = 1,
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p=> p.ProviderProfileID == 3),
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Tuesday,
                    StartTime = new DateTime(2024, 3, 26, 13, 0, 0),
                    EndTime = new DateTime(2024, 3, 26, 14, 0, 0),
                },

                new Availability {
                    //Rich Fry availabile wednesdays 2-3
                    //Id = 2,
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p=> p.ProviderProfileID == 3),
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Wednesday,
                    StartTime = new DateTime(2024, 3, 27, 14, 0, 0),
                    EndTime = new DateTime(2024, 3, 27, 15, 0, 0),

                },

                new Availability {
                    //Rich Fry availabile Fridays 11-4
                    //Id = 3,
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p=> p.ProviderProfileID == 3),
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Friday,
                    StartTime = new DateTime(2024, 3, 29, 11, 0, 0),
                    EndTime = new DateTime(2024, 3, 29, 16, 0, 0),
                },

                new Availability {
                    //Pat available mondays 12-1
                    //Id = 4,
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p=> p.ProviderProfileID == 4),
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 3, 25, 12, 0, 0),
                    EndTime = new DateTime(2024, 3, 25, 13, 0, 0),
                },

                new Availability {
                    //Pat available thursadys 9-1
                    //Id = 5,
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p=> p.ProviderProfileID == 4),
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Thursday,
                    StartTime = new DateTime(2024, 3, 28, 9, 0, 0),
                    EndTime = new DateTime(2024, 3, 28, 13, 0, 0),
                },


                new Availability {
                    //Julie available thursadys 3-4
                    //Id = 6,
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p=> p.ProviderProfileID == 5),
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Thursday,
                    StartTime = new DateTime(2024, 3, 28, 15, 0, 0),
                    EndTime = new DateTime(2024, 3, 28, 16, 0, 0),
                },


                new Availability {
                    //Julie available mondays 9-10
                    //Id = 6,
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p=> p.ProviderProfileID == 5),
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 3, 28, 9, 0, 0),
                    EndTime = new DateTime(2024, 3, 28, 10, 0, 0),
                },
            };

            foreach (var c in Availabilites)
            {
                _db.Availabilities.Add(c);
            }
            _db.SaveChanges();


            var AvailabilityGroups = new List<AvailabilityGroup>
            {
                new AvailabilityGroup // weekly
                {
                    //Id = 1,
                    AvailabilityList = new List<Availability>(),
                    RecurringType = _unitOfWork.RecurringType.Get(p=> p.Id == 1),
                    RecurringEndDate = new DateTime(2024, 5, 1, 0, 0, 0),
                },
                new AvailabilityGroup //bi weekly
                {
                    //Id = 2,
                    AvailabilityList = new List<Availability>(),
                    RecurringType = _unitOfWork.RecurringType.Get(p=> p.Id == 2),
                    RecurringEndDate = new DateTime(2024, 5, 1, 0, 0, 0),
                },

            };

            foreach (var c in AvailabilityGroups)
            {
                _db.AvailabilityGroups.Add(c);
            }
            _db.SaveChanges();


        }
    }
}

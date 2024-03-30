using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        //private readonly RoleManager<IdentityRole> _roleManager;
        
        public DbInitializer(AppDbContext db, UnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            //_roleManager = roleManager;
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

            if (_db.ApplicationUsers.Any())
            {
                return; //DB has been seeded
            }

            if (_db.Availabilities.Any())
            {
                return; //DB has been seeded
            }

            if (_db.Bookings.Any())
            {
                return; //DB has been seeded
            }

            if (_db.CustomerProfiles.Any())
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


            if (_db.RecurringTypes.Any())
            {
                return; //DB has been seeded
            }




            //_roleManager.CreateAsync(new IdentityRole("STUDENT")).GetAwaiter().GetResult();
            //_roleManager.CreateAsync(new IdentityRole("TEACHER")).GetAwaiter().GetResult();
            //_roleManager.CreateAsync(new IdentityRole("TUTOR")).GetAwaiter().GetResult();
            //_roleManager.CreateAsync(new IdentityRole("ADMIN")).GetAwaiter().GetResult();
            //_roleManager.CreateAsync(new IdentityRole("ADVISOR")).GetAwaiter().GetResult();


            var ApplicationUsers = new List<ApplicationUser>
            {
                new ApplicationUser { FirstName = "John",
                    LastName = "Doe",
                    PhoneNum = "801-555-5555",
                    ProfilePicture = "",
                    Email = "JohnDoe@JohnDoe.com",
                    
                },  
                new ApplicationUser { FirstName = "Jane",
                    LastName = "Doe",
                    PhoneNum = "801-555-5556",
                    ProfilePicture = "",
                    Email = "JaneDoe@JaneDoe.com"
                    //Role
                },
                new ApplicationUser { FirstName = "Richard",
                    LastName = "Fry",
                    PhoneNum = "801-555-5557",
                    ProfilePicture = "",
                    Email = "RichardFry@RichardFry.com"
                },
                new ApplicationUser { FirstName = "Pat",
                    LastName = "DeJong",
                    PhoneNum = "801-555-5558",
                    ProfilePicture = "",
                    Email = "PatDeJong@PatDeJong.com"
                },
                new ApplicationUser { FirstName = "Julie",
                    LastName = "Christensen",
                    PhoneNum = "801-555-5559",
                    ProfilePicture = "",
                    Email = "JulieChristensen@JulieChristensen.com"
                },
            };

            foreach (var c in ApplicationUsers)
            {
                _db.ApplicationUsers.Add(c);
            }
            _db.SaveChanges();


            var Departments = new List<Department>
            {
                new Department
                {
                    Name = "Computer Science"
                },
                new Department
                {
                    Name = "General Studies"
                },
                new Department
                {
                    Name = "Networking"
                },
                new Department
                {
                    Name = "Web Development"
                },
                new Department
                {
                    Name = "Cybersecurity"
                }
            };

            foreach (var d in Departments)
            {
                _db.Department.Add(d);
            }
            _db.SaveChanges();

            var ProviderProfiles = new List<ProviderProfile>
            {
                //Pat Dejong
                new ProviderProfile {
                    User =  _unitOfWork.ApplicationUser.Get(u => u.Email == "PatDeJong@PatDeJong.com").Id,
                    DeparmentId = _unitOfWork.Department.Get(d => d.Name == "Computer Science").Id,
                    userFullName = _unitOfWork.ApplicationUser.Get(u => u.Email == "PatDeJong@PatDeJong.com").FullName
                    },
                //Rich Fry
                new ProviderProfile {
                    //UserId = "3",
                    User =  _unitOfWork.ApplicationUser.Get(u => u.Email == "RichardFry@RichardFry.com").Id,
                    DeparmentId = _unitOfWork.Department.Get(d => d.Name == "Computer Science").Id,
                    userFullName = _unitOfWork.ApplicationUser.Get(u=> u.Id == "RichardFry@RichardFry.com").FullName
                    },
                //Julie Christenson
                new ProviderProfile {
                    User =  _unitOfWork.ApplicationUser.Get(u => u.Email == "JulieChristensen@JulieChristensen.com").Id,
                    DeparmentId = _unitOfWork.Department.Get(d => d.Name == "Computer Science").Id,
                    userFullName = _unitOfWork.ApplicationUser.Get(u => u.Email == "JulieChristensen@JulieChristensen.com").FullName,
                    }
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
                    User =  _unitOfWork.ApplicationUser.Get(u => u.Email == "JohnDoe@JohnDoe.com").Id,
                    WNumber = 01333732 },
                //Jane Doe
                new CustomerProfile {
                    User =  _unitOfWork.ApplicationUser.Get(u => u.Email == "JaneDoe@JaneDoe.com").Id,
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
                    RoomNumber = "101"
                },
                new Location {
                    LocationName = "Davis Campus",
                    Address1 = "2750 University Park Blvd",
                    Address2 = "",
                    AddressCity = "Layton",
                    BuildingName = "D2",
                    RoomNumber = "102"
                },
                new Location {
                    LocationName = "Farmington Campus",
                    Address1 = "240 N East Promontory",
                    Address2 = "",
                    AddressCity = "Farmington",
                    BuildingName = "Main",
                    RoomNumber = "103"
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
                    DaysBetween = 7
                },
                new RecurringType {
                    Name = "Bi-Weekly",
                    DaysBetween = 14
                }
            };

            foreach (var c in RecurringTypes)
            {
                _db.RecurringTypes.Add(c);
            }
            _db.SaveChanges();


            var patId = _unitOfWork.ApplicationUser.Get(u => u.Email == "PatDeJong@PatDeJong.com").Id;
            var richId = _unitOfWork.ApplicationUser.Get(u => u.Email == "RichardFry@RichardFry.com").Id;
            var julieId = _unitOfWork.ApplicationUser.Get(u => u.Email == "JulieChristensen@JulieChristensen.com").Id;

            var Availabilitys = new List<Availability>
            {
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Thursday,
                    StartTime = new DateTime(2024, 4, 25, 9, 0, 0), // April 25, 2024, 9:00 AM
                    EndTime = new DateTime(2024, 4, 25, 10, 0, 0), // April 25, 2024, 5:00 PM
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Friday,
                    StartTime = new DateTime(2024, 4, 26, 9, 0, 0),
                    EndTime = new DateTime(2024, 4, 26, 10, 0, 0),
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Saturday,
                    StartTime = new DateTime(2024, 4, 27, 2, 0, 0),
                    EndTime = new DateTime(2024, 4, 27, 2, 30, 0),
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Saturday,
                    StartTime = new DateTime(2024, 4, 27, 3, 0, 0),
                    EndTime = new DateTime(2024, 4, 27, 3, 45, 0),
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Sunday,
                    StartTime = new DateTime(2024, 4, 28, 7, 0, 0),
                    EndTime = new DateTime(2024, 4, 28, 8, 0, 0),
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 29, 7, 0, 0),
                    EndTime = new DateTime(2024, 4, 29, 8, 0, 0),
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 29, 8, 0, 0),
                    EndTime = new DateTime(2024, 4, 29, 9, 0, 0),
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 29, 9, 0, 0),
                    EndTime = new DateTime(2024, 4, 29, 9, 30, 0),
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 29, 9, 45, 0),
                    EndTime = new DateTime(2024, 4, 29, 10, 30, 0),
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Tuesday,
                    StartTime = new DateTime(2024, 4, 30, 9, 30, 0),
                    EndTime = new DateTime(2024, 4, 30, 10, 30, 0),
                },

            };

            foreach (var c in Availabilitys)
            {
                _db.Availabilities.Add(c);
            }
            _db.SaveChanges();

            var bookAvailabilities = _unitOfWork.Availability.GetAll().Take(5).ToList();
            for (int i = 1; i <= 5; i++)
            {
                var user = _unitOfWork.ApplicationUser.Get(u=> u.Id == patId);
                var booking = new Booking
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    User = _unitOfWork.ApplicationUser.Get(u => u.Email == "JaneDoe@JaneDoe.com").Id,
                    MeetingTitle = "Coolest Test Meeting Ever",
                    //WNumber = 00000 + i, // Example WNumber
                    Subject = $"Seed Data: {i + 1}",
                    Note = $"Testing Note:  {i + 1}",
                    StartTime = bookAvailabilities[i].StartTime,
                    Duration = (int)(bookAvailabilities[i].EndTime - bookAvailabilities[i].StartTime).TotalMinutes,
                    objAvailability = bookAvailabilities[i].Id
                };
                _db.Bookings.Add(booking);
            }
            _db.SaveChanges();



            //var AvailabilityGroups = new List<AvailabilityGroup>
            //{
            //    new AvailabilityGroup // weekly
            //    {
            //        //Id = 1,
            //        AvailabilityList = new List<Availability>(),
            //        RecurringType = _unitOfWork.RecurringType.Get(p=> p.Id == 1),
            //        RecurringEndDate = new DateTime(2024, 5, 1, 0, 0, 0),
            //    },
            //    new AvailabilityGroup //bi weekly
            //    {
            //        //Id = 2,
            //        AvailabilityList = new List<Availability>(),
            //        RecurringType = _unitOfWork.RecurringType.Get(p=> p.Id == 2),
            //        RecurringEndDate = new DateTime(2024, 5, 1, 0, 0, 0),
            //    },

            //};

            //foreach (var c in AvailabilityGroups)
            //{
            //    _db.AvailabilityGroups.Add(c);
            //}
            //_db.SaveChanges();


        }
    }
}

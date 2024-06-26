using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public DbInitializer(AppDbContext db, UnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
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




            _roleManager.CreateAsync(new IdentityRole("STUDENT")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("TEACHER")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("TUTOR")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("ADMIN")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("ADVISOR")).GetAwaiter().GetResult();

            _db.SaveChanges();

           // Create the super admin account
           var adminUser = new ApplicationUser
           {
               FirstName = "Supreme",
               LastName = "Admin",
               UserName = "Admin@Admin.com", // Ensure UserName is set for IdentityUser
               Email = "Admin@Admin.com",
               PhoneNum = "801-555-5555",
           };
            //_db.ApplicationUsers.AddAsync(adminUser);

            _userManager.CreateAsync(adminUser, "Password123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, "ADMIN").GetAwaiter().GetResult();


            //other accounts
            var studentUser = new ApplicationUser
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "JohnDoe@JohnDoe.com",
                PhoneNum = "801-555-5555",
                Email = "JohnDoe@JohnDoe.com",
            };
            _db.ApplicationUsers.AddAsync(studentUser);

            _userManager.CreateAsync(studentUser, "Password123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(studentUser, "STUDENT").GetAwaiter().GetResult();

            var tutorUser = new ApplicationUser
            {
                FirstName = "Jane",
                LastName = "Doe",
                UserName = "JaneDoe@JaneDoe.com",
                PhoneNum = "801-555-5556",
                Email = "JaneDoe@JaneDoe.com",

            };
            _db.ApplicationUsers.AddAsync(tutorUser);

            _userManager.CreateAsync(tutorUser, "Password123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(tutorUser, "TUTOR").GetAwaiter().GetResult();

            var teacherUser = new ApplicationUser
            {
                FirstName = "Richard",
                LastName = "Fry",
                UserName = "RichardFry@RichardFry.com",
                PhoneNum = "801-555-5557",
                Email = "RichardFry@RichardFry.com"
            };
            _db.ApplicationUsers.AddAsync(teacherUser);

            _userManager.CreateAsync(teacherUser, "Password123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(teacherUser, "TEACHER").GetAwaiter().GetResult();

            var advisorUser = new ApplicationUser
            {
                FirstName = "Pat",
                LastName = "DeJong",
                UserName = "PatDeJong@PatDeJong.com",
                PhoneNum = "801-555-5558",

                Email = "PatDeJong@PatDeJong.com"
            };
            _db.ApplicationUsers.AddAsync(advisorUser);

            _userManager.CreateAsync(advisorUser, "Password123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(advisorUser, "ADVISOR").GetAwaiter().GetResult();


            var advisorUser2 = new ApplicationUser
            {
                FirstName = "Julie",
                LastName = "Christensen",
                UserName = "JulieChristensen@JulieChristensen.com",
                PhoneNum = "801-555-5559",
                Email = "JulieChristensen@JulieChristensen.com"
            };
            _db.ApplicationUsers.AddAsync(advisorUser2);

            _userManager.CreateAsync(advisorUser2, "Password123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(advisorUser2, "ADVISOR").GetAwaiter().GetResult();

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
                    User =  _unitOfWork.ApplicationUser.Get(u => u.Email == "RichardFry@RichardFry.com").Id,
                    DeparmentId = _unitOfWork.Department.Get(d => d.Name == "Computer Science").Id,
                    userFullName = _unitOfWork.ApplicationUser.Get(u=> u.Email == "RichardFry@RichardFry.com").FullName
                    },
                //Julie Christenson
                new ProviderProfile {
                    User =  _unitOfWork.ApplicationUser.Get(u => u.Email == "JulieChristensen@JulieChristensen.com").Id,
                    DeparmentId = _unitOfWork.Department.Get(d => d.Name == "Computer Science").Id,
                    userFullName = _unitOfWork.ApplicationUser.Get(u => u.Email == "JulieChristensen@JulieChristensen.com").FullName,
                    },
                new ProviderProfile {
                    User =  _unitOfWork.ApplicationUser.Get(u => u.Email == "JaneDoe@JaneDoe.com").Id,
                    DeparmentId = _unitOfWork.Department.Get(d => d.Name == "Computer Science").Id,
                    userFullName = _unitOfWork.ApplicationUser.Get(u => u.Email == "JaneDoe@JaneDoe.com").FullName,
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

            var locationTypes = new List<LocationType> {
                new LocationType
                {
                    LocationTypeName = "Virtual Zoom Link",
                    Address1="",
                    Address2="",
                    AddressCity=""
                },
                new LocationType
                {
                    LocationTypeName = "Ogden Campus",
                    Address1 = "3848 Harrison Blvd",
                    Address2 = "",
                    AddressCity = "Ogden"
                },
                new LocationType
                {
                    LocationTypeName = "Davis Campus",
                    Address1 = "2750 University Park Blvd",
                    Address2 = "",
                    AddressCity = "Layton"
                },
                new LocationType
                {
                    LocationTypeName = "Farmington Campus",
                   Address1 = "240 N East Promontory",
                    Address2 = "",
                    AddressCity = "Farmington"
                },
            };
            foreach (var locationType in locationTypes)
            {
                _db.LocationTypes.Add(locationType);
            }
            _db.SaveChanges();

            var patId = _unitOfWork.ApplicationUser.Get(u => u.Email == "PatDeJong@PatDeJong.com").Id;
            var richId = _unitOfWork.ApplicationUser.Get(u => u.Email == "RichardFry@RichardFry.com").Id;
            var julieId = _unitOfWork.ApplicationUser.Get(u => u.Email == "JulieChristensen@JulieChristensen.com").Id;

            var Locations = new List<Location>
            {
                new Location {
                    LocationName = "Norda Ogden Campus",
                    BuildingName = "Norda",
                    RoomNumber = "101",
                    LocationType=2,
                    ProfileId = _unitOfWork.ProviderProfile.Get(p => p.User == julieId).Id,
                },
                new Location {
                    LocationName = "D2 Davis Campus",
                    BuildingName = "D2",
                    RoomNumber = "102",
                    LocationType=3,
                    ProfileId = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                },
                new Location {
                    LocationName = "Main Farmington Campus",
                    BuildingName = "Main",
                    RoomNumber = "103",
                    LocationType=4,
                    ProfileId = _unitOfWork.ProviderProfile.Get(p => p.User == richId).Id,
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

            var cat = new List<Category>
            {
                new Category
                {
                    Name = "General Studies",
                    ProviderProfile = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    Color = "4C0E7D"
                }
            };

            foreach (var c in cat)
            {
                _db.Categories.Add(c);
            }
            _db.SaveChanges();

            int pId = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id;
            List<int> cList = new List<int>();
            cList.Add(_unitOfWork.Category.Get(c => c.ProviderProfile == pId).Id);

            var Availabilitys = new List<Availability>
            {
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Thursday,
                    StartTime = new DateTime(2024, 4, 25, 9, 0, 0), // April 25, 2024, 9:00 AM
                    EndTime = new DateTime(2024, 4, 25, 10, 0, 0), // April 25, 2024, 5:00 PM
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Friday,
                    StartTime = new DateTime(2024, 4, 26, 9, 0, 0),
                    EndTime = new DateTime(2024, 4, 26, 10, 0, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Saturday,
                    StartTime = new DateTime(2024, 4, 27, 2, 0, 0),
                    EndTime = new DateTime(2024, 4, 27, 2, 30, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Saturday,
                    StartTime = new DateTime(2024, 4, 27, 3, 0, 0),
                    EndTime = new DateTime(2024, 4, 27, 3, 45, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Sunday,
                    StartTime = new DateTime(2024, 4, 28, 7, 0, 0),
                    EndTime = new DateTime(2024, 4, 28, 8, 0, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 29, 7, 0, 0),
                    EndTime = new DateTime(2024, 4, 29, 8, 0, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 29, 8, 0, 0),
                    EndTime = new DateTime(2024, 4, 29, 9, 0, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 29, 9, 0, 0),
                    EndTime = new DateTime(2024, 4, 29, 9, 30, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 29, 9, 45, 0),
                    EndTime = new DateTime(2024, 4, 29, 10, 30, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
                },
                new Availability
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    LocationId = 1,
                    DayOfTheWeek = DayOfWeek.Tuesday,
                    StartTime = new DateTime(2024, 4, 30, 9, 30, 0),
                    EndTime = new DateTime(2024, 4, 30, 10, 30, 0),
                    ProviderFullName = "Pat Dejong",
                    Category = cList,
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
                var booking = new Booking
                {
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == patId).Id,
                    User = _unitOfWork.ApplicationUser.Get(u => u.Email == "JaneDoe@JaneDoe.com").Id,
                    //MeetingTitle = "Coolest Test Meeting Ever",
                    //WNumber = 00000 + i, // Example WNumber
                    Subject = $"Seed Data: {i + 1}",
                    Note = $"Testing Note:  {i + 1}",
                    StartTime = bookAvailabilities[i].StartTime,
                    Duration = (int)(bookAvailabilities[i].EndTime - bookAvailabilities[i].StartTime).TotalMinutes,
                    objAvailability = bookAvailabilities[i].Id,
                    CategoryID = _unitOfWork.Category.Get(c => c.ProviderProfile == pId).Id
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

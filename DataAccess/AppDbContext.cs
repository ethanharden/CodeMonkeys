﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Models;

namespace DataAccess
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet <ProviderProfile> ProviderProfiles { get; set; }
        public DbSet <ApplicationUser> ApplicationUsers { get; set; }
        public DbSet <CustomerProfile> CustomerProfiles { get; set; }
        public DbSet <RecurringType> RecurringTypes { get; set; }
        public DbSet<AvailabilityGroup> AvailabilityGroups { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<LocationType> LocationTypes { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}

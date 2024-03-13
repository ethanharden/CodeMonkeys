using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        public IGenericRepository<Availability> Availability { get; }
        public IGenericRepository<AvailabilityGroup> AvailabilityGroup { get; }
        public IGenericRepository<Booking> Booking { get; }
        public IGenericRepository<Location> Location { get;}
        public IGenericRepository<ProviderProfile> ProviderProfile { get; }
        public IGenericRepository<ApplicationUser> ApplicationUser { get; }
        public IGenericRepository<RecurringType> RecurringType { get; }
        public IGenericRepository<CustomerProfile> CustomerProfile { get; }
        int Commit();

        Task<int> CommitAsync();

    }
}

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
    public class UnitOfWork
    {
        private readonly AppDbContext _DbContext;
        public UnitOfWork(AppDbContext appDbContext)
        {
            _DbContext = appDbContext;
        }
        public IGenericRepository<Availability> _Availability;
        public IGenericRepository<Booking> _Booking;
        public IGenericRepository<Location> _Location;
        public IGenericRepository<ProviderProfile> _ProviderProfile;
        public IGenericRepository<RecurringType> _RecurringType;
        public IGenericRepository<CustomerProfile> _CustomerProfile;
        public IGenericRepository<ApplicationUser> _ApplicationUser;
        public IGenericRepository<AvailabilityGroup> _AvailabilityGroup;
        public IGenericRepository<Department> _Department;
        public IGenericRepository<LocationType> _LocationType;

        public IGenericRepository<LocationType> LocationType
        {
            get
            {
                if (_LocationType == null)
                {
                    _LocationType = new GenericRepository<LocationType>(_DbContext);
                }
                return _LocationType;
            }
        }

        public IGenericRepository<Department> Department
        {
            get
            {
                if (_Department == null)
                {
                    _Department = new GenericRepository<Department>(_DbContext);
                }
                return _Department;
            }
        }
        public IGenericRepository<Availability> Availability
        {
            get
            {
                if (_Availability == null)
                {
                    _Availability = new GenericRepository<Availability>(_DbContext);
                }
                return _Availability;
            }
        }
        public IGenericRepository<AvailabilityGroup> AvailabilityGroup
        {
            get
            {
                if (_AvailabilityGroup == null)
                {
                    _AvailabilityGroup = new GenericRepository<AvailabilityGroup>(_DbContext);
                }
                return _AvailabilityGroup;
            }
        }
        public IGenericRepository<Booking> Booking
        {
            get
            {
                if (_Booking == null)
                {
                    _Booking = new GenericRepository<Booking>(_DbContext);
                }
                return _Booking;
            }
        }
        public IGenericRepository<Location> Location
        {
            get
            {
                if (_Location == null)
                {
                    _Location = new GenericRepository<Location>(_DbContext);
                }
                return _Location;
            }
        }
        public IGenericRepository<ProviderProfile> ProviderProfile
        {
            get
            {
                if (_ProviderProfile == null)
                {
                    _ProviderProfile = new GenericRepository<ProviderProfile>(_DbContext);
                }
                return _ProviderProfile;
            }
        }

        public IGenericRepository<RecurringType> RecurringType
        {
            get
            {
                if (_RecurringType == null)
                {
                    _RecurringType = new GenericRepository<RecurringType>(_DbContext);
                }
                return _RecurringType;
            }
        }

        public IGenericRepository<CustomerProfile> CustomerProfile
        {
            get
            {
                if (_CustomerProfile == null)
                {
                    _CustomerProfile = new GenericRepository<CustomerProfile>(_DbContext);
                }
                return _CustomerProfile;
            }
        }

        public IGenericRepository<ApplicationUser> ApplicationUser
        {
            get
            {
                if (_ApplicationUser == null)
                {
                    _ApplicationUser = new GenericRepository<ApplicationUser>(_DbContext);
                }
                return _ApplicationUser;
            }
        }


        public int Commit()
        {
            return _DbContext.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _DbContext.Dispose();
        }

    }
}

using Infrastructure.Interfaces;
using Infrastructure.Models;
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

    }
}

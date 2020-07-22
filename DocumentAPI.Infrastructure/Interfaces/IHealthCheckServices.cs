using DocumentAPI.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentAPI.Infrastructure.Interfaces
{
    public interface IHealthCheckServices
    {
        Task<HealthCheckResponse> CheckInternalDependencies();
        //Task<HealthCheckResponse> CheckExternalDependencies();
    }
}

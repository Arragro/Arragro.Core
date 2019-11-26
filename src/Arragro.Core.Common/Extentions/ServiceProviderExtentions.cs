﻿using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Arragro.Core.Common.Extentions
{
    public static class ServiceProviderExtentions
    {
        public static IServiceCollection Remove<T>(this IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
            if (serviceDescriptor != null)
                services.Remove(serviceDescriptor);

            return services;
        }
    }
}

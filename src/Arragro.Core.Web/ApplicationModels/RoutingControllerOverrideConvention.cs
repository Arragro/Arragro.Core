using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;

namespace Arragro.Core.Web.ApplicationModels
{
    public class RoutingControllerOverrideConvention : IApplicationModelConvention
    {
        private readonly List<Type> _removals;

        public RoutingControllerOverrideConvention(List<Type> removals)
        {
            _removals = removals;
        }

        public void Apply(ApplicationModel application)
        {
            var removeList = new List<ControllerModel>();
            foreach (var controller in application.Controllers)
            {
                if (_removals.Contains(controller.ControllerType))
                {
                    removeList.Add(controller);
                }
            }
            foreach (var remove in removeList)
                application.Controllers.Remove(remove);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using Nancy;
using Nancy.Routing.Constraints;
using WmiEndpoint.Queries;
using WmiEndpoint.ViewModels;

namespace WmiEndpoint.Modules
{
    public class Win32ServiceModule : NancyModule
    {
        private static readonly TimeSpan TimeToWait = TimeSpan.FromSeconds(30);

        public Win32ServiceModule()
            : base("/wmi/win32_service")
        {
            Get["/"] = GetAllWindowsServices;
            Get["/{serviceName}"] = GetWindowsServiceByName;
            Get["/{serviceName}/start"] = StartWindowsService;
            Get["/{serviceName}/stop"] = StopWindowsService;
            Get["/{serviceName}/restart"] = RestartWindowsService;
        }

        public dynamic GetAllWindowsServices(dynamic parameters)
        {
            var model = new WindowsServicesQueryHandler().Handle(new WindowsServicesQuery());
            return Negotiate
                .WithModel(model)
                .WithView("Win32Services.cshtml");
        }

        public dynamic GetWindowsServiceByName(dynamic parameters)
        {
            string serviceName = parameters.serviceName;
            var model = InternalGetServiceByName(serviceName);

            return Negotiate
                .WithModel(model)
                .WithView("Win32Service.cshtml");
        }

        public dynamic StartWindowsService(dynamic parameters)
        {
            var windowsService = InternalGetServiceByName(parameters.serviceName);
            if (windowsService == null)
            {
                return HttpStatusCode.NotFound;
            }

            var serviceController = new ServiceController(parameters.serviceName);
            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeToWait);

            return InternalGetServiceByName(parameters.serviceName);
        }

        public dynamic StopWindowsService(dynamic parameters)
        {
            var windowsService = InternalGetServiceByName(parameters.serviceName);
            if (windowsService == null)
            {
                return HttpStatusCode.NotFound;
            }

            var serviceController = new ServiceController(parameters.serviceName);
            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeToWait);

            return InternalGetServiceByName(parameters.serviceName);
        }

        public dynamic RestartWindowsService(dynamic parameters)
        {
            throw new NotImplementedException();
        }

        private WindowsServiceViewModel InternalGetServiceByName(string serviceName)
        {
            var services =
                new WindowsServicesQueryHandler()
                    .Handle(new WindowsServicesQuery { ServiceName = serviceName })
                    .ToArray();

            WindowsServiceViewModel model = services.Length > 0 ? services[0] : null;
            return model;
        }
    }
}

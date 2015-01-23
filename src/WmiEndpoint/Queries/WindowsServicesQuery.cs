using System;
using System.Collections.Generic;
using WmiEndpoint.Infrastructure;
using WmiEndpoint.ViewModels;

namespace WmiEndpoint.Queries
{
    public class WindowsServicesQuery : IQuery<IEnumerable<WindowsServiceViewModel>>
    {
        public WindowsServicesQuery()
        {
            ComputerName = Environment.MachineName;
        }

        public string ComputerName { get; set; }
        public string ServiceName { get; set; }
    }
}

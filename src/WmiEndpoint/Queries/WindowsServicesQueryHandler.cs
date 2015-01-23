using System.Collections.Generic;
using System.Management;
using WmiEndpoint.Infrastructure;
using WmiEndpoint.ViewModels;

namespace WmiEndpoint.Queries
{
    public class WindowsServicesQueryHandler : IQueryHandler<WindowsServicesQuery, IEnumerable<WindowsServiceViewModel>>
    {
        public IEnumerable<WindowsServiceViewModel> Handle(WindowsServicesQuery query)
        {
            var path = string.Format(@"\\{0}\root\cimv2", query.ComputerName);
            var scope = new ManagementScope(path);

            var wmiQuery = new SelectQuery();
            wmiQuery.QueryString = "select * from win32_service";
            if (!string.IsNullOrWhiteSpace(query.ServiceName))
            {
                wmiQuery.QueryString += string.Format(" where name = '{0}'", query.ServiceName);
            }

            var viewModels = new List<WindowsServiceViewModel>();
            using (var searcher = new ManagementObjectSearcher(scope, wmiQuery))
            using (var results = searcher.Get())
            {
                foreach (var result in results)
                {
                    var viewModel = 
                        new WindowsServiceViewModel
                        {
                            Name = (string) result["Name"],
                            DisplayName = (string) result["DisplayName"],
                            Description = (string) result["Description"],
                            Status = (string)result["State"]
                        };
                    viewModels.Add(viewModel);
                }
            }

            return viewModels;
        }
    }
}

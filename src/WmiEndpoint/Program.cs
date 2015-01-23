using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Nancy.Hosting.Self;
using Topshelf;
using WmiEndpoint.Configuration;
using WmiEndpoint.Queries;

namespace WmiEndpoint
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Program));

        static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.ConfigureAndWatch(
                    new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

                HostFactory.Run(x =>                                 
                {
                    x.UseLog4Net();
                    x.Service<NancyHost>(s =>                        
                    {
                        s.ConstructUsing(host => 
                            new NancyHost(
                                new Uri("http://localhost:9292"),
                                new Bootstrapper(),
                                new HostConfiguration
                                {
                                    UrlReservations = { CreateAutomatically = true }
                                }));

                        s.WhenStarted(host => host.Start());            
                        s.WhenStopped(host => host.Stop());             
                    });

                    x.SetDescription("WMI Endpoint");       
                    x.SetDisplayName("WMI Endpoint");                      
                    x.SetServiceName("wmiendpoint");                      
                }); 
            }
            catch (Exception e)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error("An unhandled exception has slipped through the cracks", e);
                }
            }
        }
    }
}

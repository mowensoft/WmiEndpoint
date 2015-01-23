using System.Collections.Generic;
using System.Reflection;
using Nancy.ViewEngines.Razor;

namespace WmiEndpoint.Configuration
{
    public class RazorConfiguration : IRazorConfiguration
    {
        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

        public IEnumerable<string> GetAssemblyNames()
        {
            yield return Assembly.GetExecutingAssembly().GetName().Name;
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            yield return assemblyName + ".ViewModels";
        }
    }
}

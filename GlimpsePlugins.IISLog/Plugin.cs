using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using System.Web;
using System.DirectoryServices;
using System.Collections;

namespace GlimpsePlugins.IISLog
{
    [GlimpsePlugin]
    public class Plugin : IGlimpsePlugin
    {
        public object GetData(HttpContextBase context)
        {
            return Test().ToList();
        }

        private IEnumerable<object> Test()
        {
            yield return new object[] {"Log Path","Comment" };

            DirectoryEntry W3SVC = new DirectoryEntry("IIS://localhost/w3svc");
            foreach (DirectoryEntry site in W3SVC.Children)
            {
                if (site.SchemaClassName == "IIsWebServer")
                {
                    string LogFilePath = System.IO.Path.Combine(
                        site.Properties["LogFileDirectory"].Value.ToString(),
                        "W3CSVC" + site.Name);

                    yield return new object[] { LogFilePath, site.Properties["ServerComment"].Value };
                }
            }
    
        }

        public string Name
        {
            get { return "IIS Log"; }
        }

        public void SetupInit()
        {
        }
    }
}

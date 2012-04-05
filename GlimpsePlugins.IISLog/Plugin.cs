using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using System.Web;
using System.Collections;
using Microsoft.Web.Administration;
using System.IO;

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

            ServerManager manager = new ServerManager();
  
            foreach (Site site in manager.Sites)
            {
                string logFilePath = Environment.ExpandEnvironmentVariables(site.LogFile.Directory) + "\\W3svc" + site.Id.ToString();
                yield return new object[] { site.Name, ReadLog(logFilePath) };
            }    
        }

        private string ReadLog(string logPath)
        {
            FileStream stream =File.OpenRead(logPath);
            if (stream.Length > 1000)
                stream.Seek(1000, SeekOrigin.End);

            StreamReader reader = new StreamReader(stream);            
            return reader.ReadToEnd();
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

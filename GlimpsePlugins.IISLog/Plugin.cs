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
        const int bytesToRead = 10000;

        public object GetData(HttpContextBase context)
        {
            return Test().ToList();
        }

        private IEnumerable<object> Test()
        {
            yield return new object[] { "Log Path", "Comment" };

            string logFolder = @"C:\inetpub\logs\LogFiles\W3SVC1";

            foreach (string file in Directory.EnumerateFiles(logFolder))
            {
                yield return new object[] { file, ReadLog(file) };
            }
        }

        private string ReadLog(string logPath)
        {
            try
            {
                FileStream stream = File.Open(logPath, FileMode.Open, FileAccess.Read,FileShare.ReadWrite);
                if (stream.Length > bytesToRead)
                    stream.Seek(-bytesToRead, SeekOrigin.End);

                StreamReader reader = new StreamReader(stream);
                reader.ReadLine();
                return reader.ReadToEnd();
            }
            catch(IOException)
            {
                return string.Empty;
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

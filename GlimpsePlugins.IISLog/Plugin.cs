using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using System.Web;

namespace GlimpsePlugins.IISLog
{
    [GlimpsePlugin]
    public class Plugin : IGlimpsePlugin
    {
        public object GetData(HttpContextBase context)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return "IIS Log"; }
        }

        public void SetupInit()
        {
            throw new NotImplementedException();
        }
    }
}

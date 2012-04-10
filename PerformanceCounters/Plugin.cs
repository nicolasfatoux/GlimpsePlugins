using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using System.Web;
using System.Diagnostics;

namespace PerformanceCounters
{
    [GlimpsePlugin]
    public class Plugin : IGlimpsePlugin
    {
        private readonly static object[][] Counters = new object[][]{
                new object[] {
                    "ASP.NET",
                    new[] {
                        "Application Restarts",
                        "Requests Queued",
                        "Worker Process Restarts",
                    },
                },
                new object[]
                {
                    "ASP.NET Applications",
                    new[]
                    {
                        "Errors Total",
                        "Requests/Sec",
                        "Pipeline Instance Count",
                    }
                },
                new object[]
                {
                    ".NET CLR Exceptions",
                    new[]
                    {
                        "# of Exceps Thrown",
                    }
                },
                new object[]
                {
                    "Processor",
                    new[]
                    {
                        "% CPU Utilization",
                    }
                },
                new object[]
                {
                    "System",
                    new[]
                    {
                        "Context Switches/sec",
                    }
                }
        };

        private IEnumerable<object> ReadCounters()
        {
            yield return new[] { "Category", "Name", "Value" };

            foreach (var category in Counters)
                foreach (var counter in (string[])category[1])
                {
                    PerformanceCounter perf = new PerformanceCounter((string)category[0], counter);
                    yield return new object[] { perf.CategoryName, perf.CounterName, perf.RawValue };
                }
        }

        public object GetData(HttpContextBase context)
        {
            return ReadCounters().ToList();
        }

        public string Name
        {
            get { return "Performance counters"; }
        }

        public void SetupInit()
        {
            throw new NotImplementedException();
        }
    }
}

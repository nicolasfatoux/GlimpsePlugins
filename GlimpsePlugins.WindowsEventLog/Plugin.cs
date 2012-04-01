using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using System.Web;
using System.Diagnostics;
using System.Security;
using System.Collections;

namespace GlimpsePlugins.WindowsEventLog
{
    [GlimpsePlugin]
    public class Plugin : IGlimpsePlugin
    {
        /// <summary>
        /// The maximum number of log entries to fetch.
        /// </summary>
        const int MaxEntries = 50;

        /// <summary>
        /// Gets the plugin data.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public object GetData(HttpContextBase context)
        {
            var data = new List<object[]>();

            // Event Log header
            data.Add(new[] { "Log", "Entries" });

            // Event Log rows
            foreach (var eventLog in EventLog.GetEventLogs())
            {
                List<object[]> entriesData = null;
                try
                {
                    if (eventLog.Entries.Count > 0)
                    {
                        int lastEntryIndex = eventLog.Entries.Count > MaxEntries ? eventLog.Entries.Count - MaxEntries : 0;

                        entriesData = new List<object[]>();
                        
                        // Entries header
                        entriesData.Add(new[] { "Category", "EntryType", "Message", "TimeGenerated" });

                        // Entries rows
                        for (int i = eventLog.Entries.Count - 1; i > lastEntryIndex; i--)
                        {
                            var entry = eventLog.Entries[i];

                            // Log Entry data
                            var entryData = new ArrayList() { entry.Category, entry.EntryType.ToString(), entry.Message, entry.TimeGenerated };

                            // Meta data
                            string meta = GetMeta(entry);
                            if (meta != null)
                                entryData.Add(meta);

                            entriesData.Add(entryData.ToArray());
                        }
                    }
                }
                catch (SecurityException)
                {
                }

                data.Add(new object[] { eventLog.Log, entriesData });
            }

            return data;
        }

        /// <summary>
        /// Gets the meta data for an EventLogEntry.
        /// </summary>
        /// <param name="eventLogEntry"></param>
        /// <returns></returns>
        private static string GetMeta(EventLogEntry eventLogEntry)
        {
            string meta = null;
            switch (eventLogEntry.EntryType)
            {
                case EventLogEntryType.Error:
                    meta = "error";
                    break;
                case EventLogEntryType.FailureAudit:
                    meta = "fail";
                    break;
                case EventLogEntryType.Information:
                    meta = "info";
                    break;
                case EventLogEntryType.SuccessAudit:
                    break;
                case EventLogEntryType.Warning:
                    meta = "warn";
                    break;
                default:
                    break;
            }
            return meta;
        }

        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        public string Name
        {
            get { return "Windows Event Viewer"; }
        }

        /// <summary>
        /// Initializes the plugin.
        /// </summary>
        public void SetupInit()
        {
        }
    }
}

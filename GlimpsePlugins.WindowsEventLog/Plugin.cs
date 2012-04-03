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
            return ReadLogs().ToList();
        }

        /// <summary>
        /// Reads the event logs.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<object> ReadLogs()
        {
            // Header
            yield return new[] { "Log", "Entries" };

            // Logs
            foreach (var eventLog in EventLog.GetEventLogs())
            {
                object[] row = null;

                try
                {
                    var entries = ReadEntries(eventLog).ToList();

                    if (entries.Count() > 0)
                        row = new object[] { eventLog.LogDisplayName, entries };
                }
                catch (SecurityException)
                {
                }

                if (row != null)
                    yield return row;
            }
        }

        /// <summary>
        /// Reads event log entries.
        /// </summary>
        /// <param name="log">The event log.</param>
        /// <returns></returns>
        private IEnumerable<object> ReadEntries(EventLog log)
        {
            if (log.Entries.Count > 0)
            {
                // Header
                yield return new[] { "Level", "Date", "Source", "Event ID", "Category", "Message" };

                // Entries
                int lastEntryIndex = log.Entries.Count > MaxEntries ? log.Entries.Count - MaxEntries : 0;

                for (int i = log.Entries.Count - 1; i > lastEntryIndex; i--)
                {
                    var entry = log.Entries[i];

                    // Log Entry data
                    var entryData = new ArrayList() { entry.EntryType.ToString(), entry.TimeGenerated.ToString("G"), entry.Source, entry.InstanceId, entry.Category, entry.Message };

                    // Meta data
                    string meta = GetMeta(entry);
                    if (meta != null)
                        entryData.Add(meta);

                    yield return entryData.ToArray();
                }
            }
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

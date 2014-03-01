using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glimpse.Core.Extensibility;
using System.Web;
using System.Diagnostics;
using System.Security;
using System.Collections;
using Glimpse.Core.Tab.Assist;

namespace NfxLab.GlimpsePlugins.WindowsEventLog
{
    public class WindowsEventLogPlugin : TabBase, ITab
    {
        /// <summary>
        /// The maximum number of log entries to fetch.
        /// </summary>
        const int MaxEntries = 50;

        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        public override string Name
        {
            get { return "Windows Event Logs"; }
        }

        /// <summary>
        /// Initializes the plugin.
        /// </summary>
        public void SetupInit()
        {
        }

        /// <summary>
        /// Gets the plugin data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override object GetData(ITabContext context)
        {
            var plugin = Plugin.Create("Log", "Entries");

            foreach (var log in EventLog.GetEventLogs())
            {
                if (log.Entries.Count > 0)
                {
                    TabSection section = CreateEventLogSection(log);

                    plugin.AddRow()
                          .Column(log.LogDisplayName)
                          .Column(section);
                }
            }
            return plugin;
        }

        /// <summary>
        /// Creates an TabSection from an event log.
        /// </summary>
        /// <param name="log">An EventLog.</param>
        /// <returns></returns>
        private static TabSection CreateEventLogSection(EventLog log)
        {
            TabSection section;
            section = new TabSection("Level", "Date", "Source", "Event ID", "Category", "Message");

            int lastEntryIndex = log.Entries.Count > MaxEntries ? log.Entries.Count - MaxEntries : 0;

            for (int i = log.Entries.Count - 1; i >= lastEntryIndex; i--)
            {
                var entry = log.Entries[i];

                var row = section.AddRow()
                                 .Column(entry.EntryType.ToString())
                                 .Column(entry.TimeGenerated.ToString("G"))
                                 .Column(entry.Source)
                                 .Column(entry.InstanceId)
                                 .Column(entry.Category != "(0)" ? entry.Category : string.Empty)
                                 .Column(entry.Message);

                SetIndicator(row, entry.EntryType);
            }

            return section;
        }

        /// <summary>
        /// Sets the indicator for a row, depending of the entry type;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="entryType"></param>
        private static void SetIndicator(TabSectionRow row, EventLogEntryType entryType)
        {
            switch (entryType)
            {
                case EventLogEntryType.Error:
                    row.Error();
                    break;
                case EventLogEntryType.FailureAudit:
                    row.Fail();
                    break;
                case EventLogEntryType.Information:
                    row.Info();
                    break;
                case EventLogEntryType.SuccessAudit:
                    row.Info();
                    break;
                case EventLogEntryType.Warning:
                    row.Warn();
                    break;
                default:
                    break;
            }
        }

    }
}

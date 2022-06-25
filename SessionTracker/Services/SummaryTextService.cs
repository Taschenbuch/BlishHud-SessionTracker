using System;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Models;

namespace SessionTracker.Services
{
    public class SummaryTextService
    {
        public string ResetAndReturnSummaryText(Entry entry)
        {
            entry.SessionHistory.Clear();
            InsertNewHistoryEntryAtBeginning(0, entry.SessionHistory);

            _startTime = DateTime.Now;
            return CreateSummaryText(entry, 0, new TimeSpan(0));
        }

        public string UpdateAndReturnSummaryText(Entry entry)
        {
            InsertNewHistoryEntryAtBeginning(entry.Value.Session, entry.SessionHistory);

            if (HistoryIsTooLong(entry.SessionHistory.Count))
                RemoveOldestHistoryEntry(entry.SessionHistory);

            var sessionDuration = DateTime.Now - _startTime;
            var sessionValuePerHour = sessionDuration.TotalHours == 0
                ? 0
                : entry.Value.Session / sessionDuration.TotalHours;

            return CreateSummaryText(entry, sessionValuePerHour, sessionDuration);
        }

        private string CreateSummaryText(Entry entry, double sessionValuePerHour, TimeSpan sessionDuration)
        {
            return $"== TOTAL ==\n" +
                   $"{entry.Value.Total} {entry.PlaceholderInTooltip}\n" +
                   $"\n== CURRENT SESSION ==\n" +
                   $"{sessionValuePerHour:0} {entry.PlaceholderInTooltip}/hour\n" +
                   $"{sessionDuration:hh':'mm} hour : minute\n\n" +
                   $"time | {entry.PlaceholderInTooltip}\n" +
                   $"{string.Join("\n", entry.SessionHistory)}";
        }

        private static void InsertNewHistoryEntryAtBeginning(int sessionValue, List<string> historyEntries)
        {
            historyEntries.Insert(0, $"{DateTime.Now:HH:mm} | {sessionValue}");
        }

        private static bool HistoryIsTooLong(int entriesCount)
        {
            return entriesCount > MAX_NUMBER_OF_ENTRIES;
        }

        private static void RemoveOldestHistoryEntry(List<string> historyEntries)
        {
            historyEntries.Remove(historyEntries.Last());
        }

        private DateTime _startTime;
        private const int MAX_NUMBER_OF_ENTRIES = 12;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime;
using System.Text.Json;
using System.Threading.Tasks;

namespace LaParola.Services
{
    public class AppMessage
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string TitleIt { get; set; } = string.Empty;
        public string ContentIt { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public static class MessageService
    {
        // Reuse HttpClient instance to prevent socket exhaustion
        private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(10) };
        private const string MessageUrl = "https://www.laparola.net/programma/messaggi.json";

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task CheckForNewMessagesAsync()
        {
            int CheckIntervalDays = MainWindow.settings.ControlloMessaggi;
            // 1. Check if the user opted out
            if (CheckIntervalDays == 0)
                return;

            // 2. Throttle checks (Max once a day/week/month)
            DateTime lastCheck = MainWindow.settings.UltimoControlloMessaggi;
            if ((DateTime.Now - lastCheck).TotalDays < CheckIntervalDays)
                return;

            // Track if this is the absolute first time the app is checking for messages
            // (Assuming default value for LastMessageCheck is 1/1/2000 or DateTime.MinValue)
            bool isFirstLaunch = (lastCheck < new DateTime(2001, 1, 1));

            // Update timestamp immediately
            MainWindow.settings.UltimoControlloMessaggi = DateTime.Now;
            App.Settings.Save(MainWindow.settings);

            try
            {
                // 3. Fetch the JSON file from server
                string jsonString = await _httpClient.GetStringAsync(MessageUrl);

                List<AppMessage>? allMessages = JsonSerializer.Deserialize<List<AppMessage>>(jsonString, jsonSerializerOptions);

                if (allMessages == null || allMessages.Count == 0)
                    return;

                int maxServerId = allMessages.Max(m => m.Id);

                // 4. Handle First-Time Users
                if (isFirstLaunch)
                {
                    // Mark everything currently on the server as "read" so they start clean
                    MainWindow.settings.UltimoMessaggioControllatoId = maxServerId;
                    App.Settings.Save(MainWindow.settings);

                    // OPTIONAL: If you want them to see just the ONE latest message on their first run, 
                    // uncomment the lines below:
                    /*
                    var latestMessage = allMessages.OrderByDescending(m => m.Id).First();
                    if ((DateTime.Now - latestMessage.Date).TotalDays <= 90) // Only if it's recent
                    {
                        DisplayMessages(new List<AppMessage> { latestMessage });
                    }
                    */
                    return;
                }

                // 5. Handle Regular Users with TTL Filtering (e.g., max 90 days old)
                int lastReadId = MainWindow.settings.UltimoMessaggioControllatoId;
                DateTime cutoffDate = DateTime.Now.AddDays(-90);

                List<AppMessage> newMessages = [.. allMessages
                    .Where(m => m.Id > lastReadId && m.Date > cutoffDate)
                    .OrderBy(m => m.Id)];

                if (newMessages.Count != 0)
                {
                    // 6. Process/Display the unread messages
                    DisplayMessages(newMessages);
                }

                // Always update the last read ID to the highest server ID 
                // to prevent old expired messages from being evaluated next time.
                MainWindow.settings.UltimoMessaggioControllatoId = maxServerId;
                App.Settings.Save(MainWindow.settings);
            }
            catch (Exception)
            {
                // Fail Silently
            }
        }
        private static void DisplayMessages(List<AppMessage> messages)
        {
            // Run on the UI thread to display your notification/window safely
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // Example: Open a custom announcement window, 
                // or load them into a notification area in your main window.
                foreach (var msg in messages)
                {
                    if (MainWindow.settings.Language == "it")
                    {
                        /*System.Windows.MessageBox.Show(
                            msg.ContentIt,
                            msg.TitleIt,
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Information);*/
                        MainWindow.CreaEditorDocument(msg.ContentIt, msg.TitleIt);
                    }
                    else
                    {
                        /*System.Windows.MessageBox.Show(
                        msg.Content,
                        msg.Title,
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);*/
                        MainWindow.CreaEditorDocument(msg.Content, msg.Title);
                    }
                }
            });
        }
    }
}
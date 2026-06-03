using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AvaloniaRentalApp.Services
{
    /// <summary>
    /// Persists application configuration back to the appsettings.json that the app loads at startup
    /// </summary>
    public static class AppConfigService
    {
        private static string ConfigPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        /// <summary>
        /// Updates ConnectionStrings:DefaultConnection in appsettings.json
        /// </summary>
        public static void SaveConnectionString(string connectionString)
        {
            var path = ConfigPath;

            JsonObject root;
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                root = JsonNode.Parse(json) as JsonObject ?? new JsonObject();
            }
            else
            {
                root = new JsonObject();
            }

            if (root["ConnectionStrings"] is not JsonObject connStrings)
            {
                connStrings = new JsonObject();
                root["ConnectionStrings"] = connStrings;
            }

            connStrings["DefaultConnection"] = connectionString;

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, root.ToJsonString(options));
        }
    }
}

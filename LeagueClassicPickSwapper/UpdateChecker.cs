using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LeagueClassicPickSwapper {
    public static class UpdateChecker {
        public static string LatestReleaseUrl { get; private set; } = "https://github.com/mayiflex/LeagueClassicPickSwapper/releases";
        private const string ReleaseApiUrl = "https://api.github.com/repos/mayiflex/LeagueClassicPickSwapper/releases/latest";

        public static async Task<(bool isUpdateAvailable, string latestVersionTag)> CheckForUpdatesAsync() {
            try {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("LeagueClassicPickSwapper");

                var response = await client.GetStringAsync(ReleaseApiUrl);
                var json = JObject.Parse(response);

                string? tagName = json["tag_name"]?.ToString();
                string? htmlUrl = json["html_url"]?.ToString() ?? "https://github.com/mayiflex/LeagueClassicPickSwapper/releases";

                if (string.IsNullOrWhiteSpace(tagName)) return (false, "");

                string versionString = tagName.TrimStart('v', 'V').Trim();
                if (Version.TryParse(versionString, out Version? latestVersion)) {
                    Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

                    // Normalize version comparison if revision is default (-1)
                    if (latestVersion > currentVersion) {
                        LatestReleaseUrl = htmlUrl;
                        return (true, tagName);
                    }
                }
            } catch {
                // Fail silently if offline, rate limited, or release doesn't exist yet
            }

            return (false, "");
        }
    }
}

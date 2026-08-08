using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using mayLCU;
using LeagueClassicPickSwapper.DTO_s;
using LeagueClassicPickSwapper.Enums;
using Newtonsoft.Json;

namespace LeagueClassicPickSwapper {
    internal static class LCU_Handler {
        private static LCU? lcu;
        public static string? CurrentSummoner { get; private set; }
        public static bool IsConnected => lcu != null && lcu.IsConnected;
        public static bool IsInChampSelect { get; private set; }
        public static Team Team { get; private set; }
        public static ChampSelect? champSelect { get; private set; }
        public static List<AvailableSwap>? AvailableSwaps { get; set; }
        public static nint GetHookedProcessMainWindowHandle => lcu != null && lcu.IsConnected ? lcu.GetHookedProcessMainWindowHandle : nint.Zero;

        public static void Innit() {
            var connectionAssurerTask = RunConnectionAssurerTask();
            var lobbyStatusTask = RunLobbyStatusTask();
            var availableSwapsTask = RunAvailableSwapsTask();
        }

        public static Task RunConnectionAssurerTask() => Task.Run(() => {
            while (true) {
                if (!IsConnected) {
                    lcu = LCU.HookLeagueClient();
                }
                Thread.Sleep(5000);
            }
        });

        public static Task RunLobbyStatusTask() => Task.Run(async () => {
            while (true) {
                if (IsConnected) {
                    try {
                        var response = await lcu.RequestAsync("/lol-champ-select/v1/session");
                        champSelect = JsonConvert.DeserializeObject<ChampSelect>(response);
                        if(champSelect?.actions == null) throw new Exception("Champ select actions is null");
                        Myteam? selfSummoner = JsonConvert.DeserializeObject<Myteam>(await lcu.RequestAsync("/lol-champ-select/v1/session/my-selection"));
                        if (selfSummoner?.gameName == null) throw new Exception("Self summoner is null");
                        CurrentSummoner = $"{selfSummoner.gameName}#{selfSummoner.tagLine}";
                        Team = (Team)selfSummoner.team;
                        IsInChampSelect = true;
                    } catch {
                        IsInChampSelect = false;
                    }
                }
                Thread.Sleep(5000);
            }
        });

        public static Task RunAvailableSwapsTask() => Task.Run(async () => {
            while (true) {
                if (IsConnected && IsInChampSelect) {
                    var response = await lcu.RequestAsync("/lol-champ-select/v1/session/pick-order-swaps");
                    try {
                        AvailableSwaps = JsonConvert.DeserializeObject<List<AvailableSwap>?>(response);
                    } catch {
                        AvailableSwaps = new List<AvailableSwap>();
                    }
                }
                Thread.Sleep(250);
            }
        });

        public static async Task<string> SendSwapRequest(int cellId) {
            if (lcu == null || !IsConnected) return "LCU not connected";
            if (!IsInChampSelect) return "Not in champ select";

            //Adjust for team side, as button-cellId is 0-4  for both teams, but the API uses 0-9
            cellId += ((int)Team - 1) * 5;

            AvailableSwap? swapTarget = AvailableSwaps?.Where(x => x.cellId == cellId).FirstOrDefault();
            if (swapTarget?.state != "AVAILABLE") return "Target not available for swapping";

            string responseSwapRequest = await lcu.RequestAsync(RequestMethod.POST, $"/lol-champ-select/v1/session/pick-order-swaps/{swapTarget.id}/request");
            return responseSwapRequest.Contains("SENT") ? "Swap request has been send" : "Failed to send swap request";
        }
    }
}

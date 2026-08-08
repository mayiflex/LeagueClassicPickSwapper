using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueClassicPickSwapper.DTO_s {
    public class ChampSelect {
        public Action[][] actions { get; set; }
        public bool allowBattleBoost { get; set; }
        public bool allowDuplicatePicks { get; set; }
        public bool allowLockedEvents { get; set; }
        public bool allowPlayerPickSameChampion { get; set; }
        public bool allowRerolling { get; set; }
        public bool allowSkinSelection { get; set; }
        public bool allowSubsetChampionPicks { get; set; }
        public Bans bans { get; set; }
        public object[] benchChampions { get; set; }
        public bool benchEnabled { get; set; }
        public int boostableSkinCount { get; set; }
        public Chatdetails chatDetails { get; set; }
        public int counter { get; set; }
        public bool disallowBanningTeammateHoveredChampions { get; set; }
        public long gameId { get; set; }
        public bool hasSimultaneousBans { get; set; }
        public bool hasSimultaneousPicks { get; set; }
        public string id { get; set; }
        public bool isCustomGame { get; set; }
        public bool isLegacyChampSelect { get; set; }
        public bool isSpectating { get; set; }
        public int localPlayerCellId { get; set; }
        public int lockedEventIndex { get; set; }
        public Myteam[] myTeam { get; set; }
        public Pickorderswap[] pickOrderSwaps { get; set; }
        public Positionswap[] positionSwaps { get; set; }
        public int queueId { get; set; }
        public int rerollsRemaining { get; set; }
        public bool showQuitButton { get; set; }
        public bool skipChampionSelect { get; set; }
        public Theirteam[] theirTeam { get; set; }
        public Timer timer { get; set; }
        public object[] trades { get; set; }
    }

    public class Bans {
        public object[] myTeamBans { get; set; }
        public int numBans { get; set; }
        public object[] theirTeamBans { get; set; }
    }

    public class Chatdetails {
        public Mucjwtdto mucJwtDto { get; set; }
        public string multiUserChatId { get; set; }
        public string multiUserChatPassword { get; set; }
    }

    public class Mucjwtdto {
        public string channelClaim { get; set; }
        public string domain { get; set; }
        public string jwt { get; set; }
        public string targetRegion { get; set; }
    }

    public class Timer {
        public int adjustedTimeLeftInPhase { get; set; }
        public long internalNowInEpochMs { get; set; }
        public bool isInfinite { get; set; }
        public string phase { get; set; }
        public int totalTimeInPhase { get; set; }
    }

    public class Action {
        public int actorCellId { get; set; }
        public int championId { get; set; }
        public bool completed { get; set; }
        public int duration { get; set; }
        public int id { get; set; }
        public bool isAllyAction { get; set; }
        public bool isInProgress { get; set; }
        public int pickTurn { get; set; }
        public string type { get; set; }
    }

    public class Myteam {
        public string assignedPosition { get; set; }
        public int cellId { get; set; }
        public int championId { get; set; }
        public int championPickIntent { get; set; }
        public string gameName { get; set; }
        public string internalName { get; set; }
        public bool isAutofilled { get; set; }
        public bool isHumanoid { get; set; }
        public string nameVisibilityType { get; set; }
        public string obfuscatedPuuid { get; set; }
        public long obfuscatedSummonerId { get; set; }
        public int pickMode { get; set; }
        public int pickTurn { get; set; }
        public string playerAlias { get; set; }
        public string playerType { get; set; }
        public string puuid { get; set; }
        public int selectedSkinId { get; set; }
        public int spell1Id { get; set; }
        public int spell2Id { get; set; }
        public long summonerId { get; set; }
        public string tagLine { get; set; }
        public int team { get; set; }
        public int wardSkinId { get; set; }
    }

    public class Pickorderswap {
        public int cellId { get; set; }
        public int id { get; set; }
        public string state { get; set; }
    }

    public class Positionswap {
        public int cellId { get; set; }
        public int id { get; set; }
        public string state { get; set; }
    }

    public class Theirteam {
        public string assignedPosition { get; set; }
        public int cellId { get; set; }
        public int championId { get; set; }
        public int championPickIntent { get; set; }
        public string gameName { get; set; }
        public string internalName { get; set; }
        public bool isAutofilled { get; set; }
        public bool isHumanoid { get; set; }
        public string nameVisibilityType { get; set; }
        public string obfuscatedPuuid { get; set; }
        public int obfuscatedSummonerId { get; set; }
        public int pickMode { get; set; }
        public int pickTurn { get; set; }
        public string playerAlias { get; set; }
        public string playerType { get; set; }
        public string puuid { get; set; }
        public int selectedSkinId { get; set; }
        public int spell1Id { get; set; }
        public int spell2Id { get; set; }
        public int summonerId { get; set; }
        public string tagLine { get; set; }
        public int team { get; set; }
        public int wardSkinId { get; set; }
    }

}

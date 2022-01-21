using System.ComponentModel;

namespace RR.Core
{
     public static class Enums
     {
          public enum ContestType
          {
               BigTeamContest = 1,
               [Description("3x3 Contest")]
               Contest = 2
          }

          public enum TransactionType
          {
               Token = 1,
               Paypal = 2
          }

          public enum CowBoyCoins
        {
               [Description("100 CowBoy Coins for $10")] First = 1,
               [Description("1000 CowBoy Coins for $85")] Second = 2,
               [Description("5000 CowBoy Coins for $350")] Third = 3
          }

          public enum PlayerType
          {
               [Description("INTERMEDIATE PLAYER")]
               NOVICETEAMMANAGER = 1,
               [Description("PRO PLAYER")]
               PROTEAMMANAGER = 2,
               [Description("NOVICE PLAYER")]
               TEAMMANAGER = 3
          }

          public enum LogType
          {
               Exception = 1,
               Success = 2
          }

        public enum ApiEnum
        {
            GetBullsRecord = 1,
            GetRidersRecord = 2,
            GetPastEventRecord = 3,
            GetFutureEventRecord = 4,
            GetCurrentEventRecord = 5,
            VelocityLevelEvents = 6,
        }

      
    }
}
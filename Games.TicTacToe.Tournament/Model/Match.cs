using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    public class Match
    {
        [JsonIgnore]
        public AbstractPlayer Player1 { get; set; }

        public string Player1Name
        {
            get { return Player1?.Name; }
            private set { Player1Name = value; }
        }

        public int Player1Score { get; set; }

        [JsonIgnore]
        public AbstractPlayer Player2 { get; set; }

        public string Player2Name
        {
            get { return Player2?.Name; }

            private set { Player2Name = value; }
        }

        public int Player2Score { get; set; }

        public List<Tuple<int, int>> MoveHistory { get; set; }

        public string ReasonLosing { get; set; }

        public int Round { get; set; }

        [JsonIgnore]
        public AbstractPlayer WinningPlayer { get; set; }

        public string WinningPlayerName
        {
            get
            {
                if (WinningPlayer != null)
                    return WinningPlayer.Name;
                else
                    return string.Empty;
            }
            private set { WinningPlayerName = value; }
        }

        public Match(AbstractPlayer player1, AbstractPlayer player2, List<Tuple<int, int>> moveHistory, AbstractPlayer winner, string reasonLosing, int round)
        {
            Player1 = player1;
            Player2 = player2;
            MoveHistory = moveHistory;
            WinningPlayer = winner;
            ReasonLosing = reasonLosing;
            Round = round;
        }
    }
}

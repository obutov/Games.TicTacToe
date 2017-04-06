using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    public class Bracket
    {
        [JsonIgnore]
        public AbstractPlayer Player1 { get; set; }

        public string Player1Name
        {
            get { return Player1?.Name; }

            private set { Player1Name = value; }
        }

        [JsonIgnore]
        public AbstractPlayer Player2 { get; set; }
        public string Player2Name
        {
            get { return Player2?.Name; }

            private set { Player2Name = value; }
        }
        public List<Match> Matches { get; set; }

        /// <summary>
        /// Gets winning player, 
        /// Whenever there's a tie both players are returned
        /// </summary>
        [JsonIgnore]
        public List<AbstractPlayer> WinningPlayers
        {
            get
            {
                List<AbstractPlayer> winners = new List<AbstractPlayer>();

                if (Player1.Score == Player2.Score)
                    winners.AddRange(new List<AbstractPlayer>() { Player1, Player2 });
                else if (Player1.Score > Player2.Score)
                    winners.Add(Player1);
                else
                    winners.Add(Player2);

                return winners;
            }
        }
    }
}

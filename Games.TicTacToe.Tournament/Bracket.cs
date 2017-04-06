using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    public class Bracket
    {
        private const int SCORE = 1;
        public AbstractPlayer Player1 { get; set; }
        public AbstractPlayer Player2 { get; set; }
        public List<Match> Matches { get; set; }

        /// <summary>
        /// Gets winning player, 
        /// Whenever there's a tie both players are returned
        /// </summary>
        public List<AbstractPlayer> WinningPlayers
        {
            get
            {
                List<AbstractPlayer> winners = new List<AbstractPlayer>();
                int player1Score = 0;
                int player2Score = 0;
                foreach (var match in Matches)
                {
                    if (match.WinningPlayer != null)
                    {
                        if (match.WinningPlayer.Equals(Player1))
                            player1Score += SCORE;
                        else if (match.WinningPlayer.Equals(Player2))
                            player2Score += SCORE;
                    }
                }

                if (player1Score == player2Score)
                    winners.AddRange(new List<AbstractPlayer>() { Player1, Player2 });
                else if (player1Score > player2Score)
                    winners.Add(Player1);
                else if (player2Score > player1Score)
                    winners.Add(Player2);

                return winners;
            }
        }
    }
}

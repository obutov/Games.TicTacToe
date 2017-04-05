using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    public class Match
    {
        public string Player1Name { get; set; }

        public string Player2Name { get; set; }

        public List<Tuple<int, int>> MoveHistory { get; set; }

        public int Player1Score { get; set; }

        public int Player2Score { get; set; }

        public int Round { get; set; }
    }
}

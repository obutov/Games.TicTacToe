using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    public class Round
    {
        public int RoundNumber { get; set; }
        public List<Bracket> Brackets { get; set; }

        public Round(int roundNumber)
        {
            RoundNumber = roundNumber;
            Brackets = new List<Bracket>();
        }
    }
}

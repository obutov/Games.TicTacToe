using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    public class TournamentResults
    {
        public TournamentResults()
        {
            Matches = new List<Match>();
        }

        public List<Match> Matches { get; set; }

        public int NumberOfRounds { get; set; }

        public int BoardSize { get; set; }
    }
}

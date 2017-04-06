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
            Rounds = new List<Round>();
        }

        public List<Round> Rounds { get; set; }

        public int NumberOfRounds { get { return Rounds.Count; } }

        public int BoardSize { get; set; }
    }
}

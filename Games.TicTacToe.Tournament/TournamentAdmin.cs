using Games.TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    public class TournamentAdmin
    {
        public TournamentResults Results { get; set; }
        public List<Round> Rounds { get; set; }

        public TournamentAdmin(string participantsFile)
        {

        }

        public bool IsFinished
        {
            get
            {
                return false; 
            }
        }

        public void CreateRound(int roundNumber)
        {

        }

        public void PlayRound()
        {

        }

        public void SaveResults(string fileName)
        {

        }

        private void LoadParticipants()
        {

        }
    }

    public class Bracket
    {
        public AbstractPlayer Player1 { get; set; }
        public AbstractPlayer Player2 { get; set; }
        public List<Match> Matches { get; set; }
    }

    public class Round
    {
        public int RoundNumber { get; set; }
        public List<Bracket> Brackets { get; set; }
    }
}

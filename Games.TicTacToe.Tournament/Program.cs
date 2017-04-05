using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    class Program
    {
        static void Main(string[] args)
        {
            TournamentAdmin tournament = new TournamentAdmin("Participants.xml");

            int round = 0;

            while (!tournament.IsFinished)
            {
                tournament.CreateRound(round);
                tournament.PlayRound();
                round++;
            }

            tournament.SaveResults("TournamentResults.json");
        }
    }
}

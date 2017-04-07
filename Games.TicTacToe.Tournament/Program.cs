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
            try
            {
                int boardSize = 5;
                Console.WriteLine("Welcome to Tic Tac Toe Tournament! \n");
                TournamentAdmin tournament = new TournamentAdmin("Participants.xml", boardSize);

                // set this to false to exclude default players from advancing
                // default players are added to brackets that cannot be filled with non-default players
                tournament.AdvanceDefaultPlayers = true;

                while (!tournament.IsFinished)
                {
                    tournament.CreateNextRound();
                    tournament.PlayRound();
                }

                var winningPlayers = tournament.Rounds.Last().Brackets.Single().WinningPlayers;
                if (winningPlayers.Count == 1)
                    Console.WriteLine($"\nTournament was completed. {winningPlayers.Single().Name} was a winner. \n");
                else if (winningPlayers.Count == 2)
                    Console.WriteLine($"\nTournament was completed. There was a tie between {winningPlayers.First().Name} and {winningPlayers.Last().Name}. \n");
                else
                    Console.WriteLine("\nSomething odd happened, as there were more than 2 winners. Results are being annulled. Please investigate!");


                Console.WriteLine("Saving results. \n");

                tournament.SaveResults($"TournamentResults-{DateTime.UtcNow.ToFileTimeUtc()}.json");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nNo can do. Try again! \n[{ex.Message}]");

            }
            Console.WriteLine("\n\nPress ENTER to exit.");
            Console.ReadLine();
        }
    }
}

using Games.TicTacToe.Tournament.Players;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Games.TicTacToe.Tournament
{
    public class TournamentAdmin
    {
        private const int NUM_MATCHES = 4;
        private const int TIMEOUT_MULTIPLIER = 30;

        private Participants _participants;
        private int _boardSize;

        public TournamentResults Results { get; set; }
        public List<Round> Rounds { get; set; }
        public int CurrentRound { get; set; }

        public TournamentAdmin(string participantsFile, int boardSize)
        {
            if (string.IsNullOrEmpty(participantsFile) || !File.Exists(participantsFile))
                throw new Exception("Invalid filename or file doesn't exist.");

            LoadParticipants(participantsFile);

            CurrentRound = 0;

            Rounds = new List<Round>();
            Results = new TournamentResults();
            Results.BoardSize = boardSize;

            _boardSize = boardSize;
        }

        public bool IsFinished
        {
            get
            {
                var roundsLeft = Rounds.Where(r => r.RoundNumber == CurrentRound - 1).SingleOrDefault()?.Brackets.Count - 1;
                return roundsLeft == 0;
            }
        }

        public void CreateNextRound()
        {
            // create round from a list of participants
            if (CurrentRound == 0)
            {
                Round round = Create0Round();
                Rounds.Add(round);
            }
            else
            // otherwise create round from a list of winners of the previous round
            {
                Round round = CreateRoundFromPreviousRound();
                Rounds.Add(round);
            }
        }

        public void PlayRound()
        {
            Console.WriteLine($"\nPlaying round {CurrentRound}:");
            var brackets = Rounds.Where(r => r.RoundNumber == CurrentRound).SelectMany(s => s.Brackets);

            int i = 0;
            foreach (var bracket in brackets)
            {
                Console.WriteLine($"\nPlaying bracket {i}:");
                PlayMatches(bracket, CurrentRound);

                Results.Matches.AddRange(bracket.Matches);

                i++;
            }
            CurrentRound++;
        }

        public void SaveResults(string fileName)
        {
            using (StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, Results);
            }
            Console.WriteLine($"Results saved to {fileName}");
        }

        private void LoadParticipants(string participantsFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Participants));
            using (StreamReader reader = new StreamReader(participantsFile))
            {
                _participants = (Participants)serializer.Deserialize(reader);
            }
        }

        private object GetInstance(string className, string param)
        {
            Type type = Type.GetType($"Games.TicTacToe.Tournament.Players.{className}");
            if (type != null)
                return Activator.CreateInstance(type, param);
            return null;
        }

        private void PlayMatches(Bracket bracket, int round)
        {
            int matchNumber = 0;
            while (matchNumber < NUM_MATCHES)
            {
                Console.WriteLine($"\nPlaying match {matchNumber}:");
                if (matchNumber % 2 == 0)
                    bracket.Matches.Add(PlayMatch(bracket.Player1, bracket.Player2, round));
                else
                    bracket.Matches.Add(PlayMatch(bracket.Player2, bracket.Player1, round));
                matchNumber++;
            }
        }

        private Match PlayMatch(AbstractPlayer player1, AbstractPlayer player2, int round)
        {
            Console.WriteLine($"{ player1.Name} against { player2.Name}");
            Board game = new Board(player1, player2, _boardSize, _boardSize, TIMEOUT_MULTIPLIER * _boardSize);

            var result = game.Play();

            Match match = new Match(player1, player2, result.MoveHistory, result.Player, result.Message, round);
            return match;
        }

        private Round CreateRoundFromPreviousRound()
        {
            // get previous round brackets
            var previousRoundBrackets = Rounds.Where(r => r.RoundNumber == CurrentRound - 1).Single().Brackets;

            // get winning players
            var winningPlayers = previousRoundBrackets.SelectMany(w => w.WinningPlayers).ToList();

            if (winningPlayers.Count == 0)
                throw new Exception("There should be at least one participant.");

            // add RandomPlayer if the list cannot be split evenly
            if (winningPlayers.Count % 2 != 0)
                winningPlayers.Add(new RandomPlayer() { Name = $"RandomPlayer{Guid.NewGuid()}" });

            Round round = new Round(CurrentRound);

            var e = winningPlayers.GetEnumerator();

            while (e.MoveNext())
            {
                var bracket = new Bracket();
                bracket.Player1 = e.Current;

                e.MoveNext();

                bracket.Player2 = e.Current;

                bracket.Matches = new List<Match>();
                round.Brackets.Add(bracket);
            }

            return round;
        }

        private Round Create0Round()
        {
            if (_participants.List.Length == 0)
                throw new Exception("There should be at least one participant.");

            var participantsList = _participants.List.ToList();

            // add RandomPlayer if the list cannot be split evenly
            if (participantsList.Count % 2 != 0)
                participantsList.Add(new Participant { Name = $"RandomPlayer{Guid.NewGuid()}", ClassName = "RandomPlayer" });

            // randomize players in the list
            var rnd = new Random();
            participantsList = participantsList.OrderBy(item => rnd.Next()).ToList();

            Round round = new Round(CurrentRound);

            var e = participantsList.GetEnumerator();

            while (e.MoveNext())
            {
                var bracket = new Bracket();
                bracket.Player1 = (AbstractPlayer)GetInstance(e.Current.ClassName, e.Current.Name);

                e.MoveNext();

                bracket.Player2 = (AbstractPlayer)GetInstance(e.Current.ClassName, e.Current.Name);

                bracket.Matches = new List<Match>();
                round.Brackets.Add(bracket);
            }

            return round;
        }
    }
}

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
        private const int SCORE = 1;
        private const int NUM_MATCHES = 4;
        private const int TIMEOUT_MULTIPLIER = 30;

        private Participants _participants;
        private int _boardSize;

        public TournamentResults Results { get; set; }
        public List<Round> Rounds { get; set; }
        public int CurrentRound { get; set; }
        public int ParticipantsCount
        {
            get
            {
                if (_participants != null)
                    return _participants.List.Count();
                return 0;
            }
        }

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

        public bool AdvanceDefaultPlayers { get; set; }

        /// <summary>
        /// Gets wheither the tournament is finished
        /// </summary>
        public bool IsFinished
        {
            get
            {
                var roundsLeft = Rounds.Where(r => r.RoundNumber == CurrentRound - 1).SingleOrDefault()?.Brackets.Count - 1;
                return roundsLeft == 0;
            }
        }

        /// <summary>
        /// Create next round
        /// </summary>
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

        /// <summary>
        /// Play a round
        /// </summary>
        public void PlayRound()
        {
            Console.WriteLine($"\nPlaying round {CurrentRound}:");
            var round = Rounds.Where(r => r.RoundNumber == CurrentRound).SingleOrDefault();
            var brackets = round.Brackets;

            int i = 0;
            foreach (var bracket in brackets)
            {
                // save current score for each player prior to running matches
                // this will be used in play visialization
                bracket.Player1Score = bracket.Player1.Score;
                bracket.Player2Score = bracket.Player2.Score;

                Console.WriteLine($"\nPlaying bracket {i}:");
                PlayMatches(bracket, CurrentRound);

                i++;
            }
            Results.Rounds.Add(round);
            CurrentRound++;
        }

        /// <summary>
        /// Save results into json file
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveResults(string fileName)
        {
            using (StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, Results);
            }
            Console.WriteLine($"Results saved to {fileName}");
        }

        #region Private Methods

        /// <summary>
        /// Plays all matches in the bracket
        /// </summary>
        /// <param name="bracket">Current bracket</param>
        /// <param name="round">Current round</param>
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

        /// <summary>
        /// Playes a match in the round
        /// </summary>
        /// <param name="player1">Player1 (X) object</param>
        /// <param name="player2">Player2 (O) object</param>
        /// <param name="round">Round number</param>
        /// <returns>Match object with game results</returns>
        private Match PlayMatch(AbstractPlayer player1, AbstractPlayer player2, int round)
        {
            Console.WriteLine($"{ player1.Name} against { player2.Name}");
            Board game = new Board(player1, player2, _boardSize, _boardSize, TIMEOUT_MULTIPLIER * _boardSize);

            var result = game.Play();

            if (result.Player != null)
                result.Player.Score += SCORE;

            Match match = new Match(player1, player2, result.MoveHistory, result.Player, result.Message, round);
            match.Player1Score = player1.Score;
            match.Player2Score = player2.Score;
            return match;
        }

        /// <summary>
        /// Creates 0 round from a list of participants
        /// Method will use participants' ClassName (stored in XML) to load AbstractPlayer object
        /// All players' classes should be stored in Players folder and be under Games.TicTacToe.Tournament.Players namespace
        /// </summary>
        /// <returns>Round object</returns>
        private Round Create0Round()
        {
            if (_participants.List.Length == 0)
                throw new Exception("There should be at least one participant.");

            var participantsList = _participants.List.ToList();

            // add RandomPlayer if the list cannot be split evenly
            if (participantsList.Count % 2 != 0)
                participantsList.Add(new Participant { Name = $"Player{Guid.NewGuid().ToString().Substring(0, 2)}", ClassName = "RandomPlayer" });

            // shuffle players in the list
            var rnd = new Random();
            participantsList = participantsList.OrderBy(item => rnd.Next()).ToList();

            Round round = new Round(CurrentRound);

            var e = participantsList.GetEnumerator();

            while (e.MoveNext())
            {
                var bracket = new Bracket();
                // load player object based on the class name
                bracket.Player1 = (AbstractPlayer)GetInstance(e.Current.ClassName, e.Current.Name);

                e.MoveNext();

                bracket.Player2 = (AbstractPlayer)GetInstance(e.Current.ClassName, e.Current.Name);

                bracket.Matches = new List<Match>();
                round.Brackets.Add(bracket);
            }

            return round;
        }

        /// <summary>
        /// Creates a round from the winners of the previous round
        /// If there was a tie, both players would advance to the next round
        /// </summary>
        /// <returns>Round object</returns>
        private Round CreateRoundFromPreviousRound()
        {
            // get previous round brackets
            var previousRoundBrackets = Rounds.Where(r => r.RoundNumber == CurrentRound - 1).Single().Brackets;

            // get winning players, do not advance default players e.g. RandomPlayer
            var winningPlayers = previousRoundBrackets.SelectMany(w => w.WinningPlayers).Where(w => (!AdvanceDefaultPlayers && !w.IsDefault) || AdvanceDefaultPlayers).ToList();

            if (winningPlayers.Count == 0)
                throw new Exception("There should be at least one participant.");

            // add RandomPlayer if the list cannot be split evenly
            if (winningPlayers.Count % 2 != 0)
                winningPlayers.Add(new RandomPlayer() { Name = $"Player{Guid.NewGuid().ToString().Substring(0, 2)}" });

            // order players by score to avoid pairing lower score players with players who have high score
            winningPlayers = winningPlayers.OrderBy(item => item.Score).ToList();

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

        /// <summary>
        /// Loads participants from XML file
        /// </summary>
        /// <param name="participantsFile">XML filename</param>
        private void LoadParticipants(string participantsFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Participants));
            using (StreamReader reader = new StreamReader(participantsFile))
            {
                _participants = (Participants)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Loads Player object
        /// </summary>
        /// <param name="className">Player Class name, must be a child of AbstractPlayer</param>
        /// <param name="param">Constructor parameter, player name in this case</param>
        /// <returns>Player object</returns>
        private object GetInstance(string className, string param)
        {
            Type type = Type.GetType($"Games.TicTacToe.Tournament.Players.{className}");
            if (type != null)
                return Activator.CreateInstance(type, param);
            return null;
        }

        #endregion
    }
}

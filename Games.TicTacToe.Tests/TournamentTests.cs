using Games.TicTacToe.Tournament;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Games.TicTacToe.Tests
{
    public class TournamentTests
    {
        [Fact]
        public void TestConstructor_Success()
        {
            int boardSize = 3;
            TournamentAdmin admin = new TournamentAdmin("TestFiles\\Participants.xml", boardSize);

            Assert.Equal(boardSize, admin.Results.BoardSize);
            Assert.Equal(0, admin.CurrentRound);
            Assert.Equal(0, admin.Rounds.Count);
            Assert.False(admin.IsFinished);
            Assert.Equal(3, admin.ParticipantsCount);
        }

        [Fact]
        public void TestConstructor_FileNotFound()
        {
            int boardSize = 3;

            Assert.Throws<Exception>(() =>
            {
                TournamentAdmin admin = new TournamentAdmin("TestFiles\\Participants2.xml", boardSize);
            });
        }

        [Fact]
        public void TestIsFinished_ReturnsFalse()
        {
            int boardSize = 3;
            TournamentAdmin admin = new TournamentAdmin("TestFiles\\Participants.xml", boardSize);

            Assert.False(admin.IsFinished);
        }

        [Fact]
        public void TestIsFinished_ReturnsTrue()
        {
            int boardSize = 3;
            TournamentAdmin admin = new TournamentAdmin("TestFiles\\TwoParticipants.xml", boardSize);

            admin.CreateNextRound();
            admin.PlayRound();

            Assert.True(admin.IsFinished);
        }

        [Fact]
        public void TestCreateNextRound_ShouldCreateTwoBrackets()
        {
            int boardSize = 3;
            TournamentAdmin admin = new TournamentAdmin("TestFiles\\Participants.xml", boardSize);
            admin.AdvanceDefaultPlayers = true;
            admin.CreateNextRound();

            var brackets = admin.Rounds.SelectMany(r => r.Brackets);

            Assert.Equal(2, brackets.Count());
        }

        [Fact]
        public void TestCreateNextRound_FromPreviousRound()
        {
            int boardSize = 3;
            TournamentAdmin admin = new TournamentAdmin("TestFiles\\Participants.xml", boardSize);
            admin.AdvanceDefaultPlayers = true;
            admin.CreateNextRound();
            admin.PlayRound();
            admin.CreateNextRound();

            var brackets = admin.Rounds.Where(r=>r.RoundNumber == admin.CurrentRound).SelectMany(r => r.Brackets);

            // since matches are non-deterministic we cannot predict how many brackets there will be after 0 round
            // if there's a tie in at least one bracket in round 0 - there will be 2 brackets in round 1 as well
            // if there were 2 winners in round 0, there will be only 1 bracket
            Assert.InRange(brackets.Count(), 1, 2);
        }

        [Fact]
        public void TestPlayRound()
        {
            int boardSize = 3;
            TournamentAdmin admin = new TournamentAdmin("TestFiles\\Participants.xml", boardSize);

            admin.CreateNextRound();
            admin.PlayRound();

            Assert.Equal(1, admin.CurrentRound);
        }
    }
}

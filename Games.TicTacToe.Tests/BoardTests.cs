using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Games.TicTacToe.Board;

namespace Games.TicTacToe.Tests
{
    public class BoardTests
    {
        [Fact]
        public void TestBoardInitialization()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            Assert.Equal(player1, game.ActivePlayer);
            Assert.Equal(player2, game.InactivePlayer);
            Assert.Equal(width, game.Width);
            Assert.Equal(height, game.Height);
            Assert.Equal(0, game.MoveCount);
        }

        [Fact]
        public void TestBoardInitialization_NonSquareBoard()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 4;            

            Assert.Throws<Exception>(() =>
            {
                var game = new Board(player1, player2, width, height);
            });
        }

        [Fact]
        public void TestBoardInitialization_SamePlayer()
        {
            var player1 = new TestPlayer("TestPlayer1");

            var width = 3;
            var height = 3;

            Assert.Throws<Exception>(() =>
            {
                var game = new Board(player1, player1, width, height);
            });
        }

        [Fact]
        public void TestGetOpponent()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            Assert.Equal(player2, game.GetOpponent(player1));
            Assert.Equal(player1, game.GetOpponent(player2));
        }

        [Fact]
        public void TestForecastMove()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            var newGame = game.ForecastMove(Tuple.Create(1, 1));

            Assert.Equal(player2, newGame.ActivePlayer);
            Assert.Equal(player1, newGame.InactivePlayer);
            Assert.Equal(width, newGame.Width);
            Assert.Equal(height, newGame.Height);
            Assert.Equal(1, newGame.MoveCount);
            Assert.Equal(false, newGame.MoveIsLegal(Tuple.Create(1, 1)));
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(1, 2, true)]
        [InlineData(2, 0, true)]
        [InlineData(1, 1, false)]
        [InlineData(-1, -1, false)]
        [InlineData(3, 3, false)]
        [InlineData(-1, 2, false)]
        public void TestMoveIsLegal(int row, int column, bool expected)
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(1, 1));

            Assert.Equal(expected, game.MoveIsLegal(Tuple.Create(row, column)));
        }

        [Theory]
        [InlineData(3, 3, 9)]
        [InlineData(5, 5, 25)]
        public void TestGetBlankSpacesDefault(int width, int height, int expected)
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var game = new Board(player1, player2, width, height);

            var blankSpaces = game.GetBlankSpaces();

            Assert.Equal(expected, blankSpaces.Count());
        }

        [Theory]
        [InlineData(3, 3, 8)]
        [InlineData(5, 5, 24)]
        public void TestGetBlankSpacesOneFilled(int width, int height, int expected)
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(0, 0));

            var blankSpaces = game.GetBlankSpaces();

            Assert.Equal(expected, blankSpaces.Count());
        }
        [Theory]
        [InlineData(1, 1, 1, 1, true)]
        [InlineData(0, 1, 0, 1, true)]
        [InlineData(2, 1, 0, 1, false)]
        [InlineData(3, 1, 0, 2, false)]
        public void TestGetPlayerLocation(int column, int row, int expectedColumn, int expectedRow, bool expected)
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(row, column));

            var loc = game.GetPlayerLocation(player1);

            Assert.Equal(expected, loc.Equals(Tuple.Create(expectedRow, expectedColumn)));
        }

        [Fact]
        public void TestGetLegalMoves()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(1, 1));

            var player1legalMoves = game.GetLegalMoves(player1);

            var expected = new List<Tuple<int, int>> {
                new Tuple<int, int>(0, 0), new Tuple<int, int>(0, 1), new Tuple<int, int>(0, 2),
                new Tuple<int, int>(1, 0), new Tuple<int, int>(1, 2),
                new Tuple<int, int>(2, 0), new Tuple<int, int>(2, 1), new Tuple<int, int>(2, 2)};

            Assert.Equal(player1, game.InactivePlayer);
            Assert.Equal(expected, player1legalMoves);
            Assert.Equal(1, game.MoveCount);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 0)]
        [InlineData(1, 2)]
        public void TestApplyMove(int column, int row)
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);
            game._ApplyMove(Tuple.Create(row, column));

            var loc = game.GetPlayerLocation(player1);

            Assert.Equal(Tuple.Create(row, column), loc);
            Assert.Equal(1, game.MoveCount);
            Assert.Equal(player2, game.ActivePlayer);
        }

        [Fact]
        public void TestToString()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            string expected = "      0   1   2\n[0] |   |   |   | \n[1] |   | X |   | \n[2] |   |   |   | \n";
            var game = new Board(player1, player2, width, height);
            game._ApplyMove(Tuple.Create(1, 1));

            var boardString = game.ToString();

            Assert.Equal(expected, boardString);

        }
        [Fact]
        public void TestBoardCopy()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(1, 1));

            var newGame = game._CopyBoard();

            Assert.Equal(game.GetLegalMoves(), newGame.GetLegalMoves());
            Assert.Equal(game.MoveCount, newGame.MoveCount);
            Assert.Equal(game.ActivePlayer, newGame.ActivePlayer);
            Assert.Equal(game.InactivePlayer, newGame.InactivePlayer);
            Assert.Equal(game.Width, newGame.Width);
            Assert.Equal(game.Height, newGame.Height);
        }

        [Fact]
        public void TestCheckPlayerState_Player1Wins()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(1, 1));
            game._ApplyMove(Tuple.Create(0, 0));
            game._ApplyMove(Tuple.Create(1, 0));
            game._ApplyMove(Tuple.Create(2, 0));
            game._ApplyMove(Tuple.Create(1, 2));

            var player1State = game._CheckPlayerState(player1);
            var player2State = game._CheckPlayerState(player2);

            Assert.Equal(_PlayerState.Win, player1State);
            Assert.Equal(_PlayerState.Lose, player2State);
        }

        [Fact]
        public void TestCheckPlayerState_Player2Wins()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(1, 1));
            game._ApplyMove(Tuple.Create(0, 0));
            game._ApplyMove(Tuple.Create(1, 0));
            game._ApplyMove(Tuple.Create(0, 1));
            game._ApplyMove(Tuple.Create(2, 1));
            game._ApplyMove(Tuple.Create(0, 2));

            var player1State = game._CheckPlayerState(player1);
            var player2State = game._CheckPlayerState(player2);

            Assert.Equal(_PlayerState.Win, player2State);
            Assert.Equal(_PlayerState.Lose, player1State);
        }

        [Fact]
        public void TestCheckPlayerState_Draw()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(1, 1));
            game._ApplyMove(Tuple.Create(0, 0));
            game._ApplyMove(Tuple.Create(1, 0));
            game._ApplyMove(Tuple.Create(0, 1));
            game._ApplyMove(Tuple.Create(2, 1));
            game._ApplyMove(Tuple.Create(1, 2));
            game._ApplyMove(Tuple.Create(0, 2));
            game._ApplyMove(Tuple.Create(2, 0));
            game._ApplyMove(Tuple.Create(2, 2));

            var player1State = game._CheckPlayerState(player1);
            var player2State = game._CheckPlayerState(player2);

            Assert.Equal(_PlayerState.Draw, player2State);
            Assert.Equal(_PlayerState.Draw, player1State);
        }

        [Fact]
        public void TestCheckPlayerState_InProgress()
        {
            var player1 = new TestPlayer("TestPlayer1");
            var player2 = new TestPlayer("TestPlayer2");

            var width = 3;
            var height = 3;

            var game = new Board(player1, player2, width, height);

            game._ApplyMove(Tuple.Create(1, 1));
            game._ApplyMove(Tuple.Create(0, 0));
            game._ApplyMove(Tuple.Create(1, 0));
            game._ApplyMove(Tuple.Create(0, 1));
            game._ApplyMove(Tuple.Create(2, 1));
            game._ApplyMove(Tuple.Create(1, 2));
            game._ApplyMove(Tuple.Create(0, 2));

            var player1State = game._CheckPlayerState(player1);
            var player2State = game._CheckPlayerState(player2);

            Assert.Equal(_PlayerState.InProgress, player2State);
            Assert.Equal(_PlayerState.InProgress, player1State);
        }

        [Fact]
        public void TestPlay_Player1Wins()
        {
            var player1 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer1");
            var player2 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer2");

            player1.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(1, 1))
                .Returns(Tuple.Create(1, 0))
                .Returns(Tuple.Create(1, 2));

            player2.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(0, 0))
                .Returns(Tuple.Create(2, 0));

            var width = 3;
            var height = 3;

            var game = new Board(player1.Object, player2.Object, width, height, 150000000);

            var result = game.Play();

            Assert.Equal(player1.Object, result.Player);
        }

        [Fact]
        public void TestPlay_Player2Wins()
        {
            var player1 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer1");
            var player2 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer2");

            player1.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(1, 1))
                .Returns(Tuple.Create(1, 0))
                .Returns(Tuple.Create(2, 1));

            player2.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(0, 0))
                .Returns(Tuple.Create(0, 1))
                .Returns(Tuple.Create(0, 2));

            var width = 3;
            var height = 3;

            var game = new Board(player1.Object, player2.Object, width, height, 150000000);

            var result = game.Play();

            Assert.Equal(player2.Object, result.Player);
        }

        [Fact]
        public void TestPlay_Draw()
        {
            var player1 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer1");
            var player2 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer2");

            player1.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(1, 1))
                .Returns(Tuple.Create(1, 0))
                .Returns(Tuple.Create(2, 1))
                .Returns(Tuple.Create(0, 2))
                .Returns(Tuple.Create(2, 2));

            player2.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(0, 0))
                .Returns(Tuple.Create(0, 1))
                .Returns(Tuple.Create(1, 2))
                .Returns(Tuple.Create(2, 0));

            var width = 3;
            var height = 3;

            var game = new Board(player1.Object, player2.Object, width, height, 150000000);

            var result = game.Play();

            Assert.Equal(null, result.Player);
            Assert.Equal("draw", result.Message);
        }

        [Fact]
        public void TestPlay_Player1Forfeit()
        {
            var player1 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer1");
            var player2 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer2");

            player1.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(1, 1))
                .Returns(Tuple.Create(-1, -1));

            player2.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(0, 0));

            var width = 3;
            var height = 3;

            var game = new Board(player1.Object, player2.Object, width, height, 150000000);

            var result = game.Play();

            Assert.Equal(player2.Object, result.Player);
            Assert.Equal("forfeit", result.Message);
        }

        [Fact]
        public void TestPlay_Player2IllegalMove()
        {
            var player1 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer1");
            var player2 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer2");

            player1.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(1, 1))
                .Returns(Tuple.Create(1, 0));

            player2.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(0, 0))
                .Returns(Tuple.Create(1, 0));

            var width = 3;
            var height = 3;

            var game = new Board(player1.Object, player2.Object, width, height, 150000000);

            var result = game.Play();

            Assert.Equal(player1.Object, result.Player);
            Assert.Equal("illegal move", result.Message);
        }

        [Fact]
        public void TestPlay_Player2TimesOut()
        {
            var player1 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer1");
            var player2 = new Mock<TestPlayer>(MockBehavior.Strict, "TestPlayer2");

            player1.SetupSequence(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(Tuple.Create(1, 1));

            player2.Setup(m => m.GetMove(It.IsAny<Board>(), It.IsAny<Func<int>>()))
                .Returns(()=>
                {
                    Thread.Sleep(1500);
                    return Tuple.Create(0, 0);
                });

            var width = 3;
            var height = 3;

            var game = new Board(player1.Object, player2.Object, width, height, 150);

            var result = game.Play();

            Assert.Equal(player1.Object, result.Player);
            Assert.Equal("timeout", result.Message);
        }

    }
}

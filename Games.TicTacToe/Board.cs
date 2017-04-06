using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Games.TicTacToe
{
    public class Board
    {
        private const int BLANK = 0;
        private const int NOT_MOVED = -1;        

        private AbstractPlayer _player1;
        private AbstractPlayer _player2;
        private AbstractPlayer _activePlayer;
        private AbstractPlayer _inactivePlayer;

        private int TIME_LIMIT_MS;
        private int _moveCount;
        private int[] _boardState;
        private int _width;
        private int _height;

        /// <summary>
        /// Game board constructor
        /// </summary>
        /// <param name="player1">An object with a get_move() function. This is the only function directly called by the Board class for each player.</param>
        /// <param name="player2">An object with a get_move() function. This is the only function directly called by the Board class for each player.</param>
        /// <param name="width">The number of columns that the board should have.</param>
        /// <param name="height">The number of rows that the board should have.</param>
        public Board(AbstractPlayer player1, AbstractPlayer player2, int width = 5, int height = 5, int timeLimitMs = 150)
        {
            if (width != height)
                throw new Exception("Non-square boards are not supported.");

            if (player1.Name.Equals(player2.Name))
                throw new Exception("Players should have different names.");

            _player1 = player1;
            _player2 = player2;
            _width = width;
            _height = height;
            _moveCount = 0;
            _activePlayer = player1;
            _inactivePlayer = player2;
            _boardState = Enumerable.Repeat(BLANK, _width * _height + 2).ToArray();
            _boardState[_boardState.Length - 1] = NOT_MOVED;
            _boardState[_boardState.Length - 2] = NOT_MOVED;
            TIME_LIMIT_MS = timeLimitMs;
        }

        /// <summary>
        /// Gets board hash value
        /// </summary>
        public int Hash {  get { return _boardState.GetHashCode(); } }

        /// <summary>
        /// Gets board Width
        /// </summary>
        public int Width { get { return _width; } }

        /// <summary>
        /// Gets board Height
        /// </summary>
        public int Height { get { return _height; } }

        /// <summary>
        /// Gets active player reference
        /// </summary>
        public AbstractPlayer ActivePlayer { get { return _activePlayer;  } }

        /// <summary>
        /// Gets inactive player reference
        /// </summary>
        public AbstractPlayer InactivePlayer { get { return _inactivePlayer; } }

        /// <summary>
        /// Gets a current move count
        /// </summary>
        public int MoveCount { get { return _moveCount; } }

        /// <summary>
        /// Get the opponent of the supplied player.
        /// </summary>
        /// <param name="player">Player object</param>
        /// <returns>Opponent player</returns>
        public AbstractPlayer GetOpponent(AbstractPlayer player)
        {
            if (player.Equals(_activePlayer))
                return _inactivePlayer;
            else if (player.Equals(_inactivePlayer))
                return _activePlayer;

            throw new Exception("Player must be associated with a current game board.");
        }

        /// <summary>
        /// Get a deep copy of the current game with an input move applied to advance the game
        /// NOTE: Active game state is not affected by this action
        /// </summary>
        /// <param name="move">Move coordinates</param>
        /// <returns>A deep copy of the current game with the move applied</returns>
        public Board ForecastMove(Tuple<int, int> move)
        {
            var newBoard = _CopyBoard();
            newBoard._ApplyMove(move);

            return newBoard;
        }

        /// <summary>
        /// Test whether a move is legal in the current game state.
        /// </summary>
        /// <param name="move">Move coordinates</param>
        /// <returns>Whither the move is legal</returns>
        public bool MoveIsLegal(Tuple<int, int> move)
        {
            int idx = move.Item1 + move.Item2 * _height;

            return (0 <= move.Item1 && move.Item1 < _height && 0 <= move.Item2 && move.Item2 < _width && _boardState[idx] == BLANK);
        }

        /// <summary>
        /// Get a list of the locations that are still available on the board.
        /// </summary>
        /// <returns>A list of blank spaces' coordinates</returns>
        public IEnumerable<Tuple<int, int>> GetBlankSpaces()
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                {
                    if (_boardState[i + j * _height] == BLANK)
                        yield return Tuple.Create(i, j);
                }
        }

        /// <summary>
        /// Find the current location of the specified player on the board.
        /// </summary>
        /// <param name="player">Player object</param>
        /// <returns>Player location coordinates</returns>
        public Tuple<int, int> GetPlayerLocation(AbstractPlayer player)
        {
            if (player == null)
                return null;

            int idx = 0;
            if (player.Equals(_player1))
            {
                if (_boardState[_boardState.Length - 1] == NOT_MOVED)                
                    return null;
                idx = _boardState[_boardState.Length - 1];
            }
            else if (player.Equals(_player2))
            {
                if (_boardState[_boardState.Length - 2] == NOT_MOVED)
                    return null;
                idx = _boardState[_boardState.Length - 2];
            }
            else
            {
                throw new Exception("Invalid player");
            }

            int w = idx / _height;
            int h = idx % _height;

            return Tuple.Create(h, w);
        }

        /// <summary>
        /// Get a list of all legal moves for the specified player.
        /// </summary>
        /// <param name="player">Player object</param>
        /// <returns>List of legal moves</returns>
        public IEnumerable<Tuple<int, int>> GetLegalMoves(AbstractPlayer player = null)
        {
            return _GetMoves(GetPlayerLocation(player));
        }

        /// <summary>
        /// Test whether the player has won the game
        /// </summary>
        /// <param name="player">Player to test</param>
        /// <returns>True if player is the winner</returns>
        public bool IsWinner(AbstractPlayer player)
        {
            return _CheckPlayerState(player).Equals(_PlayerState.Win);
        }

        /// <summary>
        /// Test whether the player has lost the game
        /// </summary>
        /// <param name="player">Player to test</param>
        /// <returns>True if player is the loser</returns>
        public bool IsLoser(AbstractPlayer player)
        {
            return _CheckPlayerState(player).Equals(_PlayerState.Lose);
        }

        /// <summary>
        /// Test wether the game is a Draw
        /// </summary>
        /// <param name="player">Player to test</param>
        /// <returns>True if there is a draw</returns>
        public bool IsDraw(AbstractPlayer player)
        {
            return _CheckPlayerState(player).Equals(_PlayerState.Draw);
        }

        /// <summary>
        /// Test wether the game is a Draw
        /// </summary>
        /// <param name="player">Player to test</param>
        /// <returns>True if there is a draw</returns>
        public bool IsInProgress(AbstractPlayer player)
        {
            return _CheckPlayerState(player).Equals(_PlayerState.InProgress);
        }


        /// <summary>
        /// Generate a string representation of the current game state
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int columnMargin = (_height - 1).ToString().Length + 1;
            string prefix = "[{0}]";
            string offset = "".PadLeft(columnMargin + 4, ' ');
            StringBuilder result = new StringBuilder();

            result.Append("\n" + offset + String.Join("   ", Enumerable.Range(0, _width)) + "\n");

            foreach (var i in Enumerable.Range(0, _height))
            {
                result.Append(string.Format(prefix, i) + " | ");
                foreach (var j in Enumerable.Range(0, _width))
                {
                    int idx = i + j * _height;
                    if (_boardState[idx] == BLANK)
                        result.Append(" ");
                    else if (_boardState[idx] == 1)
                        result.Append("X");
                    else if (_boardState[idx] == -1)
                        result.Append("O");
                    result.Append(" | ");
                }
                result.Append("\n");
            }
            return result.ToString();
        }

        /// <summary>
        /// Play a match between the players by soliciting them to select a move and applying in in the game
        /// </summary>
        /// <returns>Game Result object, where 
        /// Player is a winning player or null if match outcome is a draw,
        /// MoveHistory is a list of all moves applied in the match
        /// Message is string indicating the reason for losing</returns>
        public GameResult Play()
        {
            List<Tuple<int, int>> moveHistory = new List<Tuple<int, int>>();

            while (IsInProgress(_activePlayer))
            {
                var legalPlayerMoves = GetLegalMoves();

                // create a deep copy of the game board
                var gameCopy = _CopyBoard();

                // timer initializer
                DateTime moveStart = DateTime.UtcNow;

                // timer delegate (closure)
                Func<int> timer = delegate ()
                {
                    DateTime now = DateTime.UtcNow;
                    return TIME_LIMIT_MS - (int)(now - moveStart).TotalMilliseconds;
                };

                // solicit active player for the next move
                var currentMove = _activePlayer.GetMove(gameCopy, timer);

                // get timer value after player returns the move
                int moveEnd = timer();

                // return timeout
                if (moveEnd < 0)
                    return new GameResult(_inactivePlayer, moveHistory, "timeout");               

                // if current move is not included in legal moves list
                if (!legalPlayerMoves.Contains(currentMove))
                {
                    // moves left, return a forfeit
                    if (legalPlayerMoves.Count() > 0)
                        return new GameResult(_inactivePlayer, moveHistory, "forfeit");
                    // no moves left, return a draw
                    else if (GetBlankSpaces().Count() == 0)
                        break;
                }

                moveHistory.Add(currentMove);

                _ApplyMove(currentMove);
                //Debug.WriteLine(ToString());
                //Debug.WriteLine("\n");
            }

            if (IsDraw(_activePlayer))
                return new GameResult(null, moveHistory, "draw");
            else return new GameResult(IsWinner(_activePlayer) ? _activePlayer : _inactivePlayer, moveHistory, "won");
        }

        /// <summary>
        /// Move the active player to a specified location.
        /// </summary>
        /// <param name="move">Move coordinates to apply</param>
        public void _ApplyMove(Tuple<int, int> move)
        {
            int idx = move.Item1 + move.Item2 * _height;
            int lastMoveIdx = (_activePlayer == _player2 ? 1 : 0) + 1;
            _boardState[_boardState.Length - lastMoveIdx] = idx;
            _boardState[idx] = lastMoveIdx == 1 ? 1 : -1;
            var activePlayerRef = _activePlayer;
            _activePlayer = _inactivePlayer;
            _inactivePlayer = activePlayerRef;
            _moveCount += 1;
        }

        public _PlayerState _CheckPlayerState(AbstractPlayer player)
        {
            // get player multiplier value, player1 = 1 and player2 = -1
            int playerMultiplier = (player == _player2 ? 0 : 2) - 1;

            // get opponent player multiplier value, player1 = 1 and player2 = -1
            int opponentPlayerMultiplier = playerMultiplier == 1 ? -1 : 1;

            Dictionary<int, int> vSums = new Dictionary<int, int>();
            Dictionary<int, int> hSums = new Dictionary<int, int>();
            Dictionary<int, int> dSums = new Dictionary<int, int>();

            // get diagonal positions
            List<int> diagonalPositions1 = new List<int>().GetIntList(0, _width*_height, _width + 1);
            List<int> diagonalPositions2 = new List<int>().GetIntList(_width - 1, _width * _height - _width, _width - 1);

            // sum up player values horizontally, vertically, and diagonally
            for (int i = 0; i < _width*_height; i++)
            {
                // horizontal sums
                hSums.TryAdd((i % _width), _boardState[i]);

                // vertical sums
                vSums.TryAdd((i / _height), _boardState[i]);

                // diagonal sums
                if (diagonalPositions1.Contains(i))
                    dSums.TryAdd(0, _boardState[i]);

                if (diagonalPositions2.Contains(i))
                    dSums.TryAdd(1, _boardState[i]);
            }

            // if any of the sums add up to winning value return PlayerState
            // diagonals
            if (dSums[0] == playerMultiplier * _width || dSums[1] == playerMultiplier * _width)
                return _PlayerState.Win;
            else if (dSums[0] == opponentPlayerMultiplier * _width || dSums[1] == opponentPlayerMultiplier * _width)
                return _PlayerState.Lose;

            // verticals
            foreach (var key in vSums.Keys)
            {
                if (vSums[key] == playerMultiplier * _height)
                    return _PlayerState.Win;
                else if (vSums[key] == opponentPlayerMultiplier * _height)
                    return _PlayerState.Lose;
            }

            // horizontals
            foreach (var key in hSums.Keys)
            {
                if (hSums[key] == playerMultiplier * _width)
                    return _PlayerState.Win;
                else if (hSums[key] == opponentPlayerMultiplier * _width)
                    return _PlayerState.Lose;
            }

            // if not blank spaces left - its a draw
            if (GetBlankSpaces().Count() == 0)
                return _PlayerState.Draw;

            // in all other cases return InProgress state
            return _PlayerState.InProgress;
        }

        public Board _CopyBoard()
        {
            Board copy = new Board(this._player1, this._player2, this._width, this._height);
            copy._moveCount = this._moveCount;
            copy._activePlayer = this._activePlayer;
            copy._inactivePlayer = this._inactivePlayer;
            Array.Copy(this._boardState, copy._boardState, this._boardState.Length);

            return copy;
        }

        public IEnumerable<Tuple<int, int>> _GetMoves(Tuple<int, int> location = null)
        {
            if (location == null)
                return GetBlankSpaces();

            int r = location.Item1;
            int c = location.Item2;

            List<Tuple<int, int>> moves = new List<Tuple<int, int>>();
            // get  moves
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (x != c && y == r && MoveIsLegal(Tuple.Create(r, x)))
                         moves.Add(Tuple.Create(r, x));

                    if (y != r && x == c && MoveIsLegal(Tuple.Create(y, c)))
                        moves.Add(Tuple.Create(y, c));

                    if (x != c && y != r && Math.Abs(x-c) - Math.Abs(y-r) == 0 && MoveIsLegal(Tuple.Create(y, x)))
                        moves.Add(Tuple.Create(y, x));
                }
            }

            return moves;
        }

        public enum _PlayerState
        {
            Win, Lose, Draw, InProgress
        }
    }


}

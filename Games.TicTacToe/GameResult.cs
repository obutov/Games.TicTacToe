using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe
{
    public class GameResult
    {
        public GameResult(AbstractPlayer player, List<Tuple<int, int>> moveHistory, string message)
        {
            Player = player;
            MoveHistory = moveHistory;
            Message = message;
        }

        public AbstractPlayer Player { get; set; }

        public List<Tuple<int, int>> MoveHistory { get; set; }

        public string Message { get; set; }
    }

}

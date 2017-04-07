using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament.Players
{
    public class RandomPlayer : AbstractPlayer
    {
        public RandomPlayer(string name = "RandomPlayer") : base(name)
        {
        }

        public override bool IsDefault
        {
            get
            {
                return true;
            }
        }

        public override Tuple<int, int> GetMove(Board board, Func<int> timeLeft)
        {
            // Make sure you assign Timeout delegate first
            // Timeout is used to check how much time is left
            Timeout = timeLeft;
            Tuple<int, int> result = null;
            try
            {
                if (this.Timeout() < TIMER_THRESHOLD_MS) throw new TimeoutException();
                Random rnd = new Random(DateTime.UtcNow.GetHashCode());

                var legalMoves = board.GetLegalMoves();

                result = legalMoves.ElementAt(rnd.Next(0, legalMoves.Count()));

            }
            catch (TimeoutException)
            {
                // Handle any actions required at timeout, if necessary
            }

            return result;
        }
    }
}

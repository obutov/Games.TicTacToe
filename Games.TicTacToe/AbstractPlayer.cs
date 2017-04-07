using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe
{
    public abstract class AbstractPlayer : EqualityComparer<AbstractPlayer>
    {
        public const int TIMER_THRESHOLD_MS = 10;

        public Func<int> Timeout { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public virtual bool IsDefault
        {
            get
            {
                return false;
            }
        }

        public AbstractPlayer(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Search for the best move from the available legal moves and return a result before the time limit expires
        /// NOTE: if timeLeft is less than 0 when this function returns, the player will forfeit the game due to timeout
        /// You must return BEFORE the timer reaches 0
        /// Use TIMER_THRESHOLD_MS and Timeout to return before timer reaches 0
        /// TIMER_THRESHOLD_MS is set to 10 ms
        /// Usage Example:
        /// try
        /// {
        ///     if (this.Timeout() < TIMER_THRESHOLD_MS) throw new TimeoutException();
        /// }
        /// catch (TimeoutException)
        /// {
        ///  // Handle any actions required at timeout, if necessary
        /// }
        /// </summary>
        /// <param name="board"></param>
        /// <param name="timeLeft"></param>
        /// <returns></returns>
        public virtual Tuple<int, int> GetMove(Board board, Func<int> timeLeft)
        {
            // Make sure you assign Timeout delegate first
            // Timeout is used to check how much time is left
            this.Timeout = timeLeft;
            try
            {
                if (this.Timeout() < TIMER_THRESHOLD_MS) throw new TimeoutException();
            }
            catch (TimeoutException)
            {
                // Handle any actions required at timeout, if necessary
            }

            return Tuple.Create(-1, -1);
        }

        public override bool Equals(AbstractPlayer p1, AbstractPlayer p2)
        {
            if (p1.Name.Equals(p2.Name))
                return true;
            else
                return false;
        }

        public override int GetHashCode(AbstractPlayer p)
        {
            return p.Name.GetHashCode();
        }
    }
}

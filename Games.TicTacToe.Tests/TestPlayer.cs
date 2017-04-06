using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tests
{
    public class TestPlayer : AbstractPlayer
    {
        public TestPlayer(string name = "TestPlayer") : base(name)
        {
        }

        public override Tuple<int, int> GetMove(Board board, Func<int> timeLeft)
        {
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
    }
}

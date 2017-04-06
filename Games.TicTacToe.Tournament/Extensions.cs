using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe.Tournament
{
    public static class Extensions
    {
        public static T[] ResizeAndAdd<T>(this T[] array, T value)
        {
            var newList = array.ToList();
            newList.Add(value);
            return newList.ToArray();
        }
    }
}

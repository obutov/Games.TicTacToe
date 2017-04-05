using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe
{
    public static class Extensions
    {
        public static void TryAdd(this Dictionary<int, int> dict, int key, int value)
        {
            if (!dict.ContainsKey(key)) dict.Add(key, value);
            else dict[key] += value;
        }

        public static List<int> GetIntList(this List<int> list, int start, int end, int step)
        {
            for (int i = start; i <= end; i = i + step)
            {
                list.Add(i);
            }
            return list;
        }
    }
}

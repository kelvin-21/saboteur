using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saboteur.Models
{
    public static class ConfigModel
    {
        public const int minPlayers = 4;
        public const int maxPlayers = 8;
        public const int pathCardRatio = 70;  // from 0 to 99

        public const int numberOfHands = 5;

        public static Dictionary<int, int> actionCardProportion = new Dictionary<int, int>()
        {
            {50, 1}, {51, 1}, {52, 1}, {53, 1}
        };
        public static Dictionary<int, int> pathCardProportion = new Dictionary<int, int>()
        {
            {0, 1}, {1, 1}, {2, 1}, {3, 1}, {4, 1}, {5, 1}, {6, 1}, {7, 1}, {8, 1}, {9, 1}, {10, 1}, {11, 1}, {12, 1}, {13, 1}, {14, 1}, {15, 1}, {16, 1}, {17, 1}
        };

        public static Dictionary<int, int> CumulatieDictionaryValue(Dictionary<int, int> raw)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            for (int i = 0; i < raw.Count; i++)
            {
                int sum = 0;
                for (int j = 0; j <= i; j++)
                    sum += raw.Values.ElementAt(j);

                result.Add(raw.Keys.ElementAt(i), sum);
            }

            return result;
        }

        public static int GetDictValueSum(Dictionary<int, int> raw)
        { return raw.Sum(x => x.Value); }
    }
}

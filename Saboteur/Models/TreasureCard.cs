using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saboteur.Models
{
    public enum TreasureType {gold, stone};

    public class TreasureCard : PathCard
    {
        public TreasureType treasure;

        private TreasureCard(int id)
        {
            this.id = id;
            if (id == 88)
                treasure = TreasureType.gold;
            else if (id == 77)
                treasure = TreasureType.stone;
            
            position = Status.onBoard;
            ports = new Dictionary<string, bool>();
            reachable = false;
            Console.WriteLine("[TREASURE_CARD] Created a treasure card of type - {0}({1}), {2}", treasure, id, position);
        }

        static public TreasureCard[] CreateTreasureCards()
        {
            // the first card is gold, others are stone, but we will shuffle them in the GameBoard model
            return new TreasureCard[]
            {
                new TreasureCard(88),
                new TreasureCard(77),
                new TreasureCard(77)
            };
        }

        public void Reset(int x, int y)
        {
            reachable = false;
            //UpdateId(x, y);
            UpdatePorts();
            Console.WriteLine("[RESET] Reset a treasure card of type - {0}({1}), {2}", treasure, id, position);
        }

        /*public void UpdateId(int x, int y)
        {
            if (treasure == TreasureType.gold)
                id = 88;
            else if (treasure == TreasureType.stone)
                id = 77;
        }*/

        public override void UpdatePorts()
        {
            /*if (id == 70) { ports["toUp"] = true; ports["toDown"] = false; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 72) { ports["toUp"] = true; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 74) { ports["toUp"] = false; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 80) { ports["toUp"] = true; ports["toDown"] = false; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 82) { ports["toUp"] = true; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 84) { ports["toUp"] = false; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = false; }
            else { ports["toUp"] = false; ports["toDown"] = false; ports["toLeft"] = false; ports["toRight"] = false; }*/
            ports["toUp"] = ports["toDown"] = ports["toLeft"] = ports["toRight"] = true;
        }

        public override bool UseOnPlayer(PlayerModel player) { return false; }
        public override bool UseOnBoard(Card card, PlayerModel user) { return false; }
        public override void Reset() { Console.WriteLine("[ERROR] Be careful, it is a treasure card"); }
    }
}

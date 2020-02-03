using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saboteur.Models;

namespace Saboteur.Models
{
    public class PathCard : Card
    {
        public bool reachable;  // particularly for those cards on board
        public Dictionary<string, bool> ports;
        static private Dictionary<int, int> cumulativeProportion = ConfigModel.CumulatieDictionaryValue(ConfigModel.pathCardProportion);
        static private int proportionSum = ConfigModel.GetDictValueSum(cumulativeProportion);
        static private Random rand = new Random();


        public bool Reachable { get { return reachable; } set { reachable = value; } }


        public PathCard()
        {
            ports = new Dictionary<string, bool>();
            this.Reset();
            //Console.WriteLine("[PATH_CARD] (default) Created a path card ({0}), {1}", id, position);
        }

        public PathCard(int id, Status position)
        {
            this.id = id;
            this.position = position;
            reachable = false;
            ports = new Dictionary<string, bool>();
            UpdatePorts();
            //Console.WriteLine("[PATH_CARD] Created a path card ({0}), {1}", id, position);
        }


        public override void Reset()
        {
            int num = rand.Next(0, proportionSum);
            for (int id_iterator = 0; id_iterator <= 17; id_iterator++)
            {
                if (num < cumulativeProportion[id_iterator])
                {
                    id = id_iterator;
                    break;
                }
            }
            position = Status.onDeck;
            reachable = false;
            UpdatePorts();
            //Console.WriteLine("[RESET] Reset a path card ({0}), {1}, {2}, {3}, {4}, {5}", id, position, ports["toUp"], ports["toDown"], ports["toLeft"], ports["toRight"]);
        }

        public void ResetToEmpty()
        {
            id = 100;
            UpdatePorts();
            position = Status.onBoard;
        }

        public virtual void UpdatePorts()
        {
            if (id == 0) { ports["toUp"] = true; ports["toDown"] = false; ports["toLeft"] = false; ports["toRight"] = false; }
            else if (id == 1) { ports["toUp"] = false; ports["toDown"] = true; ports["toLeft"] = false; ports["toRight"] = false; }
            else if (id == 2) { ports["toUp"] = false; ports["toDown"] = false; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 3) { ports["toUp"] = false; ports["toDown"] = false; ports["toLeft"] = false; ports["toRight"] = true; }
            else if (id == 4) { ports["toUp"] = false; ports["toDown"] = false; ports["toLeft"] = true; ports["toRight"] = true; }
            else if (id == 5) { ports["toUp"] = false; ports["toDown"] = false; ports["toLeft"] = true; ports["toRight"] = true; }
            else if (id == 6) { ports["toUp"] = true; ports["toDown"] = true; ports["toLeft"] = false; ports["toRight"] = false; }
            else if (id == 7) { ports["toUp"] = true; ports["toDown"] = true; ports["toLeft"] = false; ports["toRight"] = false; }
            else if (id == 8) { ports["toUp"] = true; ports["toDown"] = false; ports["toLeft"] = false; ports["toRight"] = true; }
            else if (id == 9) { ports["toUp"] = false; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 10) { ports["toUp"] = false; ports["toDown"] = true; ports["toLeft"] = false; ports["toRight"] = true; }
            else if (id == 11) { ports["toUp"] = true; ports["toDown"] = false; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 12) { ports["toUp"] = true; ports["toDown"] = true; ports["toLeft"] = false; ports["toRight"] = true; }
            else if (id == 13) { ports["toUp"] = true; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = false; }
            else if (id == 14) { ports["toUp"] = false; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = true; }
            else if (id == 15) { ports["toUp"] = true; ports["toDown"] = false; ports["toLeft"] = true; ports["toRight"] = true; }
            else if (id == 16) { ports["toUp"] = true; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = true; }
            else if (id == 17) { ports["toUp"] = true; ports["toDown"] = true; ports["toLeft"] = true; ports["toRight"] = true; }
            else if (id == 100) { ports["toUp"] = false; ports["toDown"] = false; ports["toLeft"] = false; ports["toRight"] = false; }
            else { ports["toUp"] = false; ports["toDown"] = false; ports["toLeft"] = false; ports["toRight"] = false; }  // error
        }

        public void Rotate()
        {
            if (position == Status.onHand)
            {
                id = (id % 2 == 0) ? id + 1 : id - 1;
                UpdatePorts();
                Console.WriteLine("[CARD] card with id={0} rotated.", id);
            }
        }

        public override bool UseOnPlayer(PlayerModel player) { return false; }
        public override bool UseOnBoard(Card card, PlayerModel uesr) { return true; }
    }
}

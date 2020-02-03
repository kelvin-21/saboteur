using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saboteur.Models
{
    public enum ActionType { breakTool, fixTool, rockFall, readMap, ActionTypeERROR };

    public class ActionCard : Card
    {
        public ActionType type;
        static private Dictionary<int, int> cumulativeProportion = ConfigModel.CumulatieDictionaryValue(ConfigModel.actionCardProportion);
        static private int proportionSum = ConfigModel.GetDictValueSum(cumulativeProportion);
        static private Random rand = new Random();

        public ActionType Type { get { return type; } }
        
        public ActionCard()
        {
            Reset();
            UpdateType();
            //Console.WriteLine("[ACTION_CARD] (default) Created a action card of type - {0}({1}), {2}", type, id, position);
        }

        public ActionCard(int id, Status position)
        {
            this.id = id;
            this.position = position;
            UpdateType();
            //Console.WriteLine("[ACTION_CARD] Created a action card of type - {0}({1}), {2}", type, id, position);
        }


        public void UpdateType()
        {
            if (id == 50) { type = ActionType.breakTool; }
            else if (id == 51) { type = ActionType.fixTool; }
            else if (id == 52) { type = ActionType.rockFall; }
            else if (id == 53) { type = ActionType.readMap; }
            else { type = ActionType.ActionTypeERROR; }
        }

        public override void Reset()
        {
            int num = rand.Next(0, proportionSum);
            for (int id_iterator = 50; id_iterator <= 53; id_iterator++)
            {
                if (num < cumulativeProportion[id_iterator])
                {
                    id = id_iterator;
                    break;
                }
            }
            UpdateType();
            position = Status.onDeck;
            //Console.WriteLine("[RESET] Reset a action card of type - {0}({1}), {2}", type, id, position);
        }

        public override bool UseOnPlayer(PlayerModel player)
        {
            if (type == ActionType.breakTool)
                return BreakTool(player);
            else if (type == ActionType.fixTool)
                return FixTool(player);
            else
                return false;
        }

        public override bool UseOnBoard(Card card, PlayerModel user)
        {
            if (type == ActionType.rockFall)
                return RockFall(card);
            else if (type == ActionType.readMap)
                return ReadMap(card, user);
            else
                return false;
        }


        public bool BreakTool(PlayerModel player)
        {
            if (!player.toolIsGood)
                return false;

            Console.WriteLine("[ACTION] player {0}'s tool got broken!", player.name);
            player.toolIsGood = false;
            return true;
        }

        public bool FixTool(PlayerModel player)
        {
            if (player.toolIsGood)
                return false;

            Console.WriteLine("[ACTION] player {0}'s tool got fixed!", player.name);
            player.toolIsGood = true;
            return true;
        }

        public bool RockFall(Card on)
        {
            if (!(on.Position == Status.onBoard) || (on is TreasureCard) || on.Id > 20)
                return false;

            Console.WriteLine("[ACTION] Rock falls on path card ({0})!", on.Id);
            PathCard pCard = on as PathCard;
            pCard.Id = 100;
            pCard.UpdatePorts();
            return true;
        }

        public bool ReadMap(Card on, PlayerModel user)
        {
            if (!(on is TreasureCard))
                return false;

            Console.WriteLine("[ACTION] Reading map on treasure card ({0})!", on.Id);
            for(int i=0; i<3; i++)
            {
                if (GameBoard.Instance.board[i*2, 8] == on && user.readTreasure[i] == false)
                {
                    user.readTreasure[i] = true;
                    return true;
                }
            }
            return false;
        }
    }
}

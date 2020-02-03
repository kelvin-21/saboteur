using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saboteur.Models
{
    public enum SaboteurParty { miner, saboteur };

    public class PlayerModel
    {
        public string name;
        public int id;
        public SaboteurParty party;
        public List<Card> handCards = new List<Card>();
        public bool isMyTurn;
        public bool toolIsGood;
        public bool isWinner;
        public bool[] readTreasure;

        public PlayerModel(string name, int id, SaboteurParty party)
        {
            this.name = name;
            this.id = id;
            this.party = party;
            handCards = new List<Card>();
            isMyTurn = false;
            toolIsGood = true;
            isWinner = false;
            readTreasure = new bool[]{ false, false, false };
            Console.WriteLine("[PLAYER] Created player - {0} ({1}) {2}!", name, id, party.ToString());
        }

        public void UseCard(Card card)
        {
            if (handCards.Contains(card))
            {
                handCards.Remove(card);
                Console.WriteLine("[PLAYER] {0} ({1}) uses a card. #handCards={2}", name, id, handCards.Count);
            }
        }

        public void DrawCard(Card card)
        {
            handCards.Add(card);
            card.Position = Status.onHand;
            Console.WriteLine("[PLAYER] {0} ({1}) draws a new card. #handCards={2}", name, id, handCards.Count);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saboteur.Models
{
    public class CardDeck
    {
        public Stack<Card> deck;
        public Stack<Card> cardUsed;
        static private Random rand;
        static private CardDeck _instance;

        static public CardDeck Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CardDeck();
                return _instance;
            }
        }


        private CardDeck()
        {
            rand = new Random();
            Initialize();
            Reset();
        }


        private void Initialize()
        {
            deck = new Stack<Card>();
            cardUsed = new Stack<Card>();
            for (int i = 0; i < 100; i++)
            {
                int num = rand.Next(0, 100);
                if (num < ConfigModel.pathCardRatio)
                {
                    // path card, will be randomized by Reset()
                    deck.Push(new PathCard(rand.Next(0, 18), Status.onDeck));
                }
                else
                {
                    // action card, will be randomized by Reset()
                    deck.Push(new ActionCard(rand.Next(50, 54), Status.onDeck));
                }
            }
            Reset();
        }

        public void Reset()
        {
            // put cards in cardUsed to cardDeck
            while (cardUsed.Count > 0)
                deck.Push(cardUsed.Pop());

            // shuffle card deck
            List<Card> temp = new List<Card>();
            Card temp_ref = null;
            int index_a = 0, index_b = 0, total = deck.Count;

            for (int i = 0; i < total; i++)
                temp.Add(deck.Pop());

            for (int i = 0; i < total; i++)
                temp[i].Reset();

            for (int i = 0; i < total; i++)
            {
                index_a = rand.Next(0, total);
                index_b = rand.Next(0, total);
                while (index_a == index_b)
                    index_b = rand.Next(0, total);

                temp_ref = temp[index_a];
                temp[index_a] = temp[index_b];
                temp[index_b] = temp_ref;
            }

            for (int i = 0; i < total; i++)
                deck.Push(temp[i]);

            temp.Clear();
            temp = null; temp_ref = null;
            Console.WriteLine("[DECK] Reset card deck.");
        }

        public Card DrawCard()
        {
            Console.WriteLine("[DECK] Draw a card.");
            if (deck.Count == 0)
                return null;
            return deck.Pop();
        }

        public void CollectUsedCard(Card card)
        {
            Console.WriteLine("[DECK] Collect an used card.");
            if (card != null)
            {
                card.Position = Status.used;
                cardUsed.Push(card);
            }
        }
    }
}

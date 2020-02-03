using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saboteur.Models
{
    class GameFlow
    {
        public List<PlayerModel> players;
        public GameBoard gameBoard;
        public CardDeck cardDeck;
        static private GameFlow _instance;


        public GameFlow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameFlow();
                return _instance;
            }
        }


        private GameFlow()
        {
            players = new List<PlayerModel>();
            /* ... do something else for players ... */
            gameBoard = GameBoard.Instance;
            cardDeck = CardDeck.Instance;
            Reset();
        }

        public void Reset()
        {
            gameBoard.Reset();
            cardDeck.Reset();
        }

        public bool PlaceCardOnBoard(PlayerModel player, Card from, int x, int y)
        {
            Card to = gameBoard.board[x, y];
            Console.WriteLine("[GAME] player {0} trying to place ({1}) on ({2}) with position ({3}, {4})!", player.name, from.Id, to.Id, x, y);
            
            bool success = gameBoard.PlaceCard(from, x, y, player);
            if (success)
            {
                player.UseCard(from);
                cardDeck.CollectUsedCard(from);
                // I know it is quite strange, but in any case, we collect the card.
                // Even when it is a path card being put onto the board, we collect it.
                return true;
            }
            else
                return false;
        }

        public bool PlaceCardOnPlayer(PlayerModel from, PlayerModel to, Card card)
        {
            Console.WriteLine("[GAME] player {0} trying to place ({1}) on player {2}!", from.name, card.Id, to.name);
            if (!(card is ActionCard))
                return false;

            bool success = (card as ActionCard).UseOnPlayer(to);
            if (success)
            {
                from.UseCard(card);
                cardDeck.CollectUsedCard(card);
                return true;
            }
            else
                return false;
        }

        public bool DrawCard(PlayerModel player)
        {
            Card card = cardDeck.DrawCard();
            if (card == null)
                return false;

            player.DrawCard(card);
            return true;
        }
    }
}

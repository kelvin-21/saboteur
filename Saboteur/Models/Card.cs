using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saboteur.Models;

namespace Saboteur.Models
{
    public enum Status { onDeck, onHand, onBoard, used};

    public abstract class Card
    {
        public int id;
        public Status position;

        public int Id { get { return id; } set { id = value; } }
        public Status Position { get { return position; } set { position = value; } }

        public abstract void Reset();
        public abstract bool UseOnPlayer(PlayerModel target);
        public abstract bool UseOnBoard(Card card, PlayerModel user);
    }
}

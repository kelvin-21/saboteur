using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saboteur.Models
{
    public class GameBoard
    {
        public PathCard[,] board;
        static private Random rand;
        static private GameBoard _instance;
        static private TreasureCard[] _treasureCards;
        static private PathCard startCard;

        static public GameBoard Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameBoard();
                return _instance;
            }
        }

        private GameBoard()
        {
            rand = new Random();
            Initialize();
            Reset();
        }

        private void Initialize()
        {
            board = new PathCard[5, 9];
            startCard = new PathCard(16, Status.onBoard);

            _treasureCards = TreasureCard.CreateTreasureCards();

            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    if (board[i, j] == null)
                    {
                        board[i, j] = new PathCard(100, Status.onBoard);
                    }
        }

        public void Reset()
        {
            board[2, 0] = startCard;

            // shuffle treasure cards
            int index;
            List<int> shuffle = new List<int>();
            while (shuffle.Count < 3)
            {
                index = rand.Next(0, 3);
                if (!shuffle.Contains(index))
                    shuffle.Add(index);
            }
            board[0, 8] = _treasureCards[shuffle[0]];
            board[2, 8] = _treasureCards[shuffle[1]];
            board[4, 8] = _treasureCards[shuffle[2]];
            (board[0, 8] as TreasureCard).Reset(0, 8);
            (board[2, 8] as TreasureCard).Reset(2, 8);
            (board[4, 8] as TreasureCard).Reset(4, 8);
            Console.WriteLine("[RESET] gold card is on ({0}, 8)", shuffle.IndexOf(0) * 2);

            // put the empty-path-cards
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (i == 2 && j == 0) { Console.WriteLine("[RESET] meeting the start card"); }
                    else if (i == 0 && j == 8) { Console.WriteLine("[RESET] meeting the treasure card ({0})", board[0, 8].Id); }
                    else if (i == 2 && j == 8) { Console.WriteLine("[RESET] meeting the treasure card ({0})", board[2, 8].Id); }
                    else if (i == 4 && j == 8) { Console.WriteLine("[RESET] meeting the treasure card ({0})", board[4, 8].Id); }
                    else
                        board[i, j].ResetToEmpty();
                }
            UpdateReachable();
            shuffle = null;
            Console.WriteLine("[BOARD] Reset card board!");
        }

        public bool PlaceCard(Card from, int i, int j, PlayerModel user)  // from = (from player's hand); to = (to target)
        {
            if (from is null) return false;
            if (from is ActionCard)
                if ((from as ActionCard).type == ActionType.rockFall && i == 2 && j == 0)
                    return false;

                    PathCard to = board[i, j];
            Console.WriteLine("[BOARD] Trying to place ({0}) on ({1}) with location ({2}, {3})", from.Id, to.Id, i, j);

            if (from is PathCard)
            {
                if (!PathCardPlaceable(from as PathCard, i, j))
                    return false;

                to.Id = from.Id;
                to.UpdatePorts();
                from.Id = 100;  // empty
                return true;
            }
            else if (from is ActionCard)
            {
                return (from as ActionCard).UseOnBoard(to, user);
            }
            else
                return false;
        }

        public Tuple<int, int> SearchCard(PathCard pCard)
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    if (board[i, j] == pCard)
                        return Tuple.Create(i, j);

            return Tuple.Create(-1, -1);
        }

        public bool IsEmptyPathCard(PathCard pCard) => pCard.Id == 100;

        public bool BoundaryFromUp(int i, int j) => i == 0;
        public bool BoundaryFromDown(int i, int j) => i == 4;
        public bool BoundaryFromLeft(int i, int j) => j == 0;
        public bool BoundaryFromRight(int i, int j) => j == 8;

        public bool HaveCardFromUp(int i, int j) => BoundaryFromUp(i, j) ? false : !IsEmptyPathCard(board[i - 1, j]);
        public bool HaveCardFromDown(int i, int j) => BoundaryFromDown(i, j) ? false : !IsEmptyPathCard(board[i + 1, j]);
        public bool HaveCardFromLeft(int i, int j) => BoundaryFromLeft(i, j) ? false : !IsEmptyPathCard(board[i, j - 1]);
        public bool HaveCardFromRight(int i, int j) => BoundaryFromRight(i, j) ? false : !IsEmptyPathCard(board[i, j + 1]);

        // pure path card is defined to be: {path card}\{treasure card}
        public bool HavePurePathCardFromUp(int i, int j) => i == 0 ? false : board[i - 1, j] is TreasureCard;
        public bool HavePurePathCardFromDown(int i, int j) => i == 4 ? false : board[i + 1, j] is TreasureCard;
        public bool HavePurePathCardFromLeft(int i, int j) => j == 0 ? false : board[i, j - 1] is TreasureCard;
        public bool HavePurePathCardFromRight(int i, int j) => j == 8 ? false : board[i, j + 1] is TreasureCard;

        public bool NeedPortTowardsUp(int i, int j) => !HaveCardFromUp(i, j) ? false : board[i - 1, j].ports["toDown"];
        public bool NeedPortTowardsDown(int i, int j) => !HaveCardFromDown(i, j) ? false : board[i + 1, j].ports["toUp"];
        public bool NeedPortTowardsLeft(int i, int j) => !HaveCardFromLeft(i, j) ? false : board[i, j - 1].ports["toRight"];
        public bool NeedPortTowardsRight(int i, int j) => !HaveCardFromRight(i, j) ? false : board[i, j + 1].ports["toLeft"];


        public bool NotIsolated(int i, int j) => (HaveCardFromUp(i, j) || HaveCardFromDown(i, j) || HaveCardFromLeft(i, j) || HaveCardFromRight(i, j));
        
        public bool PathCardPlaceable(PathCard pCard, int i, int j)
        {
            if (i == -1 || j == -1)
            {
                Console.WriteLine("[ERROR] Coordinate tuple has -1. Serious Error");
                return false;
            }

            PathCard to = board[i, j];
            if (!IsEmptyPathCard(to))
            {
                Console.WriteLine("[BOARD] Invalid connection, target no empty");
                return false;
            }

            if (!NotIsolated(i, j))
            {
                Console.WriteLine("[BOARD] Invalid connection, target is isolated");
                return false;
            }

            if (!to.Reachable)
            {
                Console.WriteLine("[BOARD] Invalid connection, target not reachable");
                return false;
            }

            // check whether the connections of ports are appropriate
            bool flag = true;
            if (HaveCardFromUp(i, j))
                if (!(board[i - 1, j] is TreasureCard))  // it is okay not to have ports towards a TreasureCard
                    flag &= !(pCard.ports["toUp"] ^ NeedPortTowardsUp(i, j));
            if (HaveCardFromDown(i, j))
                if (!(board[i + 1, j] is TreasureCard))
                    flag &= !(pCard.ports["toDown"] ^ NeedPortTowardsDown(i, j));
            if (HaveCardFromLeft(i, j))
                if (!(board[i, j - 1] is TreasureCard))
                    flag &= !(pCard.ports["toLeft"] ^ NeedPortTowardsLeft(i, j));
            if (HaveCardFromRight(i, j))
                if (!(board[i, j + 1] is TreasureCard))
                    flag &= !(pCard.ports["toRight"] ^ NeedPortTowardsRight(i, j));

            /*if (NeedPortTowardsUp(i, j))
                flag &= pCard.ports["toUp"];
            if (NeedPortTowardsDown(i, j))
                flag &= pCard.ports["toDown"];
            if (NeedPortTowardsLeft(i, j))
                flag &= pCard.ports["toLeft"];
            if (NeedPortTowardsRight(i, j))
                flag &= pCard.ports["toRight"];
            */

            if (flag)
                Console.WriteLine("[BOARD] Yes, card({0}) placeable at ({1}, {2})", pCard.Id, i, j);
            else
                Console.WriteLine("[BOARD] No, wrong ports, card({0}) NOT placeable at ({1}, {2})", pCard.Id, i, j);

            return flag;
        }

        public void Traverse(int i, int j)
        {
            PathCard pCard = board[i, j];
            if (pCard.Reachable)  // meaning visited
                return;  // to prevent closed loop

            pCard.Reachable = true;
            Console.WriteLine("[BOARD] ({0}, {1}) is Reachable", i, j);

            if (IsEmptyPathCard(pCard))
                return;  // if empty card, then no further traverse

            if (!BoundaryFromUp(i, j))
            {
                //PathCard next = board[i - 1, j];
                //if (!IsEmptyPathCard(next))
                //    Traverse(i - 1, j);
                if (pCard.ports["toUp"])
                    Traverse(i - 1, j);
            }

            if (!BoundaryFromDown(i, j))
            {
                //PathCard next = board[i + 1, j];
                //if (!IsEmptyPathCard(next))
                //    Traverse(i + 1, j);
                if (pCard.ports["toDown"])
                    Traverse(i + 1, j);
            }

            if (!BoundaryFromLeft(i, j))
            {
                //PathCard next = board[i, j - 1];
                //if (!IsEmptyPathCard(next))
                //    Traverse(i, j - 1);
                if (pCard.ports["toLeft"])
                    Traverse(i, j - 1);
            }

            if (!BoundaryFromRight(i, j))
            {
                //PathCard next = board[i, j + 1];
                //if (!IsEmptyPathCard(next))
                //    Traverse(i, j + 1);
                if (pCard.ports["toRight"])
                    Traverse(i, j + 1);
            }
        }

        public void UpdateReachable()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    board[i, j].Reachable = false;

            Traverse(2, 0);
        }
    }
}

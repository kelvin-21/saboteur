using Saboteur.Models;
using System;
using System.Collections.Generic;

namespace Saboteur.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        public ConfigViewModel Config { get; set; }
        public List<PlayerViewModel> Players { get; set; }

        private GameBoard CardBoard;
        private CardDeck Deck;
        private List<SaboteurParty> SaboteurPartyList;
        private TreasureCard GoldenCard;
        private int CurrentPlayerID = 0;

        public GameLogDelegate PublicLog;
        GameFlowDelegate GameStart;
        GameFlowDelegate ViewUpdate;

        #region Singleton
        private static GameViewModel _currentGame;
        public static GameViewModel CurrentGame
        {
            get
            {
                if (_currentGame == null)
                    _currentGame = new GameViewModel();
                return _currentGame;
            }
        }

        private GameViewModel()
        {
            Config = new ConfigViewModel();
            CardBoard = GameBoard.Instance;
            Deck = CardDeck.Instance;
        }
        #endregion

        #region Game Flow
        public void StartGame()
        {
            SaboteurPartyList = RandomizePartyList();
            Players = InitializePlayers();
            GoldenCard = FindGoldenCard();
            PublicLog("[GAME]Game Start");
            GameStart();
            Players[CurrentPlayerID].TurnStart();
            ViewUpdate();
        }

        public void NextTurn()
        {
            if (CheckGameOver()) return;
            CurrentPlayerID = (CurrentPlayerID < Players.Count - 1) ? CurrentPlayerID + 1 : 0;
            Players[CurrentPlayerID].TurnStart();
            ViewUpdate();
        }

        private bool CheckGameOver()
        {
            if (GoldenCard.Reachable)
            {
                ViewUpdate();
                PublicLog("[GAME]" + Players[CurrentPlayerID].Name + " find the Treasure");
                PublicLog("[GAME] Party Miner Win");
                PublicLog("[GAME] Game End");
                return GoldenCard.Reachable;
            }

            if (Deck.deck.Count == 0)
            {
                ViewUpdate();
                PublicLog("[GAME] Card deck is empty");
                PublicLog("[GAME] Party Saboteur Win");
                PublicLog("[GAME] Game End");
                return true;
            }

            return false;
        }
        #endregion

        #region init
        private List<SaboteurParty> RandomizePartyList()
        {
            var PartyList = new List<SaboteurParty>();
            var rng = new Random();
            int numberOfSaboteur = (int)(Config.NumberOfPlayers / 2 - 1);
            int numberOfMiner = Config.NumberOfPlayers - numberOfSaboteur;
            for (int i = 0; i < numberOfSaboteur; i++) PartyList.Add(SaboteurParty.saboteur);
            for (int i = 0; i < numberOfMiner; i++) PartyList.Add(SaboteurParty.miner);

            int n = PartyList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                SaboteurParty value = PartyList[k];
                PartyList[k] = PartyList[n];
                PartyList[n] = value;
            }
            return PartyList;
        }

        private List<PlayerViewModel> InitializePlayers()
        {
            var initPlayers = new List<PlayerViewModel>();
            for (int ID = 0; ID < Config.NumberOfPlayers; ID++)
            {
                var playerInfo = Config.PlayerInfoList[ID];
                var newPlayer = new PlayerModel(playerInfo.Name, ID, SaboteurPartyList[ID]);
                initPlayers.Add(new PlayerViewModel(newPlayer, (GameMode)playerInfo.GameMode));
                GameStart += initPlayers[ID].Window.Show;
                GameStart += initPlayers[ID].DrawStartHands;
                PublicLog += initPlayers[ID].PrivateLog;
                ViewUpdate += initPlayers[ID].UpdateTreasureVisibility;
                ViewUpdate += initPlayers[ID].UpdateMyView;
            }
            return initPlayers;
        }

        private TreasureCard FindGoldenCard()
        {
            for (int i = 0; i < 3; i++)
                if (CardBoard.board[i * 2, 8] is TreasureCard treasurecard && treasurecard.treasure == TreasureType.gold)
                    return treasurecard;
            return null;
        }
        #endregion
    }
}

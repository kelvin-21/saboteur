using Saboteur.Models;
using Saboteur.Views;
using System.Collections.ObjectModel;

namespace Saboteur.ViewModels
{
    public enum GameMode { Desktop, Mobile, Text };
    public delegate void GameFlowDelegate();
    public delegate void GameLogDelegate(string LogMessage);

    public class PlayerViewModel : ViewModelBase
    {
        private PlayerModel _player;
        public PlayerModel Player { get => _player; }
        public GameBoard CardBoard { get => GameBoard.Instance; }
        public CardDeck Deck { get => CardDeck.Instance; }
        public ISaboteurDisplay Window { get; private set; }
        private Card SelectedCard;

        GameFlowDelegate TriggerNextTurn { get => GameViewModel.CurrentGame.NextTurn; }
        GameLogDelegate TriggerPublicLog { get => GameViewModel.CurrentGame.PublicLog; }
        public GameLogDelegate PrivateLog;

        public string[] PathCardChar = new string[]
        {
            "╨", "╥", "╡", "╞",
            "=", "=", "║", "║",
            "╚", "╗", "╔", "╝",
            "╠", "╣", "╦", "╩",
            "╬", "╬"
        };

        public PlayerViewModel(PlayerModel player, GameMode gameMode)
        {
            _player = player;
            Window = CreateGameWindow(gameMode);
            Window.SelectCard += UpdateSelectedCard;
            Window.PlaceOn += UseCardOnBoard;
            Window.TargetTo += UseCardOnTarget;
            Window.Discard += DiscardSelectedCard;
            Window.RotateCard += RotateSelectedCard;
            PrivateLog += Window.Log;
        }

        #region Observable Property
        public int ID { get => _player.id;}
        public string Name { get => _player.name; }
        public SaboteurParty Party { get => _player.party; }
        public bool IsSaboteur { get => _player.party == SaboteurParty.saboteur; }
        public bool[] ReadTreasure { get => _player.readTreasure; }

        public bool IsMyTurn
        {
            get => _player.isMyTurn;
            set { _player.isMyTurn = value; }
        }

        public bool ToolIsGood
        {
            get => _player.toolIsGood;
            set { _player.toolIsGood = value; }
        }

        public ObservableCollection<Card> Hands
        {
            get => new ObservableCollection<Card>(_player.handCards);
        }

        public ObservableCollection<ObservableCollection<PathCard>> GameBoardDisplay
        {
            get
            {
                var gameBoard = new ObservableCollection<ObservableCollection<PathCard>>();
                for (int row = 0; row < CardBoard.board.GetLength(0); row++)
                {
                    gameBoard.Add(new ObservableCollection<PathCard>());
                    for (int col = 0; col < CardBoard.board.GetLength(1); col++)
                        gameBoard[row].Add(CardBoard.board[row, col]);
                }
                return gameBoard;
            }
        }

        public ObservableCollection<PlayerViewModel> AllPlayersList
        {
            get => new ObservableCollection<PlayerViewModel>(GameViewModel.CurrentGame.Players);
        }
        #endregion

        #region method for Delegate in View
        private void UpdateSelectedCard(int index)
        {
            try
            {
                SelectedCard = Hands[index];
                if (SelectedCard is PathCard path)
                    PrivateLog("[YOU]Select PathCard - " + PathCardChar[path.Id]);
                if (SelectedCard is ActionCard action)
                    PrivateLog("[YOU]Select ActionCard - " + action.Type);
            }
            catch
            {
                PrivateLog("[SYSTEM]Incorrect Card Selection");
            }
        }

        private void UseCardOnBoard(int row, int column)
        {
            try
            {
                if (NotMyTurn() || NoSelectedCard()) return;

                if ((SelectedCard is ActionCard || ToolIsGood) && CardBoard.PlaceCard(SelectedCard, row, column, _player))
                {
                    CardBoard.UpdateReachable();
                    if (SelectedCard is PathCard)
                        TriggerPublicLog("[" + Name + "]Place Path card on Position (" + row + "," + column + ")");
                    else if (((ActionCard)SelectedCard).Type == ActionType.rockFall)
                        TriggerPublicLog("[" + Name + "]Destroy Path card on Position (" + row + "," + column + ")");
                    else if (((ActionCard)SelectedCard).Type == ActionType.readMap)
                        TriggerPublicLog("[" + Name + "]Read Treasure card on Position (" + row + "," + column + ")");
                    TurnEnd();
                }
                else PrivateLog("[SYSTEM]Unable to place this card on Position (" + row + "," + column + ")");
            }
            catch
            {
                PrivateLog("[SYSTEM]Incorrect Postion Selection");
            }
        }

        private void UseCardOnTarget(int TargetID)
        {
            try
            {
                PlayerModel Target = AllPlayersList[TargetID].Player;
                if (NotMyTurn() || NoSelectedCard()) return;

                if (SelectedCard is ActionCard Tool && Tool.UseOnPlayer(Target))
                {
                    TriggerPublicLog("[" + Name + "]Use " + Tool.Type + " on Target" + Target.name);
                    TurnEnd();
                }
                else PrivateLog("[SYSTEM]Unable to Use this card on Target: " + Target.name);
            }
            catch
            {
                PrivateLog("[SYSTEM]Incorrect target Selection");
            }
        }

        private void RotateSelectedCard()
        {
            if (NoSelectedCard()) return;

            if (SelectedCard is PathCard pathcard)
            {
                pathcard.Rotate();
                PrivateLog("[YOU]Success: Rotate a Path Card");
                RaisePropertyChanged("Hands");
            }
            else PrivateLog("[SYSTEM]Please select a Path Card for Rotation");
        }

        private void DiscardSelectedCard()
        {
            if (NotMyTurn() || NoSelectedCard()) return;
            TriggerPublicLog("[" + Name + "]Discard a Card");
            TurnEnd();
        }
        #endregion

        #region private
        private ISaboteurDisplay CreateGameWindow(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Mobile:
                    return new MobileModeWindow(this);
                case GameMode.Text:
                    return new TextModeWindow(this);
                case GameMode.Desktop:
                default:
                    return new DesktopModeWindow(this);
            }
        }

        private void DrawCard()
        {
            Card newcard = Deck.DrawCard();
            if (newcard != null)
            {
                _player.DrawCard(newcard);
                PrivateLog("[YOU]Draw a Card");
                RaisePropertyChanged("Hands");
            }
        }

        private bool NoSelectedCard()
        {
            if (SelectedCard != null) return false;
            PrivateLog("[SYSTEM]Please Select a card in hand first");
            return true;
        }

        private bool NotMyTurn()
        {
            if (IsMyTurn) return false;
            PrivateLog("[SYSTEM]Not Your turn");
            return true;
        }
        #endregion

        #region public
        public void DrawStartHands()
        {
            for (int i = 0; i < ConfigModel.numberOfHands; i++)
                DrawCard();
        }

        public void TurnStart()
        {
            IsMyTurn = true;
            TriggerPublicLog("[GAME]" + Name + " Turn Start");
            PrivateLog("[YOU]Your Turn Start");
        }

        public void TurnEnd()
        {
            IsMyTurn = false;
            _player.UseCard(SelectedCard);
            Deck.CollectUsedCard(SelectedCard);
            SelectedCard = null;
            DrawCard();
            TriggerPublicLog("[GAME]" + Name + " Turn End");
            TriggerNextTurn();
        }

        public void UpdateTreasureVisibility()
        {
            for (int i = 0; i < 3; i++)
                if (!_player.readTreasure[i] && CardBoard.board[i * 2, 8].reachable)
                    _player.readTreasure[i] = true;
                    
        }

        public void UpdateMyView()
        {
            RaisePropertyChanged("AllPlayersList");
            RaisePropertyChanged("GameBoardDisplay");
            RaisePropertyChanged("ReadTreasure");
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Saboteur.Models;
using Saboteur.ViewModels;

namespace Saboteur.Views
{
    public partial class TextModeWindow : Window, ISaboteurDisplay
    {
        public PlayerViewModel Player { get; set; }

        public IDSelectDelegate SelectCard { get; set; }
        public IDSelectDelegate TargetTo { get; set; }
        public PositionSelectDelegate PlaceOn { get; set; }
        public CardUsedDelegate RotateCard { get; set; }
        public CardUsedDelegate Discard { get; set; }

        private const string CommandPrefix = @">>> ";
        private Queue<string> Command;

        private string[,] CommandDetail = new string[,]
        {
            { "-s", "-selectcard", "int: Card index in hand", "Select a Card for Player Action" },
            { "-r", "-rotate", "none", "Rotate the selected card" },
            { "-d", "-discard", "none", "Discard the selected card, No othoer Action in this turn" },
            { "-p", "-placeon", "int: row & int: column", "Apply the selected card on this CardBoard postion" },
            { "-t", "-targeton", "int: Player ID", "Apply the selected card on this Player" },
            { "-v", "-view", "string:{-g, -gameboard, -p, -players, -h, -hands, -myparty, -mp}", "Get Current Information of gameboard, players, hands, or party" }
        };

        public TextModeWindow(PlayerViewModel player)
        {
            this.DataContext = this;
            this.Player = player;
            InitializeComponent();
            Player.PropertyChanged += Player_PropertyChanged;
            Log("-----------Saboteur---------");
            Log("[SYSTEM]You can use command '-h' or '-help' to see all Commands");
            ShowParty();
        }

        public void Log(string Message)
        {
            GameLog.Children.Insert(GameLog.Children.Count - 1, new TextBlock() { Text = Message });
            GameConsole.ScrollToBottom();
        }

        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "GameBoardDisplay":
                    ShowGameBoard();
                    break;
                case "AllPlayersList":
                    ShowPlayers();
                    break;
                case "Hands":
                    ShowHands();
                    break;
                default:
                    break;
            }
        }

        private void ShowHands()
        {
            Log("-----My Hands------");
            int index = 0;
            foreach (var card in Player.Hands)
            {
                if (card is PathCard path)
                    Log(index + ". PathCard - " + Player.PathCardChar[path.Id]);
                if (card is ActionCard action)
                    Log(index + ". ActionCard - " + action.Type);
                index++;
            }
        }

        private void ShowPlayers()
        {
            Log("------Player Status------");
            int index = 0;
            foreach (var player in Player.AllPlayersList)
            {
                if (player.ToolIsGood)
                    Log(index + ". Name: " + player.Name + "     Tool: Good     Current Turn: " + player.IsMyTurn);
                else Log(index + ". Name: " + player.Name + "     Tool: Broken   Current Turn: " + player.IsMyTurn);
                index++;
            }
        }

        private void ShowGameBoard()
        {
            Log("------Curreunt Game Board------");
            Log("  012345678");
            Log(" ┌─────────┐");
            for (int row = 0; row < 5; row++)
            {
                string rowLog = row + "│";
                for (int col = 0; col < 9; col++)
                {
                    if (Player.CardBoard.board[row, col] is TreasureCard treasure)
                    {
                        if (Player.ReadTreasure[row / 2])
                            rowLog += (treasure.treasure == TreasureType.gold) ? "G" : "S";
                        else rowLog += "█";
                    }
                    else if (Player.CardBoard.board[row, col].Id <= 17)
                        rowLog += Player.PathCardChar[Player.CardBoard.board[row, col].Id];
                    else rowLog += " ";
                }
                rowLog += "│";
                Log(rowLog);
            }
            Log(" └─────────┘");
        }

        private void UserInput_TextChanged(object sender, EventArgs e)
        {
            if (!UserInput.Text.StartsWith(CommandPrefix))
            {
                UserInput.Text = CommandPrefix;
                UserInput.SelectionStart = UserInput.Text.Length;
            }
        }

        private void Enter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Log(UserInput.Text);
                Command = new Queue<string>(UserInput.Text.Split(' '));
                CommandHandle();
                UserInput.Text = "";
            }
        }

        private void CommandHandle()
        {
            try
            {
                switch (Command.Dequeue())
                {
                    case (">>>"):
                        break;
                    case "-selectcard": case "-s":
                        SelectCard(Int32.Parse(Command.Dequeue()));
                        break;
                    case "-placeon": case "-p":
                        PlaceOn(Int32.Parse(Command.Dequeue()), Int32.Parse(Command.Dequeue()));
                        break;
                    case "-targeton": case "-t":
                        TargetTo(Int32.Parse(Command.Dequeue()));
                        break;
                    case "-rotate": case "-r":
                        RotateCard();
                        break;
                    case "-discard": case "-d":
                        Discard();
                        break;
                    case "-view": case "-v":
                        ShowInformation(Command.Dequeue());
                        break;
                    case "-help": case "-h":
                        ShowComandList();
                        break;
                    default:
                        Log("[SYSTEM]Incorrect Command");
                        break;
                }
                if (Command.Count > 0) CommandHandle();
            }
            catch
            {
                Log("[SYSTEM]Incorrect Command");
            }

        }

        private void ShowParty()
        {
            Log("[GAME]Your Party is " + Player.Party);
        }

        private void ShowInformation(string type)
        {
            try
            {
                switch (type)
                {
                    case "-gameboard": case "-g":
                        ShowGameBoard();
                        break;
                    case "-players": case "-p":
                        ShowPlayers();
                        break;
                    case "-hands": case "-h":
                        ShowHands();
                        break;
                    case "-myparty": case "-mp":
                        ShowParty();
                        break;
                    default:
                        Log("[SYSTEM]Incorrect Command: Information Selection");
                        break;
                }
            }
            catch
            {
                Log("[SYSTEM]Incorrect Command: Information Selection");
            }
        }

        private void ShowComandList()
        {
            Log("-----Command List-----");
            for (int i = 0; i < CommandDetail.GetLength(0); i++)
                LogCommandItem(CommandDetail[i, 0], CommandDetail[i, 1], CommandDetail[i, 2], CommandDetail[i, 3]);
            Log("[SYSTEM]You can use command '-h' or '-help' to see all Commands");
        }

        private void LogCommandItem(string alias, string fullCommand, string args, string description)
        {
            Log("Command: " + alias + "  ||  " + fullCommand);
            Log("          Args- " + args);
            Log("          Detail- " + description);
        }
    }
}

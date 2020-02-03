using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Saboteur.ViewModels;

namespace Saboteur.Views
{
    public partial class DesktopModeWindow : Window, ISaboteurDisplay
    {
        public IDSelectDelegate SelectCard { get; set; }
        public IDSelectDelegate TargetTo { get; set; }
        public PositionSelectDelegate PlaceOn { get; set; }
        public CardUsedDelegate RotateCard { get; set; }
        public CardUsedDelegate Discard { get; set; }

        public DesktopModeWindow(PlayerViewModel player)
        {
            InitializeComponent();
            this.DataContext = player;
            PlayerHandsView.RotateButtonTrigger += (s, e) => RotateCard();
            PlayerHandsView.DiscardButtonTrigger += (s, e) => Discard();
            PlayerHandsView.PlayerHandsTrigger += (s, e) => SelectCard(Int32.Parse(((Button)s).Content.ToString()));
            GameBoardView.GameBoardTrigger += (s, e) =>
            {
                var position = ((Button)s).Content.ToString().Split(',');
                PlaceOn(Int32.Parse(position[0]), Int32.Parse(position[1]));
            };
            AllPlayersView.PlayerInfoTrigger += (s, e) => TargetTo(Int32.Parse(((Button)s).Tag.ToString()));
        }

        public void Log(string LogMessage)
        {
            GameLog.Children.Add(new TextBlock { Text = LogMessage, });
            GameLogDisplay.ScrollToBottom();
        }
    }
}

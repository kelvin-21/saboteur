using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Saboteur.ViewModels;

namespace Saboteur.Views
{
    public partial class MobileModeWindow : Window, ISaboteurDisplay
    {
        public IDSelectDelegate SelectCard { get; set; }
        public IDSelectDelegate TargetTo { get; set; }
        public PositionSelectDelegate PlaceOn { get; set; }
        public CardUsedDelegate RotateCard { get; set; }
        public CardUsedDelegate Discard { get; set; }

        public MobileModeWindow(PlayerViewModel player)
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

        private void MyHandsButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerHandsView.Visibility = Visibility.Visible;
            PartyDisplay.Visibility = GameLogDisplay.Visibility = PlayersDisplay.Visibility = Visibility.Collapsed;
        }

        private void PlayersButton_Click(object sender, RoutedEventArgs e)
        {
            PlayersDisplay.Visibility = Visibility.Visible;
            PartyDisplay.Visibility = GameLogDisplay.Visibility = PlayerHandsView.Visibility = Visibility.Collapsed;
        }

        private void GameLogButton_Click(object sender, RoutedEventArgs e)
        {
            GameLogDisplay.Visibility = Visibility.Visible;
            PartyDisplay.Visibility = PlayersDisplay.Visibility = PlayerHandsView.Visibility = Visibility.Collapsed;
        }

        private void MyPartyButton_Click(object sender, RoutedEventArgs e)
        {
            PartyDisplay.Visibility = Visibility.Visible;
            GameLogDisplay.Visibility = PlayersDisplay.Visibility = PlayerHandsView.Visibility = Visibility.Collapsed;
        }

        private void TogglePartyVisibilty_Clicked(object sender, RoutedEventArgs e)
        {
            IsSaboteur.Visibility = IsSaboteur.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            IsHidden.Visibility = IsHidden.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace Saboteur.Views.Components
{
    public partial class PlayerHandsView : UserControl
    {
        public event EventHandler RotateButtonTrigger;
        public event EventHandler DiscardButtonTrigger;
        public event EventHandler PlayerHandsTrigger;

        public PlayerHandsView()
        {
            InitializeComponent();
        }

        private void Rotate_Clicked(object sender, RoutedEventArgs e)
        {
            RotateButtonTrigger(sender, e);
        }

        private void Discard_Clicked(object sender, RoutedEventArgs e)
        {
            DiscardButtonTrigger(sender, e);
        }

        private void PlayerHands_Clicked(object sender, RoutedEventArgs e)
        {
            PlayerHandsTrigger(sender, e);
        }

        private void TogglePartyVisibilty_Clicked(object sender, RoutedEventArgs e)
        {
            IsSaboteur.Visibility = IsSaboteur.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            IsHidden.Visibility = IsHidden.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }
    }
}

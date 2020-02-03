using System;
using System.Windows;
using System.Windows.Controls;

namespace Saboteur.Views.Components
{
    public partial class PlayerHandsVerticalView : UserControl
    {
        public event EventHandler RotateButtonTrigger;
        public event EventHandler DiscardButtonTrigger;
        public event EventHandler PlayerHandsTrigger;

        public PlayerHandsVerticalView()
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
    }
}

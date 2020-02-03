using Saboteur.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Saboteur.Views.Components
{
    public partial class AllPlayersView : UserControl
    {
        public event EventHandler PlayerInfoTrigger;

        public AllPlayersView()
        {
            InitializeComponent();
        }

        private void PlayerIcon_Clicked(object sender, RoutedEventArgs e)
        {
            PlayerInfoTrigger(sender, e);
        }
    }
}

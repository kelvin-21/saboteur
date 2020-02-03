using Saboteur.ViewModels;
using System;
using System.Windows;

namespace Saboteur.Views
{
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            DataContext = GameViewModel.CurrentGame.Config;
            NumberOfPlayer.Value = GameViewModel.CurrentGame.Config.MinPlayers;
            this.Closed += ConfigWindow_Closed;
        }

        private void ConfigWindow_Closed(object sender, EventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null) mainWindow.ConfigWindowClosed = true;
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}

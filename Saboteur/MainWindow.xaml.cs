using Saboteur.ViewModels;
using Saboteur.Views;
using System.Windows;
using System.Windows.Controls;

namespace Saboteur
{
    public partial class MainWindow : Window
    {
        public bool ConfigWindowClosed = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewGameButton_Clicked(object sender, RoutedEventArgs e)
        {
            GameViewModel.CurrentGame.StartGame();
            ((Button)sender).Content = "Game Started";
            ((Button)sender).Click -= NewGameButton_Clicked;
        }

        private void ExitButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfigButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ConfigWindowClosed)
            {
                new ConfigWindow().Show();
                ConfigWindowClosed = !ConfigWindowClosed;
            }
        }
    }
}

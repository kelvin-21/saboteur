using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Saboteur.Views.Components
{
    public partial class GameBoardView : UserControl
    {
        public event EventHandler GameBoardTrigger;

        public GameBoardView()
        {
            InitializeComponent();
            foreach (UIElement ChildElement in GameBoard.Children)
            {
                if (ChildElement is Button Card)
                {
                    Card.Click -= GameBoardCard_Click;
                    Card.Click += GameBoardCard_Click;
                }
            }
        }

        private void GameBoardCard_Click(object sender, RoutedEventArgs e)
        {
            GameBoardTrigger(sender, e);
        }
    }
}

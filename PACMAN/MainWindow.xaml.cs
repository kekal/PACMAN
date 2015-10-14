using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PACMAN
{
    public partial class MainWindow : Window
    {

        public static MainWindow Wm;
        public MainWindow()
        {
            InitializeComponent();
            Wm = this;
            BattlefieldCircumstantials.FeelField();
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BattlefieldCircumstantials.AddGhostToCastle();
        }

        
    }
}

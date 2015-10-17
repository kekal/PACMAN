using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PACMAN
{
    public partial class MainWindow
    {

        public static MainWindow Wm;
        public MainWindow()
        {
            InitializeComponent();
            Wm = this;
            BattlefieldCircumstantials.GenerateField();

            
        }

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Button) sender).IsEnabled = false;
            var templist = new[] {"red"/*, "yellow"*/};

            foreach (var s in templist.Where(s => BattlefieldCircumstantials.AddGhostToCastle(s) == null))
            {
                //MessageBox.Show("Can't add to proper place ghost " + s);
            }

            BattlefieldCircumstantials.AddPuckman();


            ((Ghost) BattlefieldCircumstantials._ghostsList[0]).MoveDecision();

            ((Button)sender).IsEnabled = true;
            
            
        }

        
    }
}

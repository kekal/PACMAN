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
            BattlefieldCircumstantials.FeelField();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Button) sender).IsEnabled = false;
            BattlefieldCircumstantials.AddGhostToCastle("red");
            BattlefieldCircumstantials.AddGhostToCastle("yellow");
            ((Button)sender).IsEnabled = true;
            
        }

        
    }
}

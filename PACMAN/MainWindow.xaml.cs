using System.Windows;

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

            Init();
        }

        private static void Init()
        {
            BattlefieldCircumstantials.AddPuckman();

            BattlefieldCircumstantials.AddCherry();
            BattlefieldCircumstantials.AddCherry();

            BattlefieldCircumstantials.AddGhosts();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (BattlefieldCircumstantials.Puckman != null)
            {
                BattlefieldCircumstantials.Puckman.MoveDefine(e);
            }
        }

        public void PlayWin()
        {
            foreach (var child in Battlfield.Children)
            {
                ((UIElement)child).Visibility = Visibility.Collapsed;
            }
            Win.Visibility = Visibility.Visible;
        }

        public void PlayDeath()
        {

            foreach (var child in Battlfield.Children)
            {
                ((UIElement)child).Visibility = Visibility.Collapsed;
            }
            Death.Visibility = Visibility.Visible;
        }
    }
}

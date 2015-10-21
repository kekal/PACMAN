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

        internal void PlayWin()
        {
            GameOver();
            Win.Visibility = Visibility.Visible;
        }

        internal void PlayDeath()
        {
            GameOver();
            Death.Visibility = Visibility.Visible;
        }

        private void GameOver()
        {
            foreach (var ghost in BattlefieldCircumstantials.GhostsList)
            {
                ((Ghost)ghost).Stop();
            }

            foreach (var child in Battlfield.Children)
            {
                ((UIElement)child).Visibility = Visibility.Collapsed;
            }
        }
    }
}

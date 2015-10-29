using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

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

            

            Log.Now = DateTime.Now;
            Log.AddLog("Program start");

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


        public static void InMainDispatch(Action dlg)
        {
            if (Thread.CurrentThread.Name == "Main Thread")
            {
                dlg();
            }
            else
            {
                Wm.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<string>(delegate { dlg(); }), "?");
            }
        }
    }
}

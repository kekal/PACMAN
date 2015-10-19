using System.Linq;
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

            //fillNimbers();

            Init();
        }

        private static void Init()
        {
            BattlefieldCircumstantials.AddPuckman();

            BattlefieldCircumstantials.AddBoost();
            BattlefieldCircumstantials.AddBoost();



            var ghosts = new[] { "red", "yellow" };

            foreach (var ghost in ghosts.Where(s => BattlefieldCircumstantials.AddGhostToCastle(s) == null))
            {
                MessageBox.Show("Can't add to proper place ghost " + ghost);
            }

            BattlefieldCircumstantials.Puckman.MoveDecision();

            ((Ghost)BattlefieldCircumstantials.GhostsList[0]).MoveDecision();
            ((Ghost)BattlefieldCircumstantials.GhostsList[1]).MoveDecision();

       
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (BattlefieldCircumstantials.Puckman != null)
            {
                BattlefieldCircumstantials.Puckman.MoveDefine(e);
            }
        }
    }
}

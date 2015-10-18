using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            BattlefieldCircumstantials.AddPuckman();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Button) sender).IsEnabled = false;
            var templist = new[] {"red", "yellow"};

            foreach (var s in templist.Where(s => BattlefieldCircumstantials.AddGhostToCastle(s) == null))
            {
                MessageBox.Show("Can't add to proper place ghost " + s);
            }

            BattlefieldCircumstantials.Puckman.MoveDecision();

            //((Ghost) BattlefieldCircumstantials._ghostsList[0]).MoveDecision();
            //((Ghost) BattlefieldCircumstantials._ghostsList[1]).MoveDecision();

            ((Button)sender).IsEnabled = true;  
        }

        public void fillNimbers()
        {
            for (var i = 0; i < BattlefieldCircumstantials._fieldElementsArray.GetLength(0); i++)
                for (var j = 0; j < BattlefieldCircumstantials._fieldElementsArray.GetLength(1); j++)
                {
                    var temp = new Label
                    {
                        Width = BattlefieldCircumstantials.Squaresize,
                        Height = BattlefieldCircumstantials.Squaresize,
                        Content = i + ", " + j,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White")),
                        FontSize = 9

                    };


                    Wm.Battlfield.Children.Add(temp);
                    Canvas.SetLeft(temp, i * BattlefieldCircumstantials.Squaresize);
                    Canvas.SetTop(temp, j * BattlefieldCircumstantials.Squaresize);

                }
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

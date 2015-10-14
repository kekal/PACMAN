using System.Windows.Media;

namespace PACMAN
{
    class YellowGhost : Ghost
    {
        private static bool _isInstantiated;

        public YellowGhost()
        {
            if (_isInstantiated) throw new AlreadyInstatiated();

            Name = "YellowGhost";
            var brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFE4FA1F"));
            pathData.Stroke = brush;
            pathData.Fill = brush;

            _isInstantiated = true;
        }
    }
}

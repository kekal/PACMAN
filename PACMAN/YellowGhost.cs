using System.Windows.Media;

namespace PACMAN
{
    class YellowGhost : Ghost
    {
        private static bool _isInstantiated;

        internal YellowGhost()
        {
            if (_isInstantiated) throw new AlreadyInstatiated();

            Name = "YellowGhost";
            var brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFE4FA1F"));
            PathData.Stroke = brush;
            PathData.Fill = brush;

            _isInstantiated = true;
        }
    }
}

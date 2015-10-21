using System.Windows.Media;

namespace PACMAN
{
    class RedGhost : Ghost
    {
        private static bool _isInstantiated;

        internal RedGhost()
        {
            if (_isInstantiated) throw new AlreadyInstatiated();

            Name = "RedGhost";
            var brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("Red"));
            PathData.Stroke = brush;
            PathData.Fill = brush;

            _isInstantiated = true;
        }
    }
}

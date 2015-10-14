using System.Windows.Media;

namespace PACMAN
{
    class RedGhost : Ghost
    {
        private static bool _isInstantiated;

        public RedGhost()
        {
            if (_isInstantiated) throw new AlreadyInstatiated();

            Name = "RedGhost";
            var brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("Red"));
            pathData.Stroke = brush;
            pathData.Fill = brush;

            _isInstantiated = true;
        }
    }
}

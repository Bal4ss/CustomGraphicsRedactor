using System.Windows.Media;

namespace CustomGraphicsRedactor.Moduls.Interface
{
    /// <summary>
    /// Интерфейс группы объектов с основными функциями для отображения описания и его изменения
    /// </summary>
    public interface IPropertiesItem
    {
        Brush FillColor { get; }
        Brush StrokeColor { get; }
        double Thickness { get; }
        void ChangeFillColor(Brush newColor);
        void ChangeThickness(double thickness);
        void ChangeStrokeColor(Brush newColor);
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomGraphicsRedactor.User_Controls
{
    /// <summary>
    /// Логика взаимодействия для ColorPickerControl.xaml
    /// Описание взаимодействия констроллера выбора цвета
    /// </summary>
    public partial class ColorPickerControl : UserControl
    {
        private Brush _color;
        
        /// <param name="color">"Кисть" со стартовым цветом</param>
        public ColorPickerControl(Brush color)
        {
            InitializeComponent();
            _color = color;

            var a = ((Color)_color.GetValue(SolidColorBrush.ColorProperty)).A;
            var g = ((Color)_color.GetValue(SolidColorBrush.ColorProperty)).G;
            var r = ((Color)_color.GetValue(SolidColorBrush.ColorProperty)).R;
            var b = ((Color)_color.GetValue(SolidColorBrush.ColorProperty)).B;

            var _new = Color.FromArgb(a,r,g,b);

            RSlider.Value = _new.R;
            GSlider.Value = _new.G;
            BSlider.Value = _new.B;

            Refresh();
        }

        /// <summary>
        /// Возвращает текущий цвет 
        /// </summary>
        public Brush ValueColor => _color;

        /// <summary>
        /// Функция обновления состояния контроллера
        /// </summary>
        private void Refresh() => ColorHolder.Background = _color;

        /// <summary>
        /// Действие перетаскивания ползунка у полосок выбора цвета 
        /// </summary>
        private void SliderAction(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var rValue = Math.Round(RSlider.Value);
            var gValue = Math.Round(GSlider.Value);
            var bValue = Math.Round(BSlider.Value);

            _color = new SolidColorBrush(
                Color.FromRgb((byte)rValue, (byte)gValue, (byte)bValue));

            RGBValueText.Text = $"R:{rValue} G:{gValue} B:{bValue}";
            Refresh();
        }
    }
}

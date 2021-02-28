using System.Windows;
using CustomGraphicsRedactor.Moduls;

namespace CustomGraphicsRedactor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Функция отлова сочетания клавишь Ctrl + Z и Delete
        /// </summary>
        private void WindowKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control &&
                e.Key == System.Windows.Input.Key.Z) CurrentSettings.Cancel();
            else if (e.Key == System.Windows.Input.Key.Delete) CurrentSettings.Remove();
        }

        /// <summary>
        /// Функция отлова изменения размеров экрана
        /// </summary>
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
            => CurrentSettings.MoveDelegate?.Invoke();
    }
}

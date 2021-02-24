using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using CustomGraphicsRedactor.Moduls;
using CustomGraphicsRedactor.Moduls.Interface;
using CustomGraphicsRedactor.User_Controls.Template;

namespace CustomGraphicsRedactor.User_Controls
{
    /// <summary>
    /// Логика взаимодействия для ToolBarControl.xaml
    /// Описание взаимодействия кнопок выбора режима работы
    /// </summary>
    public partial class ToolBarControl : UserControl, IRefreshControls
    {
        private Dictionary<ECanvasMode, ChangeModeToolButton> _buttons;

        public ToolBarControl()
        {
            InitializeComponent();
            _buttons = new Dictionary<ECanvasMode, ChangeModeToolButton>() {
                { HandToolButton.Mode, HandToolButton },
                { RectangleToolButton.Mode, RectangleToolButton },
                { BrokenLineToolButton.Mode, BrokenLineToolButton }
            };

            CurrentSettings.RefreshDelegate += Refresh;
            CurrentSettings.ChangeMode(ECanvasMode.Hand);
        }

        /// <summary>
        /// Функция обновления состояния контроллера
        /// </summary>
        public void Refresh()
        {
            _buttons
                .Where(c => c.Key != CurrentSettings.Mode)
                .Select(c => c.Value)
                .ToList()
                .ForEach(c => { c.IsEnabled = true; });

            _buttons[CurrentSettings.Mode.Value].IsEnabled = false;

            if (CurrentSettings.GetItem == null) DeleteButton.IsEnabled = false;
            else DeleteButton.IsEnabled = true;
        }

        /// <summary>
        /// Действие инициирующее удаление объекта
        /// </summary>
        private void DeleteButtonEvent(object sender, RoutedEventArgs e)
            => CurrentSettings.Remove();

        /// <summary>
        /// Действие выбора режима работы
        /// </summary>
        private void ToolButtonEvent(object sender, RoutedEventArgs e)
            => CurrentSettings.ChangeMode(((ChangeModeToolButton)sender).Mode);
    }
}

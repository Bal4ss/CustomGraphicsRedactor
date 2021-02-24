using System.Windows;
using System.Windows.Controls;
using CustomGraphicsRedactor.Moduls;

namespace CustomGraphicsRedactor.User_Controls.Template
{
    /// <summary>
    /// Логика взаимодействия кнопок выбора режима
    /// </summary>
    class ChangeModeToolButton : Button
    {
        public ChangeModeToolButton() { }

        /// <summary>
        /// Свойство описувающее тип режима работы
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                "Mode",
                typeof(ECanvasMode),
                typeof(ECanvasMode));

        /// <summary>
        /// Возвращает/задает тип режима работы
        /// </summary>
        public ECanvasMode Mode
        {
            get { return (ECanvasMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
    }
}

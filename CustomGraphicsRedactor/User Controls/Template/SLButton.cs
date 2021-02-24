using System.Windows;
using System.Windows.Controls;
using CustomGraphicsRedactor.Moduls;

namespace CustomGraphicsRedactor.User_Controls.Template
{
    /// <summary>
    /// Логика взаимодействия кнопок сохранения/загрузки/экспорта
    /// </summary>
    class SLButton : Button
    {
        public delegate void ChangeEnabled(bool isEnabled);
        public static ChangeEnabled SetEnabled;

        public SLButton()
        {
            SetEnabled += SetButtonEnabled;
        }

        /// <summary>
        /// Свойство описувающее тип операции
        /// </summary>
        public static readonly DependencyProperty SaveProperty =
            DependencyProperty.Register(
                "SLStatus",
                typeof(ESaveLoadStatus),
                typeof(ESaveLoadStatus));

        /// <summary>
        /// Возвращает/задает тип операции
        /// </summary>
        public ESaveLoadStatus SLStatus
        {
            get { return (ESaveLoadStatus)GetValue(SaveProperty); }
            set { SetValue(SaveProperty, value); }
        }

        /// <summary>
        /// Функция изменения доступа к операции
        /// </summary>
        /// <param name="isEnabled">Значение доступа (true если доступна)</param>
        private void SetButtonEnabled(bool isEnabled) => IsEnabled = isEnabled;

        /// <summary>
        /// Переопределенная функция поведения при клике
        /// </summary>
        protected override void OnClick()
        {
            SetEnabled.Invoke(false);
            SaveLoadImplement.SLERoute(SLStatus);
            SetEnabled.Invoke(true);
        }
    }
}

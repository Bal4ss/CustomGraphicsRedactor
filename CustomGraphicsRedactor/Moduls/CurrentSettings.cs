using CustomGraphicsRedactor.Moduls.Interface;

namespace CustomGraphicsRedactor.Moduls
{
    /// <summary>
    /// Логика взаимодействия элементов приложения (контекстные настройки редактора)
    /// </summary>
    public static class CurrentSettings
    {
        public delegate void RefreshDel();
        public delegate void MoveAction();
        public delegate void AppendItem(ICanvasItem item);

        private static bool _isDraw;
        private static MoveAction _move;
        private static AppendItem _append;
        private static RefreshDel _refresh;
        private static ICanvasItem _currentItem;
        private static ECanvasMode? _currentMode;
        private static CancelImplement _cancelImplement;

        static CurrentSettings()
        {
            _isDraw = false;
            _currentItem = null;
            _cancelImplement = new CancelImplement();
        }

        public static bool IsDraw => _isDraw;

        /// <summary>
        /// Возвращает режим взаимодействия с холстом и объектами на нем
        /// </summary>
        public static ECanvasMode? Mode => _currentMode;

        /// <summary>
        /// Возвращает текущий объект для манипуляций
        /// </summary>
        public static ICanvasItem GetItem => _currentItem;

        /// <summary>
        /// Возвращает/задает делегат для взаимодействия методов описания "движения"
        /// </summary>
        public static MoveAction MoveDelegate { get { return _move; } set { _move = value; } }

        /// <summary>
        /// Возвращает/задает делегат для взаимодействия методов описания добавления объектов
        /// </summary>
        public static AppendItem AppendDelegate { get { return _append; } set { _append = value; } }

        /// <summary>
        /// Возвращает/задает делегат для обновления состояния элементов описания объекта
        /// </summary>
        public static RefreshDel RefreshDelegate { get { return _refresh; } set { _refresh = value; } }

        /// <summary>
        /// Функция удаления текущего объекта
        /// </summary>
        public static void Remove()
        {
            AppendNewAction(ECancelTypes.Remove, _currentItem);
            _currentItem.Remove();
            _currentItem = null;
            _refresh?.Invoke();
            _move?.Invoke();
        }

        /// <summary>
        /// Функция инициации отмены последнего действия
        /// </summary>
        public static void Cancel()
        {
            _cancelImplement.Cancel();
            _refresh?.Invoke();
        }

        /// <summary>
        /// Функция очистки списка действий
        /// </summary>
        public static void ClearCancel() => _cancelImplement.Clear();

        public static void SetIsDraw(bool isDraw = false) => _isDraw = isDraw;

        /// <summary>
        /// Функция изменения текущего объекта для манипуляций
        /// </summary>
        /// <param name="currentItem">Новый объект</param>
        public static void SetCurrentItem(ICanvasItem currentItem = null)
        {
            _currentItem?.Deselect();
            _currentItem = currentItem;
            _currentItem?.Select();
            _refresh?.Invoke();
        }

        /// <summary>
        /// Функция добавления нового действия в список действий для отмены
        /// </summary>
        /// <param name="cType">Тип действия</param>
        /// <param name="cObject">Объект и условия для отмены</param>
        public static void AppendNewAction(ECancelTypes cType, object cObject)
            => _cancelImplement.AppendNewAction(new CancelEvents(cType, cObject));

        /// <summary>
        /// Функция изменения режима взаимодействия с холстом и объектами на нем
        /// </summary>
        /// <param name="currentMode">Новый режим</param>
        public static void ChangeMode(ECanvasMode currentMode = ECanvasMode.Hand)
        {
            _currentMode = currentMode;
            _currentItem?.Deselect();
            _currentItem = null;
            _refresh?.Invoke();
        }
    }
}

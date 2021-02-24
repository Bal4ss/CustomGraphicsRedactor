using System.Windows;

namespace CustomGraphicsRedactor.Moduls
{
    /// <summary>
    /// Модель "точки" у группы объектов на холсте
    /// </summary>
    public class CustPoint
    {
        private Point _point;
        private bool _isSelected;

        /// <param name="point">"Точка"</param>
        public CustPoint(Point point)
        {
            _point = point;
            _isSelected = false;
        }

        /// <summary>
        /// Возвращает "точку"
        /// </summary>
        public Point Point => _point;

        /// <summary>
        /// Возвращает статус "точки"
        /// </summary>
        public bool IsSelected => _isSelected;

        /// <summary>
        /// Функция "выбора" (изменение статуса на положительный) "точки"
        /// </summary>
        public void Select() => _isSelected = true;

        /// <summary>
        /// Функция "снятия выбора" (изменение статуса на отрицательный) "точки"
        /// </summary>
        public void Deselect() => _isSelected = false;

        /// <summary>
        /// Функция изменения значения "точки"
        /// </summary>
        /// <param name="_new">"Точка" с новым значением</param>
        public void ChangePoint(Point _new) => _point = new Point(_new.X, _new.Y);
    }
}

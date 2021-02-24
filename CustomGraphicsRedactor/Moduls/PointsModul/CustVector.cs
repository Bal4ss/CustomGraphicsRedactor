using System.Windows;

namespace CustomGraphicsRedactor.Moduls
{
    /// <summary>
    /// Логика поведения "точек" при движении
    /// </summary>
    public class CustVector
    {
        private Vector _delta;
        private Point _prevPoint;

        /// <param name="prev">"Точка" с предыдущими координатами</param>
        /// <param name="actual">"Точка" с актуальными координатами</param>
        public CustVector(Point prev, Point actual)
        {
            _prevPoint = prev;
            _delta = prev - actual;
        }

        /// <summary>
        /// Возвращает вектор "движения"
        /// </summary>
        public Vector Delta => _delta;

        /// <summary>
        /// Возвращает "точку" с предыдущими координатами
        /// </summary>
        public Point PrevusePoint => _prevPoint;
    }
}

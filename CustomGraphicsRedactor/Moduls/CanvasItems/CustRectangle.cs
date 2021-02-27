using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using CustomGraphicsRedactor.Moduls.Interface;

namespace CustomGraphicsRedactor.Moduls.CanvasItems
{
    /// <summary>
    /// Логика поведения объекта "Прямоугольник с заливкой"
    /// </summary>
    class CustRectangle : UIElement, ICanvasItem, IResizableItem, IRectangleItem
    {
        private double _width;
        private double _height;
        private bool _isSelected;
        private Brush _fillColor;
        private Brush _strokeColor;
        private CustPoint _tmpPoint;
        private double _strokeThickness;
        private List<CustPoint> _points;

        /// <param name="point">Стартовое положение объекта</param>
        public CustRectangle(Point point)
        {
            _width = 5;
            _height = 5;
            _isSelected = false;
            _strokeThickness = 1;
            _fillColor = Brushes.Green;
            _strokeColor = Brushes.Black;
            _points = new List<CustPoint>(1);
            _points.Add(new CustPoint(point));
        }

        /// <summary>
        /// Возвращает ширину объекта
        /// </summary>
        public double Width => _width;

        /// <summary>
        /// Возвращает высоту объекта
        /// </summary>
        public double Height => _height;

        /// <summary>
        /// Возвращает "Кисть" с цветом заполнения объекта
        /// </summary>
        public Brush FillColor => _fillColor;

        /// <summary>
        /// Возвращает статус объекта
        /// </summary>
        public bool IsSelected => _isSelected;

        /// <summary>
        /// Возвращает "Кисть" с цветом линий объекта
        /// </summary>
        public Brush StrokeColor => _strokeColor;

        /// <summary>
        /// Возвращает тощину линий объекта
        /// </summary>
        public double Thickness => _strokeThickness;

        /// <summary>
        /// Возвращает описание положения объекта
        /// </summary>
        public List<CustPoint> GetPoints => _points;

        /// <summary>
        /// Функция "выбора" (изменение статуса на положительный) объекта
        /// </summary>
        public void Select()
        {
            _isSelected = true;
            var _canvas = (Canvas)VisualParent;
            if (_canvas != null) {
                _canvas.Children.Remove(this);
                _canvas.Children.Add(this);
            }
            InvalidateVisual();
        }

        /// <summary>
        /// Функция удаления объекта
        /// </summary>
        public void Remove()
            => ((Canvas)VisualParent).Children.Remove(this);

        /// <summary>
        /// Функция "снятия выбора" (изменение статуса на отрицательный) объекта
        /// </summary>
        public void Deselect()
        {
            _isSelected = false;
            InvalidateVisual();
        }
        public void AddTmpPoint(CustPoint point = null)
        {
            _tmpPoint = point;
            InvalidateVisual();
        }

        public void AddNewPoint(CustPoint point)
        {
            if (_tmpPoint != null)
            {
                if (_tmpPoint.Point.X > _points[0].Point.X)
                    _width = _tmpPoint.Point.X - _points[0].Point.X;
                else
                {
                    _width = _points[0].Point.X - _tmpPoint.Point.X;
                    _points[0].ChangePoint(new Point(
                        _tmpPoint.Point.X,
                        _points[0].Point.Y));
                }

                if (_tmpPoint.Point.Y > _points[0].Point.Y)
                    _height = _tmpPoint.Point.Y - _points[0].Point.Y;
                else
                {
                    _height = _points[0].Point.Y - _tmpPoint.Point.Y;
                    _points[0].ChangePoint(new Point(
                        _points[0].Point.X,
                        _tmpPoint.Point.Y));
                }
            }

            _tmpPoint = null;
            InvalidateVisual();
        }

        /// <summary>
        /// Функция "движения" объекта
        /// </summary>
        /// <param name="vector">Вектор движения</param>
        public void Move(CustVector vector)
        {
            _points[0].ChangePoint(new Point(
                _points[0].Point.X - vector.Delta.X,
                _points[0].Point.Y - vector.Delta.Y));
            InvalidateVisual();
        }

        /// <summary>
        /// Функция изменения ширины объекта
        /// </summary>
        /// <param name="width">Значение ширины</param>
        public void ChangeWidth(double width)
        {
            _width = width;
            InvalidateVisual();
        }

        /// <summary>
        /// Функция изменения высоты объекта
        /// </summary>
        /// <param name="height"></param>
        public void ChangeHeight(double height)
        {
            _height = height;
            InvalidateVisual();
        }

        /// <summary>
        /// Функция изменения цвета заполнения объекта
        /// </summary>
        /// <param name="newColor">"Кисть" с цветом</param>
        public void ChangeFillColor(Brush newColor)
        {
            _fillColor = newColor;
            InvalidateVisual();
        }

        /// <summary>
        /// Функция изменения описания положения объекта
        /// </summary>
        /// <param name="points">Описание положения объекта</param>
        public void SetPoints(List<CustPoint> points)
        {
            _points = points;
            InvalidateVisual();
        }

        /// <summary>
        /// Функция изменения толщины объекта
        /// </summary>
        /// <param name="thickness">Значение толщины</param>
        public void ChangeThickness(double thickness)
        {
            _strokeThickness = thickness;
            InvalidateVisual();
        }

        /// <summary>
        /// Функция изменения цвета линий объекта
        /// </summary>
        /// <param name="newColor">"Кисть" с цветом</param>
        public void ChangeStrokeColor(Brush newColor)
        {
            _strokeColor = newColor;
            InvalidateVisual();
        }

        /// <summary>
        /// Функция "отрисовки" объекта
        /// </summary>
        /// <param name="drawingContext">Контекст "отрисовки"</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var width = _width;
            var height = _height;
            var pointX = _points[0].Point.X;
            var pointY = _points[0].Point.Y;

            if (_tmpPoint != null)
            {
                if (_tmpPoint.Point.X > _points[0].Point.X)
                    width = _tmpPoint.Point.X - _points[0].Point.X;
                else
                {
                    width = _points[0].Point.X - _tmpPoint.Point.X;
                    pointX = _tmpPoint.Point.X;
                }

                if (_tmpPoint.Point.Y > _points[0].Point.Y)
                    height = _tmpPoint.Point.Y - _points[0].Point.Y;
                else
                {
                    height = _points[0].Point.Y - _tmpPoint.Point.Y;
                    pointY = _tmpPoint.Point.Y;
                }
            }

            var _drawingGroup = new DrawingGroup();
            var rectGeom1 =
                new GeometryDrawing(
                    _fillColor,
                    new Pen(_strokeColor, _strokeThickness),
                    new RectangleGeometry(new Rect(
                        pointX,
                        pointY,
                        width,
                        height)
                    )
                );

            _drawingGroup.Children.Add(rectGeom1);

            if (_isSelected)
            {
                var selectedColor = new SolidColorBrush()
                {
                    Color = Colors.LightGray,
                    Opacity = 0.5
                };

                var rectGeom2 =
                new GeometryDrawing(
                    selectedColor,
                    new Pen(selectedColor, _strokeThickness),
                    new RectangleGeometry(new Rect(
                        pointX - 3,
                        pointY - 3,
                        width + 6,
                        height + 6)
                    )
                );

                _drawingGroup.Children.Add(rectGeom2);
            }

            drawingContext.DrawDrawing(_drawingGroup);
        }
    }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using CustomGraphicsRedactor.Moduls.Interface;

namespace CustomGraphicsRedactor.Moduls.CanvasItems
{
    /// <summary>
    /// Логика поведения объекта "Ломаная линия"
    /// </summary>
    class CustBrokenLine : UIElement, ICanvasItem, IResizableItem, IPropertiesItem
    {
        private bool _isSelected;
        private Brush _fillColor;
        private Brush _strokeColor;
        private CustPoint _tmpPoint;
        private double _strokeThickness;
        private List<CustPoint> _points;

        /// <param name="point">Стартовое положение объекта</param>
        public CustBrokenLine(Point point)
        {
            _isSelected = false;
            _strokeThickness = 2;
            _fillColor = Brushes.Red;
            _strokeColor = Brushes.Black;

            _points = new List<CustPoint>();
            _points.Add(new CustPoint(point));
        }

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
            for (int i = 0; i < _points.Count(); i++)
                _points[i].Deselect();
            InvalidateVisual();
        }

        /// <summary>
        /// Функция обновления вспомогательной точки отрисовки
        /// </summary>
        /// <param name="point">Новая "точка"</param>
        public void AddTmpPoint(CustPoint point = null)
        {
            _tmpPoint = point;
            InvalidateVisual();
        }

        /// <summary>
        /// Функция обновления точки отрисовки
        /// </summary>
        /// <param name="point">Новая "точка"</param>
        public void AddNewPoint(CustPoint point)
        {
            _tmpPoint = null;
            _points.Add(point);
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
            _tmpPoint = null;
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
        /// Функция "движения" объекта
        /// </summary>
        /// <param name="vector">Вектор движения</param>
        public void Move(CustVector vector)
        {
            var _changePoints = new Dictionary<int,CustPoint>(_points.Count());
            for (int i = 0; i < _points.Count(); i++) {
                var localPointChange = Math.Pow(_points[i].Point.X - vector.PrevusePoint.X, 2.0) +
                                       Math.Pow(_points[i].Point.Y - vector.PrevusePoint.Y, 2.0) <
                                       Math.Pow(10d, 2.0);
                if (localPointChange) _changePoints.Add(i, _points[i]);
            }

            if (_changePoints.Count() > 0) {
                var tmpSelectedCount = _changePoints
                    .Where(c => c.Value.IsSelected)
                    .ToList();
                var changeIndex = _changePoints.FirstOrDefault().Key;
                if (tmpSelectedCount.Count() == 1 && _changePoints.Count() > 1) {
                    changeIndex = tmpSelectedCount.FirstOrDefault().Key;
                }
                else if (tmpSelectedCount.Count() == 0 && _changePoints.Count() > 1) {
                    changeIndex = _changePoints.FirstOrDefault().Key;

                    var tmpPointX = Math.Abs(vector.PrevusePoint.X);
                    var tmpPointY = Math.Abs(vector.PrevusePoint.Y);
                    var absPrevPX = tmpPointX;
                    var absPrevPY = tmpPointY;

                    foreach (var a in _changePoints) {
                        var absPointAX = Math.Abs(a.Value.Point.X);
                        var absPointAY = Math.Abs(a.Value.Point.Y);
                        var tmpResX = Math.Abs(absPointAX - absPrevPX);
                        var tmpResY = Math.Abs(absPointAY - absPrevPY);
                        if (tmpResX < tmpPointX && tmpResY < tmpPointY) {
                            tmpPointX = tmpResX;
                            tmpPointY = tmpResY;
                            changeIndex = a.Key;
                        }
                    }
                }
                _points[changeIndex].Select();
                _points[changeIndex].ChangePoint(new Point(
                    _points[changeIndex].Point.X - vector.Delta.X,
                    _points[changeIndex].Point.Y - vector.Delta.Y));
            }
            else {
                for (int i = 0; i < _points.Count(); i++) {
                    _points[i].ChangePoint(new Point(
                        _points[i].Point.X - vector.Delta.X,
                        _points[i].Point.Y - vector.Delta.Y));
                }
            }
            InvalidateVisual();
        }

        /// <summary>
        /// Функция "отрисовки" объекта
        /// </summary>
        /// <param name="drawingContext">Контекст "отрисовки"</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var _drawingGroup = new DrawingGroup();

            var drawPoints = new List<CustPoint>(_points);

            if (_tmpPoint != null) drawPoints.Add(_tmpPoint);

            for (int i = 1; i < drawPoints.Count(); i++)
            {
                _drawingGroup.Children.Add(
                    new GeometryDrawing(
                        _strokeColor,
                        new Pen(_strokeColor, _strokeThickness + 5),
                        new LineGeometry(drawPoints[i - 1].Point, drawPoints[i].Point)
                    ));
            }

            for (int i = 0; i < drawPoints.Count(); i++)
            {
                var ellipseThickness = (_strokeThickness + 5 ) / 3;
                _drawingGroup.Children.Add(
                    new GeometryDrawing(
                        _strokeColor,
                        new Pen(_strokeColor, ellipseThickness),
                        new EllipseGeometry(drawPoints[i].Point, ellipseThickness, ellipseThickness)
                    ));
            }

            for (int i = 1; i < drawPoints.Count(); i++)
            {
                _drawingGroup.Children.Add(
                    new GeometryDrawing(
                        _fillColor,
                        new Pen(_fillColor, _strokeThickness),
                        new LineGeometry(drawPoints[i - 1].Point, drawPoints[i].Point)
                    ));
            }

            for (int i = 0; i < drawPoints.Count(); i++)
            {
                var ellipseThickness = _strokeThickness / 3;
                _drawingGroup.Children.Add(
                    new GeometryDrawing(
                        _fillColor,
                        new Pen(_fillColor, ellipseThickness),
                        new EllipseGeometry(drawPoints[i].Point, ellipseThickness, ellipseThickness)
                    ));
            }

            if (_isSelected)
            {
                var selectedColor = new SolidColorBrush()
                {
                    Color = Colors.LightGray,
                    Opacity = 0.5
                };
                for (int i = 1; i < drawPoints.Count(); i++)
                {
                    _drawingGroup.Children.Add(
                        new GeometryDrawing(
                            selectedColor,
                            new Pen(selectedColor, _strokeThickness + 12),
                            new LineGeometry(drawPoints[i - 1].Point, drawPoints[i].Point)
                        ));
                }
                for (int i = 0; i < drawPoints.Count(); i++)
                {
                    var tmpThickness = (12 + _strokeThickness) / 2;
                    _drawingGroup.Children.Add(
                        new GeometryDrawing(
                            Brushes.Orange,
                            new Pen(Brushes.Orange, 1),
                            new EllipseGeometry(drawPoints[i].Point, tmpThickness, tmpThickness)
                        ));
                }
            }

            drawingContext.DrawDrawing(_drawingGroup);
        }
    }
}

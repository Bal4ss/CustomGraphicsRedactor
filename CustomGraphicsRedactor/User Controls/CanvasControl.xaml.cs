using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using CustomGraphicsRedactor.Moduls;
using CustomGraphicsRedactor.Moduls.Interface;
using CustomGraphicsRedactor.Moduls.CanvasItems;

namespace CustomGraphicsRedactor.User_Controls
{
    /// <summary>
    /// Логика взаимодействия для CanvasControl.xaml
    /// Описание взаимодействия с "холстом"
    /// </summary>
    public partial class CanvasControl : UserControl
    {
        private bool _isMove;
        private Point _prevuseMousePosition;
        private List<CustPoint> _oldPosition;

        public CanvasControl()
        {
            InitializeComponent();
            _isMove = false;

            SaveLoadImplement.SetContext(MainCanvas);
            CurrentSettings.MoveDelegate += ResizeCanvas;
            CurrentSettings.AppendDelegate += AppendNewItem;
        }

        /// <summary>
        /// Возвращает список объектов холста (с интерфесом ICanvasItem)
        /// </summary>
        private List<ICanvasItem> _items
            => MainCanvas.Children.Cast<ICanvasItem>().ToList();

        /// <summary>
        /// Возвращает список выбранных объектов
        /// UPD: работа с множественным выбором объектов будет реализована в более поздних версиях программ
        /// </summary>
        private List<ICanvasItem> IsSelectedItems
            => _items.Where(c => c.IsSelected).ToList();

        /// <summary>
        /// Функция вычисления/изменения размеров холста
        /// </summary>
        private void ResizeCanvas()
        {
            double left = 0d, top = 0d, right = ActualWidth, bottom = ActualHeight;
            _items.ForEach(c => {
                c.GetPoints.ForEach(b => {
                    if (b.Point.X < 0 && b.Point.X < left) left = b.Point.X;
                    if (b.Point.Y < 0 && b.Point.Y < top) top = b.Point.Y;

                    if (c is IRectangleItem rci) {
                        var _tmpRight = b.Point.X + rci.Width;
                        var _tmpBottom = b.Point.Y + rci.Height;

                        if (_tmpRight > right) right = _tmpRight;
                        if (_tmpBottom > bottom) bottom = _tmpBottom;
                    }
                    else {
                        if (b.Point.X > right) right = b.Point.X;
                        if (b.Point.Y > bottom) bottom = b.Point.Y;
                    }
                });
            });

            if (left != 0 || top != 0) {
                _items.ForEach(c => {
                    var tmpPoints = c.GetPoints;
                    tmpPoints.ForEach(b =>
                        b.ChangePoint(new Point(
                            b.Point.X - left,
                            b.Point.Y - top)));
                    c.SetPoints(tmpPoints);
                });
            }

            MainCanvas.Width = (right - left) + 5;
            MainCanvas.Height = (bottom - top) + 5;
        }

        /// <summary>
        /// Функция вычисление/инициации добавления нового объекта
        /// </summary>
        /// <param name="pos">Координаты добавления нового объекта</param>
        private void AddNewItem(Point pos)
        {
            ICanvasItem _newObject = null;
            switch (CurrentSettings.Mode)
            {
                case ECanvasMode.BrokenLine:
                    _newObject = new CustBrokenLine(pos);
                    break;
                case ECanvasMode.Rectangle:
                    _newObject = new CustRectangle(pos);
                    break;
            }

            if (_newObject == null) return;

            AppendNewItem(_newObject);

            CurrentSettings.AppendNewAction(ECancelTypes.Add, _newObject);
            CurrentSettings.ChangeMode();
        }

        /// <summary>
        /// Функция инициации движения выбранного объекта 
        /// </summary>
        /// <param name="vector">Вектор движения</param>
        private void ForceMove(CustVector vector)
        {
            _isMove = true;
            IsSelectedItems.ForEach(c => c.Move(vector));
            CurrentSettings.MoveDelegate.Invoke();
        }

        /// <summary>
        /// Добавление нового объекта
        /// </summary>
        /// <param name="newObject">Объект для добавления</param>
        private void AppendNewItem(ICanvasItem newObject)
        {
            _items.Add(newObject);
            MainCanvas.Children.Add((UIElement)newObject);
            CurrentSettings.MoveDelegate.Invoke();
        }

        /// <summary>
        /// Действие отжатия левой кнопки мыши на холсте
        /// </summary>
        private void MainCanvasMouseUp(object sender, MouseEventArgs e)
        {
            if (_isMove) {
                var pos = e.GetPosition(MainCanvas);
                var tmpObject = (ICanvasItem)CurrentSettings.GetItem;

                var tmpCancelObject = new object[] {
                    tmpObject,
                    _oldPosition
                };

                CurrentSettings.AppendNewAction(ECancelTypes.Move, tmpCancelObject);
                _isMove = false;
            }
        }

        /// <summary>
        /// Действие передвижение мыши по холсту
        /// </summary>
        private void MainCanvasMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MainCanvas);
            var selectedCount = IsSelectedItems.Count();
            var isMouseDown = e.LeftButton == MouseButtonState.Pressed;

            if (isMouseDown && selectedCount > 0)
                ForceMove(new CustVector(_prevuseMousePosition, pos));

            _prevuseMousePosition = pos;
        }

        /// <summary>
        /// Действие нажатия левой кнопки мыши по холсту
        /// </summary>
        private void MainCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentSettings.SetCurrentItem();

            _items.ForEach(c => c.Deselect());
            var pos = e.GetPosition(MainCanvas);
            var clickResult = VisualTreeHelper.HitTest(MainCanvas, pos);

            if (clickResult.VisualHit is ICanvasItem item &&
                CurrentSettings.Mode == ECanvasMode.Hand) {
                item.Select();
                _prevuseMousePosition = pos;

                _oldPosition = new List<CustPoint>();
                item.GetPoints.ForEach(c =>
                    _oldPosition.Add(new CustPoint(c.Point)));

                CurrentSettings.SetCurrentItem((IPropertiesItem)item);
            }
            else AddNewItem(pos);
        }
    }
}

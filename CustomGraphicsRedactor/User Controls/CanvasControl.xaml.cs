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

                    if (b.Point.X > right) right = b.Point.X;
                    if (b.Point.Y > bottom) bottom = b.Point.Y;
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
            _items.ForEach(c => c.Deselect());

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

            if (_newObject == null) {
                CurrentSettings.SetCurrentItem();
                CurrentSettings.SetIsDraw(false);
                return;
            }

            AppendNewItem(_newObject);
            CurrentSettings.SetIsDraw(true);
            CurrentSettings.SetCurrentItem(_newObject);

            CurrentSettings.AppendNewAction(ECancelTypes.Add, _newObject);
        }

        /// <summary>
        /// Функция инициации движения выбранного объекта 
        /// </summary>
        /// <param name="vector">Вектор движения</param>
        private void ForceMove(CustVector vector)
        {
            if (CurrentSettings.Mode != ECanvasMode.Hand) return;
            _isMove = true;
            IsSelectedItems.ForEach(c => c.Move(vector));
            CurrentSettings.MoveDelegate?.Invoke();
        }

        /// <summary>
        /// Функция добавления вспомогательной точки отрисовки
        /// </summary>
        /// <param name="point">Новая "точка"</param>
        private void AddTmpPoint(CustPoint point)
        {
            if (CurrentSettings.GetItem == null) return;
            else if (CurrentSettings.GetItem is IResizableItem) {
                var item = (IResizableItem)CurrentSettings.GetItem;
                item.AddTmpPoint(point);
            }
            CurrentSettings.MoveDelegate?.Invoke();
        }

        /// <summary>
        /// Функция добавления точки отрисовки
        /// </summary>
        /// <param name="point">Новая "точка"</param>
        private void AddNewPoint(CustPoint point)
        {
            if (CurrentSettings.GetItem == null) return;
            else if (CurrentSettings.GetItem is IResizableItem item) {

                var tmpCancelObject = new object[] {
                    item,
                    new List<CustPoint>(((ICanvasItem)item).GetPoints)
                };

                item.AddNewPoint(point);

                CurrentSettings.AppendNewAction(ECancelTypes.Move, tmpCancelObject);
            }
            CurrentSettings.MoveDelegate?.Invoke();
        }

        /// <summary>
        /// Добавление нового объекта
        /// </summary>
        /// <param name="newObject">Объект для добавления</param>
        private void AppendNewItem(ICanvasItem newObject)
        {
            newObject.Select();
            MainCanvas.Children.Add((UIElement)newObject);
            CurrentSettings.MoveDelegate?.Invoke();
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
            var isDraw = CurrentSettings.IsReDraw;
            var selectedCount = IsSelectedItems.Count();
            var isMouseDown = e.LeftButton == MouseButtonState.Pressed;

            if (isMouseDown && selectedCount > 0)
                ForceMove(new CustVector(_prevuseMousePosition, pos));
            else if (isDraw && selectedCount > 0) 
                AddTmpPoint(new CustPoint(pos));

            _prevuseMousePosition = pos;
        }

        /// <summary>
        /// Действие нажатия левой кнопки мыши по холсту
        /// </summary>
        private void MainCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(MainCanvas);
            var isDraw = CurrentSettings.IsReDraw;
            var clickResult = VisualTreeHelper.HitTest(MainCanvas, pos);

            if (e.ClickCount > 1 &&
                clickResult.VisualHit is ICanvasItem check) {
                if (check.IsSelected) {
                    CurrentSettings.SetIsDraw(!isDraw);
                    if (isDraw) {
                        CurrentSettings.SetCurrentItem();
                        CurrentSettings.ChangeMode();
                        return;
                    }
                }
            }

            if (!isDraw && clickResult.VisualHit is ICanvasItem item &&
                CurrentSettings.Mode == ECanvasMode.Hand) {
                item.Select();
                _prevuseMousePosition = pos;

                _oldPosition = new List<CustPoint>();
                item.GetPoints.ForEach(c =>
                    _oldPosition.Add(new CustPoint(c.Point)));

                CurrentSettings.SetCurrentItem(item);
            }
            else if (isDraw) AddNewPoint(new CustPoint(pos));
            else AddNewItem(pos);
        }
    }
}

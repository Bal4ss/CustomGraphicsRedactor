using System.Windows.Media;
using System.Collections.Generic;
using CustomGraphicsRedactor.Moduls.Interface;
using CustomGraphicsRedactor.Moduls.CanvasItems;

namespace CustomGraphicsRedactor.Moduls
{
    /// <summary>
    /// Логика отмены действий в системе
    /// </summary>
    class CancelImplement
    {
        private static Stack<CancelEvents> _cancelEvents;

        public CancelImplement()
        {
            _cancelEvents = new Stack<CancelEvents>();
        }

        /// <summary>
        /// Функция очищает список действий
        /// </summary>
        public void Clear() => _cancelEvents.Clear();

        /// <summary>
        /// Функция определяет последнее действие и инициирует его отмену
        /// </summary>
        public void Cancel()
        {
            if (_cancelEvents.Count == 0) return;

            var tmpCEvent = _cancelEvents.Pop();

            object[] tmp = null;
            if (tmpCEvent.CancelObject is object[])
                tmp = (object[])tmpCEvent.CancelObject;

            if (tmpCEvent.CancelObject == null) return;

            switch (tmpCEvent.CancelType)
            {
                case ECancelTypes.Add:
                    CancelAdd((ICanvasItem)tmpCEvent.CancelObject);
                    break;
                case ECancelTypes.Remove:
                    CancelRemove((ICanvasItem)tmpCEvent.CancelObject);
                    break;
                case ECancelTypes.Move:
                    CancelMove((ICanvasItem)tmp[0], (List<CustPoint>)tmp[1]);
                    CurrentSettings.MoveDelegate?.Invoke();
                    break;
                case ECancelTypes.Width:
                    CancelWidth((IRectangleItem)tmp[0], (double)tmp[1]);
                    CurrentSettings.MoveDelegate?.Invoke();
                    break;
                case ECancelTypes.Height:
                    CancelHeight((IRectangleItem)tmp[0], (double)tmp[1]);
                    CurrentSettings.MoveDelegate?.Invoke();
                    break;
                case ECancelTypes.FillColor:
                    CancelFillColor((IPropertiesItem)tmp[0], (Brush)tmp[1]);
                    break;
                case ECancelTypes.Thickness:
                    CancelThickness((IPropertiesItem)tmp[0], (double)tmp[1]);
                    break;
                case ECancelTypes.StrokeColor:
                    CancelStrokeColor((IPropertiesItem)tmp[0], (Brush)tmp[1]);
                    break;
                case ECancelTypes.AddNewPoint:
                    CancelAddNewPoint((ICanvasItem)tmp[0], (List<CustPoint>)tmp[1]);
                    CurrentSettings.MoveDelegate?.Invoke();
                    break;
            }

            CurrentSettings.SetCurrentItem();
        }

        /// <summary>
        /// Функция добавляет новое действие в список
        /// </summary>
        /// <param name="cancel">Описание действия</param>
        public void AppendNewAction(CancelEvents cancel)
            => _cancelEvents.Push(cancel);

        /// <summary>
        /// Функция отмены действия добавления
        /// </summary>
        /// <param name="obj">Объект действия</param>
        private static void CancelAdd(ICanvasItem obj)
            => obj.Remove();

        /// <summary>
        /// Функция отмены действия удаления
        /// </summary>
        /// <param name="obj">Объект действия</param>
        private static void CancelRemove(ICanvasItem obj)
            => CurrentSettings.AppendDelegate.Invoke(obj);

        /// <summary>
        /// Функция отмены действия изменения ширины объекта
        /// </summary>
        /// <param name="obj">Объект действия</param>
        /// <param name="width">Значение ширины</param>
        private static void CancelWidth(IRectangleItem obj, double width)
            => obj.ChangeWidth(width);

        /// <summary>
        /// Функция отмены действия изменения высоты объекта
        /// </summary>
        /// <param name="obj">Объект действия</param>
        /// <param name="height">Значение высоты</param>
        private static void CancelHeight(IRectangleItem obj, double height)
            => obj.ChangeHeight(height);

        /// <summary>
        /// Функция отмены действия изменения цвета заполнения объекта
        /// </summary>
        /// <param name="obj">Объект действия</param>
        /// <param name="brush">"Кисть" с цветом</param>
        private static void CancelFillColor(IPropertiesItem obj, Brush brush)
            => obj.ChangeFillColor(brush);

        /// <summary>
        /// Функция отмены действия движения объекта
        /// </summary>
        /// <param name="obj">Объект действия</param>
        /// <param name="points">Описание положения объекта</param>
        private static void CancelMove(ICanvasItem obj, List<CustPoint> points)
            => obj.SetPoints(points);

        /// <summary>
        /// Функция отмены действия изменения цвета линий объекта
        /// </summary>
        /// <param name="obj">Объект действия</param>
        /// <param name="brush">"Кисть" с цветом</param>
        private static void CancelStrokeColor(IPropertiesItem obj, Brush brush)
            => obj.ChangeStrokeColor(brush);

        /// <summary>
        /// Функция отмены действия изменения толщины линий объекта
        /// </summary>
        /// <param name="obj">Объект действия</param>
        /// <param name="thickness">Значение толщины</param>
        private static void CancelThickness(IPropertiesItem obj, double thickness)
            => obj.ChangeThickness(thickness);


        private static void CancelAddNewPoint(ICanvasItem obj, List<CustPoint> points)
        {
            obj.SetPoints(points);
        }
    }
}

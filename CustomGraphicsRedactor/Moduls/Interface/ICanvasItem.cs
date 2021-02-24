using System.Collections.Generic;

namespace CustomGraphicsRedactor.Moduls.Interface
{
    /// <summary>
    /// Интерфейс группы объектов с основными методдами
    /// </summary>
    public interface ICanvasItem
    {
        bool IsSelected { get; }
        List<CustPoint> GetPoints { get; }
        void Select();
        void Remove();
        void Deselect();
        void Move(CustVector delta);
        void SetPoints(List<CustPoint> points);
    }
}

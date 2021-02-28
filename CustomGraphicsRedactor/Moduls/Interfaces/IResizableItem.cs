namespace CustomGraphicsRedactor.Moduls.Interface
{
    /// <summary>
    /// Интерфейс группы элементов с изменяемы точками отрисовки
    /// </summary>
    interface IResizableItem
    {
        void AddTmpPoint(CustPoint point);
        void AddNewPoint(CustPoint point);
    }
}

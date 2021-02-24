namespace CustomGraphicsRedactor.Moduls.Interface
{
    /// <summary>
    /// Интерфейс группы объектов с функциями отображения и изменения высоты и ширины
    /// </summary>
    public interface IRectangleItem : IPropertiesItem
    {
        double Width { get; }
        double Height { get; }
        void ChangeWidth(double width);
        void ChangeHeight(double height);
    }
}

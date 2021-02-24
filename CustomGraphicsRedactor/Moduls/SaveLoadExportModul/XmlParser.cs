using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using CustomGraphicsRedactor.Moduls.Interface;
using CustomGraphicsRedactor.Moduls.CanvasItems;

namespace CustomGraphicsRedactor.Moduls
{
    /// <summary>
    /// Логика преобразования объектов холста в xml и обратно
    /// </summary>
    public class XmlParser
    {
        private Canvas _ctx;

        /// <param name="ctx">Контекст для преобразования (холст)</param>
        public XmlParser(Canvas ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Функция преобразования объектов холста в xml
        /// </summary>
        /// <returns>Xml "документ"</returns>
        public XDocument SerializeXml()
        {
            var xDoc = new XDocument();
            var children = _ctx.Children;

            var items = new XElement("Items");

            for(int i = 0; i < children.Count; i++) {
                var tmpItem = children[i];

                var canvasItemAtr = GetItemAtr(tmpItem);

                var xPoints = GetItemPoints(tmpItem);

                var canvasItem = new XElement(
                    xPoints.Count > 1 ? "CustBrokenLine" :
                                        "CustRectangle");

                foreach (var atr in canvasItemAtr)
                    canvasItem.Add(atr);

                canvasItem.Add(xPoints);
                items.Add(canvasItem);
            }

            xDoc.Add(items);
            return xDoc;
        }

        /// <summary>
        /// Функция преобразования xml "документа" в объекты холста 
        /// </summary>
        /// <returns>Список объектов</returns>
        public List<UIElement> DeserializeXml(XDocument xDoc)
        {
            var result = new List<UIElement>();
            foreach(var xItem in xDoc.Root.Elements()) {
                var isBrokenLine = xItem.Name == "CustBrokenLine";

                var xPoints = xItem.Elements("Point");
                var xThickness = xItem.Attribute("Thickness");
                var xFillColor = xItem.Attribute("FillColor");
                var xStrokeColor = xItem.Attribute("StrokeColor");

                var width = 200d;
                var height = 100d;
                var Thickness = 1d;
                var points = new List<CustPoint>();

                foreach(var xPoint in xPoints) {
                    var point = new Point();

                    if (double.TryParse(xPoint.Attribute("X").Value, out double X))
                        point.X = X;

                    if (double.TryParse(xPoint.Attribute("Y").Value, out double Y))
                        point.Y = Y;

                    points.Add(new CustPoint(point));
                }

                if (double.TryParse(xThickness.Value, out double Th))
                    Thickness = Th;

                var fillColor = (Color)ColorConverter.ConvertFromString(xFillColor.Value);
                var strokeColor = (Color)ColorConverter.ConvertFromString(xStrokeColor.Value);

                if (!isBrokenLine) {
                    var xWidth = xItem.Attribute("Width");
                    var xHeight = xItem.Attribute("Height");

                    if (double.TryParse(xWidth.Value, out double Width))
                        width = Width;

                    if (double.TryParse(xHeight.Value, out double Height))
                        height = Height;
                }

                IPropertiesItem _newObject = null;
                if (isBrokenLine) {
                    _newObject = new CustBrokenLine(points.FirstOrDefault().Point);
                    ((ICanvasItem)_newObject).SetPoints(points);
                }
                else {
                    _newObject = new CustRectangle(points.FirstOrDefault().Point);
                    ((IRectangleItem)_newObject).ChangeWidth(width);
                    ((IRectangleItem)_newObject).ChangeHeight(height);
                }
                _newObject.ChangeThickness(Thickness);
                _newObject.ChangeFillColor(new SolidColorBrush(fillColor));
                _newObject.ChangeStrokeColor(new SolidColorBrush(strokeColor));

                result.Add((UIElement)_newObject);
            }

            return result;
        }

        /// <summary>
        /// Функция вычисляет атрибуты объекта
        /// </summary>
        /// <param name="tmpItem">Объект холста</param>
        /// <returns>Список атрибутов объектов</returns>
        private List<XAttribute> GetItemAtr(UIElement tmpItem)
        {
            var itemProps = (IPropertiesItem)tmpItem;

            var canvasAtr = new List<XAttribute>();

            canvasAtr.Add(new XAttribute("FillColor", itemProps.FillColor));
            canvasAtr.Add(new XAttribute("Thickness", itemProps.Thickness));
            canvasAtr.Add(new XAttribute("StrokeColor", itemProps.StrokeColor));

            if (tmpItem is IRectangleItem rci) {
                canvasAtr.Add(new XAttribute("Width", rci.Width));
                canvasAtr.Add(new XAttribute("Height", rci.Height));
            }

            return canvasAtr;
        }

        /// <summary>
        /// Функция вычисляет описание положения объекта на холсте
        /// </summary>
        /// <param name="tmpItem">Объект холста</param>
        /// <returns>Список xml элементов ("точек")</returns>
        private List<XElement> GetItemPoints(UIElement tmpItem)
        {
            var xPoints = new List<XElement>();

            if (tmpItem is ICanvasItem mpi) {
                foreach (var point in mpi.GetPoints) {
                    xPoints.Add(new XElement("Point",
                                    new XAttribute("X", point.Point.X),
                                    new XAttribute("Y", point.Point.Y)));
                }
            }

            return xPoints;
        }
    }
}

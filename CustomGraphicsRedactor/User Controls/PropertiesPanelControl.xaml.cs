using System.Windows;
using System.Windows.Controls;
using CustomGraphicsRedactor.Moduls;
using CustomGraphicsRedactor.Moduls.Interface;

namespace CustomGraphicsRedactor.User_Controls
{
    /// <summary>
    /// Логика взаимодействия для PropertiesPanelControl.xaml
    /// Описание взаимодействия свойст объекта на холсте
    /// </summary>
    public partial class PropertiesPanelControl : UserControl, IRefreshControls
    {
        private IPropertiesItem _item;
        private NumberBoxControl _numberThickBox;
        private NumberBoxControl _numberWidthBox;
        private NumberBoxControl _numberHeightBox;
        private ColorPickerControl _fillColorPicker;
        private ColorPickerControl _strokeColorPicker;

        public PropertiesPanelControl()
        {
            InitializeComponent();

            _item = null;
            CurrentSettings.RefreshDelegate += Refresh;
        }

        /// <summary>
        /// Функция обновления состояния контроллера
        /// </summary>
        public void Refresh()
        {
            Clear();

            _item = CurrentSettings.GetItem;
            if (_item == null) return;

            var tabControl = new TabControl();
            tabControl.Items.Add(CreateFillColorTabItem());
            tabControl.Items.Add(CreateStrokeColorTabItem());
            tabControl.Items.Add(CreateThicknessTabItem());

            if (_item is IRectangleItem) {
                tabControl.Items.Add(CreateWidthTabItem());
                tabControl.Items.Add(CreateHeightTabItem());
            }

            PropertiesPanel.Children.Add(tabControl);
        }

        /// <summary>
        /// Функция создания вкладки со свойством изменения ширины объекта
        /// </summary>
        /// <returns>Вкладка</returns>
        private TabItem CreateWidthTabItem()
        {
            var fillTabItem = new TabItem();
            fillTabItem.Header = new TextBlock() { Text = "Width" };
            var propFillPanel = new StackPanel();
            _numberWidthBox = new NumberBoxControl(((IRectangleItem)_item).Width);
            var buttonFillColor = new Button();
            buttonFillColor.Content = "Set Width";
            buttonFillColor.Click += new RoutedEventHandler(SetWidth);
            propFillPanel.Children.Add(_numberWidthBox);
            propFillPanel.Children.Add(buttonFillColor);
            fillTabItem.Content = propFillPanel;
            return fillTabItem;
        }

        /// <summary>
        /// Функция создания вкладки со свойством изменения высоты объекта
        /// </summary>
        /// <returns>Вкладка</returns>
        private TabItem CreateHeightTabItem()
        {
            var fillTabItem = new TabItem();
            fillTabItem.Header = new TextBlock() { Text = "Height" };
            var propFillPanel = new StackPanel();
            _numberHeightBox = new NumberBoxControl(((IRectangleItem)_item).Height);
            var buttonFillColor = new Button();
            buttonFillColor.Content = "Set Height";
            buttonFillColor.Click += new RoutedEventHandler(SetHeight);
            propFillPanel.Children.Add(_numberHeightBox);
            propFillPanel.Children.Add(buttonFillColor);
            fillTabItem.Content = propFillPanel;
            return fillTabItem;
        }

        /// <summary>
        /// Функция создания вкладки со свойством изменения толщины линий
        /// </summary>
        /// <returns>Вкладка</returns>
        private TabItem CreateThicknessTabItem()
        {
            var fillTabItem = new TabItem();
            fillTabItem.Header = new TextBlock() { Text = "Stroke Thickness" };
            var propFillPanel = new StackPanel();
            _numberThickBox = new NumberBoxControl(_item.Thickness);
            var buttonFillColor = new Button();
            buttonFillColor.Content = "Set Thickness";
            buttonFillColor.Click += new RoutedEventHandler(SetThickness);
            propFillPanel.Children.Add(_numberThickBox);
            propFillPanel.Children.Add(buttonFillColor);
            fillTabItem.Content = propFillPanel;
            return fillTabItem;
        }

        /// <summary>
        /// Функция создания вкладки со свойством изменения цвета заливки объекта
        /// </summary>
        /// <returns>Вкладка</returns>
        private TabItem CreateFillColorTabItem()
        {
            var fillTabItem = new TabItem();
            fillTabItem.Header = new TextBlock() { Text = "Fill Color" };
            var propFillPanel = new StackPanel();
            _fillColorPicker = new ColorPickerControl(_item.FillColor);
            var buttonFillColor = new Button();
            buttonFillColor.Content = "Set Color";
            buttonFillColor.Click += new RoutedEventHandler(SetFillColor);
            propFillPanel.Children.Add(_fillColorPicker);
            propFillPanel.Children.Add(buttonFillColor);
            fillTabItem.Content = propFillPanel;
            return fillTabItem;
        }

        /// <summary>
        /// Функция создания вкладки со свойством изменения цвета линий
        /// </summary>
        /// <returns>Вкладка</returns>
        private TabItem CreateStrokeColorTabItem()
        {
            var fillTabItem = new TabItem();
            fillTabItem.Header = new TextBlock() { Text = "Stroke Color" };
            var propFillPanel = new StackPanel();
            _strokeColorPicker = new ColorPickerControl(_item.StrokeColor);
            var buttonFillColor = new Button();
            buttonFillColor.Content = "Set Color";
            buttonFillColor.Click += new RoutedEventHandler(SetStrokeColor);
            propFillPanel.Children.Add(_strokeColorPicker);
            propFillPanel.Children.Add(buttonFillColor);
            fillTabItem.Content = propFillPanel;
            return fillTabItem;
        }

        /// <summary>
        /// Действие изменения ширины объекта
        /// </summary>
        private void SetWidth(object sender, RoutedEventArgs e)
        {
            CurrentSettings.AppendNewAction(ECancelTypes.Width, 
                new object[] {
                    _item,
                    ((IRectangleItem)_item).Width
                });

            ((IRectangleItem)_item).ChangeWidth(_numberWidthBox.ValueNumber);
            CurrentSettings.MoveDelegate.Invoke();
        }

        /// <summary>
        /// Действие изменения высоты объекта
        /// </summary>
        private void SetHeight(object sender, RoutedEventArgs e)
        {
            CurrentSettings.AppendNewAction(ECancelTypes.Height,
                new object[] {
                    _item,
                    ((IRectangleItem)_item).Height
                });

            ((IRectangleItem)_item).ChangeHeight(_numberHeightBox.ValueNumber);
            CurrentSettings.MoveDelegate.Invoke();
        }

        /// <summary>
        /// Действие изменения толщины линий
        /// </summary>
        private void SetThickness(object sender, RoutedEventArgs e)
        {
            CurrentSettings.AppendNewAction(ECancelTypes.Thickness,
                new object[] {
                    _item,
                    _item.Thickness
                });

            _item.ChangeThickness(_numberThickBox.ValueNumber);
        }

        /// <summary>
        /// Действие изменения цвета заливки объекта
        /// </summary>
        private void SetFillColor(object sender, RoutedEventArgs e)
        {
            CurrentSettings.AppendNewAction(ECancelTypes.FillColor,
                new object[] {
                    _item,
                    _item.FillColor
                });

            _item.ChangeFillColor(_fillColorPicker.ValueColor);
        }

        /// <summary>
        /// Действие изменения цвета линий
        /// </summary>
        private void SetStrokeColor(object sender, RoutedEventArgs e)
        {
            CurrentSettings.AppendNewAction(ECancelTypes.StrokeColor,
                new object[] {
                    _item,
                    _item.StrokeColor
                });

            _item.ChangeStrokeColor(_strokeColorPicker.ValueColor);
        }

        /// <summary>
        /// Функция очистки поля свойств
        /// </summary>
        private void Clear() => PropertiesPanel.Children.Clear();
    }
}

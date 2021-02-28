using System.Windows;
using System.Windows.Input;
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

            if (CurrentSettings.GetItem == null) return;
            else if (CurrentSettings.GetItem is IRectangleItem)
                _item = (IRectangleItem)CurrentSettings.GetItem;
            else if (CurrentSettings.GetItem is IPropertiesItem)
                _item = (IPropertiesItem)CurrentSettings.GetItem;

            if (_item == null) return;

            var tabControl = new TabControl();
            tabControl.Items.Add(CreateFillColorTabItem());
            tabControl.Items.Add(CreateStrokeColorTabItem());
            PropertiesPanel.Children.Add(CreateThicknessStackPanel());
            if (_item is IRectangleItem) {
                PropertiesPanel.Children.Add(CreateWidthStackPanel());
                PropertiesPanel.Children.Add(CreateHeightStackPanel());
            }

            PropertiesPanel.Children.Add(tabControl);
        }

        /// <summary>
        /// Функция создания вкладки со свойством изменения ширины объекта
        /// </summary>
        /// <returns>Вкладка</returns>
        private Grid CreateWidthStackPanel()
        {
            var grid = new Grid();
            var colDef1 = new ColumnDefinition();
            var colDef2 = new ColumnDefinition();
            colDef1.Width = new GridLength(75);
            grid.ColumnDefinitions.Add(colDef1);
            grid.ColumnDefinitions.Add(colDef2);
            var tBlock = new TextBlock()
            {
                Text = "Width:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 5, 0)
            };
            grid.Children.Add(tBlock);
            _numberWidthBox = new NumberBoxControl(((IRectangleItem)_item).Width);
            _numberWidthBox.TextChangedDelegate += SetWidth;
            grid.Children.Add(_numberWidthBox);
            Grid.SetColumn(tBlock, 0);
            Grid.SetColumn(_numberWidthBox, 1);
            return grid;
        }

        /// <summary>
        /// Функция создания вкладки со свойством изменения высоты объекта
        /// </summary>
        /// <returns>Вкладка</returns>
        private Grid CreateHeightStackPanel()
        {
            var grid = new Grid();
            var colDef1 = new ColumnDefinition();
            var colDef2 = new ColumnDefinition();
            colDef1.Width = new GridLength(75);
            grid.ColumnDefinitions.Add(colDef1);
            grid.ColumnDefinitions.Add(colDef2);
            var tBlock = new TextBlock()
            {
                Text = "Height:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 5, 0)
            };
            grid.Children.Add(tBlock);
            _numberHeightBox = new NumberBoxControl(((IRectangleItem)_item).Height);
            _numberHeightBox.TextChangedDelegate += SetHeight;
            grid.Children.Add(_numberHeightBox);
            Grid.SetColumn(tBlock, 0);
            Grid.SetColumn(_numberHeightBox, 1);
            return grid;
        }

        /// <summary>
        /// Функция создания вкладки со свойством изменения толщины линий
        /// </summary>
        /// <returns>Вкладка</returns>
        private Grid CreateThicknessStackPanel()
        {
            var grid = new Grid();
            var colDef1 = new ColumnDefinition();
            var colDef2 = new ColumnDefinition();
            colDef1.Width = new GridLength(75);
            grid.ColumnDefinitions.Add(colDef1);
            grid.ColumnDefinitions.Add(colDef2);
            var tBlock = new TextBlock() {
                Text = "Thickness:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment =VerticalAlignment.Center,
                Padding = new Thickness(0,0,5,0)
            };
            grid.Children.Add(tBlock);
            _numberThickBox = new NumberBoxControl(_item.Thickness);
            _numberThickBox.TextChangedDelegate += SetThickness;
            grid.Children.Add(_numberThickBox);
            Grid.SetColumn(tBlock, 0);
            Grid.SetColumn(_numberThickBox, 1);
            return grid;
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
            _fillColorPicker.ColorChangedDelegate += SetFillColor;
            propFillPanel.Children.Add(_fillColorPicker);
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
            _strokeColorPicker.ColorChangedDelegate += SetStrokeColor;
            propFillPanel.Children.Add(_strokeColorPicker);
            fillTabItem.Content = propFillPanel;
            return fillTabItem;
        }

        /// <summary>
        /// Действие изменения ширины объекта
        /// </summary>
        private void SetWidth()
        {
            CurrentSettings.AppendNewAction(ECancelTypes.Width, 
                new object[] {
                    _item,
                    ((IRectangleItem)_item).Width
                });

            ((IRectangleItem)_item).ChangeWidth(_numberWidthBox.ValueNumber);
            CurrentSettings.MoveDelegate?.Invoke();
        }

        /// <summary>
        /// Действие изменения высоты объекта
        /// </summary>
        private void SetHeight()
        {
            CurrentSettings.AppendNewAction(ECancelTypes.Height,
                new object[] {
                    _item,
                    ((IRectangleItem)_item).Height
                });

            ((IRectangleItem)_item).ChangeHeight(_numberHeightBox.ValueNumber);
            CurrentSettings.MoveDelegate?.Invoke();
        }

        /// <summary>
        /// Действие изменения толщины линий
        /// </summary>
        private void SetThickness()
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
        private void SetFillColor()
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
        private void SetStrokeColor()
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

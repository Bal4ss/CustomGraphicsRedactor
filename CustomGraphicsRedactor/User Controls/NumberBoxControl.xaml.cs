using System.Windows.Input;
using System.Windows.Controls;

namespace CustomGraphicsRedactor.User_Controls
{
    /// <summary>
    /// Логика взаимодействия для NumberBoxControl.xaml
    /// Описание взаимодействия поля для ввода чисел
    /// </summary>
    public partial class NumberBoxControl : UserControl
    {
        public delegate void TextChanged();

        private double _number;
        private TextChanged _textChanged;

        /// <param name="number">Стартовое число</param>
        public NumberBoxControl(double number)
        {
            InitializeComponent();
            _number = number;
            NumberHolder.Text = _number.ToString();
        }

        /// <summary>
        /// Возвращает числовое значение поля
        /// </summary>
        public double ValueNumber => _number;

        public TextChanged TextChangedDelegate { get { return _textChanged; } set { _textChanged = value; } }

        /// <summary>
        /// Действие ввода текста в поле для ввода (запрещает вводить не цифры (кроме ','))
        /// </summary>
        private void NumberHolderPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789,".IndexOf(e.Text) < 0;
        }

        /// <summary>
        /// Действие изменения текста внутри поля для ввода
        /// </summary>
        private void NumberHolderTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!double.TryParse(NumberHolder.Text, out double number)) NumberHolder.Text = _number.ToString();
            else {
                if (number < 1) number = 1;
                else if (number > 10000000000000000000)
                    number = 10000000000000000000; 
                _number = number;
                NumberHolder.Text = $"{_number}";
            }
            _textChanged?.Invoke();
        }
    }
}

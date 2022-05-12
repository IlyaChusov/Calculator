using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Заполнение бит-панели
            for (int i = 0; i < 32; i++)
            {
                TopBinarypanel.Children.Add(new Label { Name = "Bin" + i.ToString(), Content = 0 });    // Верхняя строка (8 бит)
                if (i < 16)
                    Sec1Binarypanel.Children.Add(new Label { Name = "Bin" + (i + 32).ToString(), Content = 0 });    // Левая нижняя строка (4 бита)
                if (i < 8)
                {
                    Sec2Binarypanel.Children.Add(new Label { Name = "Bin" + (i + 48).ToString(), Content = 0 });    // Строка внизу посередине (2 бита)
                    Sec3Binarypanel.Children.Add(new Label { Name = "Bin" + (i + 56).ToString(), Content = 0 });    // Правая нижняя строка (1 бит)
                }
            }
        }

        // Обработка нажатий на цифры
        private void Numbers(object sender, RoutedEventArgs e)
        {
            pressNum = true;
            Button numb = (Button)sender;
            switch (numb.Content)
            { 
                case "0": 
                    if (Tablo.Text != "0")
                        Tablo.Text += 0;
                    break;
                default: 
                    if (Tablo.Text == "0")
                        Tablo.Text = (string)numb.Content;
                    else
                        Tablo.Text += numb.Content;
                    break;
            }
        }

        // Обработка нажатия на запятую
        private void Comma_Click(object sender, RoutedEventArgs e)
        {
            if (!Tablo.Text.Contains (","))
                Tablo.Text += ",";
        }
        
        // Обработка удаления цифр
        private void Delete_But(object sender, RoutedEventArgs e)
        {
            Button cancel = (Button)sender;
            switch (cancel.Content)
            {
                case "CE":
                    Tablo.Text = "0";
                    break;
                case "C":
                    Tablo.Text = "0";
                    Up_Tablo.Text = "";
                    history = false;
                    break;
                default:
                    if (Tablo.Text.Length != 1)
                        Tablo.Text = Tablo.Text.Substring(0, Tablo.Text.Length - 1);
                    else
                        Tablo.Text = "0";
                    break;
            }
        }

        string Marks = "+/-*";  // Массив со знаками
        bool pressNum = false;  // Переменная, позволяющая рассчитывать выражение после нажатия знака (потому что после нажатия знака необходимо вводить число, либо менять + на - и наоборот)
                                // В общем, означает, была ли нажата цифра (или +-)

        // Обработка нажатия на знак
        private void Func_But(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string tabloText = Tablo.Text;

            switch (button.Content.ToString())
            {
                case "+-":
                    if (tabloText != "0")
                    {
                        if (tabloText.Contains("-"))
                            Tablo.Text = tabloText.Replace("-", "");
                        else
                            Tablo.Text = "-" + tabloText;
                        pressNum = true;
                    }
                    break;
                case "/":
                    Func_Work("/");
                    break;
                case "*":
                    Func_Work("*");
                    break;
                case "-":
                    Func_Work("-");
                    break;
                case "+":
                    Func_Work("+");
                    break;
                case "Sqrt":
                    break;
                case "%":
                    break;
                case "1/x":
                    break;
                case "=":
                    Calculate(true);
                    break;
            }
        }

        // Обработка нажатия на знак 2 (для удобства и сокращения кода)
        private void Func_Work(string mark)
        {
            string tabloText = Tablo.Text;
            string upTabloText = Up_Tablo.Text;

            if (upTabloText == "")
                Up_Tablo.Text += tabloText + " " + mark;
            else
            {
                if (!pressNum)
                {
                    if (!Marks.Contains(upTabloText.Last()))
                        Up_Tablo.Text += " " + mark;
                    else
                        Up_Tablo.Text = upTabloText.Substring(0, upTabloText.Length - 1) + mark;
                }
                else
                    Calculate(false);
            }
            pressNum = false;
        }


        double tempSum; // Промежуточный результат расчётов
        bool history = false; //Переменная, означающая, есть ли последовательность операций (история) или же операция первая
                              // Нужна для использования tempSum

        // Расчёт выражения
        private void Calculate(bool eq)
        {
            string tabloText = Tablo.Text;
            string upTabloText = Up_Tablo.Text;

            string[] textArr = upTabloText.Split(' ');
            string mark = textArr.Last();       // Знак, с которым необходимо рассчитать выражение
            double num = double.Parse(textArr[textArr.Length - 2]);         // Левая часть выражения (последнее число в истории)
            if (history)
                num = tempSum;

            switch (mark)
            {
                case "/":
                    tempSum = num / double.Parse(tabloText);
                    Tablo.Text = tempSum.ToString();
                    history = true;
                    break;
                case "*":
                    tempSum = num * double.Parse(tabloText);
                    Tablo.Text = tempSum.ToString();
                    history = true;
                    break;
                case "-":
                    tempSum = num - double.Parse(tabloText);
                    Tablo.Text = tempSum.ToString();
                    history = true;
                    break;
                case "+":
                    tempSum = num + double.Parse(tabloText);
                    Tablo.Text = tempSum.ToString();
                    history = true;
                    break;
            }
            if (!eq)        // eq означает, что метод был вызван кнопкой "равно", то есть историю необходимо убрать
                Up_Tablo.Text += " " + tabloText + " " + mark;      // Вывод результата
            else
                Up_Tablo.Text = "";     // Очистка истории
        }

        // Обработка переключения режимов калькулятора
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            usual.IsChecked = false;
            engin.IsChecked = false;
            progr.IsChecked = false;
            stat.IsChecked = false;

            MenuItem AnyMenuItem = (MenuItem)sender;
            AnyMenuItem.IsChecked = true;
        }

        // Метод, обрабатывающий разворачивание режимов калькулятора
        private void Checked(object sender, RoutedEventArgs e)
        {
            MenuItem AnyMenuItem = (MenuItem)sender;
            switch (AnyMenuItem.Name)
            {
                case "usual":
                    break;
                case "engin":
                    Application.Current.MainWindow.Width = Application.Current.MainWindow.Width * 2;
                    EnginPanel.Visibility = Visibility.Visible;
                    leftCol.Width = new GridLength(1, GridUnitType.Star);
                    break;
                case "progr":
                    Application.Current.MainWindow.Width = Application.Current.MainWindow.Width * 2;
                    ProgramPanel.Visibility = Visibility.Visible;
                    binaryField.Height = new GridLength(50);
                    leftCol.Width = new GridLength(1, GridUnitType.Star);
                    SecondCol.Width = new GridLength(10);
                    But_Sqrt.IsEnabled = false;
                    But_Perc.IsEnabled = false;
                    But_1x.IsEnabled = false;
                    But_Point.IsEnabled = false;
                    Dec.IsChecked = true;
                    Eight.IsChecked = true;
                    break;
                case "stat":
                    break;
            }
        }

        // Метод, обрабатывающий сворачивание режимов калькулятора
        private void UnChecked(object sender, RoutedEventArgs e)
        {
            MenuItem AnyMenuItem = (MenuItem)sender;
            switch (AnyMenuItem.Name)
            {
                case "usual":
                    break;
                case "engin":
                    Application.Current.MainWindow.Width = Application.Current.MainWindow.Width / 2;
                    EnginPanel.Visibility = Visibility.Collapsed;
                    leftCol.Width = GridLength.Auto;
                    break;
                case "progr":
                    Application.Current.MainWindow.Width = Application.Current.MainWindow.Width / 2;
                    ProgramPanel.Visibility = Visibility.Collapsed;
                    binaryField.Height = new GridLength(0);
                    leftCol.Width = GridLength.Auto;
                    But_Sqrt.IsEnabled = true;
                    But_Perc.IsEnabled = true;
                    But_1x.IsEnabled = true;
                    But_Point.IsEnabled = true;
                    break;
                case "stat":
                    break;
            }
        }

        // Переключение кнопок, использующихся 16-ричной системой
        private void check_Hex (bool check)
        {
            But_A.IsEnabled = check;
            But_B.IsEnabled = check;
            But_C.IsEnabled = check;
            But_D.IsEnabled = check;
            But_E.IsEnabled = check;
            But_F.IsEnabled = check;
        }

        // Переключение кнопок, использующихся 10-ричной системой
        private void check_Dec (bool check)
        {
            But_8.IsEnabled = check;
            But_9.IsEnabled = check;
        }

        // Переключение кнопок, использующихся 8-ричной системой
        private void check_Oct (bool check)
        {
            But_2.IsEnabled = check;
            But_3.IsEnabled = check;
            But_4.IsEnabled = check;
            But_5.IsEnabled = check;
            But_6.IsEnabled = check;
            But_7.IsEnabled = check;
        }

        // Обработка переключения систем исчисления и битности
        private void Checked_progr (object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            switch (radio.Content.ToString())
            {
                case "Hex":
                    check_Hex(true);
                    check_Dec(true);
                    check_Oct(true);
                    break;

                case "Dec":
                    check_Hex(false);
                    check_Dec(true);
                    check_Oct(true);
                    break;

                case "Oct":
                    check_Hex(false);
                    check_Dec(false);
                    check_Oct(true);
                    break;

                case "Bin":
                    check_Hex(false);
                    check_Dec(false);
                    check_Oct(false);
                    break;

                case "8 байт":
                    TopBinarypanel.Visibility = Visibility.Visible;
                    Sec1Binarypanel.Visibility = Visibility.Visible;
                    Sec2Binarypanel.Visibility = Visibility.Visible;
                    break;

                case "4 байта":
                    TopBinarypanel.Visibility = Visibility.Hidden;
                    Sec1Binarypanel.Visibility = Visibility.Visible;
                    Sec2Binarypanel.Visibility = Visibility.Visible;
                    break;

                case "2 байта":
                    TopBinarypanel.Visibility = Visibility.Hidden;
                    Sec1Binarypanel.Visibility = Visibility.Hidden;
                    Sec2Binarypanel.Visibility = Visibility.Visible;
                    break;

                case "1 байт":
                    TopBinarypanel.Visibility = Visibility.Hidden;
                    Sec1Binarypanel.Visibility = Visibility.Hidden;
                    Sec2Binarypanel.Visibility = Visibility.Hidden;
                    break;
            }
        }

        public int[] BinValueArray = new int[64];

        // Обработка нажатий на бит-панель
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Label ChildElem = (Label)e.Source;
                bool PinVal = Convert.ToBoolean(Convert.ToInt16(ChildElem.Content));
                ChildElem.Content = Convert.ToString(Convert.ToInt16(!PinVal));
                int index = Convert.ToInt32(ChildElem.Name.Substring(3));
                BinValueArray[index] = Convert.ToInt16(ChildElem.Content);
                string TextBinVal = string.Concat<int>(BinValueArray);
                Tablo.Text = Convert.ToString(Convert.ToInt64(TextBinVal, 2));
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Нажимайте на цифры!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
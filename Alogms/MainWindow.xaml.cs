﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Alogms
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void clearElems_Click(object sender, RoutedEventArgs e)
        {
            set.Clear();
        }

        private void clearSubsets_Click(object sender, RoutedEventArgs e)
        {
            subsets.Clear();
        }

        private void clearSet_Click(object sender, RoutedEventArgs e)
        {
            set.Clear();
            time.Clear();
            startElement.Clear();
            endElement.Clear();
            sumBox.Clear();
            subsets.Clear();
            countSubsets.Clear();
        }

        private void autofill_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(startElement.Text, out int startEl))
            {
                MessageBox.Show("Начальный элемент не указан");
                return;
            }
            if (!int.TryParse(endElement.Text, out int endEl))
            {
                MessageBox.Show("Конечный элемент не указан");
                return;
            }
            if (startEl <= endEl)
            {
                for (int i = startEl; i <= endEl; i++)
                {
                    set.AppendText(i.ToString() + Environment.NewLine);
                }
            }
            else
            {
                for (int i = startEl; i >= endEl; i--)
                {
                    set.AppendText(i.ToString() + Environment.NewLine);
                }
            }
            set.ScrollToEnd();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(sumBox.Text, out int sum))
            {
                MessageBox.Show("Сумма не может быть пустой");
                return;
            }

            var buf = set.Text.Split(new Char[] { ' ', ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int[] arraySet = new int[buf.Length];

            for (int i = 0; i < arraySet.Length; i++)
            {
                if (!int.TryParse(buf[i], out arraySet[i]))
                {
                    MessageBox.Show("Превышено максимально допустимое значение числа, лмбо допущена ошибка ввода " + buf[i]);
                    return;
                }
            }

            subsets.Clear();

            Stopwatch timer = new Stopwatch();
            timer.Restart();

            var subSetSum = new SubSetSum();
            subSetSum.Search(arraySet, sum);

            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            time.Text = String.Format("{0:00}:{1:00}:{2:00}.{3:0000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

            countSubsets.Text = subSetSum.Subsets.Count().ToString();

            if (subSetSum.Subsets.Count == 0 || subSetSum.Subsets[0].Count == 0)
            {
                subsets.AppendText("Подмножеств с заданной суммой нет");
            }
            else
            {
                string output;
                if (subSetSum.Subsets.Count() <= 5000)
                {
                    foreach (var subset in subSetSum.Subsets)
                    {
                        output = "{ ";
                        for (int i = 0; i < subset.Count(); i++)
                        {
                            output += subset[i].ToString();
                            if (i != subset.Count() - 1)
                            {
                                output += ",";
                            }
                            output += " ";
                        }
                        output += "}" + Environment.NewLine;
                        subsets.AppendText(output);
                    }
                    subsets.ScrollToEnd();
                }
                else
                {
                    subsets.AppendText("Число подмножеств превышает 5000, подмножества буду записаны в текстовый файл output.txt");
                    StreamWriter file = new StreamWriter(@"..\..\..\output.txt", false);
                    foreach (var subset in subSetSum.Subsets)
                    {
                        output = "{ ";
                        for (int i = 0; i < subset.Count(); i++)
                        {
                            output += subset[i].ToString();
                            if (i != subset.Count() - 1)
                            {
                                output += ",";
                            }
                            output += " ";
                        }
                        output += "}" + Environment.NewLine;
                        file.WriteLine(output);
                    }
                    file.Close();
                }
            }
        }

        #region Запрет ввода ненужных символов в поля
        private void OnPreviewTextInputDigits(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsDigits);
        }
        private void OnPastingDigits(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsDigits))
                e.CancelCommand();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }
        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
        }

        private bool IsGood(char c)
        {
            if (Char.IsDigit(c) || c == ',' || c == '\n' || c == '\r')
                return true;
            return false;
        }

        private bool IsDigits(char c)
        {
            if (Char.IsDigit(c) && !Char.IsWhiteSpace(c))
                return true;
            return false;
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        #endregion

    }
}

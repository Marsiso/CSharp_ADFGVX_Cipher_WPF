using CSharp_ADFGVX_Cipher_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CSharp_ADFGVX_Cipher_WPF.Views
{
    /// <summary>
    /// Interaction logic for MyWindowView.xaml
    /// </summary>
    public partial class MyWindowView : Window
    {
        //private readonly MyWindowModel myWindowModel;

        public MyWindowView()
        {
            //myWindowModel = new MyWindowModel();
            //DataContext = myWindowModel;
            InitializeComponent();
            //LblGridSusTblOptRow.Visibility = Visibility.Collapsed;
            //LblGridSusTblOptColumn.Visibility = Visibility.Collapsed;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^5-6]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                _ = ((sender as TextBox)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)));
            }
        }

        private void ButtonEnglish_Click(object sender, RoutedEventArgs e)
        {
            if (!myWindowModel.IsLocalizationEnglish)
            {
                myWindowModel.IsLocalizationEnglish = true;
                if (!myWindowModel.IsFullSize)
                {
                    ClearSubsTable();
                }
                ButtonEnglish.Background = new SolidColorBrush(Color.FromRgb(107, 142, 35));
                ButtonCzech.Background = new SolidColorBrush(Color.FromRgb(219, 112, 147));
            }
        }

        private void ButtonCzech_Click(object sender, RoutedEventArgs e)
        {
            if (myWindowModel.IsLocalizationEnglish)
            {
                myWindowModel.IsLocalizationEnglish = false;
                if (!myWindowModel.IsFullSize)
                {
                    ClearSubsTable();
                }
                ButtonCzech.Background = new SolidColorBrush(Color.FromRgb(107, 142, 35));
                ButtonEnglish.Background = new SolidColorBrush(Color.FromRgb(219, 112, 147));
            }
        }

        private void ButtonIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (!myWindowModel.IsFullSize)
            {
                myWindowModel.IsFullSize = true;
                ButtonIncrease.Background = new SolidColorBrush(Color.FromRgb(107, 142, 35));
                ButtonDecrease.Background = new SolidColorBrush(Color.FromRgb(219, 112, 147));
            }
        }

        private void ButtonDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (myWindowModel.IsFullSize)
            {
                myWindowModel.IsFullSize = false;
                ClearSubsTable();
                ButtonDecrease.Background = new SolidColorBrush(Color.FromRgb(107, 142, 35));
                ButtonIncrease.Background = new SolidColorBrush(Color.FromRgb(219, 112, 147));
            }
        }

        private void ButtonEmpty_Click(object sender, RoutedEventArgs e) => ClearSubsTable();

        private void ClearSubsTable()
        {
            for (int i = 0; i < 6; ++i)
            {
                myWindowModel.SubsTblRows[i].Char0 = myWindowModel.SubsTblRows[i].Char1 =
                    myWindowModel.SubsTblRows[i].Char2 = myWindowModel.SubsTblRows[i].Char3 =
                    myWindowModel.SubsTblRows[i].Char4 = myWindowModel.SubsTblRows[i].Char5 = ' ';
            }
        }

        private void RandomiseSubsTable()
        {
            int GetNextInt32(RNGCryptoServiceProvider rnd)
            {
                byte[] randomInt = new byte[4];
                rnd.GetBytes(randomInt);
                return Convert.ToInt32(randomInt[0]);
            }

            RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
            char[] rndReorderedUsableChars = myWindowModel.SubsTblCharsCounter.
                Where(entry => entry.Value.Equals(0)).
                Select(entry => entry.Key).
                OrderBy(key => GetNextInt32(rnd)).
                ToArray();

            //myWindowModel.Output = new string(rndReorderedUsableChars);

            int multiplier = myWindowModel.IsFullSize ? 6 : 5;
            for (int i = 0; i < multiplier; ++i)
            {
                myWindowModel.SubsTblRows[i].Char0 = rndReorderedUsableChars[i * multiplier];
                myWindowModel.SubsTblRows[i].Char1 = rndReorderedUsableChars[i * multiplier + 1];
                myWindowModel.SubsTblRows[i].Char2 = rndReorderedUsableChars[i * multiplier + 2];
                myWindowModel.SubsTblRows[i].Char3 = rndReorderedUsableChars[i * multiplier + 3];
                if (!myWindowModel.IsFullSize)
                {
                    myWindowModel.SubsTblRows[i].Char5 = rndReorderedUsableChars[i * multiplier + 4];
                    continue;
                }
                myWindowModel.SubsTblRows[i].Char4 = rndReorderedUsableChars[i * multiplier + 4];
                myWindowModel.SubsTblRows[i].Char5 = rndReorderedUsableChars[i * multiplier + 5];
            }
        }

        private void ButtonRandom_Click(object sender, RoutedEventArgs e) => RandomiseSubsTable();

        private void ComboBox_GotMouseCapture(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Items.Clear();

            foreach (char c in myWindowModel.CountSubsTblChaOccurrences())
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem 
                { 
                    Content = c,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                comboBox.Items.Add(comboBoxItem);
            }
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //ComboBox comboBox = sender as ComboBox;
            //comboBox.SelectedItem = comboBox.Text;
            //comboBox.SelectedItem ??= string.Empty;
        }
    }
}

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CSharp_ADFGVX_Cipher_WPF.Views
{
    /// <summary>
    /// Interaction logic for MyWindowView.xaml
    /// </summary>
    public partial class MyWindowView : Window
    {
        private bool isSubsTblCharChckEnabled = true;

        public MyWindowView() => InitializeComponent();

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^5-6]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Key.Equals(Key.Enter))
                return;
            _ = ((sender as TextBox)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)));
        }

        private void ButtonEnglish_Click(object sender, RoutedEventArgs e)
        {
            if (myWindowModel.IsLocalizationEnglish)
                return;
            myWindowModel.IsLocalizationEnglish = true;
            if (!myWindowModel.IsFullSize)
                ClearSubsTable();
            ButtonEnglish.Background = new SolidColorBrush(Color.FromRgb(107, 142, 35));
            ButtonCzech.Background = new SolidColorBrush(Color.FromRgb(219, 112, 147));
        }

        private void ButtonCzech_Click(object sender, RoutedEventArgs e)
        {
            if (!myWindowModel.IsLocalizationEnglish)
                return;
            myWindowModel.IsLocalizationEnglish = false;
            if (!myWindowModel.IsFullSize)
                ClearSubsTable();
            ButtonCzech.Background = new SolidColorBrush(Color.FromRgb(107, 142, 35));
            ButtonEnglish.Background = new SolidColorBrush(Color.FromRgb(219, 112, 147));
        }

        private void ButtonIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (myWindowModel.IsFullSize)
                return;
            myWindowModel.IsFullSize = true;
            myWindowModel.SubstitutionTableEntries[4].EntryHeight = 25;
            GridViewSubsTable.Columns[5].Width = 60;
            ButtonIncrease.Background = new SolidColorBrush(Color.FromRgb(107, 142, 35));
            ButtonDecrease.Background = new SolidColorBrush(Color.FromRgb(219, 112, 147));
        }

        private void ButtonDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (!myWindowModel.IsFullSize)
                return;
            myWindowModel.IsFullSize = false;
            myWindowModel.SubstitutionTableEntries[4].EntryHeight = 0;
            GridViewSubsTable.Columns[5].Width = 0;
            ClearSubsTable();
            ButtonDecrease.Background = new SolidColorBrush(Color.FromRgb(107, 142, 35));
            ButtonIncrease.Background = new SolidColorBrush(Color.FromRgb(219, 112, 147));
        }

        private void ButtonEmpty_Click(object sender, RoutedEventArgs e) => ClearSubsTable();

        private void ClearSubsTable()
        {
            isSubsTblCharChckEnabled = false;

            for (int i = 0; i < 6; ++i)
                myWindowModel.SubstitutionTableEntries[i].Col0Char = myWindowModel.SubstitutionTableEntries[i].Col1Char =
                                    myWindowModel.SubstitutionTableEntries[i].Col2Char = myWindowModel.SubstitutionTableEntries[i].Col3Char =
                                    myWindowModel.SubstitutionTableEntries[i].Col4Char = myWindowModel.SubstitutionTableEntries[i].Col5Char = ' ';

            isSubsTblCharChckEnabled = true;
        }

        private void RandomizeSubsTable()
        {
            static int GetNextInt32(RNGCryptoServiceProvider rnd)
            {
                byte[] randomInt = new byte[4];
                rnd.GetBytes(randomInt);
                return Convert.ToInt32(randomInt[0]);
            }

            RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
            char[] rndReorderedUsableChars = myWindowModel.SubstitutionTableChars.
                Where(entry => entry.Value.Equals(0)).
                Select(entry => entry.Key).
                OrderBy(key => GetNextInt32(rnd)).
                ToArray();

            isSubsTblCharChckEnabled = false;

            int multiplier = myWindowModel.IsFullSize ? 6 : 5;
            for (int i = 0; i < multiplier; ++i)
            {
                if (myWindowModel.IsFullSize || !i.Equals(4))
                {
                    myWindowModel.SubstitutionTableEntries[i].Col0Char = rndReorderedUsableChars[i * multiplier];
                    myWindowModel.SubstitutionTableEntries[i].Col1Char = rndReorderedUsableChars[i * multiplier + 1];
                    myWindowModel.SubstitutionTableEntries[i].Col2Char = rndReorderedUsableChars[i * multiplier + 2];
                    myWindowModel.SubstitutionTableEntries[i].Col3Char = rndReorderedUsableChars[i * multiplier + 3];
                    if (!myWindowModel.IsFullSize)
                    {
                        myWindowModel.SubstitutionTableEntries[i].Col5Char = rndReorderedUsableChars[i * multiplier + 4];
                        continue;
                    }

                    myWindowModel.SubstitutionTableEntries[i].Col4Char = rndReorderedUsableChars[i * multiplier + 4];
                    myWindowModel.SubstitutionTableEntries[i].Col5Char = rndReorderedUsableChars[i * multiplier + 5];
                }
                else
                {
                    myWindowModel.SubstitutionTableEntries[i + 1].Col0Char = rndReorderedUsableChars[i * multiplier];
                    myWindowModel.SubstitutionTableEntries[i + 1].Col1Char = rndReorderedUsableChars[i * multiplier + 1];
                    myWindowModel.SubstitutionTableEntries[i + 1].Col2Char = rndReorderedUsableChars[i * multiplier + 2];
                    myWindowModel.SubstitutionTableEntries[i + 1].Col3Char = rndReorderedUsableChars[i * multiplier + 3];
                    myWindowModel.SubstitutionTableEntries[i + 1].Col5Char = rndReorderedUsableChars[i * multiplier + 4];
                    break;
                }
            }

            isSubsTblCharChckEnabled = true;
        }

        private void ButtonRandom_Click(object sender, RoutedEventArgs e) => RandomizeSubsTable();

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text))
                textBox.SelectAll();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isSubsTblCharChckEnabled)
                return;

            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text) || !char.TryParse(textBox.Text, out char c))
                return;

            if (!myWindowModel.SubstitutionTableChars.TryGetValue(c, out int num) || num.Equals(1))
            {
                textBox.Text = "";
                return;
            }

            for (int i = 0; i < 6; ++i)
                for (int j = 0; j < 6; ++j)
                    if (myWindowModel.SubstitutionTable[i, j].Equals(c))
                    {
                        textBox.Text = "";
                        return;
                    }

            Keyboard.ClearFocus();
        }
    }
}
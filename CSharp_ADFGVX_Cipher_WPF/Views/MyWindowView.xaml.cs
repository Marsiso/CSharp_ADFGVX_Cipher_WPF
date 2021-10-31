using System;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CSharp_ADFGVX_Cipher_WPF.Views
{
    /// <summary>
    /// Interaction logic for MyWindowView.xaml
    /// </summary>
    public partial class MyWindowView : Window
    {
        public MyWindowView()
        {
            InitializeComponent();
            ToolTipSubstitutionTable.TextBlockPopUp.DataContext = myWindowModel;
        }

        //private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        //{
        //    Regex regex = new Regex("[^5-6]+");
        //    e.Handled = regex.IsMatch(e.Text);
        //}

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Key.Equals(Key.Enter))
            {
                return;
            }

            _ = ((sender as TextBox)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)));
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text))
                textBox.SelectAll();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox textBox = sender as TextBox;
            //if (string.IsNullOrWhiteSpace(textBox.Text) || !char.TryParse(textBox.Text, out char c))
            //{
            //    return;
            //}

            //if (!myWindowModel.SubstitutionTableChars.TryGetValue(c, out int num) || num.Equals(1))
            //{
            //    textBox.Text = "";
            //    return;
            //}

            //for (int i = 0; i < 6; ++i)
            //{
            //    for (int j = 0; j < 6; ++j)
            //    {
            //        if (myWindowModel.SubstitutionTable[i, j].Equals(c))
            //        {
            //            textBox.Text = "";
            //            return;
            //        }
            //    }
            //}

            //LabelSubsTbl.Focus();
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(myWindowModel.CharsRemainingSubsTblStr))
            {
                return;
            }

            TextBox textBox = sender as TextBox;
            PopUpSubsTableToolTip.PlacementTarget = textBox;
            PopUpSubsTableToolTip.Placement = PlacementMode.Right;
            PopUpSubsTableToolTip.IsOpen = true;
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            PopUpSubsTableToolTip.Visibility = Visibility.Collapsed;
            PopUpSubsTableToolTip.IsOpen = false;
        }

        private void ButtonExit_OnClick(object sender, RoutedEventArgs e) => Close();

        private void ButtonMinimize_OnClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void ButtonMaximize_OnClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Equals(WindowState.Normal)
            ? WindowState.Maximized
            : WindowState.Normal;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.ChangedButton.Equals(MouseButton.Left))
            {
                return;
            }
            DragMove();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text))
                textBox.SelectAll();
        }

        private void ButtonSubsTblMaxSize_Click(object sender, RoutedEventArgs e) => GridViewSubsTable.Columns[5].Width = 40;

        private void ButtonSubsTblMinSize_Click(object sender, RoutedEventArgs e) => GridViewSubsTable.Columns[5].Width = 0;
    }
}
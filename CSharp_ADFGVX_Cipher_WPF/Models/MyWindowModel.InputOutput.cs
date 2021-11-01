using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        private string input;
        private string output;
        private bool mode;
        private readonly Dictionary<char, string> encryptionCharFilter;
        private const string cipherName = "ADFGVX";
        private const string cipherNameShort = "ADFGX";

        public ICommand CommandModeEncrypt
        { get => new CommandHandler(() => 
        { 
            if (!Mode)
            {
                Mode = true;
                Output = Encrypt(Input);
            }
        }, () => true); }

        public ICommand CommandModeDecrypt
        {
            get => new CommandHandler(() =>
            {
                if (Mode)
                {
                    Mode = false;
                    Output = Decrypt(Input);
                }
            }, () => true);
        }

        public ICommand CommandInputOpen
        {
            get => new CommandHandler(() =>
            {
                OpenFileDialog openFileDialog = new ();
                openFileDialog.Filter = "Text file (*.txt)|*.txt|Data file (*.dat)|*.dat";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                    Input = File.ReadAllText(openFileDialog.FileName).ToUpper();
            }, () => true);
        }

        public ICommand CommandOutputSave
        {
            get => new CommandHandler(() =>
            {
                SaveFileDialog saveFileDialog = new ();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt|Data file (*.dat)|*.dat";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (saveFileDialog.ShowDialog() == true)
                    File.WriteAllText(saveFileDialog.FileName, Output);
            }, () => true);
        }

        public ICommand CommandOutputCopy => new CommandHandler(() => Clipboard.SetText(Output), () => true);

        public ICommand CommandInputPaste => new CommandHandler(() => Input = Clipboard.GetText(), () => true);

        public string Input
        {
            get => input;
            set
            {
                SetValue(ref input, value);
                Output = Mode ? Encrypt(value) : Decrypt(value);
            }
        }

        public string Output
        {
            get => output;
            set => SetValue(ref output, value);
        }

        public bool Mode 
        { 
            get => mode; 
            set => mode = value; 
        }

        private void SetValue<T>(ref T store, T value, [CallerMemberName] string name = null)
        {
            if (Equals(store, value))
            {
                return;
            }

            store = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string FilterInputText(in string str)
        {
            if (input.Length.Equals(0))
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder(capacity: str.Length << 1);
            foreach (char c in str)
            {
                if (encryptionCharFilter.TryGetValue(c, out string s))
                {
                    _ = stringBuilder.Append(s);
                }
            }
            return stringBuilder.ToString();
        }

        private string Substitute(in char c)
        {
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    if (c.Equals(substitutionTable[i, j]))
                    {
                        return $"{cipherName[i]}{cipherName[j]}";
                    }
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        private char Substitute(in string str)
        {
            char c0 = str[0];
            char c1 = str[1];
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    if (c0.Equals(cipherName[i]) && c1.Equals(cipherName[j]))
                    {
                        return cipherName[i];
                    }
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        private string Encrypt(in string str)
        {
            if (!ValidateSubstitutionTable() || !ValidateKeyWord())
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder(capacity: str.Length << 1);
            foreach (char c in FilterInputText(str))
            {
                stringBuilder.Append(Substitute(c));
            }

            return SortAndSplitByKeyWord(stringBuilder.ToString());
        }

        private bool ValidateKeyWord()
        {
            if (keyWord.Length > 0 && keyWord.Length <= Input.Length << 1)
            {
                return true;
            }

            return false;
        }

        private bool ValidateSubstitutionTable()
        {
            for (int i = 0; i < 6; ++i)
            {
                if (!isFullSize && i.Equals(4))
                {
                    continue;
                }
                for (int j = 0; j < 6; ++j)
                {
                    if (!isFullSize && j.Equals(4))
                    {
                        continue;
                    }
                    if (!char.IsLetterOrDigit(substitutionTable[i, j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private string SortAndSplitByKeyWord(in string str)
        {
            // Length of substrings
            int subStringLength = Convert.ToInt32(Math.Ceiling(str.Length / (double)keyWord.Length));

            // Initialize field of StringBuilders
            List<Tuple<char, StringBuilder>> stringBuilders = new();
            for (int i = 0; i < keyWord.Length; ++i)
            {
                stringBuilders.Add(new Tuple<char, StringBuilder>(keyWord[i], new StringBuilder(capacity: subStringLength)));
            }

            // Split
            for (int j = 0; j < str.Length; ++j)
            {
                _ = stringBuilders[j % keyWord.Length].Item2.Append(str[j]);
            }

            return string.Join(" ", stringBuilders.OrderBy(entry => entry.Item1).Select(entry => entry.Item2));
        }

        private string Decrypt(in string str)
        {
            // Check input
            if (!ValidateSubstitutionTable() || !ValidateKeyWord())
            {
                return string.Empty;
            }

            // Filter input
            string strFiltered = new string(str.Where(c => isFullSize ? "ADFGVX".Contains(c) : "ADFGX".Contains(c)).ToArray());
            if (strFiltered.Length % 2 > 0)
            {
                return string.Empty;
            }
            int origLen = strFiltered.Length;

            // Length of substrings
            int lenSubstring = strFiltered.Length / keyWord.Length;
            int numLongerSubstrings = strFiltered.Length % keyWord.Length;

            // Initialize field of StringBuilders
            List<Tuple<int, char, int, char[]>> stringBuilders = new();
            for (int i = 0; i < keyWord.Length; ++i)
            {
                if (numLongerSubstrings > 0)
                {
                    stringBuilders.Add(new Tuple<int, char, int, char[]>(i, keyWord[i], lenSubstring + 1, new char[lenSubstring + 1]));
                    --numLongerSubstrings;
                    continue;
                }

                stringBuilders.Add(new Tuple<int, char, int, char[]>(i, keyWord[i], lenSubstring, new char[lenSubstring]));
            }

            // Order list
            stringBuilders = stringBuilders.OrderBy(entry => entry.Item2).ToList();

            // Split input
            int index = 0;
            foreach (Tuple<int, char, int, char[]> stringBuilder in stringBuilders)
            {
                //stringBuilder.Item4.Append(strFiltered.Take(stringBuilder.Item3));
                //strFiltered = strFiltered.Remove(0, stringBuilder.Item3 - 1);
                for (int i = 0; i < stringBuilder.Item3; ++i)
                {
                    stringBuilder.Item4[i] = strFiltered[index];
                    ++index;
                }
            }

            // Reorder list
            stringBuilders = stringBuilders.OrderBy(entry => entry.Item1).ToList();

            // Build output
            StringBuilder outputStrBuilder = new StringBuilder();
            for (int i = 0; i < origLen; ++i)
            {
                outputStrBuilder.Append(stringBuilders[i % KeyWord.Length].Item4[i / KeyWord.Length]);
                //_ = stringBuilders[i % KeyWord.Length].Item4.Remove(0, 1);
            }

            return outputStrBuilder.ToString();
        }
    }
}
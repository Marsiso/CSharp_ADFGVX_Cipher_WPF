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
            const string V = "ADFGVX";
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    if (c.Equals(substitutionTable[i, j]))
                    {
                        return $"{V[i]}{V[j]}";
                    }
                }
            }

            return "\n\n";
        }

        private char Substitute(in char c0, in char c1)
        {
            const string V = "ADFGVX";
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    if (c0.Equals(V[i]) && c1.Equals(V[j]))
                    {
                        return SubstitutionTable[i, j];
                    }
                }
            }

            return '\n';
        }

        private string Encrypt(in string str)
        {
            if (!ValidateSubstitutionTable() || !ValidateKeyWord())
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new(capacity: str.Length << 1);
            foreach (char c in FilterInputText(str))
            {
                _ = stringBuilder.Append(Substitute(c));
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
            int lenSubstring = str.Length / keyWord.Length;
            int numLongerSubstrings = str.Length % keyWord.Length;

            List<Tuple<char, char[]>> charArrays = new();
            for (int i = 0; i < keyWord.Length; ++i)
            {
                if (numLongerSubstrings > 0)
                {
                    charArrays.Add(new Tuple<char, char[]>(keyWord[i], new char[lenSubstring + 1]));
                    --numLongerSubstrings;
                    continue;
                }
                charArrays.Add(new Tuple<char, char[]>(keyWord[i], new char[lenSubstring]));
            }

            for (int j = 0; j < str.Length; ++j)
            {
                charArrays[j % keyWord.Length].Item2[j / keyWord.Length] = str[j];
            }

            charArrays = charArrays.OrderBy(entry => entry.Item1).ToList();
            StringBuilder stringBuilder = new(capacity: str.Length);
            charArrays.ForEach(entry =>
            {
                _ = stringBuilder.Append(entry.Item2);
            });

            return stringBuilder.ToString();
        }

        private string Decrypt(in string str)
        {
            if (!ValidateSubstitutionTable() || !ValidateKeyWord())
            {
                return string.Empty;
            }

            string strFiltered = new(str.Where(c =>
            {
                const string V = "ADFGVX";
                const string V1 = "ADFGX";
                return isFullSize ? V.Contains(c) : V1.Contains(c);
            }).ToArray());

            if (strFiltered.Length % 2 > 0)
            {
                return string.Empty;
            }

            int origLen = strFiltered.Length;
            int lenSubstring = strFiltered.Length / keyWord.Length;
            int numLongerSubstrings = strFiltered.Length % keyWord.Length;
            List<Tuple<int, char, int, char[]>> charArrays = new();
            for (int i = 0; i < keyWord.Length; ++i)
            {
                if (numLongerSubstrings > 0)
                {
                    charArrays.Add(new Tuple<int, char, int, char[]>(i, keyWord[i], lenSubstring + 1, new char[lenSubstring + 1]));
                    --numLongerSubstrings;
                    continue;
                }

                charArrays.Add(new Tuple<int, char, int, char[]>(i, keyWord[i], lenSubstring, new char[lenSubstring]));
            }

            charArrays = charArrays.OrderBy(entry => entry.Item2).ToList();
            int index = 0;
            foreach (Tuple<int, char, int, char[]> stringBuilder in charArrays)
            {
                for (int i = 0; i < stringBuilder.Item3; ++i)
                {
                    stringBuilder.Item4[i] = strFiltered[index];
                    ++index;
                }
            }

            charArrays = charArrays.OrderBy(entry => entry.Item1).ToList();
            StringBuilder outputStrBuilder = new();
            for (int i = 0; i < origLen; i += 2)
            {
                _ = outputStrBuilder.Append(Substitute(charArrays[i % KeyWord.Length].Item4[i / KeyWord.Length],
                    charArrays[(i + 1) % KeyWord.Length].Item4[(i + 1) / KeyWord.Length]));
            }

            return outputStrBuilder.ToString();
        }
    }
}
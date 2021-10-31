using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System;
using System.Linq;
using System.Windows.Input;

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

        ICommand CommandSetModeEncryption { get => new CommandHandler(() => 
        { 
            if (!Mode)
            {
                Mode = true;
            }
        }, () => true); }

        ICommand CommandSetModeDecryption
        {
            get => new CommandHandler(() =>
            {
                if (Mode)
                {
                    Mode = false;
                }
            }, () => true);
        }

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

        // TODO Re-Encrypt/Decrypt Input
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
            string strFiltered = str.Where(c => isFullSize ? "ADFGVX".Contains(c) : "ADFGX".Contains(c)).ToString();
            if (strFiltered.Length % 2 > 0)
            {
                return string.Empty;
            }

            // Length of substrings
            int subStringLength = Convert.ToInt32(Math.Ceiling(strFiltered.Length / (double)keyWord.Length));

            // Initialize field of StringBuilders
            List<Tuple<char, StringBuilder>> stringBuilders = new();
            for (int i = 0; i < keyWord.Length; ++i)
            {
                stringBuilders.Add(new Tuple<char, StringBuilder>(keyWord[i], new StringBuilder(capacity: subStringLength)));
            }

            // Order list
            stringBuilders = stringBuilders.OrderBy(entry => entry.Item1).ToList();

            // Build list of substrings
            int j = 0;
            for (int i = 0; i < strFiltered.Length; i += subStringLength, ++j)
            {
                _ = stringBuilders[j].Item2.Append(strFiltered, i, subStringLength);
            }

            // Reorder list
            StringBuilder[] stringBuildersReordered = new StringBuilder[keyWord.Length];
            if (keyWord.Length > 1)
            {
                for (int i = 0; i < keyWord.Length; ++i)
                {
                    char c = keyWord[i];
                    for (j = 0; j < stringBuilders.Count; ++j)
                    {
                        if (stringBuilders[j].Item1.Equals(c))
                        {
                            stringBuildersReordered[i] = stringBuilders[j].Item2;
                            stringBuilders.RemoveAt(j);
                            break;
                        }
                    }
                }
            }

            StringBuilder stringBuilder = new StringBuilder(capacity: strFiltered.Length);
            for (int i = 0; i < strFiltered.Length; ++i)
            {
                _ = stringBuilder.Append(stringBuildersReordered[i][0]);
                _ = stringBuildersReordered[i].Remove(0, 1);
            }

            // Substitute substring
            for (int i = 0; i < strFiltered.Length; i += 2)
            {
                string temp = new string($"{stringBuilder[i]}{stringBuilder[i + 1]}");
                stringBuilder[i] = temp[0];
                stringBuilder[i + 1] = temp[1];
            }

            // Join substrings and return value
            return stringBuilder.ToString();
        }
    }
}
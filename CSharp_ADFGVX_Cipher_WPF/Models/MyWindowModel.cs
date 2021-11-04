using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel : INotifyPropertyChanged
    {
        private bool isFullSize;
        private bool isLocalizationEnglish;
        private ObservableCollection<InputFilterObservableEntry> inputFilterObservableEntries;
        private readonly Dictionary<char, int> substitutionTableChars;
        private static readonly IReadOnlyList<string> charAsStringEnglish = new string[] { "XNULAX", "XIEDNAX", "XDVAX", "XTRIX", "XCTYRYX", "XPETX", "XSESTX", "XSEDUMX", "XOSUMX", "XDEVETX" };
        private static readonly IReadOnlyList<string> charAsStringCzech = new string[] { "XNULAX", "XJEDNAX", "XDVAX", "XTRIX", "XCTYRYX", "XPETX", "XSESTX", "XSEDUMX", "XOSUMX", "XDEVETX" };

        public ICommand CommandLocalizationEnglish => new CommandHandler(() =>
        {
            IsLocalizationEnglish = true;
            if (!isFullSize)
            {
                foreach (var entry in SubstitutionTableEntries)
                {
                    const char c = 'J';
                    if (entry.Col0Char.Equals(c))
                    {
                        entry.Col0Char = ' ';
                        Output = string.Empty;
                    }
                    else if (entry.Col1Char.Equals(c))
                    {
                        entry.Col1Char = ' ';
                        Output = string.Empty;
                    }
                    else if (entry.Col2Char.Equals(c))
                    {
                        entry.Col2Char = ' ';
                        Output = string.Empty;
                    }
                    else if (entry.Col3Char.Equals(c))
                    {
                        entry.Col3Char = ' ';
                        Output = string.Empty;
                    }
                    else if (entry.Col5Char.Equals(c))
                    {
                        entry.Col5Char = ' ';
                        Output = string.Empty;
                    }
                }
            }
        }, () => true);

        public ICommand CommandLocalizationCzech => new CommandHandler(() =>
        {
            IsLocalizationEnglish = false;
            if (!isFullSize)
            {
                foreach (var entry in SubstitutionTableEntries)
                {
                    const char c = 'Q';
                    if (entry.Col0Char.Equals(c))
                    {
                        entry.Col0Char = ' ';
                        Output = string.Empty;
                    }
                    else if (entry.Col1Char.Equals(c))
                    {
                        entry.Col1Char = ' ';
                        Output = string.Empty;
                    }
                    else if (entry.Col2Char.Equals(c))
                    {
                        entry.Col2Char = ' ';
                        Output = string.Empty;
                    }
                    else if (entry.Col3Char.Equals(c))
                    {
                        entry.Col3Char = ' ';
                        Output = string.Empty;
                    }
                    else if (entry.Col5Char.Equals(c))
                    {
                        entry.Col5Char = ' ';
                        Output = string.Empty;
                    }
                }
            }
        }, () => true);

        public Dictionary<char, int> SubstitutionTableChars { get => substitutionTableChars; }

        public record InputFilterObservableEntry(char Key, string Value);

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets localization.
        /// True means that English localization is in effect instead of Czech localization.
        /// </summary>
        public bool IsLocalizationEnglish
        {
            get => isLocalizationEnglish;
            set => SetLocalization(ref isLocalizationEnglish, value);
        }

        /// <summary>
        /// Gets or sets whenever the substitution table is at maximum size. (6x6)
        /// </summary>
        public bool IsFullSize
        {
            get => isFullSize;
            set => SetSize(ref isFullSize, value);
        }

        /// <summary>
        /// Parameterless constructor of class MyWindowModel.
        /// </summary>
        public MyWindowModel()
        {
            keyWordCharFilter = new Dictionary<char, char>
            {
                {'A', 'A'}, {'B', 'B'}, {'C', 'C'},
                {'D', 'D'}, {'E', 'E'}, {'F', 'F'},
                {'G', 'G'}, {'H', 'H'}, {'I', 'I'},
                {'J', 'I'}, {'K', 'K'}, {'L', 'L'},
                {'M', 'M'}, {'N', 'N'}, {'O', 'O'},
                {'P', 'P'}, {'Q', 'Q'}, {'R', 'R'},
                {'S', 'S'}, {'T', 'T'}, {'U', 'U'},
                {'V', 'V'}, {'W', 'W'}, {'X', 'X'},
                {'Y', 'Y'}, {'Z', 'Z'}, {'Á', 'A'},
                {'Č', 'C'}, {'Ď', 'D'}, {'É', 'E'},
                {'Ě', 'E'}, {'Í', 'I'}, {'Ň', 'N'},
                {'Ó', 'O'}, {'Ř', 'R'}, {'Š', 'S'},
                {'Ť', 'T'}, {'Ú', 'U'}, {'Ů', 'U'},
                {'Ý', 'Y'}, {'Ž', 'Z'}
            };
            encryptionCharFilter = new Dictionary<char, string>
            {
                {'A', "A"}, {'B', "B"}, {'C', "C"},
                {'D', "D"}, {'E', "E"}, {'F', "F"},
                {'G', "G"}, {'H', "H"}, {'I', "I"},
                {'J', "J"}, {'K', "K"}, {'L', "L"},
                {'M', "M"}, {'N', "N"}, {'O', "O"},
                {'P', "P"}, {'Q', "Q"}, {'R', "R"},
                {'S', "S"}, {'T', "T"}, {'U', "U"},
                {'V', "V"}, {'W', "W"}, {'X', "X"},
                {'Y', "Y"}, {'Z', "Z"}, {'0', "0"},
                {'1', "1"}, {'2', "2"}, {'3', "3"},
                {'4', "4"}, {'5', "5"}, {'6', "6"},
                {'7', "7"}, {'8', "8"}, {'9', "9"},
                {'Á', "A"}, {'Č', "C"}, {'Ď', "D"},
                {'É', "E"}, {'Ě', "E"}, {'Í', "I"},
                {'Ň', "N"}, {'Ó', "O"}, {'Ř', "R"},
                {'Š', "S"}, {'Ť', "T"}, {'Ú', "U"},
                {'Ů', "U"}, {'Ý', "Y"} ,{'Ž', "Z"},
                {' ', "XMEZERAX"}, {'\n', "XMEZERAX"}
            };
            decryptionCharFilter = new Dictionary<char, string>
            {
                {'A', "A"}, {'Á', "A"}, {'D', "D"}, {'Ď', "D"}, {'F', "F"},
                {'G', "G"}, {'V', "V"}, {'X', "X"}
            };
            substitutionTableChars = new Dictionary<char, int>()
            {
                {'A', 0}, {'B', 0}, {'C', 0},
                {'D', 0}, {'E', 0}, {'F', 0},
                {'G', 0}, {'H', 0}, {'I', 0},
                {'J', 0}, {'K', 0}, {'L', 0},
                {'M', 0}, {'N', 0}, {'O', 0},
                {'P', 0}, {'Q', 0}, {'R', 0},
                {'S', 0}, {'T', 0}, {'U', 0},
                {'V', 0}, {'W', 0}, {'X', 0},
                {'Y', 0}, {'Z', 0}, {'0', 0},
                {'1', 0}, {'2', 0}, {'3', 0},
                {'4', 0}, {'5', 0}, {'6', 0},
                {'7', 0}, {'8', 0}, {'9', 0}
            };
            keyWordCharCounter = new Dictionary<char, int>();
            isFullSize = true;
            isLocalizationEnglish = true;
            keyWord = string.Empty;
            input = string.Empty;
            output = string.Empty;
            mode = true;
            substitutionTable = new char[6, 6]
            {
                { ' ', ' ', ' ', ' ', ' ', ' '},
                { ' ', ' ', ' ', ' ', ' ', ' '},
                { ' ', ' ', ' ', ' ', ' ', ' '},
                { ' ', ' ', ' ', ' ', ' ', ' '},
                { ' ', ' ', ' ', ' ', ' ', ' '},
                { ' ', ' ', ' ', ' ', ' ', ' '}
            };
            inputFilterObservableEntries = new ObservableCollection<InputFilterObservableEntry>();
            substitutionTableEntries = new ObservableCollection<SubstitutionTableEntry>();
            charsRemainingSubsTblStr = string.Empty;

            IsFullSize = true;
            SubstitutionTableEntries = new ObservableCollection<SubstitutionTableEntry>();
            CharsRemainingSubsTblStr = string.Empty;
        }

        /// <summary>
        /// Sets localization and re-evaluates substitution table and its dependencies.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        private void SetLocalization(ref bool store, bool value, [CallerMemberName] string name = null)
        {
            int indexer = 0;
            switch (value, IsFullSize)
            {
                case (true, true):
                case (false, true):
                    foreach (KeyValuePair<char, string> keyValuePair in encryptionCharFilter.Where(entry => char.IsDigit(entry.Key)))
                    {
                        encryptionCharFilter[keyValuePair.Key] = $"{indexer}";
                        ++indexer;
                    }
                    encryptionCharFilter['J'] = "J";
                    encryptionCharFilter['Q'] = "Q";
                    foreach (KeyValuePair<char, int> keyValuePair in SubstitutionTableChars)
                    {
                        SubstitutionTableChars[keyValuePair.Key] = 0;
                    }
                    if (!decryptionCharFilter.ContainsKey('V'))
                    {
                        decryptionCharFilter.Add('V', "V");
                    }
                    break;

                case (true, false):
                    foreach (KeyValuePair<char, string> keyValuePair in encryptionCharFilter.Where(entry => char.IsDigit(entry.Key)))
                    {
                        encryptionCharFilter[keyValuePair.Key] = charAsStringEnglish[indexer];
                        ++indexer;
                    }
                    encryptionCharFilter['J'] = "I";
                    encryptionCharFilter['Q'] = "Q";
                    foreach (KeyValuePair<char, int> keyValuePair in SubstitutionTableChars)
                    {
                        if (char.IsDigit(keyValuePair.Key) || keyValuePair.Key.Equals('J'))
                        {
                            SubstitutionTableChars[keyValuePair.Key] = 1;
                            continue;
                        }
                        SubstitutionTableChars[keyValuePair.Key] = 0;
                    }
                    if (decryptionCharFilter.ContainsKey('V'))
                    {
                        decryptionCharFilter.Remove('V');
                    }
                    break;

                case (false, false):
                    foreach (KeyValuePair<char, string> keyValuePair in encryptionCharFilter.Where(entry => char.IsDigit(entry.Key)))
                    {
                        encryptionCharFilter[keyValuePair.Key] = charAsStringCzech[indexer];
                        ++indexer;
                    }
                    encryptionCharFilter['J'] = "J";
                    encryptionCharFilter['Q'] = "K";
                    foreach (KeyValuePair<char, int> keyValuePair in SubstitutionTableChars)
                    {
                        if (Char.IsDigit(keyValuePair.Key) || keyValuePair.Key.Equals('Q'))
                        {
                            SubstitutionTableChars[keyValuePair.Key] = 1;
                            continue;
                        }
                        SubstitutionTableChars[keyValuePair.Key] = 0;
                    }
                    if (decryptionCharFilter.ContainsKey('V'))
                    {
                        decryptionCharFilter.Remove('V');
                    }
                    break;

                default:
            }

            store = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            InputFilterObservableEntries = new ObservableCollection<InputFilterObservableEntry>();
        }

        /// <summary>
        /// Sets size and re-evaluates substitution table and its dependencies.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="store"></param>
        private void SetSize(ref bool store, bool value)
        {
            store = value;
            IsLocalizationEnglish = isLocalizationEnglish;
        }

        public ObservableCollection<InputFilterObservableEntry> InputFilterObservableEntries
        {
            get => inputFilterObservableEntries;
            set => SetInputFilterObservable(ref inputFilterObservableEntries, value);
        }

        private void SetInputFilterObservable(ref ObservableCollection<InputFilterObservableEntry> store,
            ObservableCollection<InputFilterObservableEntry> value, [CallerMemberName] string name = null)
        {
            if (Mode)
            {
                foreach (KeyValuePair<char, string> keyValuePair in encryptionCharFilter)
                {
                    value.Add(new InputFilterObservableEntry(keyValuePair.Key, keyValuePair.Value));
                }
            }
            else
            {
                foreach (KeyValuePair<char, string> keyValuePair in decryptionCharFilter)
                {
                    value.Add(new InputFilterObservableEntry(keyValuePair.Key, keyValuePair.Value));
                }
            }

            store = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
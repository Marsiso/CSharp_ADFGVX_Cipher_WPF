using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Text;
using System.Collections.ObjectModel;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel : INotifyPropertyChanged
    {
        private bool isFullSize;
        private bool isLocalizationEnglish;
        private ObservableCollection<ObsCollKeyValuePairInFilter> obsCollInFilter;
        private readonly Dictionary<char, int> subsTblCharsCounter;

        public Dictionary<char, int> SubsTblCharsCounter { get => subsTblCharsCounter; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets localization. 
        /// True means that english localization is in effect instead of czech localization.
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
            encryptionFilter = new Dictionary<char, string>
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
            subsTblCharsCounter = new Dictionary<char, int>()
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
            keyWordCharCounter = new Dictionary<char, int>() { };
            isFullSize = true;
            isLocalizationEnglish = true;
            keyWord = string.Empty;
            input = string.Empty;
            output = string.Empty;
            SubstitutionTable = new char[6, 6]
            {
                { ' ', ' ', ' ', ' ', ' ', ' '},
                { ' ', ' ', 'A', ' ', ' ', ' '},
                { ' ', ' ', ' ', ' ', ' ', ' '},
                { ' ', ' ', ' ', 'A', ' ', ' '},
                { ' ', ' ', ' ', ' ', 'A', ' '},
                { ' ', ' ', ' ', ' ', ' ', ' '}
            };
            obsCollInFilter = new ObservableCollection<ObsCollKeyValuePairInFilter>();
            subsTblRows = new ObservableCollection<SubsTblRow>();

            IsFullSize = true;
            SubsTblRows = new ObservableCollection<SubsTblRow>();
        }

        /// <summary>
        /// Sets localization and re-evaluates substitution table and its dependencies.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        private void SetLocalization(ref bool store, bool value, [CallerMemberName] string name = null)
        {
            switch (value, IsFullSize)
            {
                case (true, true):
                case (false, true):
                    encryptionFilter['0'] = "0";
                    encryptionFilter['1'] = "1";
                    encryptionFilter['2'] = "2";
                    encryptionFilter['3'] = "3";
                    encryptionFilter['4'] = "4";
                    encryptionFilter['5'] = "5";
                    encryptionFilter['6'] = "6";
                    encryptionFilter['7'] = "7";
                    encryptionFilter['8'] = "8";
                    encryptionFilter['9'] = "9";
                    encryptionFilter['J'] = "J";
                    encryptionFilter['Q'] = "Q";
                    foreach (KeyValuePair<char, int> keyValuePair in SubsTblCharsCounter)
                    {
                        SubsTblCharsCounter[keyValuePair.Key] = 0;
                    }
                    break;
                case (true, false):
                    encryptionFilter['0'] = "XNULAX";
                    encryptionFilter['1'] = "XIEDNAX";
                    encryptionFilter['2'] = "XDVAX";
                    encryptionFilter['3'] = "XTRIX";
                    encryptionFilter['4'] = "XCTYRYX";
                    encryptionFilter['5'] = "XPETX";
                    encryptionFilter['6'] = "XSESTX";
                    encryptionFilter['7'] = "XSEDMX";
                    encryptionFilter['8'] = "XOSMX";
                    encryptionFilter['9'] = "XDEVETX";
                    encryptionFilter['J'] = "I";
                    encryptionFilter['Q'] = "Q";
                    foreach (KeyValuePair<char, int> keyValuePair in SubsTblCharsCounter)
                    {
                        if (Char.IsDigit(keyValuePair.Key) || keyValuePair.Key.Equals('J'))
                        {
                            SubsTblCharsCounter[keyValuePair.Key] = 1;
                            continue;
                        }
                        SubsTblCharsCounter[keyValuePair.Key] = 0;
                    }
                    break;
                case (false, false):
                    encryptionFilter['0'] = "XNULAX";
                    encryptionFilter['1'] = "XJEDNAX";
                    encryptionFilter['2'] = "XDVAX";
                    encryptionFilter['3'] = "XTRIX";
                    encryptionFilter['4'] = "XCTYRYX";
                    encryptionFilter['5'] = "XPETX";
                    encryptionFilter['6'] = "XSESTX";
                    encryptionFilter['7'] = "XSEDMX";
                    encryptionFilter['8'] = "XOSMX";
                    encryptionFilter['9'] = "XDEVETX";
                    encryptionFilter['J'] = "J";
                    encryptionFilter['Q'] = "K";
                    foreach (KeyValuePair<char, int> keyValuePair in SubsTblCharsCounter)
                    {
                        if (Char.IsDigit(keyValuePair.Key) || keyValuePair.Key.Equals('Q'))
                        {
                            SubsTblCharsCounter[keyValuePair.Key] = 1;
                            continue;
                        }
                        SubsTblCharsCounter[keyValuePair.Key] = 0;
                    }
                    break;
                default:
            }

            store = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            ObsCollInFilter = new ObservableCollection<ObsCollKeyValuePairInFilter>();
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

        public ObservableCollection<ObsCollKeyValuePairInFilter> ObsCollInFilter
        {
            get => obsCollInFilter;
            set => SetObsCollInFilter(ref obsCollInFilter, value);
        }

        private void SetObsCollInFilter(ref ObservableCollection<ObsCollKeyValuePairInFilter> store,
            ObservableCollection<ObsCollKeyValuePairInFilter> value, [CallerMemberName] string name = null)
        {
            foreach (KeyValuePair<char, string> keyValuePair in encryptionFilter)
            {
                value.Add(new ObsCollKeyValuePairInFilter(keyValuePair.Key, keyValuePair.Value));
            }

            store = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public record ObsCollKeyValuePairInFilter(char Key, string Value);
    }
}

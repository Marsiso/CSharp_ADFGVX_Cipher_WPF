using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using System.Collections.Specialized;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        private readonly ObservableCollection<SubstitutionTableEntry> substitutionTableEntries;

        private readonly char[,] substitutionTable;
        private string charsRemainingSubsTblStr;

        public ICommand CommandSubsTblRandomize { get => new CommandHandler(() => 
        {
            EmptySubstitutionTable();
            RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
            char[] rndReorderedUsableChars = SubstitutionTableChars.
                Where(entry => entry.Value.Equals(0)).
                Select(entry => entry.Key).
                OrderBy(key => GetNextInt32(rnd)).
                ToArray();

            int multiplier = IsFullSize 
            ? 6 
            : 5;


            for (int i = 0; i < multiplier; ++i)
            {
                SubstitutionTableEntries[i].IsGeneratorActive = true;
                if (IsFullSize || !i.Equals(4))
                {
                    SubstitutionTableEntries[i].Col0Char = rndReorderedUsableChars[i * multiplier];
                    SubstitutionTableEntries[i].Col1Char = rndReorderedUsableChars[(i * multiplier) + 1];
                    SubstitutionTableEntries[i].Col2Char = rndReorderedUsableChars[(i * multiplier) + 2];
                    SubstitutionTableEntries[i].Col3Char = rndReorderedUsableChars[(i * multiplier) + 3];
                    if (!IsFullSize)
                    {
                        SubstitutionTableEntries[i].Col5Char = rndReorderedUsableChars[(i * multiplier) + 4];
                        continue;
                    }

                    SubstitutionTableEntries[i].Col4Char = rndReorderedUsableChars[(i * multiplier) + 4];
                    SubstitutionTableEntries[i].Col5Char = rndReorderedUsableChars[(i * multiplier) + 5];
                }
                else
                {
                    SubstitutionTableEntries[i + 1].Col0Char = rndReorderedUsableChars[i * multiplier];
                    SubstitutionTableEntries[i + 1].Col1Char = rndReorderedUsableChars[(i * multiplier) + 1];
                    SubstitutionTableEntries[i + 1].Col2Char = rndReorderedUsableChars[(i * multiplier) + 2];
                    SubstitutionTableEntries[i + 1].Col3Char = rndReorderedUsableChars[(i * multiplier) + 3];
                    SubstitutionTableEntries[i + 1].Col5Char = rndReorderedUsableChars[(i * multiplier) + 4];
                    break;
                }
                SubstitutionTableEntries[i].IsGeneratorActive = false;
            }

            CharsRemainingSubsTblStr = string.Empty;
            Output = Mode
                        ? Encrypt(Input)
                        : Decrypt(Input);
        },() => true); }

        private void EmptySubstitutionTable()
        {
            for (int i = 0; i < SubstitutionTableEntries.Count; i++)
            {
                SubstitutionTableEntries[i].IsGeneratorActive = true;
                SubstitutionTableEntry entry = SubstitutionTableEntries[i];
                entry.Col0Char = entry.Col1Char = entry.Col2Char = entry.Col3Char = entry.Col4Char = entry.Col5Char = ' ';
                SubstitutionTableEntries[i].IsGeneratorActive = false;
            }
        }

        public ICommand CommandSubsTblEmpty
        {
            get => new CommandHandler(() =>
            {
                EmptySubstitutionTable();
                Output = string.Empty;
            }, () => true);
        }

        public ICommand CommandSubsTblMaxSize
        {
            get => new CommandHandler(() =>
            {
                EmptySubstitutionTable();
                IsFullSize = true;
                SubstitutionTableEntries[4].EntryHeight = 25;
            }, () => true);
        }

        public ICommand CommandSubsTblMinSize
        {
            get => new CommandHandler(() =>
            {
                EmptySubstitutionTable();
                IsFullSize = false;
                SubstitutionTableEntries[4].EntryHeight = 0;
            }, () => true);
        }

        public ObservableCollection<SubstitutionTableEntry> SubstitutionTableEntries
        {
            get => substitutionTableEntries;
            init
            {
                substitutionTableEntries = value;
                for (int i = 0; i < 6; ++i)
                {
                    substitutionTableEntries.Add(new SubstitutionTableEntry(i, SubstitutionTable[i, 0], SubstitutionTable[i, 1], SubstitutionTable[i, 2],
                        SubstitutionTable[i, 3], SubstitutionTable[i, 4], SubstitutionTable[i, 5], this));
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubstitutionTableEntries)));
            }
        }

        public char[,] SubstitutionTable => substitutionTable;

        public string CharsRemainingSubsTblStr
        {
            get => charsRemainingSubsTblStr;
            set => SetCharsRemainingSubsTblEntries(ref charsRemainingSubsTblStr, value);
        }
     
        private void SetCharsRemainingSubsTblEntries(ref string store, string value,
            [CallerMemberName] string name = null)
        {
            StringBuilder stringBuilder = new StringBuilder(capacity: 36);
            foreach (char c in substitutionTableChars.Where(entry => entry.Value.Equals(0)).Select(entry => entry.Key))
            {
                bool isContained = false;
                for (int i = 0; i < 6; ++i)
                {
                    for (int j = 0; j < 6; ++j)
                    {
                        if (char.IsLetterOrDigit(SubstitutionTable[i, j]) && SubstitutionTable[i, j].Equals(c))
                        {
                            isContained = true;
                            break;
                        }
                    }
                    if (isContained)
                    {
                        break;
                    }
                }
                if (!isContained)
                {
                    stringBuilder.Append(c);
                }
            }

            store = stringBuilder.ToString();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

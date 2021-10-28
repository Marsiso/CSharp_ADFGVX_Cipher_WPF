using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        private readonly ObservableCollection<SubstitutionTableEntry> substitutionTableEntries;

        private readonly char[,] substitutionTable;
        private string charsRemainingSubsTblStr;

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

        public void DisplayContentsSubsTbl(int row, int col, char c)
        {
            SubstitutionTable[row, col] = c;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    _ = stringBuilder.Append(SubstitutionTable[i, j]);
                    _ = stringBuilder.Append(' ');
                }
                _ = stringBuilder.Append('\n');
            }
            Input = stringBuilder.ToString();
        }

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
            Input = stringBuilder.ToString();
        }
    }
}

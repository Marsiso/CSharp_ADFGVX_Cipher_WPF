using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        private readonly ObservableCollection<SubstitutionTableEntry> substitutionTableEntries;

        private readonly char[,] substitutionTable;

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
    }
}

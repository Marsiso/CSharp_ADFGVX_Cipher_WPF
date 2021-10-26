using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        private readonly ObservableCollection<SubsTblRow> subsTblRows;

        public readonly char[,] SubstitutionTable;

        public ObservableCollection<SubsTblRow> SubsTblRows
        {
            get => subsTblRows;
            init
            {
                subsTblRows = value;
                for (int i = 0; i < 6; ++i)
                {
                    subsTblRows.Add(new SubsTblRow(i, SubstitutionTable[i, 0], SubstitutionTable[i, 1], SubstitutionTable[i, 2],
                        SubstitutionTable[i, 3], SubstitutionTable[i, 4], SubstitutionTable[i, 5], this));
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubsTblRows)));
            }
        }

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

        public List<char> CountSubsTblChaOccurrences()
        {
            IEnumerable<char> allowedChars = SubsTblCharsCounter.Where(entry => entry.Value.Equals(0)).Select(entry => entry.Key);
            List<char> alreadyUsedChars = new List<char>();
            foreach (SubsTblRow entry in SubsTblRows)
            {
                if (!alreadyUsedChars.Contains(entry.Char0) && allowedChars.Contains(entry.Char0))
                {
                    alreadyUsedChars.Add(entry.Char0);
                }
                if (!alreadyUsedChars.Contains(entry.Char1) && allowedChars.Contains(entry.Char1))
                {
                    alreadyUsedChars.Add(entry.Char1);
                }
                if (!alreadyUsedChars.Contains(entry.Char2) && allowedChars.Contains(entry.Char2))
                {
                    alreadyUsedChars.Add(entry.Char2);
                }
                if (!alreadyUsedChars.Contains(entry.Char3) && allowedChars.Contains(entry.Char3))
                {
                    alreadyUsedChars.Add(entry.Char3);
                }
                if (!alreadyUsedChars.Contains(entry.Char4) && allowedChars.Contains(entry.Char4))
                {
                    alreadyUsedChars.Add(entry.Char4);
                }
                if (!alreadyUsedChars.Contains(entry.Char5) && allowedChars.Contains(entry.Char5))
                {
                    alreadyUsedChars.Add(entry.Char5);
                }
            }

            return Enumerable.Except(allowedChars, alreadyUsedChars).ToList();
        }
    }
}

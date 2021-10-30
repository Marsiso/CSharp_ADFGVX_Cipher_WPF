using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        public sealed class SubstitutionTableEntry : INotifyPropertyChanged
        {
            private char colHeader;
            private char col0Char;
            private char col1Char;
            private char col2Char;
            private char col3Char;
            private char col4Char;
            private char colChar5;
            private int entryHeight;

            public event PropertyChangedEventHandler PropertyChanged;
            public MyWindowModel MyWindowModel { get; set; }
            public int Id { get; init; }

            public SubstitutionTableEntry(int id, char c0, char c1, char c2, char c3,
                char c4, char c5, in MyWindowModel myWiewModel, int hight = 25)
            {
                Id = id;
                MyWindowModel = myWiewModel;
                col0Char = col1Char = col2Char = col3Char = col4Char = colChar5 =  ' ';
                entryHeight = hight;
                ColHeader = Id switch
                {
                    0 => 'A',
                    1 => 'D',
                    2 => 'F',
                    3 => 'G',
                    4 => 'V',
                    5 => 'X',
                    _ => ' '
                };
                Col0Char = c0;
                Col1Char = c1;
                Col2Char = c2;
                Col3Char = c3;
                Col4Char = c4;
                Col5Char = c5;
            }

            public char ColHeader
            {
                get => colHeader;
                init
                {
                    colHeader = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColHeader)));
                }
            }
            public char Col0Char 
            { 
                get => col0Char; 
                set => SetValue(ref col0Char, value); 
            }
            public char Col1Char 
            { 
                get => col1Char; 
                set => SetValue(ref col1Char, value); 
            }
            public char Col2Char 
            { 
                get => col2Char; 
                set => SetValue(ref col2Char, value); 
            }
            public char Col3Char 
            { 
                get => col3Char; 
                set => SetValue(ref col3Char, value); 
            }
            public char Col4Char 
            { 
                get => col4Char; 
                set => SetValue(ref col4Char, value); 
            }
            public char Col5Char 
            { 
                get => colChar5; 
                set => SetValue(ref colChar5, value); 
            }

            public int EntryHeight
            {
                get => entryHeight;
                set
                {
                    entryHeight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntryHeight)));
                }
            }


            private void SetValue(ref char store, char value, [CallerMemberName] string name = null)
            {
                store = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                int column = name switch
                {
                    nameof(Col0Char) => 0,
                    nameof(Col1Char) => 1,
                    nameof(Col2Char) => 2,
                    nameof(Col3Char) => 3,
                    nameof(Col4Char) => 4,
                    nameof(Col5Char) => 5,
                    _ => throw new ArgumentOutOfRangeException()
                };
                MyWindowModel.SubstitutionTable[Id, column] = value;
                MyWindowModel.CharsRemainingSubsTblStr = string.Empty;
            }
        }
    }
}

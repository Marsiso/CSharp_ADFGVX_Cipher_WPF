using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        public sealed class SubsTblRow : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public MyWindowModel MyWindowModel;
            private char char0;
            private char char1;
            private char char2;
            private char char3;
            private char char4;
            private char char5;

            public int Id { get; init; }

            public SubsTblRow(int id, char c0, char c1, char c2, char c3,
                char c4, char c5, in MyWindowModel myWindowModel)
            {
                Id = id;
                MyWindowModel = myWindowModel;
                char0 = c0;
                char1 = c1;
                char2 = c2;
                char3 = c3;
                char4 = c4;
                char5 = c5;
            }

            public char Char0 
            { 
                get => char0; 
                set => SetValue(ref char0, value); 
            }
            public char Char1 
            { 
                get => char1; 
                set => SetValue(ref char1, value); 
            }
            public char Char2 
            { 
                get => char2; 
                set => SetValue(ref char2, value); 
            }
            public char Char3 
            { 
                get => char3; 
                set => SetValue(ref char3, value); 
            }
            public char Char4 
            { 
                get => char4; 
                set => SetValue(ref char4, value); 
            }
            public char Char5 
            { 
                get => char5; 
                set => SetValue(ref char5, value); 
            }

            private void SetValue(ref char store, char value, [CallerMemberName] string name = null)
            {
                store = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                switch (name)
                {
                    case nameof(Char0):
                        MyWindowModel.DisplayContentsSubsTbl(Id, 0, value);
                        break;
                    case nameof(Char1):
                        MyWindowModel.DisplayContentsSubsTbl(Id, 1, value);
                        break;
                    case nameof(Char2):
                        MyWindowModel.DisplayContentsSubsTbl(Id, 2, value);
                        break;
                    case nameof(Char3):
                        MyWindowModel.DisplayContentsSubsTbl(Id, 3, value);
                        break;
                    case nameof(Char4):
                        MyWindowModel.DisplayContentsSubsTbl(Id, 4, value);
                        break;
                    case nameof(Char5):
                        MyWindowModel.DisplayContentsSubsTbl(Id, 5, value);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Security.Cryptography;

namespace CSharp_ADFGVX_Cipher_WPF.Models
{
    public sealed partial class MyWindowModel
    {
        /// <summary>
        /// Keeps copy of the string.
        /// </summary>
        private string keyWord;

        /// <summary>
        /// Dictionary used to filter chars in property KeyWord
        /// </summary>
        private readonly Dictionary<char, char> keyWordCharFilter;
        private readonly Dictionary<char, int> keyWordCharCounter;

        /// <summary>
        /// Gets or sets field KeyWord.
        /// </summary>
        public string KeyWord
        {
            get => keyWord;
            set => SetKeyWord(ref keyWord, value);
        }

        /// <summary>
        /// Sets value to the field KeyWord and invokes interface INotifyPropertyChanged.
        /// </summary>
        /// <param name="store"> Reference to an already existing field. </param>
        /// <param name="value"> Value to be set to the param store. </param>
        /// <param name="name"> Name of the calling method. </param>
        private void SetKeyWord(ref string store, string value, [CallerMemberName] string name = null)
        {
            if (store.Equals(value))
            {
                return;
            }

            keyWordCharCounter.Clear();
            StringBuilder strBuilder = new StringBuilder(capacity: value.Length);
            foreach (char c in value)
            {
                if (keyWordCharFilter.TryGetValue(c, out char newC))
                {
                    strBuilder.Append(newC);
                    if (keyWordCharCounter.ContainsKey(newC))
                    {
                        ++keyWordCharCounter[newC];
                    }
                    else
                    {
                        keyWordCharCounter.Add(newC, 1);
                    }
                }
            }

            store = strBuilder.ToString();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ICommand CommandKeyWordEmpty
        {
            get => new CommandHandler(() =>
            {
                KeyWord = string.Empty;
                Output = string.Empty;
            }, () => true);
        }

        public ICommand CommandKeyWordRandomize
        {
            get => new CommandHandler(() =>
            {
                if (!Mode)
                {
                    return;
                }

                RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                if (Input.Length > 0)
                {
                    KeyWord = new string(Enumerable
                        .Repeat(chars, Input
                        .Where(c => encryptionCharFilter.ContainsKey(c))
                        .Count() << 1)
                        .Select(s => s.OrderBy(c => GetNextInt32(rnd))
                        .First())
                        .ToArray());
                    Output = Mode
                        ? Encrypt(Input)
                        : Decrypt(Input);
                }
                else
                {
                    KeyWord = string.Empty;
                    Output = string.Empty;
                }
            }, () => true);
        }

        private static int GetNextInt32(RNGCryptoServiceProvider rnd)
        {
            byte[] randomInt = new byte[4];
            rnd.GetBytes(randomInt);
            return Convert.ToInt32(randomInt[0]);
        }
    }
}

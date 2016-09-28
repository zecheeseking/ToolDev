using System;
using System.Text;

namespace EX4
{
    internal class Program
    {
        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Enter a sentence: ");
            string sentence = Console.ReadLine();

            int start = 0, end = 0, digit = 0, letter = 0, punc = 0, symb = 0;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < sentence.Length; ++i)
            {
                if (Char.IsDigit(sentence[i]))
                    digit++;
                else if (Char.IsLetter(sentence[i]))
                    letter++;
                else if (Char.IsPunctuation(sentence[i]))
                    punc++;
                else if (Char.IsSymbol(sentence[i]))
                    symb++;

                if (i == sentence.Length - 1)
                {
                    string s = sentence.Substring(start, end - start + 1);
                    Console.WriteLine($"word: '{s}' at {start} length {end - start + 1}");
                    sb.Append($"{Reverse(s)} ");
                }
                else if (!Char.IsLetterOrDigit(sentence[i]) || i == sentence.Length - 1)
                {
                    string s = sentence.Substring(start, end - start);
                    if (end - start > 0)
                        Console.WriteLine($"word: '{s}' at {start} length {end - start}");
                    sb.Append($"{Reverse(s)} ");
                    start = end + 1;
                }
                end++;
            }

            Console.WriteLine();
            Console.WriteLine("New sentence: ");
            Console.WriteLine(sb.ToString());
            Console.WriteLine($"{digit} digit characters");
            Console.WriteLine($"{letter} letter characters");
            Console.WriteLine($"{punc} punctuation characters");
            Console.WriteLine($"{symb} symbol characters");

            Console.ReadLine();
        }
    }
}
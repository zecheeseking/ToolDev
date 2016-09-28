using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX5
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            char loop = 'y';

            Console.WriteLine("Guess a number between 0 and 9");

            Random rand = new Random();
            while (loop == 'y')
            {
                int target = rand.Next(0, 9);
                int guesses = 0;
                int guess = -1;

                while (target != guess)
                {
                    int.TryParse(Console.ReadLine(), out guess);
                    guesses++;
                    if (guess == target)
                        Console.WriteLine($"Your guess: {guess} was correct in {guesses} guesses!");
                    else
                        Console.WriteLine($"Your guess: {guess} was incorrect!");
                }



                Console.WriteLine("Play again? Y/N");
                loop = Console.ReadLine().ToLowerInvariant()[0];
            }
        }
    }
}
/*********************************************************************
* Floor(lg lg n) calculator program using C#.
*
* Sophia Sun, 9/2022
* *********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_1
{
    class Program
    {
        static void Main(string[] args)
        {
            /* The console interface of the program. */
            Console.WriteLine("Floor(lg lg n) computer!");
            while (true)
            {
                Console.Write("Enter N: ");
                int n = int.Parse(Console.ReadLine());
                int floor = Logn(Logn(n));
                Console.WriteLine("The value of Floor(lg lg {0})) is {1}.", n, floor);
            }
        }
        static int Logn(int n)
        {
            /* Define a new function calculating lg(n) using a simple while loop. */
            /* Call this function recursively to get the value of floor(lg lg n). */
            int i = 0;
            while (n > 1)
            {
                n = (int)Math.Floor((decimal) n / 2);
                i++;
            }
            return i;
        }
    }
}
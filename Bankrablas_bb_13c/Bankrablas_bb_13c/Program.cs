using System;
using System.Threading;

namespace Bankrablas_bb_13c
{
    /// <summary>
    /// Program futtatasa
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hányszor fusson le a szimuláció?");
            Console.Write("Darabszám:");
            int count = int.Parse(Console.ReadLine());
            Console.Clear();
            for (int i = 0; i < count; i++)
            {
                Town V = new Town();
                V.Simulation();
                Thread.Sleep(1500);
                Console.Clear();
            }
            Console.Write("Szimuláció vége");
            Environment.Exit(0);
            Console.ReadLine();
        }

    }
}

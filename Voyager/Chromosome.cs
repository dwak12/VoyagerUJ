using System;
using System.Collections.Generic;

namespace Voyager
{
    class Chromosome
    {
        private static Random randomGenerator = new Random();
        private readonly int BITS = Voyager.CITIES;
        private static readonly bool DEBUG = false;

        public byte[] Path { get; private set; }

        public void Seed()
        {           
            Path = SeedBits();          
        }

        public void Mutation()
        {
            var index = randomGenerator.Next(0, BITS);

            Path[index] = (byte)randomGenerator.Next(0, BITS);
        }

        private byte[] SeedBits()
        {
            var bitArray = new byte[BITS];
            for (int i = 0; i < BITS; i++)
                bitArray[i] = (byte)randomGenerator.Next(0, 255);

            return bitArray;
        }

        public void PrintCityPath()
        {
            var iter = BITS;
            foreach (var city in Path)
            {
                if (iter <= 0)
                {
                    iter = BITS;
                    Console.WriteLine();
                }
                iter--;
                Console.Write(city + " ");              
            }

            Console.WriteLine();
        }

        public void Crossing(byte[] firstParent, byte[] secondParent, int cutIndex)
        {
            Path = new byte[BITS];
            for (int i=0; i < BITS; i++)
            {
                if (i <= cutIndex)
                    Path[i] = firstParent[i];
                else
                    Path[i] = secondParent[i];
            }

            if (DEBUG)
                PrintCityPath();
        }
    }
}

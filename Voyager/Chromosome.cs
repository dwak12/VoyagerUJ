using System;

namespace Voyager
{
    class Chromosome
    {
        private static Random _randomGenerator = new Random();
        private readonly int BITS = Voyager.CITIES;
        private readonly bool DEBUG = false;

        public byte[] Path { get; private set; }

        public void Seed()
        {           
            Path = SeedBits();          
        }

        public void Mutate()
        {
            var index = _randomGenerator.Next(0, BITS);

            Path[index] = (byte)_randomGenerator.Next(0, BITS);
        }

        private byte[] SeedBits()
        {
            var bitArray = new byte[BITS];
            for (int i = 0; i < BITS; i++)
                bitArray[i] = (byte)_randomGenerator.Next(0, 255);

            return bitArray;
        }

        public void PrintCityPath(int number = 0)
        {
            var iter = BITS;
            ILogger logger = Logger.Instance;
            logger.AppendLog("\n [Chromosome "+ number+" Path]");
            foreach (var city in Path)
            {
                if (iter <= 0)
                {
                    iter = BITS;
                    logger.AppendLog("\n");
                }
                iter--;
                logger.AppendLog(city + " ");
            }

            logger.AppendLog("\n");
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
        }
    }
}

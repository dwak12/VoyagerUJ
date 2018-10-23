using System;
using System.Collections.Generic;

namespace Voyager
{
    class Voyager
    {
        public static readonly int CITIES = 100;
        public static readonly int GENERATIONS = 10000;
        public static readonly int GENERATIONS_GUARD = 1000; //if there will be no improve in 1000 generations -> stop
        public static readonly int MUTATION_IN_GENERATION = 5;
        public static readonly int RANGE = 255;
        public static readonly int POPULATION = 100;
        private static readonly bool DEBUG = true;
        private static Random randomGenerator = new Random();

        private int MinCost = int.MaxValue;
        private List<Chromosome> Population = new List<Chromosome>();
        private int WithoutChange = 0;
        public int[,] Prices { get; } = new int[CITIES, CITIES];

        public void CreateVoyage()
        {
            //prices
            for (int i = 0; i < Voyager.CITIES; i++)
                for (int j = 0; j < Voyager.CITIES; j++)
                    Prices[i, j] = randomGenerator.Next(10, 60);

            //create 1st generation
            for (int i = 0; i < Voyager.CITIES; i++)
                Population.Add(new Chromosome());

            foreach (var city in Population) //seed paths
                city.Seed();

            bool hasResult = false;
            for (int step = 0; step < GENERATIONS; step++)
            {
                for (int i = 0; i < CITIES; i++)
                {
                    var result = CalculateCost(i);
                    if (result < MinCost)
                    {
                        MinCost = result;
                        hasResult = true;
                    }
                }

                if (!hasResult)
                    WithoutChange++;
                else
                {
                    WithoutChange = 0;
                    hasResult = false;
                }

                if (DEBUG)
                    Console.WriteLine("[Minimal Cost] " + MinCost);

                if (WithoutChange == GENERATIONS_GUARD)
                {
                    Console.WriteLine("[INFO] " + GENERATIONS_GUARD + " generations without improve cost. Ending...");
                    return;
                }

                //crossing
                var pairsList = PreparePairs();
                CrossPairs(pairsList);

                //mutations
                CreateMutations(randomGenerator.Next(1, 11));

                //tournament selection
                while (Population.Count > 100)
                    TournamentSelection();
            }
        }

        private int CalculateCost(int childNumber)
        {
            int value = 0;
            var uncodedList = CreateCityList();
            bool firstIter = false;
            byte currentCity = 0;

            foreach (var city in Population[childNumber].Path)
            {
                var tempCity = city;

                while (tempCity >= uncodedList.Count)
                    tempCity -= (byte)uncodedList.Count;

                var realNumber = uncodedList[tempCity]; //real city number

                if (firstIter)
                    value += Prices[currentCity, realNumber];
                else
                    firstIter = true;

                currentCity = (byte)realNumber;
            }

            if (DEBUG)
                Console.WriteLine("[COST] " + value);

            return value;
        }

        private List<int> CreateCityList()
        {
            var cityList = new List<int>();
            for (int i = 0; i < CITIES; i++)
                cityList.Add(i);

            return cityList;
        }

        private List<Tuple<int, int>> PreparePairs()
        {
            var citiesCrossing = new List<Chromosome>(Population);
            var pairsList = new List<Tuple<int, int>>();

            for (int i = 0; i < CITIES / 2; i++)
            {
                var firstParent = randomGenerator.Next(0, citiesCrossing.Count);
                var secondParent = randomGenerator.Next(0, citiesCrossing.Count);

                while (firstParent == secondParent)
                    secondParent = randomGenerator.Next(0, citiesCrossing.Count);

                var pair = new Tuple<int, int>(firstParent, secondParent);
                pairsList.Add(pair);

                citiesCrossing.RemoveAt(firstParent);
                if (firstParent < secondParent)
                    citiesCrossing.RemoveAt(secondParent - 1);
                else
                    citiesCrossing.RemoveAt(secondParent);
            }

            return pairsList;
        }

        private void CrossPairs(List<Tuple<int, int>> pairsList)
        {
            foreach (var pair in pairsList)
            {
                var firstChild = new Chromosome();
                var secondChild = new Chromosome();
                var firstParent = Population[pair.Item1];
                var secondParent = Population[pair.Item2];

                var cutIndex = randomGenerator.Next(1, 99);
                firstChild.Crossing(firstParent.Path, secondParent.Path, cutIndex);
                secondChild.Crossing(secondParent.Path, firstParent.Path, cutIndex);

                Population.Add(firstChild);
                Population.Add(secondChild);
            }
        }

        private void CreateMutations(int howMany)
        {
            for (int i = 0; i < howMany; i++)
                Population[randomGenerator.Next(0, Population.Count)].Mutation();
        }

        private void TournamentSelection()
        {
            var firstPath = randomGenerator.Next(0, Population.Count);
            var secondPath = randomGenerator.Next(0, Population.Count);

            while (firstPath == secondPath)
                secondPath = randomGenerator.Next(0, Population.Count);

            if (CalculateCost(firstPath) > CalculateCost(secondPath))
                Population.RemoveAt(secondPath);
            else
                Population.RemoveAt(firstPath);
        }
    }
}

using System;
using System.Collections.Generic;

namespace Voyager
{
    class Voyager
    {
        public static readonly int CITIES = 100;
        private readonly int GENERATIONS = 10000;
        private readonly int GENERATIONS_GUARD = 1000; //if there will be no improve in 1000 generations -> stop
        private readonly int MUTATION_IN_GENERATION_MIN = 5;
        private readonly int MUTATION_IN_GENERATION_MAX = 11;
        private readonly int RANGE = 255;
        private readonly int POPULATION = 100;
        private readonly bool DEBUG = false;
        private readonly int MIN_PRICE = 10;
        private readonly int MAX_PRICE = 60;
        private static Random _randomGenerator = new Random();

        private int _minCost = int.MaxValue;
        private int _currentMinCost = int.MaxValue;
        private List<Chromosome> _population = new List<Chromosome>();
        private int _withoutChange = 0;

        public int[,] Prices { get; } = new int[CITIES, CITIES];

        public void CreateVoyage()
        {
            ILogger logger = Logger.Instance;

            //prices
            for (int i = 0; i < CITIES; i++)
                for (int j = 0; j < CITIES; j++)
                    Prices[i, j] = _randomGenerator.Next(MIN_PRICE, MAX_PRICE);

            PrintPricesTable();

            //create 1st generation
            for (int i = 0; i < CITIES; i++)
                _population.Add(new Chromosome());

            foreach (var city in _population) //seed paths
                city.Seed();

            var hasResult = false;
            for (int step = 0; step < GENERATIONS; step++)
            {
                logger.AppendLog("\n[" + (step + 1) + " Generation]");

                for (int i = 0; i < _population.Count; i++)
                    _population[i].PrintCityPath(i + 1);

                _currentMinCost = int.MaxValue;
                for (int i = 0; i < CITIES; i++)
                {
                    var result = CalculateCost(i);
                    logger.AppendLog("\n[COST] "+ (i+1) + " city cost -> " + result);

                    if (result < _minCost)
                    {
                        _minCost = result;
                        hasResult = true;
                    }

                    if (result < _currentMinCost)
                        _currentMinCost = result;
                }

                if (!hasResult)
                {
                    _withoutChange++;

                    if (DEBUG)
                        Console.WriteLine("[No changes] Minimal cost didn't changed");

                    logger.AppendLog("\n[No changes] Minimal cost didn't changed");
                }
                else
                {
                    _withoutChange = 0;
                    hasResult = false;
                }

                if (DEBUG)
                    Console.WriteLine("[Minimal Cost] Minimal cost in " + (step + 1) + " generation is " + _currentMinCost);

                logger.AppendLog("\n[Minimal Cost] Minimal cost in " + (step + 1) + " generation is " + _currentMinCost);

                if (_withoutChange == GENERATIONS_GUARD)
                {
                    Console.WriteLine("[INFO] " + GENERATIONS_GUARD + " generations without improve cost. Ending...");
                    logger.AppendLog("\n[INFO] " + GENERATIONS_GUARD + " generations without improve cost. Ending...");
                    Console.WriteLine("[END] After " + (step + 1) + " generations we receive minimal cost -> " + _minCost);
                    logger.AppendLog("\n[END] After " + (step + 1) + " generations we receive minimal cost -> " + _minCost);
                    return;
                }

                //crossing
                var pairsList = PreparePairs();
                CrossPairs(pairsList);

                //mutations
                CreateMutations(_randomGenerator.Next(MUTATION_IN_GENERATION_MIN, MUTATION_IN_GENERATION_MAX));

                //tournament selection
                while (_population.Count > 100)
                    TournamentSelection();
            }

            logger.AppendLog("\n[END] After 1000 generations we receive minimal cost -> " + _minCost);
            Console.WriteLine("\n[END] After 1000 generations we receive minimal cost -> " + _minCost);
        }

        private int CalculateCost(int childNumber)
        {
            int value = 0;
            var uncodedList = CreateCityList();
            bool firstIter = false;
            byte currentCity = 0;

            foreach (var city in _population[childNumber].Path)
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
            var citiesCrossing = new List<Chromosome>(_population);
            var pairsList = new List<Tuple<int, int>>();

            for (int i = 0; i < CITIES / 2; i++)
            {
                var firstParent = _randomGenerator.Next(0, citiesCrossing.Count);
                var secondParent = _randomGenerator.Next(0, citiesCrossing.Count);

                while (firstParent == secondParent)
                    secondParent = _randomGenerator.Next(0, citiesCrossing.Count);

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
                var firstParent = _population[pair.Item1];
                var secondParent = _population[pair.Item2];

                var cutIndex = _randomGenerator.Next(1, 99);
                firstChild.Crossing(firstParent.Path, secondParent.Path, cutIndex);
                secondChild.Crossing(secondParent.Path, firstParent.Path, cutIndex);

                _population.Add(firstChild);
                _population.Add(secondChild);
            }
        }

        private void CreateMutations(int howMany)
        {
            for (int i = 0; i < howMany; i++)
                _population[_randomGenerator.Next(0, _population.Count)].Mutate();
        }

        private void TournamentSelection()
        {
            var firstPath = _randomGenerator.Next(0, _population.Count);
            var secondPath = _randomGenerator.Next(0, _population.Count);

            while (firstPath == secondPath)
                secondPath = _randomGenerator.Next(0, _population.Count);

            if (CalculateCost(firstPath) > CalculateCost(secondPath))
                _population.RemoveAt(secondPath);
            else
                _population.RemoveAt(firstPath);
        }

        private void PrintPricesTable()
        {
            ILogger logger = Logger.Instance;
            logger.AppendLog("\n [Price Table]");
            for (int i = 0; i < CITIES; i++)
            {
                logger.AppendLog("\n City number " + i + "\n");
                for (int j = 0; j < CITIES; j++)
                    logger.AppendLog(Prices[i, j].ToString() + "  ");
            }
        }
    }
}

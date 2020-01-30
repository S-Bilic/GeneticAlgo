using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeneticAlgo.DataReader;

namespace GeneticAlgo
{
    class Program
    {
        private static int seeds = 0;
        private static int boundry = 0;
        private static double mutationChance = 0.0;
        private static int PopulationNumber = 0;
        private static Random rng = new Random();
        private static List<double> Coefficienten;
        private static double SSE;
        private static int TimesToRun = 10;
        private static ICrossover crossover;

        static void Main(string[] args)
        {
            var reader = new DataReader();
            var users = reader.ReadData();

            Console.WriteLine("Hoeveel seeds in de populatie?");
            seeds = int.Parse(Console.ReadLine());
            Console.WriteLine("Wat is de boundry waarde?");
            boundry = int.Parse(Console.ReadLine());
            Console.WriteLine("Hoe groot is de kanse op een mutatie? Tussen de 0,0 en 0,8");
            mutationChance = double.Parse(Console.ReadLine());

            //Pas hier crossover aan
            crossover = new SinglePointCrossover();

            RandomCoefficienten();
            User BestUser = null;

            List<Tuple<int, double, User, List<User>, List<double>>> Generations = new List<Tuple<int, double, User, List<User>, List<double>>>();

            for (int i = 0; i < TimesToRun; i++)
            {
                users.ForEach(x => x.CalculatePrediction(Coefficienten));
                users.ForEach(x => x.CalculateSE());

                var populationBest = GetBestUser(users);
                if (BestUser == null || BestUser.SE >= populationBest.SE)
                {
                    BestUser = populationBest;
                }
                else {
                    //Best user lost add instead of NOOBIE
                    var noobie = users.OrderByDescending(x => x.SE).FirstOrDefault();
                    int index = users.IndexOf(noobie);
                    users[index] = BestUser;
                }
                SSE = CalculateSSE(users);

                var totalFitness = users.Sum(x => SSE - x.SE);
                var chance = 0.00;
                foreach (var user in users)
                {
                    user.LowerBoundry = chance;
                    chance += (SSE - user.SE) / totalFitness;
                    user.UpperBoundry = chance;
                }

                //Getting new population
                var newPopulation = new List<User>();
                for (int y = 0; y < users.Count; y += 2)
                {
                    var child1 = new User();
                    var child2 = new User();
                    crossover.Use(GetParentRoulette(users), GetParentRoulette(users), Coefficienten.Count, out child1, out child2);
                    child1.Name = "Child :" + i + "-" + y;
                    child2.Name = "Child :" + i + "-" + (y + 1);
                    newPopulation.Add(child1);
                    newPopulation.Add(child2);
                }

                //Mutation
                var newCoefficienten = new List<double>();
                foreach(var item in Coefficienten)
                {
                    //returns the same value when no mutation happend
                    newCoefficienten.Add( Mutate(item));
                }

                Generations.Add(new Tuple<int, double, User, List<User>, List<double>>(i, SSE, BestUser, users, Coefficienten));

                users = newPopulation;
                Coefficienten = newCoefficienten;
            }
        }

        public static void RandomCoefficienten()
        {
            if (1 == 0)
            {
                Coefficienten = new List<double> { -0.10, -0.03, -0.03, -0.01, 0.22, -0.27, -0.24, 0.35, 0.29, 0.33,
                    0.19, 0.23, 0.15, 0.16, -0.16, 0.16, 0.19, -0.21, 0.24, 0.48 };
            }
            else
            {
                Coefficienten = new List<double>();
                for (int i = 0; i < seeds; i++)
                {
                    Coefficienten.Add(GetRandomCoefficient());
                }
            }
        }

        public static double GetRandomCoefficient()
        {
            return rng.NextDouble() * (boundry - (boundry * -1)) + (boundry * -1);
        }

        public static User GetParentRoulette(List<User> users)
        {
            var percent = rng.NextDouble();
            return users.FirstOrDefault(x => x.LowerBoundry <= percent && x.UpperBoundry >= percent);
        }

        public static double CalculateSSE(List<User> users)
        {
            return users.Sum(x => x.SE);
        }

        private static User GetBestUser(List<User> Users)
        {
            return Users.OrderBy(x => x.SE).FirstOrDefault();
        }

        public static double Mutate(double value)
        {
            if (rng.NextDouble() <= mutationChance)
            {
                return GetRandomCoefficient();
            }
            return value;
        }
    }
}

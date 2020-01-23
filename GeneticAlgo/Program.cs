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

        static void Main(string[] args)
        {
            var reader = new DataReader();
            var users = reader.ReadData();

            Console.WriteLine("Hoeveel seeds in de populatie?");
            seeds = int.Parse(Console.ReadLine());
            Console.WriteLine("Wat is de boundry waarde?");
            boundry = int.Parse(Console.ReadLine());
            Console.WriteLine("Hoe groot is de kanse op een mutatie? Tussen de 0.0 en 0.8");
            mutationChance = double.Parse(Console.ReadLine());

            RandomCoefficienten();

            users.ForEach(x => x.CalculatePrediction(Coefficienten));
            users.ForEach(x => x.CalculateSE());
            SSE = CalculateSSE(users);
            GetNewUserRoulette(users);
        }

        public static void RandomCoefficienten()
        {
            if (1 == 1)
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

        public static User GetNewUserRoulette(List<User> users)
        {
            var percent = rng.NextDouble();
            double currChance = 0.00;
            var returnUser = new User();
            foreach (var user in users)
            {
                var userPercent = user.SE / SSE;
                if (percent > currChance && percent < currChance + userPercent)
                {
                    returnUser = user;
                    break;
                }
                else {
                    currChance += userPercent;
                }
            }
            return returnUser;
        }

        public static double CalculateSSE(List<User> users)
        {
            return users.Sum(x => x.SE);
        }

        public static int Mutate(int value)
        {
            if (rng.NextDouble() <= mutationChance)
            {
                value = rng.Next(0, 2);
            }
            return value;
        }
    }
}

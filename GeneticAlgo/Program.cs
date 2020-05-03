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
        private static int coeffecients = 0;
        private static int boundry = 0;
        private static double mutationChance = 0.0;
        private static int PopulationNumber = 0;
        private static Random rng = new Random();
        private static List<double> Coefficienten;
        private static double SSE;
        private static int TimesToRun = 0;
        private static string input = "";
        private static ICrossover crossover;

        static void Main(string[] args)
        {
            var reader = new DataReader();
            var users = reader.ReadData();

            Console.WriteLine("Hoeveel wegingscoeffcoefficienten rijen in de populatie?");
            coeffecients = int.Parse(Console.ReadLine());
            Console.WriteLine("Hoeveel seeds in de populatie?");
            TimesToRun = int.Parse(Console.ReadLine());
            Console.WriteLine("Wat is de boundry waarde?");
            boundry = int.Parse(Console.ReadLine());
            Console.WriteLine("Hoe groot is de kanse op een mutatie? Tussen de 0,0 en 0,8");
            mutationChance = double.Parse(Console.ReadLine());

            //Pas hier crossover aan
            Console.WriteLine("Kies een crossover type: ( 1 = SinglePointCrossover, 2 = TwoPointCrossover");
            input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    crossover = new SinglePointCrossover();
                    break;
                case "2":
                    crossover = new TwoPointCrossover();
                    break;
            }
            
            RandomCoefficienten();
            User BestUser = null;

            List<Tuple<int, double, User, List<User>, List<double>>> Generations = new List<Tuple<int, double, User, List<User>, List<double>>>();

            var finalGenerations = new List<string>();

            for (int i = 0; i < TimesToRun; i++)
            {
                // Voor iedere user(klant) bereken prediction
                users.ForEach(x => x.CalculatePrediction(Coefficienten));

                // Voor iedere user(klant) bereken SE
                users.ForEach(x => x.CalculateSE());

                // Beste user
                var populationBest = GetBestUser(users);

                // Als de BestUser SE groter of gelijk is aan de populationBest (user) SE
                // -> Krjgt BestUser de lagere SE value van populationBest
                if (BestUser == null || BestUser.SE >= populationBest.SE)
                {
                    BestUser = populationBest;
                }
                else {
                    // Best user lost add instead of NOOBIE (slechtste user eruit en beste erin)
                    var normal = users.OrderByDescending(x => x.SE).FirstOrDefault();
                    int index = users.IndexOf(normal);
                    users[index] = BestUser;
                }
                SSE = CalculateSSE(users);

                // Bereken fitness
                var totalFitness = users.Sum(x => SSE - x.SE);
                // Begin kans met 0.00
                var chance = 0.00;
                // De kans die je hebt van alle personen om naar de volgende generatie meegenomen te worden
                foreach (var user in users)
                {
                    user.LowerBoundry = chance;
                    chance += (SSE - user.SE) / totalFitness;
                    user.UpperBoundry = chance;
                }

                // Pak een nieuwe population
                var newPopulation = new List<User>();
                for (int y = 0; y < users.Count; y += 2)
                {
                    var child1 = new User();
                    var child2 = new User();
                    // Zet twee mensen bij elkaar bij crossover
                    crossover.Use(GetParentRoulette(users), GetParentRoulette(users), Coefficienten.Count, out child1, out child2);
                    child1.Name = "Child :" + i + "-" + y;
                    child2.Name = "Child :" + i + "-" + (y + 1);
                    newPopulation.Add(child1);
                    newPopulation.Add(child2);
                }

                // Mutation
                var newCoefficienten = new List<double>();
                foreach(var item in Coefficienten)
                {
                    // Returns dezelfde value als er geen mutation is
                    newCoefficienten.Add( Mutate(item));
                }

                // Add new generation
                Generations.Add(new Tuple<int, double, User, List<User>, List<double>>(i, SSE, BestUser, users, Coefficienten));

                users = newPopulation;
                Coefficienten = newCoefficienten;       
            }

            // Console output de nieuwe generations
            foreach (var generation in Generations)
            {
                finalGenerations.Add(generation.Item3.Name);
            }

            foreach (var user in finalGenerations)
            {
                Console.WriteLine(user);
            }
            Console.ReadLine();
        }

        public static void RandomCoefficienten()
        {
            // Gebruik standaard coeffecienten
            if (1 == 1)
            {
                Coefficienten = new List<double> { -0.0983106577130155, -0.0268214662524457, -0.0279307030767681, -0.0132976378558994, 0.21639365104045, -0.274081437598624, -0.238067676313213, 0.345585082985844, 0.294124576583616, 0.32535035300671,
                    0.193626383239627, 0.229924694623707, 0.145772728647483, 0.16053320805572, -0.159073123120079, 0.164688584886269, 0.187782769602548, -0.207459419313849, 0.239908530788513, 0.484221514164208 };
            }
            // Anders gebruik de nieuwe random coeffecienten
            else
            {
                Coefficienten = new List<double>();
                for (int i = 0; i < coeffecients; i++)
                {
                    Coefficienten.Add(GetRandomCoefficient());
                }
            }
        }

        // Random coefficienten berekening
        public static double GetRandomCoefficient()
        {
            return rng.NextDouble() * (boundry - (boundry * -1)) + (boundry * -1);
        }

        // A lower -> 0.00 upper 3.00
        // B Lower -> 3.00 upper 4.50
        // C Lower -> 4.50 upper 5.50
        public static User GetParentRoulette(List<User> users)
        {
            var percent = rng.NextDouble();
            return users.FirstOrDefault(x => x.LowerBoundry <= percent && x.UpperBoundry >= percent);
        }

        public static double CalculateSSE(List<User> users)
        {
            return users.Sum(x => x.SE);
        }

        // Pak de beste user method
        private static User GetBestUser(List<User> Users)
        {
            return Users.OrderBy(x => x.SE).FirstOrDefault();
        }
    
        // Mutation kans berekenen
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

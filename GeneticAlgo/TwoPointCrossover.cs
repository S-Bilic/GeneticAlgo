using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeneticAlgo.DataReader;

namespace GeneticAlgo
{
    public class TwoPointCrossover : ICrossover
    {
        private Random rng = new Random();
        private Tuple<int, int> CrossoverPoints;

        public void Use(User Parent1, User Parent2, int TotalCoefficienten, out User Child1, out User Child2)
        {
            SetCrossoverPoint(TotalCoefficienten);
            NewChildren(Parent1, Parent2, out Child1, out Child2);
        }

        public void SetCrossoverPoint(int TotalCoefficienten)
        {
            double rng1 = rng.Next(25, 40) * 1.00 / 100;
            double rng2 = rng.Next(60, 75) * 1.00 / 100;
            int value1 = (int)Math.Round((1 - rng1) * TotalCoefficienten, 0);
            int value2 = (int)Math.Round((1 - rng2) * TotalCoefficienten, 0);
            CrossoverPoints = new Tuple<int, int>(value2, value1);
        }

        public void NewChildren(User Parent1, User Parent2, out User Child1, out User Child2)
        {
            Child1 = new User
            {
                Pregnant = Parent1.Pregnant,
                DNA = new List<int>()
            };

            Child2 = new User
            {
                Pregnant = Parent2.Pregnant,
                DNA = new List<int>()
            };

            Child1.DNA.AddRange(Parent1.DNA.GetRange(0, CrossoverPoints.Item1));
            Child1.DNA.AddRange(Parent2.DNA.GetRange(CrossoverPoints.Item1, CrossoverPoints.Item2 - CrossoverPoints.Item1));
            Child1.DNA.AddRange(Parent1.DNA.GetRange(CrossoverPoints.Item2, Parent1.DNA.Count - CrossoverPoints.Item2));

            Child2.DNA.AddRange(Parent2.DNA.GetRange(0, CrossoverPoints.Item1));
            Child2.DNA.AddRange(Parent1.DNA.GetRange(CrossoverPoints.Item1, CrossoverPoints.Item2 - CrossoverPoints.Item1));
            Child2.DNA.AddRange(Parent2.DNA.GetRange(CrossoverPoints.Item2, Parent2.DNA.Count - CrossoverPoints.Item2));

        }
    }
}

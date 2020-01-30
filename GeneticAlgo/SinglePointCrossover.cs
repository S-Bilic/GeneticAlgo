using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgo;
using static GeneticAlgo.DataReader;

namespace GeneticAlgo
{
    public class SinglePointCrossover : ICrossover
    {
        private Random rng = new Random();
        private int CrossoverPoint;

        public void Use(User Parent1, User Parent2, int TotalCoefficienten, out User Child1, out User Child2)
        {
            SetCrossoverPoint(TotalCoefficienten);
            NewChildren(Parent1, Parent2, out Child1, out Child2);
        }

        public void SetCrossoverPoint(int TotalCoefficienten)
        {
            double randomValue = rng.Next(64, 85) * 1.00 / 100;
            CrossoverPoint = (int)Math.Round((1 - randomValue) * TotalCoefficienten, 0);
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

            Child1.DNA.AddRange(Parent1.DNA.GetRange(0, CrossoverPoint));
            Child1.DNA.AddRange(Parent2.DNA.GetRange(CrossoverPoint, Parent2.DNA.Count - CrossoverPoint));
            Child2.DNA.AddRange(Parent2.DNA.GetRange(0, CrossoverPoint));
            Child2.DNA.AddRange(Parent1.DNA.GetRange(CrossoverPoint, Parent1.DNA.Count - CrossoverPoint));
        }
    }
}

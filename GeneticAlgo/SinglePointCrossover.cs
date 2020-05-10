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
            // RandomValue = 0.66 (bijv)
            double randomValue = rng.Next(64, 85) * 1.00 / 100;
            // CrossoverPoint = 7 (bijv)
            CrossoverPoint = (int)Math.Round((1 - randomValue) * TotalCoefficienten, 0);
        }

        // Children worden nieuwe users met de waardes van hun parent
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
            
            // DNA = 20 (Bijv)
            // Child 1 heeft een crossoverpunt bij 7e getal van zijn parent DNA: 0101010-(7) 
            Child1.DNA.AddRange(Parent1.DNA.GetRange(0, CrossoverPoint));
            // Child 1 krijgt de rest van de punten van parent 2, dus 20-7 = 13 : 0101010-(7)-0101010101010(13)
            Child1.DNA.AddRange(Parent2.DNA.GetRange(CrossoverPoint, Parent2.DNA.Count - CrossoverPoint));

            // Repeat voor child 2 maar andersom
            Child2.DNA.AddRange(Parent2.DNA.GetRange(0, CrossoverPoint));
            Child2.DNA.AddRange(Parent1.DNA.GetRange(CrossoverPoint, Parent1.DNA.Count - CrossoverPoint));
        }
    }
}

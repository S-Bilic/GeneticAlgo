using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeneticAlgo.DataReader;

namespace GeneticAlgo
{
    public interface ICrossover
    {
        void Use(User Parent1, User Parent2, int TotalCoefficienten, out User Child1, out User Child2);

        void SetCrossoverPoint(int TotalCoefficienten);

        void NewChildren(User Parent1, User Parent2, out User Child1, out User Child2);
    }
}

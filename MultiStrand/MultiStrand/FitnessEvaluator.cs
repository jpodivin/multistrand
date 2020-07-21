using System;
using System.Collections.Generic;
using System.Text;

namespace MultiStrand
{
    static class FitnessEvaluator
    {
        public static Tuple<double, Genome> EvaluateGenome(string target, Genome genome)
        {
            double fitness = 0.0;
            for (int i = 0; i < genome.GenomeLength; i++)
            {
                if (genome.Genes[i].Equals(target[i]))
                {
                    fitness++;
                }
            }

            return new Tuple<double, Genome>(fitness/genome.GenomeLength, genome);
        }

    }
}

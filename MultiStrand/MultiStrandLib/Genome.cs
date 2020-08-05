using System;

namespace MultiStrandLib
{
    public class Genome
    {
        private string genes;
        private int genomeLength;
        public string Genes { get => genes; set => genes = value; }
        public int GenomeLength { get => genomeLength; set => genomeLength = value; }
        Random randomGenerator;
        public Genome(int genomeLength, int randomGeneratorSeed = 0)
        {
            /*
             Default randomGeneratorSeed value will instantiate the randomGenerator with default constructor.
             Othrewise we use the passed value as seed.
             */
            if (randomGeneratorSeed == 0)
            {
                randomGenerator = new Random();
            }
            else
            {
                randomGenerator = new Random(randomGeneratorSeed);
            }

            this.genomeLength = genomeLength;
            genes = "";

            for (int i = 0; i < genomeLength; i++)
            {
                /*
                Range 8 - 128 is bit wider than needed.
                But it includes all chars needed to replicate normal latin text,
                while still not making it too easy for the algorithm.
                 */
                genes += Convert.ToChar(randomGenerator.Next(8, 128));
            }
                    

            
        }

        public Genome(string genes, int randomGeneratorSeed = 0)
        {
            /*
             Default randomGeneratorSeed value will instantiate the randomGenerator with default constructor.
             Othrewise we use the passed value as seed.
             */
            if (randomGeneratorSeed == 0)
            {
                randomGenerator = new Random();
            }
            else
            {
                randomGenerator = new Random(randomGeneratorSeed);
            }

            this.genes = genes;
            this.genomeLength = this.genes.Length;                         
        }

        public int Mutate(double mutationRate = 0.1)
        {
            /*
             TODO: Get rid of this random generator. 
            It's overhead that serves nothing and leftover from prototype. 
             */
            for (int i = 0; i < genomeLength; i++)
            {
                if (randomGenerator.NextDouble() <= mutationRate)
                {
                    genes = genes.Substring(0, i)
                        + Convert.ToChar(randomGenerator.Next(8, 128))
                        + genes.Substring(i + 1);
                }
            }

            return 1;
        }

        public Genome Cross(Genome otherGenome)
        {
            string crossedGenome = otherGenome.Genes.Substring(0, genomeLength / 2)
                + genes.Substring(genomeLength / 2);

            return new Genome(crossedGenome, randomGenerator.Next());
        }

    }
}

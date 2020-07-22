using System;

namespace MultiStrandLib
{
    public class Genome
    {
        private string genes;
        private int genomeLength;
        public string Genes { get => genes; set => genes = value; }
        public int GenomeLength { get => genomeLength; set => genomeLength = value; }

        public Genome(int genomeLength)
        {
            this.genomeLength = genomeLength;

            Random random = new Random();

            genes = "";

            for (int i = 0; i < genomeLength; i++)
            {
                genes += Convert.ToChar(random.Next(8, 128));
            }
        }

        public Genome(string genes)
        {
            this.genes = genes;
            this.genomeLength = this.genes.Length;
        }

        public int Mutate(double mutationRate = 0.1)
        {
            Random random = new Random();
            for (int i = 0; i < genomeLength; i++)
            {
                if (random.NextDouble() <= mutationRate)
                {
                    genes = genes.Substring(0, i)
                        + Convert.ToChar(random.Next(8, 128))
                        + genes.Substring(i + 1);
                    break;
                }
            }

            return 1;
        }

        public Genome Cross(Genome otherGenome)
        {
            string crossedGenome = otherGenome.Genes.Substring(0, genomeLength / 2)
                + genes.Substring(genomeLength / 2);

            return new Genome(crossedGenome);
        }

    }
}

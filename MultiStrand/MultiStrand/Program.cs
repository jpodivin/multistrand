using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MultiStrandLib;
using System.IO;

namespace MultiStrand
{
    class Program
    {
        static void Main(string[] args)
        {
            string targetPhenotype;
            int popSize;
            long maxGenerations;
            long totalGenerationsPassed = 0;
            double bestFitness = 0.0;
            double mutationRate;
            double crossoverRate;

            Random random = new Random();
            Stopwatch stopwatch = new Stopwatch();
            List<Genome> population = new List<Genome>();

            List<Tuple<double, Genome>> evaluatedPopulation = new List<Tuple<double, Genome>>();

            List<Task<Tuple<double, Genome>>> taskList = new List<Task<Tuple<double, Genome>>>();

            (popSize, 
                maxGenerations, 
                mutationRate, 
                crossoverRate, 
                targetPhenotype) = MultiStrandCLISetup();

            for (int i = 0; i < popSize; i++)
            {
                /*
                The initial population has random generators seeded with randomly generated numbers.
                That way the states of generators won't be the same and mutations will be applied in more stochastic manner. 
                 */
                population.Add(new Genome(targetPhenotype.Length, random.Next()));
            }

            stopwatch.Start();

            while (bestFitness != 1 && maxGenerations <= totalGenerationsPassed)
            {                   
                foreach (Genome genome in population)
                {
                    taskList.Add(
                        Task.Run(
                            () => FitnessEvaluator.EvaluateGenome(targetPhenotype, genome)));
                }
                for (int i = 0; i < popSize; i++)
                {
                    evaluatedPopulation.Add(taskList[i].Result);
                }

                evaluatedPopulation = evaluatedPopulation.OrderBy(
                    genomeFitnessPair => genomeFitnessPair.Item1).ToList();

                //Clear lists 
                population.Clear();
                taskList.Clear();

                //Keep only the 10 best individuals
                evaluatedPopulation = evaluatedPopulation.TakeLast(10).ToList();

                bestFitness = evaluatedPopulation.Last().Item1;

                //Print results every 10th generations or if the optimum fitness is met.
                if (totalGenerationsPassed % 10 == 0 || bestFitness == 1.0) {
                    Console.WriteLine("Generation: {2} \nBest fitness: {0} \nText: {1} \n\n",
                        bestFitness,
                        evaluatedPopulation.First().Item2.Genes,
                        totalGenerationsPassed);
                }                

                for (int index = 0; index < popSize; index++)
                {
                    if (random.NextDouble() <= crossoverRate)
                    {
                        population.Add(
                            evaluatedPopulation[(index + 1) % evaluatedPopulation.Count].Item2.Cross(
                            evaluatedPopulation[index % evaluatedPopulation.Count].Item2));
                    }
                    else
                    {
                        population.Add(evaluatedPopulation[index % evaluatedPopulation.Count].Item2);
                    }
                }

                foreach (Genome genome in population)
                {
                    genome.Mutate(mutationRate);
                }

                population.Add(evaluatedPopulation[0].Item2);

                totalGenerationsPassed++;
            }

            stopwatch.Stop();

            Console.WriteLine("Time elapsed: {0:T}", stopwatch.Elapsed);
            Console.WriteLine("Over {0} generations.", totalGenerationsPassed);
        }

        private static Tuple<int, long, double, double, string> MultiStrandCLISetup()
        {
            string input;
            /*
               Default values for population size, maximum generations, mutation and crossover rate as well as target phenotype. 
               The parameters should be chosen with respect to your systems resources.
            */

            int selectedPopsize = 1000;
            long selectedMaxGenerations = -1;
            double selectedMutationRate = 0.01;
            double selectedCrossoverRate = 1.0;

            string selectedTargetPhenotype;
            using (StreamReader reader = File.OpenText("../../../loremipsum.txt"))
            {
                selectedTargetPhenotype = reader.ReadToEnd();
            }

            Console.Write("Enter desired population size (default 1000): ");
            input = Console.ReadLine();
            selectedPopsize = input == "" ? selectedPopsize : Convert.ToInt32(input);

            Console.Write("Enter desired mutation rate (default = 0.01): ");
            input = Console.ReadLine();
            selectedMutationRate = input == "" ? selectedMutationRate : Convert.ToDouble(input);

            Console.Write("Enter desired crossover rate (default = 1.0): ");
            input = Console.ReadLine();
            selectedCrossoverRate = input == "" ? selectedCrossoverRate : Convert.ToDouble(input);

            Console.Write("Enter target phenotype (default = Lorem ipsum...): ");
            input = Console.ReadLine();
            selectedTargetPhenotype = input == "" ? selectedTargetPhenotype : input;

            Console.Write("Enter maximum number of generations (default = until fitness 1.0 reached): ");
            input = Console.ReadLine();
            selectedMaxGenerations = input == "" ? selectedMaxGenerations : Convert.ToInt64(input);

            return new Tuple<int, long, double, double, string>(
                selectedPopsize, 
                selectedMaxGenerations, 
                selectedMutationRate, 
                selectedCrossoverRate, 
                selectedTargetPhenotype);
        }
    }
}

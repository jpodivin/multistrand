﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MultiStrandLib;

namespace MultiStrand
{
    class Program
    {
        static void Main(string[] args)
        {
            string targetPhenotype;
            int popSize;
            double bestFitness = 0.0;
            double mutationRate;
            double crossoverRate;

            Random random = new Random();
            Stopwatch stopwatch = new Stopwatch();
            List<Genome> population = new List<Genome>();

            List<Tuple<double, Genome>> evaluatedPopulation = new List<Tuple<double, Genome>>();

            List<Task<Tuple<double, Genome>>> taskList = new List<Task<Tuple<double, Genome>>>();

            (popSize, mutationRate, crossoverRate, targetPhenotype) = MultiStrandCLISetup();

            for (int i = 0; i < popSize; i++)
            {
                population.Add(new Genome(targetPhenotype.Length));
            }

            stopwatch.Start();

            while (bestFitness != 1)
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

                evaluatedPopulation = evaluatedPopulation.OrderBy(genomeFitnessPair => genomeFitnessPair.Item1).Reverse().ToList();

                //Clear lists 
                population.Clear();
                taskList.Clear();

                //Keep only the 10 best individuals
                evaluatedPopulation = evaluatedPopulation.Take(10).ToList();

                bestFitness = evaluatedPopulation.First().Item1;

                Console.WriteLine("Best fitness: {0}\nText: {1} \n\n",
                    bestFitness,
                    evaluatedPopulation.First().Item2.Genes);

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
            }

            stopwatch.Stop();

            Console.WriteLine("Time elapsed: {0:T}", stopwatch.Elapsed);

        }

        private static Tuple<int, double, double, string> MultiStrandCLISetup()
        {
            string input;
            /*
               Default values for population size and target phenotype. 
               The population size and target phenotype should be chosen with respect to your systems resources.
            */

            int selectedPopsize = 1000;
            double selectedMutationRate = 0.01;
            double selectedCrossoverRate = 1.0;

            string selectedTargetPhenotype = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, \n" +
                "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\n" +
                " Ut enim ad minim veniam, quis nostrud exercitation ullamco \n" +
                "laboris nisi ut aliquip ex ea commodo consequat.\n" +
                " Duis aute irure dolor in reprehenderit in voluptate \n" +
                "velit esse cillum dolore eu fugiat nulla pariatur. \n" +
                "Excepteur sint occaecat cupidatat non proident, \n" +
                "sunt in culpa qui officia deserunt mollit anim id est laborum.";

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

            return new Tuple<int, double, double,string>(selectedPopsize, selectedMutationRate, selectedCrossoverRate, selectedTargetPhenotype);
        }
    }

}

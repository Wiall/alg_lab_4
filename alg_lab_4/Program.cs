using System;
using System.Collections.Generic;
using System.Linq;

namespace alg_lab_4
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AntColonyOptimization.GenerateDistanceMatrix();
            AntColonyOptimization.InitializePheromones();
            //AntColonyOptimization.PrintAll();
        
            double Lmin = AntColonyOptimization.FindGreedySolution();
            Console.WriteLine($"Initial greedy solution: Lmin = {Lmin}");

            double bestLength = double.MaxValue;
            List<int> bestTour = null;
            List<double> qualityOverIterations = new List<double>();

            for (int iteration = 0; iteration < AntColonyOptimization.MaxIterations; iteration++)
            {
                List<(double length, List<int> tour)> allTours = new List<(double, List<int>)>();

                for (int ant = 0; ant < AntColonyOptimization.M; ant++)
                {
                    List<int> tour = AntColonyOptimization.ConstructSolution();
                    double tourLength = AntColonyOptimization.CalculateTourLength(tour);
                    allTours.Add((tourLength, tour));
                }

                var (bestLocalLength, bestLocalTour) = allTours.OrderBy(t => t.length).First();
                if (bestLocalLength < bestLength)
                {
                    bestLength = bestLocalLength;
                    bestTour = bestLocalTour;
                }

                AntColonyOptimization.UpdatePheromones(allTours);

                // Запис значення цільової функції кожні 20 ітерацій
                if (iteration % 20 == 0)
                {
                    Console.WriteLine($"Iteration {iteration}, Best Length: {bestLength}");
                    qualityOverIterations.Add(bestLength);
                }
            }

            Console.WriteLine("Final best tour length: " + bestLength);
            AntColonyOptimization.PlotGraph(qualityOverIterations);
        }
    }
}
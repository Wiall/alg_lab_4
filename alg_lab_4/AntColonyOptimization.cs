using System;
using System.Collections.Generic;
using System.Linq;

class AntColonyOptimization
{
    const int N = 100;          // cities count
    public const int M = 30;    // ants count
    public const int MaxIterations = 1000;
    const double Alpha = 2.0;
    const double Beta = 4.0;
    const double Rho = 0.4;
    const double InitialPheromone = 1.0;

    static Random random = new Random();
    static double[,] distances = new double[N, N];
    static double[,] pheromones = new double[N, N];
    
    public static void GenerateDistanceMatrix()
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = i + 1; j < N; j++)
            {
                distances[i, j] = distances[j, i] = random.Next(5, 51);
            }
        }
    }

    public static void InitializePheromones()
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                pheromones[i, j] = InitialPheromone;
            }
        }
    }

    public static double FindGreedySolution()
    {
        List<int> tour = new List<int> { 0 };
        while (tour.Count < N)
        {
            int lastCity = tour.Last();
            int nextCity = Enumerable.Range(0, N)
                                     .Where(c => !tour.Contains(c))
                                     .OrderBy(c => distances[lastCity, c])
                                     .First();
            tour.Add(nextCity);
        }
        return CalculateTourLength(tour);
    }

    public static List<int> ConstructSolution()
    {
        List<int> tour = new List<int> { random.Next(N) };
        while (tour.Count < N)
        {
            int currentCity = tour.Last();
            int nextCity = SelectNextCity(tour, currentCity);
            tour.Add(nextCity);
        }
        return tour;
    }

    static int SelectNextCity(List<int> tour, int currentCity)
    {
        double[] probabilities = new double[N];
        double sum = 0;

        for (int j = 0; j < N; j++)
        {
            if (tour.Contains(j)) continue;
            double tau = Math.Pow(pheromones[currentCity, j], Alpha);
            double eta = Math.Pow(1.0 / distances[currentCity, j], Beta);
            probabilities[j] = tau * eta;
            sum += probabilities[j];
        }

        double rand = random.NextDouble() * sum;
        for (int j = 0; j < N; j++)
        {
            if (tour.Contains(j)) continue;
            rand -= probabilities[j];
            if (rand <= 0) return j;
        }

        return -1;
    }

    public static double CalculateTourLength(List<int> tour)
    {
        double length = 0;
        for (int i = 0; i < N - 1; i++)
        {
            length += distances[tour[i], tour[i + 1]];
        }
        length += distances[tour[N - 1], tour[0]];  // returns to start city 
        return length;
    }

    public static void UpdatePheromones(List<(double length, List<int> tour)> tours)
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                pheromones[i, j] *= (1 - Rho);
            }
        }

        foreach (var (length, tour) in tours)
        {
            double delta = 1.0 / length;
            for (int i = 0; i < N - 1; i++)
            {
                pheromones[tour[i], tour[i + 1]] += delta;
            }
            pheromones[tour[N - 1], tour[0]] += delta;
        }
    }

    public static void PlotGraph(List<double> qualityOverIterations)
    {
        Console.WriteLine("Quality over iterations:");
        for (int i = 0; i < qualityOverIterations.Count; i++)
        {
            Console.WriteLine($"{i * 20}: {qualityOverIterations[i]}");
        }
    }

    public static void PrintAll()
    {
        for (var i = 0; i < N; i++)
        {
            for (var j = 0; j < N; j++)
            {
                Console.Write(distances[i,j] + ",");
            }
            Console.Write('\n');
        }
    }
}

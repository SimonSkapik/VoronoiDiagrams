using FortuneVoronoi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

internal class Program
{
    //static void Main(string[] args)
    //{
    //    // Create a new GameGrid object
    //    GameGrid grid = new GameGrid();

    //    // Initialize the grid with a size of 50x50
    //    grid.CreateGrid(50, 50);

    //    // Populate the grid with 10 different seeds and Voronoi diagrams
    //    grid.PopulateGridWithVoronoi(10);

    //    // Write the grid to the console output
    //    grid.WriteGrid();

    //    Console.ReadKey();
    //}
}

internal class GameTile
{
    public char TileType { get; set; }

    public GameTile(char tileType)
    {
        TileType = tileType;
    }
}

internal class GameGrid
{
    private GameTile[,] grid;

    public void CreateGrid(int rows, int columns)
    {
        grid = new GameTile[rows, columns];
    }

    public void PopulateGridWithVoronoi(int numSeeds)
    {
        // Generate random seed points
        Random rand = new Random();
        List<Vector2> seedPoints = new List<Vector2>();
        for (int i = 0; i < numSeeds; i++)
        {
            float x = rand.Next(grid.GetLength(0));
            float y = rand.Next(grid.GetLength(1));
            seedPoints.Add(new Vector2(x, y));
        }

        // Generate Voronoi diagram using seed points
        VoronoiGraph voronoi = Fortune.ComputeVoronoiGraph(seedPoints);
        var edges = voronoi.Edges;//GenerateVoronoiDiagram(seedPoints, grid.GetLength(0), grid.GetLength(1));

        // Assign biomes based on Voronoi regions
        Dictionary<Vector2, char> biomeMap = new Dictionary<Vector2, char>();
        char biomeType = 'A';
        foreach (Edge edge in edges)
        {
            Vector2 p1 = new Vector2((float)Math.Round(edge.LeftData.X), (float)Math.Round(edge.LeftData.Y));
            Vector2 p2 = new Vector2((float)Math.Round(edge.RightData.X), (float)Math.Round(edge.RightData.Y));
            var regionPoints = voronoi.Vertices; //voronoi.GetRegion(edge);
            foreach (Vector2 point in regionPoints)
            {
                if (!biomeMap.ContainsKey(point))
                {
                    biomeMap[point] = biomeType;
                }
            }
            biomeType = (char)(biomeType + 1);
        }

        // Assign tile types based on biomes
        foreach (KeyValuePair<Vector2, char> kvp in biomeMap)
        {
            int x = (int)kvp.Key.X;
            int y = (int)kvp.Key.Y;
            grid[x, y] = new GameTile(kvp.Value);
        }
    }

    public void WriteGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Console.Write(grid[i, j].TileType + " ");
            }
            Console.WriteLine();
        }
    }
}
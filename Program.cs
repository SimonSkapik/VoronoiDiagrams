using System;

class Program
{
    static void Main(string[] args)
    {
        // Create a new GameGrid object
        GameGrid grid = new GameGrid();

        // Initialize the grid with a size of 10x10
        grid.CreateGrid(28, 90);

        // Populate the grid with random tiles
        grid.PopulateGrid(20);

        // Write the grid to the console output
        grid.WriteGrid();

        Console.ReadKey();
    }
}

class GameTile
{
    public List<(int, double)> TileType { get; set; }

    public GameTile(List<(int, double)> tileType)
    {
        TileType = tileType;
    }
}

class GameGrid
{
    private GameTile[,] grid;

    public void CreateGrid(int rows, int columns)
    {
        grid = new GameTile[rows, columns];
    }

    ConsoleColor[] lightColors = new ConsoleColor[]
    {
        ConsoleColor.Yellow,
        ConsoleColor.Cyan,
        ConsoleColor.Magenta,
        ConsoleColor.White,
        ConsoleColor.DarkYellow,
        ConsoleColor.DarkCyan,
        ConsoleColor.DarkMagenta,
        ConsoleColor.Gray,
        ConsoleColor.DarkGray,
        ConsoleColor.Blue,
        ConsoleColor.Red,
        ConsoleColor.Green,
        ConsoleColor.DarkGreen,
        ConsoleColor.DarkBlue,
        ConsoleColor.DarkRed,
        ConsoleColor.DarkCyan,
        ConsoleColor.DarkYellow,
        ConsoleColor.DarkGray,
        ConsoleColor.DarkMagenta,
        ConsoleColor.DarkGreen
    };

    private List<(int x, int y, int tileType)> seeds;

    public void PopulateGrid(int numSeeds)
    {
        var aaa = ConsoleColor.DarkCyan;

        // Create seeds
        seeds = new List<(int x, int y, int tileType)>();
        var Width = grid.GetLength(0);
        var Height = grid.GetLength(1);
        var random = new Random();
        for (int i = 0; i < numSeeds; i++)
        {
            bool isCloseToOtherSeed = false;
            do
            {
                int x = random.Next(0, Width);
                int y = random.Next(0, Height);
                int tileType = i % 6;

                // Check if seed is too close to existing seeds
                isCloseToOtherSeed = seeds.Any(seed =>
                {
                    double distance = Math.Sqrt((seed.x - x) * (seed.x - x) + (seed.y - y) * (seed.y - y));
                    return distance < 3;
                });

                if (!isCloseToOtherSeed)
                {
                    seeds.Add((x, y, tileType));
                }
            } while (isCloseToOtherSeed);
        }

        // Populate grid with closest seed type
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Find closest seed to current tile
                
                
                var seedDists = seeds.Select(s => (Seed: s, Distance: Math.Sqrt((s.x - x) * (s.x - x) + (s.y - y) * (s.y - y)))).ToList();
                var closestSeed = seedDists.MinBy(s => s.Distance);
                var neighbouringSeeds = seedDists.Where(s => s.Distance >= closestSeed.Distance && s.Distance <= (closestSeed.Distance + 2) && s.Seed.tileType != closestSeed.Seed.tileType)/*.DistinctBy(s => s.Seed.tileType)*/.ToList();

                if (neighbouringSeeds.Any())
                {
                    var influences = neighbouringSeeds.Union(new[] { closestSeed }).Select(s => (Type: s.Seed.tileType,
                        Influence: (4 - (s.Distance - closestSeed.Distance)))).ToList();
                    var totalInfluence = influences.Sum(i => i.Influence);
                    for (int i = 0; i < influences.Count; i++)
                    {
                        influences[i] = (influences[i].Type, influences[i].Influence / totalInfluence);
                    }
                    grid[x, y] = new GameTile(influences);
                }
                else
                {
                    grid[x, y] = new GameTile(new List<(int, double)>() { ( closestSeed.Seed.tileType, 1) }); 
                }
                
                // Set tile type to closest seed type
                
                
            }
        }
    }

    public ConsoleColor mixColors(List<(int, double)> influences)
    {
        //if (colors.Length != probabilities.Length || probabilities.Sum() != 1)
        //{
        //    throw new ArgumentException("Invalid input");
        //}

        double cumulativeProbability = 0;
        double randomNumber = new Random().NextDouble();

        //var colors = influences.Keys.ToList();
        //var probabilities = influences.Values.ToList();
        for (int i = 0; i < influences.Count; i++)
        {
            cumulativeProbability += influences[i].Item2;
            if (randomNumber < cumulativeProbability)
            {
                return lightColors[influences[i].Item1];
            }
        }

        return ConsoleColor.Black; // fallback color if no color is selected
    }

    public void WriteGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                //Console.Write(Chars[.TileType] + " ");
                Console.ForegroundColor = mixColors(grid[i, j].TileType);
                //Console.Write(Chars[grid[i, j].TileType] + " ");
                Console.Write(seeds.Any(s => s.x == i && s.y == j) ? "X" : "█");
            }
            Console.WriteLine();
        }
    }
}

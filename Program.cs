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
    public int TileType { get; set; }

    public GameTile(int tileType)
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
                double minDistance = double.MaxValue;
                int closestTileType = ' ';
                foreach (var seed in seeds)
                {
                    double distance = Math.Sqrt((seed.x - x) * (seed.x - x) + (seed.y - y) * (seed.y - y));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTileType = seed.tileType;
                    }
                }

                // Set tile type to closest seed type
                grid[x, y] = new GameTile(closestTileType);
            }
        }
    }

    public void WriteGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Console.ForegroundColor = lightColors[grid[i, j].TileType];
                //Console.Write(Chars[grid[i, j].TileType] + " ");
                Console.Write(seeds.Any(s => s.x == i && s.y == j) ? "X" : "█");
            }
            Console.WriteLine();
        }
    }
}

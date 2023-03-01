using FortuneVoronoi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GridTest
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int GRID_SIZE = 1;
        public const int BIOME_EDGE = 6;
        public const int SEEDS = 60;
        public const int SEEDS_OFFSET = 30;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var gg = new GameGrid();
            gg.CreateGrid((int)(Canvas.ActualWidth/GRID_SIZE), (int)(Canvas.ActualHeight/ GRID_SIZE));
            gg.PopulateGrid(SEEDS);
            gg.DrawGrid(Canvas);

            Loaded -= OnLoaded;
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
        const int GRID_SIZE = MainWindow.GRID_SIZE;
        const int BIOME_EDGE = MainWindow.BIOME_EDGE;
        const int SEEDS_OFFSET = MainWindow.SEEDS_OFFSET;

        private GameTile[,] grid;

        public void CreateGrid(int rows, int columns)
        {
            grid = new GameTile[rows, columns];
        }

        Color[] lightColors = new Color[]
        {
            Color.FromRgb(255, 255, 0),      // Yellow
            Color.FromRgb(0, 255, 255),      // Cyan
            Color.FromRgb(255, 0, 255),      // Magenta
            Color.FromRgb(255, 255, 255),    // White
            Color.FromRgb(255, 215, 0),      // DarkYellow
            Color.FromRgb(0, 139, 139),      // DarkCyan
            Color.FromRgb(139, 0, 139),      // DarkMagenta
            Color.FromRgb(128, 128, 128),    // Gray
            Color.FromRgb(169, 169, 169),    // DarkGray
            Color.FromRgb(0, 0, 255)         // Blue
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
                    int tileType = i % 10;

                    // Check if seed is too close to existing seeds
                    isCloseToOtherSeed = seeds.Any(seed =>
                    {
                        double distance = Math.Sqrt((seed.x - x) * (seed.x - x) + (seed.y - y) * (seed.y - y));
                        return distance < SEEDS_OFFSET;
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
                    var neighbouringSeeds = seedDists.Where(s => s.Distance >= closestSeed.Distance && s.Distance <= (closestSeed.Distance + BIOME_EDGE) && s.Seed.tileType != closestSeed.Seed.tileType).ToList();

                    if (neighbouringSeeds.Any())
                    {
                        var influences = neighbouringSeeds.Union(new[] { closestSeed }).Select(s =>
                        {
                            var type = s.Seed.tileType;
                            var inf = (BIOME_EDGE - (s.Distance - closestSeed.Distance));
                            inf *= inf;
                            return (Type: type, Influence: inf);
                        }).ToList();
                        var totalInfluence = influences.Sum(i => i.Influence);
                        for (int i = 0; i < influences.Count; i++)
                        {
                            influences[i] = (influences[i].Type, influences[i].Influence / totalInfluence);
                        }
                        grid[x, y] = new GameTile(influences);
                    }
                    else
                    {
                        grid[x, y] = new GameTile(new List<(int, double)>() { (closestSeed.Seed.tileType, 1) });
                    }

                    // Set tile type to closest seed type


                }
            }
        }

        public Color mixColors(List<(int, double)> influences)
        {
            //double cumulativeProbability = 0;
            //double randomNumber = new Random().NextDouble();


            //for (int i = 0; i < influences.Count; i++)
            //{
            //    cumulativeProbability += influences[i].Item2;
            //    if (randomNumber < cumulativeProbability)
            //    {
            //        return lightColors[influences[i].Item1];
            //    }
            //}

            //return Colors.Black;
            {{double totalWeight = 0;}}
            double red = 0, green = 0, blue = 0;

            foreach ((int colorIndex, double weight) in influences)
            {
                //totalWeight += weight;
                red += weight * lightColors[colorIndex].R;
                green += weight * lightColors[colorIndex].G;
                blue += weight * lightColors[colorIndex].B;
            }

            //if (totalWeight == 0) return Colors.Black;}}

            //red /= totalWeight;
            //green /= totalWeight;
            //blue /= totalWeight;

            return Color.FromRgb((byte)red, (byte)green, (byte)blue);
        }

        public void DrawGrid(Canvas canvas)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    //Console.Write(Chars[.TileType] + " ");

                    //Console.Write(Chars[grid[i, j].TileType] + " ");
                    if(seeds.Any(s => s.x == i && s.y == j))
                        _drawRect(new Point(i * GRID_SIZE, j * GRID_SIZE), Colors.Black, canvas);
                    else
                        _drawRect(new Point(i * GRID_SIZE, j * GRID_SIZE), mixColors(grid[i, j].TileType), canvas);
                    //Console.Write(seeds.Any(s => s.x == i && s.y == j) ? "X" : "█");
                }
                Console.WriteLine();
            }

            void _drawRect(Point pos, Color color, Canvas canvas)
            {
                Rectangle r = new Rectangle
                {
                    Width = GRID_SIZE,
                    Height = GRID_SIZE,
                    Fill = new SolidColorBrush(color)
                };

                // ... Set canvas position based on Rect object.
                Canvas.SetLeft(r, pos.X);
                Canvas.SetTop(r, pos.Y);

                // ... Add to canvas.
                canvas.Children.Add(r);
            }
        }
    }
}

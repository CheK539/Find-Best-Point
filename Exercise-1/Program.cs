using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Exercise_1
{
    public static class Program
    {
		public static void Main()
		{
			Console.WriteLine(@"Enter amount energy:");
			var energy = int.Parse(Console.ReadLine() ?? throw new NullReferenceException());
			var map = GetMap();
			var price = GetPrice(map);
			var pathPoint = BreadthSearch(map, energy, price);
			var farPoint = GetFarSavePoint(map, pathPoint);
			Console.WriteLine(farPoint);
			Console.ReadKey();
		}

		private static char[,] GetMap()
		{
			var strings = File.ReadAllLines("map.txt");
			var map = new char[strings.Length, strings[0].Length];
			for (var i = 0; i < map.GetLength(0); i++)
				for (var j = 0; j < map.GetLength(1); j++)
					map[i, j] = strings[i][j];
			return map;
		}

		private static Dictionary<char, int> GetPrice(char[,] map)
		{
			var price = new Dictionary<char, int>();
			foreach (var symbol in map)
			{
				if (price.ContainsKey(symbol)) continue;
				price.Add(symbol, symbol == 'F' ? 1 : int.Parse(symbol.ToString()));
			}

			return price;
		}

		private static Point GetFarSavePoint(char[,] map, IEnumerable<Point> points)
		{
			var farPoint = new Point(0, 0);
			var isMore = new Func<Point, Point, bool>((firstPoint, secondPoint) =>
				firstPoint.X > secondPoint.X || firstPoint.Y > secondPoint.Y);
			foreach (var point in points)
			{
				if (map[point.X, point.Y] == 'F' && isMore(point, farPoint))
					farPoint = point;
				else
					for (var dy = -1; dy <= 1; dy++)
					{
						for (var dx = -1; dx <= 1; dx++)
						{
							var neighborPoint = new Point(point.X + dx, point.Y + dy);
							if (dx == 0 && dy == 0 || !InBounds(map, neighborPoint)) continue;
							if (map[neighborPoint.X, neighborPoint.Y] == 'F' && isMore(neighborPoint, farPoint))
								farPoint = neighborPoint;
						}
					}
			}

			return farPoint;
		}

		private static bool InBounds(char[,] map, Point point) =>
			point.X >= 0 && point.X < map.GetLength(0) && point.Y >= 0 && point.Y < map.GetLength(1);

		private static IEnumerable<Point> BreadthSearch(char[,] map, int energy, Dictionary<char, int> price)
		{
			var firstEnergyPoint = new EnergyPoint(new Point(0, 0), energy);
			var visited = new HashSet<Point>();
			var queue = new Queue<EnergyPoint>();
			queue.Enqueue(firstEnergyPoint);
			while (queue.Count != 0)
			{
				var energyPoint = queue.Dequeue();
				if (!InBounds(map, energyPoint.Point) || map[energyPoint.Point.X, energyPoint.Point.Y] == '0'
				                                          || visited.Contains(energyPoint.Point))
					continue;
				visited.Add(energyPoint.Point);
				yield return energyPoint.Point;
				EnqueueNextPoints(map, queue, energyPoint, price);
			}
		}

		private static void EnqueueNextPoints(char[,] map, Queue<EnergyPoint> queue, EnergyPoint energyEnergyPoint, Dictionary<char, int> price)
		{
			for (var dy = -1; dy <= 1; dy++)
				for (var dx = -1; dx <= 1; dx++)
				{
					if (dx != 0 && dy != 0 || dx == 0 && dy == 0) continue;
					var nextEnergyPoint = new EnergyPoint(new Point(energyEnergyPoint.Point.X + dx, energyEnergyPoint.Point.Y + dy));
					if (!InBounds(map, nextEnergyPoint.Point) || map[nextEnergyPoint.Point.X, nextEnergyPoint.Point.Y] == '0')
						continue;
					nextEnergyPoint.Energy = energyEnergyPoint.Energy - price[map[nextEnergyPoint.Point.X, nextEnergyPoint.Point.Y]];
					if (nextEnergyPoint.Energy < 0) continue;
					queue.Enqueue(nextEnergyPoint);
				}
		}
    }
}

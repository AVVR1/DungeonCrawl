using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
	internal class Monster
	{
		public string name;
		public Vector2 position;
		public int hitpoints;
		public int damage;
		public char symbol;
		public ConsoleColor color;

		static void CreateMonster(string name, int hitpoints, int damage, char symbol, ConsoleColor color, Vector2 position)
		{
			name = name;
			hitpoints = hitpoints;
			damage = damage;
			symbol = symbol;
			color = color;
			position = position;
		}
		static Monster CreateRandomMonster(Random random, Vector2 position)
		{
			int type = random.Next(4);
			return type switch
			{
				0 => CreateMonster("Goblin", 5, 2, 'g', ConsoleColor.Green, position),
				1 => CreateMonster("Bat Man", 2, 1, 'M', ConsoleColor.Magenta, position),
				2 => CreateMonster("Orc", 4, 3, 'o', ConsoleColor.Red, position),
				3 => CreateMonster("Bunny", 1, 0, 'B', ConsoleColor.Yellow, position)
			};
		}
	}
}

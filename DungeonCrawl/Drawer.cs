using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DungeonCrawl.Map;

namespace DungeonCrawl
{
	internal class Drawer
	{
		public static void DrawBrickBg()
		{
			// Draw tiles
			Console.BackgroundColor = ConsoleColor.DarkGray;
			for (int y = 0; y < Console.WindowHeight; y++)
			{
				Console.SetCursorPosition(0, y);
				for (int x = 0; x < Console.WindowWidth; x++)
				{
					if ((x + y) % 3 == 0)
					{
						Printer.Print("|", ConsoleColor.Black);
					}
					else
					{
						Printer.Print(" ", ConsoleColor.DarkGray);
					}
				}
			}
		}
		public static void DrawRectangle(int x, int y, int width, int height, ConsoleColor color)
		{
			Console.BackgroundColor = color;
			for (int dy = y; dy < y + height; dy++)
			{
				Console.SetCursorPosition(x, dy);
				for (int dx = x; dx < x + width; dx++)
				{
					Printer.Print(" ");
				}
			}
		}
		public static void DrawRectangleBorders(int x, int y, int width, int height, ConsoleColor color, string symbol)
		{
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = color;
			for (int dx = x; dx < x + width; dx++)
			{
				Printer.Print(symbol);
			}

			for (int dy = y; dy < y + height; dy++)
			{
				Console.SetCursorPosition(x, dy);

				Printer.Print(symbol);
				Console.SetCursorPosition(x + width - 1, dy);
				Printer.Print(symbol);
			}
		}
		public void DrawCharacterCreationMenu()
		{

		}
		public static void DrawEndScreen(Random random)
		{
			// Run death animation: blood flowing down the screen in columns
			// Wait until keypress
			byte[] speeds = new byte[Console.WindowWidth];
			byte[] ends = new byte[Console.WindowWidth];
			for (int i = 0; i < speeds.Length; i++)
			{
				speeds[i] = (byte)random.Next(1, 4);
				ends[i] = 0;
			}
			Console.BackgroundColor = ConsoleColor.DarkRed;
			Console.ForegroundColor = ConsoleColor.White;


			for (int row = 0; row < Console.WindowHeight - 2; row++)
			{
				Console.SetCursorPosition(0, row);
				for (int i = 0; i < Console.WindowWidth; i++)
				{
					Console.Write(" ");
				}
				Thread.Sleep(100);
			}
		}
		public static void DrawTile(byte x, byte y, Tile tile)
		{
			Console.SetCursorPosition(x, y);
			switch (tile)
			{
				case Tile.Floor:
				Printer.Print(".", ConsoleColor.Gray); break;

				case Tile.Wall:
				Printer.Print("#", ConsoleColor.DarkGray); break;

				case Tile.Door:
				Printer.Print("+", ConsoleColor.Yellow); break;
				case Tile.Stairs:
				Printer.Print(">", ConsoleColor.Yellow); break;

				default: break;
			}
		}
		public static void DrawMapAll(Map level)
		{
			for (byte y = 0; y < level.height; y++)
			{
				for (byte x = 0; x < level.width; x++)
				{
					int ti = y * level.width + x;
					Tile tile = (Tile)level.Tiles[ti];
					DrawTile(x, y, tile);
				}
			}
		}
		public static void DrawMap(Map level, List<int> dirtyTiles)
		{
			if (dirtyTiles.Count == 0)
			{
				DrawMapAll(level);
			}
			else
			{
				foreach (int dt in dirtyTiles)
				{
					byte x = (byte)(dt % level.width);
					byte y = (byte)(dt / level.width);
					Tile tile = (Tile)level.Tiles[dt];
					DrawTile(x, y, tile);
				}
			}
		}
		public static void DrawEnemies(List<Monster> enemies)
		{
			foreach (Monster m in enemies)
			{
				Console.SetCursorPosition((int)m.position.X, (int)m.position.Y);
				Printer.Print(m.symbol, m.color);
			}
		}
		public static void DrawItems(List<Item> items)
		{
			foreach (Item m in items)
			{
				Console.SetCursorPosition((int)m.position.X, (int)m.position.Y);
				char symbol = '$';
				ConsoleColor color = ConsoleColor.Yellow;
				switch (m.type)
				{
					case ItemType.Armor:
					symbol = '[';
					color = ConsoleColor.White;
					break;
					case ItemType.Weapon:
					symbol = '/';
					color = ConsoleColor.Cyan;
					break;
					case ItemType.Treasure:
					symbol = '$';
					color = ConsoleColor.Yellow;
					break;
					case ItemType.Potion:
					symbol = '&';
					color = ConsoleColor.Red;
					break;
				}
				Printer.Print(symbol, color);
			}
		}
		public static void DrawPlayer(PlayerCharacter character)
		{
			Console.SetCursorPosition((int)character.position.X, (int)character.position.Y);
			Printer.Print("@", ConsoleColor.White);
		}
		public static void DrawCommands()
		{
			int cx = Console.WindowWidth - Program.COMMANDS_WIDTH + 1;
			int ln = 1;
			Console.SetCursorPosition(cx, ln); ln++;
			Printer.Print(":Commands:", ConsoleColor.Yellow);
			Console.SetCursorPosition(cx, ln); ln++;
			Printer.Print("I", ConsoleColor.Cyan); Printer.Print("nventory", ConsoleColor.White);
		}
		public static void DrawInfo(PlayerCharacter player, List<Monster> enemies, List<Item> items, List<string> messages)
		{
			int infoLine1 = Console.WindowHeight - Program.INFO_HEIGHT;
			Console.SetCursorPosition(0, infoLine1);
			Printer.Print($"{player.name}: hp ({player.hitpoints}/{player.maxHitpoints}) gold ({player.gold}) ", ConsoleColor.White);
			int damage = 1;
			if (player.weapon != null)
			{
				damage = player.weapon.quality;
			}
			Printer.Print($"Weapon damage: {damage} ");
			int armor = 0;
			if (player.armor != null)
			{
				armor = player.armor.quality;
			}
			Printer.Print($"Armor: {armor} ");



			// Printer.Print last INFO_HEIGHT -1 messages
			DrawRectangle(0, infoLine1 + 1, Console.WindowWidth, Program.INFO_HEIGHT - 2, ConsoleColor.Black);
			Console.SetCursorPosition(0, infoLine1 + 1);
			int firstMessage = 0;
			if (messages.Count > (Program.INFO_HEIGHT - 1))
			{
				firstMessage = messages.Count - (Program.INFO_HEIGHT - 1);
			}
			for (int i = firstMessage; i < messages.Count; i++)
			{
				Printer.Print(messages[i], ConsoleColor.Yellow);
			}
		}
		public static PlayerTurnResult DrawInventory(PlayerCharacter character, List<string> messages)
		{
			Console.SetCursorPosition(1, 1);
			Printer.PrintLine("Inventory. Select item by inputting the number next to it. Invalid input closes inventory");
			ItemType currentType = ItemType.Weapon;
			Printer.PrintLine("Weapons", ConsoleColor.DarkCyan);
			for (int i = 0; i < character.inventory.Count; i++)
			{
				Item it = character.inventory[i];
				if (currentType == ItemType.Weapon && it.type == ItemType.Armor)
				{
					currentType = ItemType.Armor;
					Printer.PrintLine("Armors", ConsoleColor.DarkRed);
				}
				else if (currentType == ItemType.Armor && it.type == ItemType.Potion)
				{
					currentType = ItemType.Potion;
					Printer.PrintLine("Potions", ConsoleColor.DarkMagenta);
				}
				Printer.Print($"{i} ", ConsoleColor.Cyan);
				Printer.PrintLine($"{it.name} ({it.quality})", ConsoleColor.White);
			}
			while (true)
			{
				Printer.Print("Choose item: ", ConsoleColor.Yellow);
				string choiceStr = Console.ReadLine();
				int selectionindex = 0;
				if (int.TryParse(choiceStr, out selectionindex))
				{
					if (selectionindex >= 0 && selectionindex < character.inventory.Count)
					{
						character.UseItem(character.inventory[selectionindex], messages);
						break;
					}
					else
					{
						messages.Add("Out of range");
						break;
					}
				}
				else
				{
					messages.Add("No such item");
					break;
				}
			};
			return PlayerTurnResult.BackToGame;
		}
		public static PlayerTurnResult DrawShop(Shop shop, List<string> messages)
		{
			if (shop == null)
			{
				return PlayerTurnResult.BackToGame;
			}
			Console.SetCursorPosition(1, 1);
			Printer.PrintLine("Shop. Select the item you want to buy by inputting the number next to it.");
			for (int i = 0; i < shop.items.Count; i++)
			{
				Item currentItem = shop.items[i];
				if (currentItem.type == ItemType.Weapon)
				{
					Printer.PrintLine(currentItem.name, ConsoleColor.Cyan);
				}
				if (currentItem.type == ItemType.Armor)
				{
					Printer.PrintLine(currentItem.name, ConsoleColor.White);
				}
				if (currentItem.type == ItemType.Potion)
				{
					Printer.PrintLine(currentItem.name, ConsoleColor.Red);
				}
			}
			while (true)
			{
				Printer.Print("Choose item: ", ConsoleColor.Yellow);
				string choiceStr = Console.ReadLine();
				int selectionindex = 0;
				if (int.TryParse(choiceStr, out selectionindex))
				{
					if (selectionindex >= 0 && selectionindex < shop.items.Count)
					{
						//select shop item index, and ask for confirmation
						break;
					}
					else
					{
						messages.Add("Out of range");
						break;
					}
				}
				else
				{
					messages.Add("No such item");
					break;
				}
			}
			return PlayerTurnResult.BackToGame;
		}
	}
}

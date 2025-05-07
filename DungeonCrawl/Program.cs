using System.Numerics;
using static DungeonCrawl.Map;

namespace DungeonCrawl
{

	enum GameState
	{
		CharacterCreation,
		GameLoop,
		Inventory,
		Shop,
		DeathScreen,
		Quit
	}

	internal class Program
	{
		public const int INFO_HEIGHT = 6;
		public const int COMMANDS_WIDTH = 12;
		public const int ENEMY_CHANCE = 3;
		public const int ITEM_CHANCE = 4;
		public const int SHOP_COUNT = 2;
		public const int SHOP_ITEMS = 3;

		static void Main(string[] args)
		{
			List<Monster> monsters = null;
			List<Item> items = null;
			List<Room> rooms = null;
			PlayerCharacter player = new PlayerCharacter();
			Map currentLevel = new Map();
			Random random = new Random();

			List<int> dirtyTiles = new List<int>();
			List<string> messages = new List<string>();

			// Main loop
			GameState state = GameState.CharacterCreation;
			while (state != GameState.Quit)
			{
				switch (state)
				{
					case GameState.CharacterCreation:
					// Character creation screen
					player.CreateCharacter();
					Console.CursorVisible = false;
					Console.Clear();

					// Map Creation 
					currentLevel.CreateMap(random);

					// Enemy init
					monsters = currentLevel.CreateEnemies(random);
					// Item init
					items = currentLevel.CreateItems(random);
					// Player init
					currentLevel.PlacePlayerToMap(player);
					currentLevel.PlaceStairsToMap();
					state = GameState.GameLoop;
					break;
					case GameState.GameLoop:
					Drawer.DrawMap(currentLevel, dirtyTiles);
					dirtyTiles.Clear();
					Drawer.DrawItems(items);
					Drawer.DrawEnemies(monsters);

					Drawer.DrawPlayer(player);
					Drawer.DrawCommands();
					Drawer.DrawInfo(player, monsters, items, messages);
					// Draw map
					// Draw information
					// Wait for player command
					// Process player command
					while (true)
					{
						messages.Clear();
						PlayerTurnResult result = player.DoTurn(currentLevel, monsters, items, dirtyTiles, messages);
						Drawer.DrawInfo(player, monsters, items, messages);
						if (result == PlayerTurnResult.TurnOver)
						{
							break;
						}
						else if (result == PlayerTurnResult.OpenInventory)
						{
							Console.Clear();
							state = GameState.Inventory;
							break;
						}
						else if (result == PlayerTurnResult.OpenShop)
						{
							Console.Clear();
							state = GameState.Shop;
							break;
						}
						else if (result == PlayerTurnResult.NextLevel)
						{
							currentLevel.CreateMap(random);
							monsters = currentLevel.CreateEnemies(random);
							items = currentLevel.CreateItems(random);
							currentLevel.PlacePlayerToMap(player);
							currentLevel.PlaceStairsToMap();
							Console.Clear();
							Drawer.DrawMapAll(currentLevel);
							break;
						}
					}
					// Either do computer turn or wait command again
					// Do computer turn
					// Process enemies
					ProcessEnemies(monsters, currentLevel, player, dirtyTiles, messages, random);

					Drawer.DrawInfo(player, monsters, items, messages);

					// Is player dead?
					if (player.hitpoints < 0)
					{
						state = GameState.DeathScreen;
					}

					break;
					case GameState.Inventory:
					// Draw inventory 
					PlayerTurnResult inventoryResult = Drawer.DrawInventory(player, messages);
					if (inventoryResult == PlayerTurnResult.BackToGame)
					{
						state = GameState.GameLoop;
						Drawer.DrawMapAll(currentLevel);
						Drawer.DrawInfo(player, monsters, items, messages);
					}
					// Read player command
					// Change back to game loop
					break;
					case GameState.Shop:
					PlayerTurnResult shopResult = Drawer.DrawShop(Shop.currentShop, player, messages);
					if (shopResult == PlayerTurnResult.BackToGame)
					{
						state = GameState.GameLoop;
                        Drawer.DrawMapAll(currentLevel);
                        Drawer.DrawInfo(player, monsters, items, messages);
                    }
					break;
					case GameState.DeathScreen:
					Drawer.DrawEndScreen(random);
					// Animation is over
					Console.SetCursorPosition(Console.WindowWidth / 2 - 4, Console.WindowHeight / 2);
					Printer.Print("YOU DIED", ConsoleColor.Yellow);
					Console.SetCursorPosition(Console.WindowWidth / 2 - 4, Console.WindowHeight / 2 + 1);
					Printer.Print("Play again (y/n)", ConsoleColor.Gray);
					while (true)
					{
						ConsoleKeyInfo answer = Console.ReadKey();
						if (answer.Key == ConsoleKey.Y)
						{
							dirtyTiles.Clear();
							state = GameState.CharacterCreation;
							break;
						}
						else if (answer.Key == ConsoleKey.N)
						{
							state = GameState.Quit;
							break;
						}
					}
					break;
				};
			}
			Console.ResetColor();
			Console.Clear();
			Console.CursorVisible = true;
		}

		static int GetDistanceBetween(Vector2 A, Vector2 B)
		{
			return (int)Vector2.Distance(A, B);
		}
		static void ProcessEnemies(List<Monster> enemies, Map level, PlayerCharacter character, List<int> dirtyTiles, List<string> messages, Random random)
		{
			foreach (Monster enemy in enemies)
			{
				int startTile = level.PositionToTileIndex(enemy.position);
				if (GetDistanceBetween(enemy.position, character.position) < 5)
				{
					Vector2 enemyMove = new Vector2(0, 0);

					if (character.position.X < enemy.position.X)
					{
						enemyMove.X = -1;
					}
					else if (character.position.X > enemy.position.X)
					{
						enemyMove.X = 1;
					}
					else if (character.position.Y > enemy.position.Y)
					{
						enemyMove.Y = 1;
					}
					else if (character.position.Y < enemy.position.Y)
					{
						enemyMove.Y = -1;
					}

					Vector2 destinationPlace = enemy.position + enemyMove;
					if (destinationPlace == character.position)
					{
						// TODO: Random change for armor to protect?
						enemy.damage -= character.GetCharacterDefense();
						if (enemy.damage < 0)
						{
							enemy.damage = 0;
						}
						character.hitpoints -= enemy.damage;
						messages.Add($"{enemy.name} hits you for {enemy.damage} damage! ");
					}
					else
					{
						Tile destination = level.GetTileAtMap(destinationPlace);
						if (destination == Tile.Floor)
						{
							enemy.position = destinationPlace;
							dirtyTiles.Add(startTile);
						}
						else if (destination == Tile.Door)
						{
							enemy.position = destinationPlace;
							dirtyTiles.Add(startTile);
						}
						else if (destination == Tile.Wall)
						{
							// NOP
						}
					}
				}
				else
				{
					int randomDirX = random.Next(2);
					int randomDirY = randomDirX ^ 1;
					Vector2 randomDestination = new Vector2(random.Next(-1, 2) * randomDirX, random.Next(-1, 2) * randomDirY);
					Vector2 enemyDestination = enemy.position + randomDestination;
					if (level.GetTileAtMap(enemyDestination) != Tile.Wall)
					{
						enemy.position = enemyDestination;
						dirtyTiles.Add(startTile);
					}
				}
			}
		}
	}
}
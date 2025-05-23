﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
	internal class Map
	{
        //public const int ROOM_AMOUNT = 12;
        public const int ROOM_MIN_W = 4;
        public const int ROOM_MAX_W = 12;
        public const int ROOM_MIN_H = 4;
        public const int ROOM_MAX_H = 8;
        public enum Tile : sbyte
	    {
		    Floor,
		    Wall,
		    Door,
		    Monster,
		    Item,
		    Player,
		    Stairs
	    }

		public int width;
		public int height;
		public int currentFloor;
		public Tile[] Tiles;
		public List<Room> rooms;

		public Map()
        {
			width = Console.WindowWidth - Program.COMMANDS_WIDTH;
			height = Console.WindowHeight - Program.INFO_HEIGHT;
		}

		public void CreateMap(Random random)
		{
			rooms = new List<Room>();
			Tiles = new Tile[width * height];

			// Create perimeter wall
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int ti = y * width + x;
					if (y == 0 || x == 0 || y == height - 1 || x == width - 1)
					{
						Tiles[ti] = Tile.Wall;
					}
					else
					{
						Tiles[ti] = Tile.Floor;
					}
				}
			}

			int roomRows = 3;
			int roomsPerRow = 6;
			int boxWidth = (Console.WindowWidth - Program.COMMANDS_WIDTH - 2) / roomsPerRow;
			int boxHeight = (Console.WindowHeight - Program.INFO_HEIGHT - 2) / roomRows;

			//temporary variables
			bool[] shops = new bool[roomRows * roomsPerRow];
			int a = 0;
			//generate shop indexes
			while (a < Program.SHOP_COUNT)
			{
				int shopIndex = random.Next(0, roomRows * roomsPerRow);
				if (shops[shopIndex] == false)
				{
					shops[shopIndex] = true;
					a++;
				}
			}
			//add rooms
			for (int roomRow = 0; roomRow < roomRows; roomRow++)
			{
				for (int roomColumn = 0; roomColumn < roomsPerRow; roomColumn++)
				{
					AddRoom(roomColumn * boxWidth + 1, roomRow * boxHeight + 1, boxWidth, boxHeight, random);
				}
			}
			//create shops
			for (int i = 0; i < shops.Count(); i++)
			{
				Room currentRoom = rooms[i];
				if (shops[i])
				{
					Shop newShop = new Shop(currentRoom.position, currentRoom.height, currentRoom.width);
					newShop.CreateShop(random);
					rooms[i] = newShop;
                }
			}

			// Add enemies and items
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int ti = y * width + x;
					if (Tiles[ti] == Tile.Floor)
					{
						int chance = random.Next(100);
						if (chance < Program.ENEMY_CHANCE)
						{
							Tiles[ti] = Tile.Monster;
							continue;
						}

						chance = random.Next(100);
						if (chance < Program.ITEM_CHANCE)
						{
							Tiles[ti] = Tile.Item;
						}
					}
				}
			}

			// Find starting place for player
			for (int i = 0; i < Tiles.Length; i++)
			{
				if (Tiles[i] == Tile.Floor)
				{
					Tiles[i] = Tile.Player;
					break;
				}
			}
		}

		public void AddRoom(int boxX, int boxY, int boxWidth, int boxHeight, Random random)
        {
            int width = random.Next(ROOM_MIN_W, boxWidth);
            int height = random.Next(ROOM_MIN_H, boxHeight);
            int sx = boxX + random.Next(0, boxWidth - width);
            int sy = boxY + random.Next(0, boxHeight - height);
            int doorX = random.Next(1, width - 1);
            int doorY = random.Next(1, height - 1);

            // Create perimeter wall
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int ti = (sy + y) * this.width + (sx + x);
                    if (y == 0 || x == 0 || y == height - 1 || x == width - 1)
                    {

                        if (y == doorY || x == doorX)
                        {
                            Tiles[ti] = Tile.Door;
                        }
                        else
                        {
                            Tiles[ti] = Tile.Wall;
                        }
                    }
                }
            }
            rooms.Add(new Room(new Vector2(sx, sy), height, width));
        }
        public void PlacePlayerToMap(PlayerCharacter character)
        {
            for (int i = 0; i < Tiles.Length; i++)
            {
                if (Tiles[i] == Map.Tile.Player)
                {
                    Tiles[i] = Map.Tile.Floor;
                    int px = i % width;
                    int py = i / width;

                    character.position = new Vector2(px, py);
                    break;
                }
            }
        }
        public void PlaceStairsToMap()
        {
            for (int i = Tiles.Length - 1; i >= 0; i--)
            {
                if (Tiles[i] == Map.Tile.Floor)
                {
                    Tiles[i] = Map.Tile.Stairs;
                    break;
                }
            }
        }
        public int PositionToTileIndex(Vector2 position)
        {
            return (int)position.X + (int)position.Y * width;
        }
        public Tile GetTileAtMap(Vector2 position)
        {
            if (position.X >= 0 && position.X < width)
            {
                if (position.Y >= 0 && position.Y < height)
                {
                    int ti = (int)position.Y * width + (int)position.X;
                    return (Tile)Tiles[ti];
                }
            }
            return Tile.Wall;
        }
		public List<Monster> CreateEnemies(Random random)
		{
			List<Monster> monsters = new List<Monster>();

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int ti = y * width + x;
					if (Tiles[ti] == Tile.Monster)
					{
						Monster m = Monster.CreateRandomMonster(random, new Vector2(x, y));
						monsters.Add(m);
						Tiles[ti] = (sbyte)Tile.Floor;
					}
				}
			}
			return monsters;
		}
		public List<Item> CreateItems(Random random)
		{
			List<Item> items = new List<Item>();

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int ti = y * width + x;
					if (Tiles[ti] == Tile.Item)
					{
						Item m = Item.CreateRandomItem(random);
						m.position = new Vector2(x, y);
						items.Add(m);
						Tiles[ti] = (sbyte)Tile.Floor;
					}
				}
			}
			return items;
		}
	}
}

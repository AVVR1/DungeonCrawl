using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
	enum PlayerTurnResult
	{
		TurnOver,
		NewTurn,
		OpenInventory,
		OpenShop,
		NextLevel,
		BackToGame
	}

	internal class PlayerCharacter
	{
		public string name;
		public int hitpoints;
		public int maxHitpoints;
		public Item weapon;
		public Item armor;
		public int gold;
		public Vector2 position;
		public List<Item> inventory;


		public void GiveItem(Item item)
		{
			// Inventory order
			// Weapons
			// Armors
			// Potions
			switch (item.type)
			{
				case ItemType.Weapon:
				if ((weapon != null && weapon.quality < item.quality)
					|| weapon == null)
				{
					weapon = item;
				}
				inventory.Insert(0, item);
				break;
				case ItemType.Armor:
				if ((armor != null && armor.quality < item.quality)
					|| armor == null)
				{
					armor = item;
				}
				int armorIndex = 0;
				while (armorIndex < inventory.Count && inventory[armorIndex].type == ItemType.Weapon)
				{
					armorIndex++;
				}
				inventory.Insert(armorIndex, item);
				break;
				case ItemType.Potion:
				inventory.Add(item);
				break;
				case ItemType.Treasure:
				gold += item.quality;
				break;
			}
		}

		public int GetCharacterDamage()
		{
			if (weapon != null)
			{
				return weapon.quality;
			}
			else
			{
				return 1;
			}
		}

		public int GetCharacterDefense()
		{
			if (armor != null)
			{
				return armor.quality;
			}
			else
			{
				return 0;
			}
		}

		public void UseItem(Item item, List<string> messages)
		{
			switch (item.type)
			{
				case ItemType.Weapon:
				weapon = item;
				messages.Add($"You are now wielding a {item.name}");
				break;
				case ItemType.Armor:
				armor = item;
				messages.Add($"You equip {item.name} on yourself.");
				break;
				case ItemType.Potion:
				hitpoints += item.quality;
				if (hitpoints > maxHitpoints)
				{
					maxHitpoints = hitpoints;
				}
				messages.Add($"You drink a potion and gain {item.quality} hitpoints");
				inventory.Remove(item);
				break;
			}
		}
	}
}

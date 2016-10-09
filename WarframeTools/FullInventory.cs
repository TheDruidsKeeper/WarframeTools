using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarframeTools {
	class FullInventory {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		// Warframes
		public List<InventoryItem> Warframes { get; set; }
		public List<InventoryItem> PrimaryWeapons { get; set; }
		public List<InventoryItem> SecondaryWeapons { get; set; }
		public List<InventoryItem> MeleeWeapons { get; set; }
		// Archwings
		public List<InventoryItem> Archwings { get; set; }
		public List<InventoryItem> ArchwingGuns { get; set; }
		public List<InventoryItem> ArchwingMelees { get; set; }
		// Companions
		public List<InventoryItem> Sentinels { get; set; }
		public List<InventoryItem> SentinelGuns { get; set; }
		public List<InventoryItem> Kubrows { get; set; }
		public List<InventoryItem> Kavats { get; set; }

		public FullInventory() {
			// Warframe
			this.Warframes = new List<InventoryItem>();
			this.PrimaryWeapons = new List<InventoryItem>();
			this.SecondaryWeapons = new List<InventoryItem>();
			this.MeleeWeapons = new List<InventoryItem>();
			// Archwing
			this.Archwings = new List<InventoryItem>();
			this.ArchwingGuns = new List<InventoryItem>();
			this.ArchwingMelees = new List<InventoryItem>();
			// Companion
			this.Sentinels = new List<InventoryItem>();
			this.SentinelGuns = new List<InventoryItem>();
			this.Kubrows = new List<InventoryItem>();
			this.Kavats = new List<InventoryItem>();
		}

		public void LoadWarframes(dynamic inventory) {
			var warframes = inventory.ExportWarframes;
			foreach (var item in warframes) {
				var inventoryItem = new InventoryItem() {
					Key = item.uniqueName,
					Name = item.name
				};
				if (inventoryItem.Key.StartsWith("/Lotus/Powersuits/Archwing")) {
					this.Archwings.Add(ScrubArchwing(inventoryItem));
				} else
					this.Warframes.Add(inventoryItem);
			}
			logger.Debug($"Loaded warframes/archwings");
		}

		public void LoadWeapons(dynamic inventory) {
			var weapons = inventory.ExportWeapons;
			foreach (var item in weapons) {
				var inventoryItem = new InventoryItem() {
					Key = item.uniqueName,
					Name = item.name
				};
				if (item.sentinel == true)
					this.SentinelGuns.Add(inventoryItem);
				else if (inventoryItem.Key.StartsWith("/Lotus/Weapons/Tenno/Archwing/Primary"))
					this.ArchwingGuns.Add(ScrubArchwing(inventoryItem));
				else if (inventoryItem.Key.StartsWith("/Lotus/Weapons/Tenno/Archwing/Melee"))
					this.ArchwingMelees.Add(ScrubArchwing(inventoryItem));
				else {
					switch ((int)item.slot) {
						case 0:
							this.SecondaryWeapons.Add(inventoryItem);
							break;
						case 1:
							this.PrimaryWeapons.Add(inventoryItem);
							break;
						case 5:
							this.MeleeWeapons.Add(inventoryItem);
							break;
						default:
							logger.Warn($"Unidentified weapon slot: {item.slot}");
							break;
					}
				}
			}
			logger.Debug($"Loaded weapons");
		}

		private InventoryItem ScrubArchwing(InventoryItem inventoryItem) {
			inventoryItem.Name = inventoryItem.Name.Replace("<ARCHWING> ", "");
			return inventoryItem;
		}

		public void LoadPets(dynamic inventory) {
			var pets = inventory.ExportSentinels;
			foreach (var item in pets) {
				var inventoryItem = new InventoryItem() {
					Key = item.uniqueName,
					Name = item.name
				};
				if (inventoryItem.Key.StartsWith("/Lotus/Types/Sentinels"))
					this.Sentinels.Add(inventoryItem);
				else if (inventoryItem.Key.StartsWith("/Lotus/Types/Game/KubrowPet"))
					this.Kubrows.Add(inventoryItem);
				else if (inventoryItem.Key.StartsWith("/Lotus/Types/Game/CatbrowPet"))
					this.Kavats.Add(inventoryItem);
				else
					logger.Warn($"Unidentified pet: {inventoryItem.Name}");
			}
			logger.Debug($"Loaded warframes/archwings");
		}
	}
	class InventoryItem {
		public string Key { get; set; }
		public string Name { get; set; }
		public override string ToString() {
			return this.Name;
		}
	}
}

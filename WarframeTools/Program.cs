using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WarframeTools {
	class Program {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		static void Main(string[] args) {
			logger.Info("Loading data...");
			try {
				var playerInventory = GetPlayerIventory();
				var fullInventory = GetFullIventory();
				var regex = new Regex("(\\B[A-Z])");
				var primes = new List<string>();
				using (var sw = new StringWriter()) {
					foreach (var property in fullInventory.GetType().GetProperties()) {
						var playerItemsProperty = playerInventory.GetType().GetProperty(property.Name);
						var playerItems = (List<string>)playerItemsProperty.GetValue(playerInventory);
						var name = regex.Replace(property.Name, " $1");
						var missingItems = (List<InventoryItem>)property.GetValue(fullInventory);
						missingItems.RemoveAll(item => playerItems.Contains(item.Key));
						missingItems.Sort(new InventoryItemComparer());
						var missingItemNames = missingItems.Select(item => item.Name).ToArray();
						sw.WriteLine($"\"{name} ({missingItemNames.Length})\"\t\"{string.Join("\"\t\"", missingItemNames)}\"");
					}
					logger.Info($"Missing Items:\n{sw.ToString()}");
				}
			} catch (Exception ex) {
				logger.Error(ex);
			}
			logger.Info("Done");
			Console.ReadKey();
		}

		private static PlayerInventory GetPlayerIventory() {
			dynamic inventory = JObject.Parse(File.ReadAllText(ConfigurationManager.AppSettings["Inventory"]));
			var playerInventory = new PlayerInventory(inventory);
			return playerInventory;
		}

		private static FullInventory GetFullIventory() {
			var dataFiles = Exporter.GetFiles();
			var fullInventory = new FullInventory();
			Load(dataFiles, "ExportWeapons", fullInventory.LoadWeapons);
			Load(dataFiles, "ExportWarframes", fullInventory.LoadWarframes);
			Load(dataFiles, "ExportSentinels", fullInventory.LoadPets);
			return fullInventory;
		}

		private static void Load(List<string> dataFiles, string dataFileName, Action<dynamic> loader) {
			var filePath = dataFiles.First(path => {
				return Path.GetFileNameWithoutExtension(path) == dataFileName;
			});
			var fileData = File.ReadAllText(filePath);
			dynamic parsedData = JObject.Parse(fileData);
			loader(parsedData);
		}
	}
}

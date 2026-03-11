using SaintCoinach;
using Newtonsoft.Json;
using SaintCoinach.Libra;

const string GameDirectory = @"C:\SteamLibrary\steamapps\common\FINAL FANTASY XIV Online";
ARealmReversed realm = new ARealmReversed(GameDirectory, SaintCoinach.Ex.Language.English);

List<DataModel> data = new List<DataModel>();

var statusSheet = realm.GameData.GetSheet<SaintCoinach.Xiv.Status>();

foreach (var status in statusSheet) {

	string gamepath = status?.Icon?.ToString() ?? "";
	string iconID = gamepath != "" ? gamepath.Substring(15, 6) : "";

	if (realm.Packs.TryGetFile(gamepath, out var file)) {
		if (file is SaintCoinach.Imaging.ImageFile imgFile) {
			var image = imgFile.GetImage();

			var target = new FileInfo(Path.Combine("output", "statusicons", iconID));
			if (!target.Directory.Exists) {
				target.Directory.Create();
			}
			var pngPath = target.FullName.Substring(0, target.FullName.Length - target.Extension.Length) + ".png";
			image.Save(pngPath);
		}
	}

	var statusData = new DataModel() {
		ID = status?.Key ?? 0,
		Name = status?.Name ?? "",
		Icon = "/images/statusicons/" + iconID + ".png",
		GamePath = gamepath
	};
	data.Add(statusData);
	Console.WriteLine("Adding Status: {0}, Saving Icon: {1}", statusData.Name, statusData.Icon);
}

string json = JsonConvert.SerializeObject(data.ToArray(), Formatting.Indented);
System.IO.File.WriteAllText("./output/status.json", json);

class DataModel {
	public int ID { get; set; }
	public string Name { get; set; }
	public string Icon { get; set; }
	public string GamePath { get; set; }
}
using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace TerraforMod
{
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			// start the game with window not shown
			window_shown = false;
			LoadConfigs();

			var harmony = new Harmony("com." + PluginInfo.PLUGIN_GUID);
			harmony.PatchAll();

			// Plugin startup logic
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			return;
		}

		// VARIABLES
		// configs
		internal static ConfigEntry<bool> config_no_send_analytics;
		internal static ConfigEntry<KeyboardShortcut> config_toggle_window_key;

		private void LoadConfigs()
		{
			config_no_send_analytics = Config.Bind("GENERAL",
				"no_send_analytics",
				true,
				"do not sent game status analytics to dev's server (http://5.196.88.188:4000)");
			config_toggle_window_key = Config.Bind("GENERAL",
				"toggle_window_key",
				new KeyboardShortcut(KeyCode.F2),
				"key to toggle plugin window panel");
			return;
		}

		// window basics
		const int window_id = 1995786727;
		private bool _window_shown = false;
		Rect window_rect = new Rect(200, 100, 1200, 800);
		private const int button_nrow = 40; // max button stacks in a column
		private GUIStyle label_text_style = new GUIStyle
		{
			normal = new GUIStyleState { textColor = Color.yellow },
			alignment = TextAnchor.MiddleCenter,
		};

		private bool window_shown
		{
			get => _window_shown;
			set
			{
				// check if in game, only show window if so; clicking buttons
				// outside of an active game results in null referecen error
				// though not necessarily crashing the game
				var v = value && (Game.State.GameId != null);

				if (_window_shown == v) return;
				_window_shown = v;
				if (v)
				{
					// prepare data to draw buttons
					FillProjectListsByCategory();
				}
			}
		}

		public void Update()
		{
			if (config_toggle_window_key.Value.IsDown())
				window_shown = !window_shown;
		}

		// the resources in our desired order to show as buttons
		private static List<Terraf.ResourceType> resources_list
			= new List<Terraf.ResourceType> {
			Terraf.ResourceType.Food,
			Terraf.ResourceType.Power,
			Terraf.ResourceType.Science,
			Terraf.ResourceType.Water,
			Terraf.ResourceType.Nitrates,
			Terraf.ResourceType.Silicates,
			Terraf.ResourceType.RareMetals,
			Terraf.ResourceType.Uranium,
		};

		// the projects to add as buttons, in categories
		private static List<int> building_projects = new List<int>();
		private static List<int> regional_projects = new List<int>();
		private static List<int> space_projects = new List<int>();
		private static List<int> technology_projects = new List<int>();
		private static string uncate_projects = ""; // track the projects not categorized into aboves

		private void FillProjectListsByCategory()
		{
			// clean old results
			building_projects.Clear();
			regional_projects.Clear();
			space_projects.Clear();
			technology_projects.Clear();

			// avoid running this over and over again
			Logger.LogInfo("FillProjectListsByCategory");
			foreach (var p in Terraf.ProjectsService.AllProjects)
			{
				// filter invalid project ids
				// 0 is hard-coded in game, not sure about negative values
				if (p.Id <= 0)
					continue;
				if (p.ProjectType == Terraf.ProjectType.Building)
					building_projects.Add(p.Id);
				else if (p.ProjectType == Terraf.ProjectType.Regional)
					regional_projects.Add(p.Id);
				else if (p.ProjectType == Terraf.ProjectType.Space)
					space_projects.Add(p.Id);
				else if (p.ProjectType == Terraf.ProjectType.Technology)
					technology_projects.Add(p.Id);
				else
					uncate_projects += p.NameLocalised + ", ";
			}
			return;
		}

		// main plugin window
		public void WindowHandler(int id)
		{
			GUILayout.BeginHorizontal();
			{
				// add resources buttons
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Add resources", label_text_style);
					GUILayout.Space(5);
					GUILayout.BeginHorizontal();
					{
						var i = 0;
						while (i < resources_list.Count)
						{
							var j = i % button_nrow;
							if (j == 0)
								GUILayout.BeginVertical(GUILayout.Width(120));
							if (GUILayout.Button("+100 " + Terraf.ResourceTypeExtensions.ToLocalisedString(resources_list[i])))
								Game.ChangeResourceStock(resources_list[i], 100, false);
							if (j == (button_nrow - 1))
								GUILayout.EndVertical();
							i += 1;
						}
						if (i % button_nrow != 0)
							GUILayout.EndVertical();
					}
					GUILayout.EndHorizontal();

					// add an 'add-all' button
					if (GUILayout.Button("+500 ALL"))
						foreach (var r in resources_list)
							Game.ChangeResourceStock(r, 500, false);
				}
				GUILayout.EndVertical();
				GUILayout.Space(20);

				// add building project buttons
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Add building projects", label_text_style);
					GUILayout.Space(5);
					GUILayout.BeginHorizontal();
					{
						var i = 0;
						while (i < building_projects.Count)
						{
							var j = i % button_nrow;
							if (j == 0)
								GUILayout.BeginVertical(GUILayout.Width(120));
							if (GUILayout.Button(Terraf.ProjectsService.GetProjectById(building_projects[i]).NameLocalised))
								CardsService.DrawCard(Terraf.ProjectsService.GetProjectById(building_projects[i]).Id);
							if (j == (button_nrow - 1))
								GUILayout.EndVertical();
							i += 1;
						}
						if (i % button_nrow != 0)
							GUILayout.EndVertical();
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.Space(20);

				// add regional project buttons
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Add regional projects", label_text_style);
					GUILayout.Space(5);
					GUILayout.BeginHorizontal();
					{
						var i = 0;
						while (i < regional_projects.Count)
						{
							var j = i % button_nrow;
							if (j == 0)
								GUILayout.BeginVertical(GUILayout.Width(120));
							if (GUILayout.Button(Terraf.ProjectsService.GetProjectById(regional_projects[i]).NameLocalised))
								CardsService.DrawCard(Terraf.ProjectsService.GetProjectById(regional_projects[i]).Id);
							if (j == (button_nrow - 1))
								GUILayout.EndVertical();
							i += 1;
						}
						if (i % button_nrow != 0)
							GUILayout.EndVertical();
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.Space(20);

				// add space project buttons
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Add space projects", label_text_style);
					GUILayout.Space(5);
					GUILayout.BeginHorizontal();
					{
						var i = 0;
						while (i < space_projects.Count)
						{
							var j = i % button_nrow;
							if (j == 0)
								GUILayout.BeginVertical(GUILayout.Width(120));
							if (GUILayout.Button(Terraf.ProjectsService.GetProjectById(space_projects[i]).NameLocalised))
								CardsService.DrawCard(Terraf.ProjectsService.GetProjectById(space_projects[i]).Id);
							if (j == (button_nrow - 1))
								GUILayout.EndVertical();
							i += 1;
						}
						if (i % button_nrow != 0)
							GUILayout.EndVertical();
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.Space(20);

				// add technology project buttons
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Add technology projects", label_text_style);
					GUILayout.Space(5);
					GUILayout.BeginHorizontal();
					{
						var i = 0;
						while (i < technology_projects.Count)
						{
							var j = i % button_nrow;
							if (j == 0)
								GUILayout.BeginVertical(GUILayout.Width(120));
							if (GUILayout.Button(Terraf.ProjectsService.GetProjectById(technology_projects[i]).NameLocalised))
								CardsService.DrawCard(Terraf.ProjectsService.GetProjectById(technology_projects[i]).Id);
							if (j == (button_nrow - 1))
								GUILayout.EndVertical();
							i += 1;
						}
						if (i % button_nrow != 0)
							GUILayout.EndVertical();
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();

			}
			GUILayout.EndHorizontal();

			return;
		}

		public void OnGUI()
		{
			if (!window_shown) return;

			window_rect = GUILayout.Window(window_id, window_rect, WindowHandler, PluginInfo.PLUGIN_NAME + " v" + PluginInfo.PLUGIN_VERSION);
			return;
		}
	}


	[HarmonyPatch(typeof(Analytics))]
	[HarmonyPatch(nameof(Analytics.Send))]
	internal class PreventSendingAnalytics
	{
		static bool Prefix()
		{
			// intercept sending analytics to server http://5.196.88.188:4000
			return !Plugin.config_no_send_analytics.Value;
		}
	}
}

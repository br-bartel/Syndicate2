using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


namespace TheSyndicate
{
    class GameEngine
    {
        private string PATH_TO_STORY = @"assets/story.json";
        private string PATH_TO_Achievements = @"assets/achievements.json";
        private Dictionary<string, Scene> Scenes { get; set; }
		public Dictionary<string, Achievement> Achievements;
        private Scene CurrentScene { get; set; }
		public string PreviousSceneId;
        private Player Player { get; set; }

        public GameEngine()
        {
            this.Player = Player.GetInstance();
            LoadScenes();
            LoadCurrentScene();
        }

        public void Start()
		{
            if (Program.isWindows)
            {
                PATH_TO_STORY = @"assets\story.json";
            }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.CursorVisible = true;
            while (CurrentScene.HasNextScenes())
            {
                PlayScene();
            }
            PlayFinalScene();
        
        }

        private void LoadScenes()
        {
            Scenes = GetScenes();			
			Scenes["achievements"].Text = LoadAchievements();
        }

        private Dictionary<string, Scene> GetScenes()
        {
            List<Scene> scenes = ConvertStoryFromJsonToScenes();
            Dictionary<string, Scene> sceneIdsToScene = new Dictionary<string, Scene>();
            
            foreach(Scene scene in scenes)
            {
                sceneIdsToScene[scene.Id] = scene;
            }

            return sceneIdsToScene;
        }

        // https://stackoverflow.com/questions/18192357/deserializing-json-object-array-with-json-net
        private List<Scene> ConvertStoryFromJsonToScenes()
        {
            string story = GetStoryFromFile();
            return JsonConvert.DeserializeObject<List<Scene>>(story);
        }

        private string GetStoryFromFile()
        {
            return File.ReadAllText(PATH_TO_STORY);
        }

        private void LoadCurrentScene()
        {
            CurrentScene = GetStartingScene();
        }

        private Scene GetStartingScene()
        {
            if (this.Player != null && this.Player.CurrentSceneId != null)
            {
                return GetSceneFromPlayer();
            }
            else
            {
                return GetFirstScene();
            }
        }

        private Scene GetSceneFromPlayer()
        {
            Scene startScene = null;
            foreach (KeyValuePair<string, Scene> scene in this.Scenes)
            {
                if (scene.Key.Equals(Player.CurrentSceneId))
                {
                    startScene = scene.Value;
                }
            }
            return startScene;
        }

        private Scene GetFirstScene()
        {
            Scene stateScene = null;
            foreach (KeyValuePair<string, Scene> scene in this.Scenes)
            {
                if (scene.Value.Start == true)
                {
                    stateScene = scene.Value;
                }
            }
            return stateScene;
        }

        private void PlayScene()
        {
            CurrentScene.Play();
            CurrentScene = GetNextScene();
        }

        private Scene GetNextScene()
        {
			if (CurrentScene.Id == "achievements")
			{
				if (CurrentScene.ActualDestinationId == "return")
				{
					return this.Scenes[PreviousSceneId];
				}
				else 
				{
					ResetAchievements();
					return this.Scenes["achievements"];
				}
			}
			else
			{
				if (CurrentScene.ActualDestinationId == "achievements")
				{
					PreviousSceneId = CurrentScene.Id;
				}
            	return this.Scenes[CurrentScene.ActualDestinationId];
			}
        }

        private void PlayFinalScene()
        {
            string firstSceneId = GetFirstScene().Id;
            Player.ResetPlayerData(firstSceneId);
            CurrentScene.Play();
			UpdateAchievements(CurrentScene.Id, true);
        }
		private string LoadAchievements()
		{
			List<Achievement> temp = JsonConvert.DeserializeObject<List<Achievement>>
													(File.ReadAllText(PATH_TO_Achievements));
			Achievements = new Dictionary<string, Achievement>();
            string text = "ACHIEVEMENTS:\n======================================\n\n";
            foreach(Achievement ach in temp)
            {
                Achievements[ach.Id] = ach;
                if (ach.State)
                {
                    text += ach.Completed + " : " + ach.Hint + "\n\n";
                }
                else
                {
                    text += ach.Hint + "\n\n";
                }
            }

			return text;
		}
		private void UpdateAchievements(string key, bool value)
		{
			Achievements[key].State = value;
			File.WriteAllText(PATH_TO_Achievements, JsonConvert.SerializeObject(Achievements.Values));
			Scenes["achievements"].Text = LoadAchievements();
		}

		private void ResetAchievements()
		{
			foreach (Achievement ach in Achievements.Values)
			{
				ach.State = false;
			}
			File.WriteAllText(PATH_TO_Achievements, JsonConvert.SerializeObject(Achievements.Values));
			Scenes["achievements"].Text = LoadAchievements();
		}	
    }
}

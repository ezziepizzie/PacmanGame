using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAlgoT2530.Engine
{
    public class SceneManager
    {
        public Dictionary<string, GameScene> Scenes;

        public GameScene CurrentScene
        {
            get; private set;
        }

        public int Count
        {
            get
            {
                return Scenes.Count;
            }
        }

        public SceneManager()
        {
            Scenes = new Dictionary<string, GameScene>();
        }

        public void ChangeScene(string sceneName)
        {
            if (Scenes.ContainsKey(sceneName))
            {
                CurrentScene = Scenes[sceneName];
            }
            else
            {
                throw new Exception($"[SceneManager]: ERROR: Scene {sceneName} not found");
            }
        }

        public void AddScene(string name, GameScene scene)
        {
            if (!Scenes.ContainsKey(name))
            {
                // By default, the first scene added is the current scene
                if (Scenes.Count == 0)
                {
                    CurrentScene = scene;
                }
                Scenes.Add(name, scene);
            }
            else
            {
                throw new Exception($"[SceneManager]: ERROR: Scene {scene.Name} already exists");
            }
        }

        public string GetSceneName(GameScene scene)
        {
            foreach (var (name, namedScene) in Scenes)
            {
                if (namedScene == scene)
                {
                    return name;
                }
            }
            return string.Empty;
        }

        public void Clear()
        {
            Scenes.Clear();
        }
    }
}

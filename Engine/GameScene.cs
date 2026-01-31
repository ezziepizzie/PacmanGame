using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAlgoT2530.Engine
{
    public abstract class GameScene
    {
        #region Object Attributes
        private string _name;

        public string Name
        {
            get
            {
                if (_name == null || _name == string.Empty)
                {
                    _name = _sceneManager.GetSceneName(this);
                    if (_name != string.Empty)
                    {
                        return _name;
                    }
                    else
                    {
                        throw new Exception($"[GameScene]: ERROR: This scene has not been registered to scene manager.");
                    }
                }
                else
                {
                    return _name;
                }
            }
        }
        #endregion

        #region Static Attributes
        private static SceneManager _sceneManager;
        public static void SetSceneManager(SceneManager sceneManager)
        {
            if (_sceneManager == null)
            {
                _sceneManager = sceneManager;
            }
        }

        protected static GameEngine _game;

        public static void SetGame(GameEngine game)
        {
            if (_game == null)
            {
                _game = game;
            }
        }
        #endregion

        protected GameScene()
        {
            // Intentionally left blank
        }

        protected void ChangeScene(string sceneName)
        {
            _sceneManager.ChangeScene(sceneName);
        }

        #region Virtual Methods

        // NOTE:
        // In most cases, only the CreateScene method will be overridden
        // The rest of the methods are optional and can be ignored.

        // The method names are inspired from Phaser.js Scene class,
        // which pretty much relates to some MonoGame's overridable methods,
        // except that the purpose of these methods are more specific to the scene.

        public virtual void Preload() { }
        public virtual void CreateScene() { }
        public virtual void Update() { }

        #endregion
    }
}

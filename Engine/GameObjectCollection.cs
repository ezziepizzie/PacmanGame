using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GAlgoT2530.Engine
{
    // NOTE:
    // Tags and Layers have not been tested.
    // For layers, GameObjects need to be sorted by layers, which have not been implemented.

    public class GameObjectCollection
    {
        public delegate void ObjectAddedHandler(GameObject obj);
        public delegate void ObjectRemovedHandler(GameObject obj);

        public static event ObjectAddedHandler ObjectAdded;
        public static event ObjectRemovedHandler ObjectRemoved;

        public static Dictionary<string, GameObject> Objects;
        public static Dictionary<string, HashSet<GameObject>> Tags;
        public static SortedDictionary<uint, HashSet<GameObject>> Layers;

        // Pending Addition = objects constructed on update call, pending addition to collection
        // Pending Removal = objects marked for destruction on update call, pending removal from collection
        private static Dictionary<string, GameObject> _pendingAdditionObjects;
        private static Dictionary<string, GameObject> _pendingRemovalObjects;

        // True if Initialize() method has been called.
        // False otherwise.
        private static bool _isInGameLoop;
        private static int _totalObjectsSinceBeginning;

        static GameObjectCollection()
        {
            Objects = new Dictionary<string, GameObject>();
            Tags = new Dictionary<string, HashSet<GameObject>>();
            Layers = new SortedDictionary<uint, HashSet<GameObject>>();

            _pendingAdditionObjects = new Dictionary<string, GameObject>();
            _pendingRemovalObjects = new Dictionary<string, GameObject>();

            _isInGameLoop = false;
            _totalObjectsSinceBeginning = 0;
        }

        public static int Count()
        {
            return _totalObjectsSinceBeginning;
        }

        // TODO: Rewrite this function using AddAsPendingObject() and AddPendingObjectToCollection.
        public static void Add(GameObject gameObject)
        {
            bool isObjectAdded;

            if (_isInGameLoop) // Add object to pending list
            {
                // The object must not exists in both the object collection and the instantiated collection
                isObjectAdded = !Objects.ContainsKey(gameObject.Name)
                             && _pendingAdditionObjects.TryAdd(gameObject.Name, gameObject);

                // Update total objects for unique naming purpose.
                _totalObjectsSinceBeginning++;

                Debug.WriteLine($"[GameObjectCollection]: Object {gameObject.Name} ADDED as pending object.");
            }
            else // Add object to actual collection
            {
                // The object must not exists in the object collection
                isObjectAdded = Objects.TryAdd(gameObject.Name, gameObject);

                // Register object by tag
                string tag = gameObject.Tag.ToLower();
                if (!Tags.ContainsKey(tag))
                {
                    Tags[tag] = new HashSet<GameObject>();
                }
                Tags[tag].Add(gameObject);

                // Register object by layer
                uint layer = gameObject.Layer;
                if (!Layers.ContainsKey(layer))
                {
                    Layers[layer] = new HashSet<GameObject>();
                }
                Layers[layer].Add(gameObject);

                Debug.WriteLine($"[GameObjectCollection]: Object {gameObject.Name} ADDED to collection.");

                // Update total objects for unique naming purpose.
                _totalObjectsSinceBeginning++;

                // Execute object added handlers for objects added to collection
                ObjectAdded?.Invoke(gameObject);
            }
             
            if (!isObjectAdded)
            {
                throw new Exception($"[GameObjectCollection]: ERROR: Attempt to add duplicated object named {gameObject.Name}");
            }
        }

        private static void AddAsPendingObject(GameObject gameObject)
        {
            bool isObjectAdded = false;

            if (_isInGameLoop) // Add object to pending list
            {
                // The object must not exists in both the object collection and the instantiated collection
                isObjectAdded = !Objects.ContainsKey(gameObject.Name)
                             && _pendingAdditionObjects.TryAdd(gameObject.Name, gameObject);

                Debug.WriteLine($"[GameObjectCollection]: Object {gameObject.Name} ADDED as pending object.");
            }
            else
            {
                Debug.WriteLine($"[GameObjectCollection]: ERROR: Attempt to add Object {gameObject.Name} as pending object outside game loop.");
            }

            if (!isObjectAdded)
            {
                throw new Exception($"[GameObjectCollection]: ERROR: Attempt to add duplicated object named {gameObject.Name}");
            }
        }

        private static void AddPendingObjectToCollection(GameObject pendingObject)
        {
            bool isObjectAdded = false;

            if (!_isInGameLoop)
            {
                // The object must not exists in the object collection
                isObjectAdded = Objects.TryAdd(pendingObject.Name, pendingObject);

                // Register object by tag
                string tag = pendingObject.Tag.ToLower();
                if (!Tags.ContainsKey(tag))
                {
                    Tags[tag] = new HashSet<GameObject>();
                }
                Tags[tag].Add(pendingObject);

                // Register object by layer
                uint layer = pendingObject.Layer;
                if (!Layers.ContainsKey(layer))
                {
                    Layers[layer] = new HashSet<GameObject>();
                }
                Layers[layer].Add(pendingObject);

                Debug.WriteLine($"[GameObjectCollection]: Object {pendingObject.Name} ADDED to collection.");

                // Execute object added handlers for objects added to collection
                ObjectAdded?.Invoke(pendingObject);
            }
            else
            {
                Debug.WriteLine($"[GameObjectCollection]: ERROR: Cannot add pending Object {pendingObject.Name} when in game loop state.");
            }

            if (!isObjectAdded)
            {
                throw new Exception($"[GameObjectCollection]: ERROR: Attempt to add duplicated pending object named {pendingObject.Name}");
            }
        }

        public static bool Remove(GameObject gameObject)
        {
            if (Objects.ContainsKey(gameObject.Name))
            {
                Debug.WriteLine($"[GameObjectCollection]: Removing object {gameObject.Name} from collection");
                Objects.Remove(gameObject.Name);

                // Remove object from tag
                if (Tags.ContainsKey(gameObject.Tag))
                {
                    Tags[gameObject.Tag].Remove(gameObject);
                }

                // Remove object from layer
                if (Layers.ContainsKey(gameObject.Layer))
                {
                    Layers[gameObject.Layer].Remove(gameObject);
                }

                // Execute object removal handlers
                ObjectRemoved?.Invoke(gameObject);

                return true;
            }

            return false;
        }

        public static void Clear()
        {
            Layers.Clear();
            Tags.Clear();
            Objects.Clear();
        }

        public static bool ChangeTag(GameObject gameObject, string tag)
        {
            if (!Objects.ContainsKey(gameObject.Name))
            {
                string oldTagLowercase = gameObject.Tag.ToLower();
                string newTagLowercase = tag.ToLower();

                if (!Tags.ContainsKey(newTagLowercase))
                {
                    Tags[newTagLowercase] = new HashSet<GameObject>();
                }

                Tags[oldTagLowercase].Remove(gameObject);
                Tags[newTagLowercase].Add(gameObject);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ChangeLayer(GameObject gameObject, uint otherLayer)
        {
            // Assume the other layer already exists
            if (Objects.ContainsKey(gameObject.Name))
            {
                if (!Layers.ContainsKey(otherLayer))
                {
                    Layers[otherLayer] = new HashSet<GameObject>();
                }

                uint currentLayer = gameObject.Layer;
                Layers[currentLayer].Remove(gameObject);
                Layers[otherLayer].Add(gameObject);

                return true;
            }
            else
            {
                Debug.WriteLine($"[GameObjectCollection]: Cannot change layer of non-existent object {gameObject.Name}");
                return false;
            }
        }

        /// <summary>
        /// Returns the game object by name, or null if not found.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <returns>
        /// GameObject - if a game object with the name exists.
        /// null - if there is no game object with such a name.
        /// </returns>
        public static GameObject FindByName(string name)
        {
            if (Objects.ContainsKey(name))
            {
                return Objects[name];
            }
            return null;
        }

        public static GameObject[] FindObjectsByTag(string tag)
        {
            string tagLowercase = tag.ToLower();

            if (Tags.ContainsKey(tagLowercase))
            {
                GameObject[] objects = new GameObject[Tags[tagLowercase].Count];
                Tags[tagLowercase].CopyTo(objects);
                return objects;
            }
            else
            {
                return null;
            }
        }

        public static GameObject[] FindObjectsByType(Type type)
        {
            List<GameObject> objectList = new List<GameObject>();

            foreach (var (_, obj) in Objects)
            {
                if (obj.GetType() == type)
                {
                    objectList.Add(obj);
                }
            }

            GameObject[] objects = null;
            if (objectList.Count > 0)
            {
                objects = new GameObject[objectList.Count];
                objectList.CopyTo(objects);
            }
            
            return objects;
        }

        // NOTE: This method should be called once before entering the game loop
        public static void LoadAndInitialize()
        {
            if (!_isInGameLoop)
            {
				foreach (var (_, obj) in Objects)
                {
                    obj.LoadContent();
                } 
				
                foreach (var (_, obj) in Objects)
                {
                    obj.Initialize();
                }

                _isInGameLoop = true;
            }
        }

        public static void Update()
        {
            foreach (var (_, obj) in Objects)
            {
                obj.Update();
            }
        }

        public static void Draw()
        {
            // Without layers
            //foreach (var (_, obj) in Objects)
            //{
            //    if (obj.Visible)
            //    {
            //        obj.Draw();
            //    }
            //}

            // Consider layers
            foreach (var (_, objSet) in Layers)
            {
                foreach (var obj in objSet)
                {
                    if (obj.Visible)
                    {
                        obj.Draw();
                    }
                }
            }
        }

        public static void EndDraw()
        {
            // Set to false to allow instantiated objects to move to collection
            _isInGameLoop = false;

            // Add new objects from instantiated dictionary 
            foreach (var (_, obj) in _pendingAdditionObjects)
            {
                AddPendingObjectToCollection(obj);
                Debug.WriteLine($"[GameObjectCollection]: Pending object {obj.Name} ADDED to collection.");
            }

            // Remove objects from deinstatiated dictionary
            foreach (var (_, obj) in _pendingRemovalObjects)
            {
                Remove(obj);
                Debug.WriteLine($"[GameObjectCollection]: Pending object {obj.Name} REMOVED from collection.");
            }

            _isInGameLoop = true;

            _pendingAdditionObjects.Clear();
            _pendingRemovalObjects.Clear();
        }

        public static void DeInstantiate(GameObject obj)
        {
            if (Objects.ContainsKey(obj.Name))
            {
                // NOTE: TryAdd because the same object could be pending removal more than once.
                _pendingRemovalObjects.TryAdd(obj.Name, obj);
            }
            else
            {
                throw new Exception($"[GameObjectCollection]: WARNING: Attempt to deinstantiate a non-existent object named \"{obj.Name}\".");
            }
        }

        public static void Debugging()
        {
            Debug.WriteLine($"[GameObjectCollection]: Objects in Collection:");
            foreach (var (name, obj) in Objects)
            {
                Debug.WriteLine($"{name}");
            }

            Debug.WriteLine($"[GameObjectCollection]: Objects Pending Addition:");
            foreach (var (name, obj) in _pendingAdditionObjects)
            {
                Debug.WriteLine($"{name}");
            }

            Debug.WriteLine($"[GameObjectCollection]: Objects Pending Removal:");
            foreach (var (name, obj) in _pendingRemovalObjects)
            {
                Debug.WriteLine($"{name}");
            }
        }
    }
}

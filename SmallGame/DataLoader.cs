using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using SmallGame.GameObjects;

namespace SmallGame
{

    /// <summary>
    /// The LevelDataObject is the basic JSON object in the level. 
    /// </summary>
    public class LevelDataObject
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public dynamic Data { get; set; }

        // experimental fields.
        public string Script { get; set; }

    }

    /// <summary>
    /// LevelData is the object that is serialized and deserialized to/from JSON. 
    /// </summary>
    public class LevelData
    {
        public string Name { get; set; }
        public dynamic Metadata { get; set; }
        public List<LevelDataObject> Objects { get; set; } 
    }

    /// <summary>
    /// A LevelDataObjectParser is responisble for parsing out one kind of object type. 
    /// </summary>
    public abstract class LevelDataObjectParser
    {
        /// <summary>
        /// The type of object this ldop parses.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets the actual parse function. This takes in a raw ldo, and should output the actual object.
        /// </summary>
        public Func<LevelDataObject, object> ParseFunction { get; protected set; } 
        protected LevelDataObjectParser(string type)
        {
            Type = type;
            ParseFunction = (ldo) => null;
        }
    }

    /// <summary>
    /// A generic specific ldop. The type that this ldop parses is specified by the generic tag. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LevelDataObjectParser<T> : LevelDataObjectParser
    {
        public LevelDataObjectParser(string type) : base(type)
        {
            ParseFunction = (ldo) =>
            {
                PreParse();
                var parsed = Parse(ldo);
                
                PostParse(parsed, ldo);
                return parsed;
            };
        }

        protected virtual void PreParse()
        {
            
        }
        protected virtual void PostParse(T obj, LevelDataObject raw)
        {
            
        }

        public virtual T Parse(LevelDataObject obj)
        {
            return default(T);
        } 
    }


    /// <summary>
    /// An ldop built for GameObjects.
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public class GameObjectParser<G> : LevelDataObjectParser<G> where G : GameObject
    {
        public GameObjectParser() : base(typeof (G).Name)
        {
            
        }

        /// <summary>
        /// in the post parse, the object gets its Type, Id, and Script field filled in. 
        /// The Id will be randomly generated if nothing is specified.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="raw"></param>
        protected override void PostParse(G obj, LevelDataObject raw)
        {
            obj.Type = Type;
            obj.Id = raw.Id;
            
            obj.Script = raw.Script;

            if (String.IsNullOrWhiteSpace(obj.Id))
            {
                obj.Id = IdFactory.NewId;
            }
            base.PostParse(obj, raw);
        }
    }

    public class StandardGameObjectParser
    {
        public static StandardGameObjectParser<G> For<G>() where G : GameObject
        {
            return new StandardGameObjectParser<G>();
        } 
    }

    /// <summary>
    /// The standard game object parser will be able to parse basic objects. use it for this with simple fields.
    /// 
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public class StandardGameObjectParser<G> : GameObjectParser<G> where G : GameObject
    {

        public override G Parse(LevelDataObject obj)
        {
            var g = Activator.CreateInstance<G>();

            try
            {
                var json = JsonConvert.SerializeObject(obj.Data);
                g = (G)JsonConvert.DeserializeObject<G>(json);
            }
            catch (Exception ex) { }
            return g;
        }
    }

    public class DataLoader
    {

        public Dictionary<string, LevelDataObjectParser> Parsers { get; private set; } 

        public DataLoader() : this(new Dictionary<string, LevelDataObjectParser>())
        { }

        public DataLoader(Dictionary<string, LevelDataObjectParser> parsers)
        {
            Parsers = parsers;
        }

        /// <summary>
        /// Registering a parser will allow that parser to try and create objects from the level json. 
        /// You may only add one parser per type. 
        /// </summary>
        /// <param name="parser"></param>
        public void RegisterParser(LevelDataObjectParser parser)
        {
            var type = parser.Type;
            if (Parsers.ContainsKey(type))
            {
                throw new Exception("A parser for this type has already been added. Type=(" + type + ")");
            }
            Parsers.Add(type, parser);
        }

        /// <summary>
        /// Registering a parser will allow that parser to try and create objects from the level json. 
        /// You may only add one parser per type. 
        /// </summary>
        /// <param name="parser"></param>
        public void RegisterParser(params LevelDataObjectParser[] parsers)
        {
            parsers.ToList().ForEach(RegisterParser);
        }

        // terrible way to read a file. TODO: fix this shit.
        private string AttemptToRead(string jsonPath)
        {
            string json = null;
            while (json == null)
            {
                try
                {
                    json = File.ReadAllText(jsonPath);
                }
                catch (Exception ex) { }
            }
            return json;
        }

        public T Load<T>(string jsonPath) where T : GameLevel
        {
            
            //var json = File.ReadAllText(jsonPath);
            //using (var fileStream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (var textReader = new StreamReader(fileStream))
            {
                var json = AttemptToRead(jsonPath);

                var levelData = JsonConvert.DeserializeObject<LevelData>(json);

                var lvl = Activator.CreateInstance<T>();
                lvl.Data = levelData;

                levelData.Objects.ForEach(obj =>
                {

                    if (String.IsNullOrWhiteSpace(obj.Type))
                        return; // don't parse any object that doesnt have a type.

                    if (Parsers.ContainsKey(obj.Type))
                    {
                        var parsedObj = Parsers[obj.Type].ParseFunction(obj);

                        if (parsedObj is GameObject)
                        {
                            lvl.Objects.Add((GameObject) parsedObj);
                        }
                    }

                });

                return lvl;
            }
        }

        public T LoadAndWatch<T>(string jsonPath, Action<T> onLoad) where T : GameLevel
        {
            
            
            var watcher = new FileSystemWatcher(Directory.GetCurrentDirectory()  + "/Data", jsonPath);
            watcher.Changed += (s, a) =>
            {
                Thread.Sleep(100);
                Console.WriteLine("FILE HAS BEEN CHANGED " + jsonPath);
                onLoad(Load<T>("Data/" + jsonPath));
                
            };
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            var lvl = Load<T>("Data/" + jsonPath);
            onLoad(lvl);
            return lvl;
        }

    }
}

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.CSharp;
using Microsoft.Xna.Framework;
using SmallGame.GameObjects;

namespace SmallGame.Services
{
    /// <summary>
    /// The ScriptService is responsible for allowing GameObjects to run dynamic behavours at runtime. 
    /// </summary>
    public interface IScriptService : IGameService
    {
        void RegisterCollection(ScriptCollection collection);
        void RegisterParameterHandler<P>(Func<P> fetcher);
        void Run(string scriptName, GameObject gob);
        void Update(GameTime time);
        void LoadAndWatch(string scriptPath, Action onLoad=null);
        void Load(string scriptPath);
    }

    public class ScriptService : IScriptService
    {

        private Dictionary<string, MethodInfo> _functionTable;
        private Dictionary<string, ScriptCollection> _collectionTable; 
        private Dictionary<Type, Func<object>> _argumentTable;
        private List<ScriptCollection> _collections; 

        public ScriptService()
        {
            _functionTable = new Dictionary<string, MethodInfo>();
            _collectionTable = new Dictionary<string, ScriptCollection>();
            _argumentTable = new Dictionary<Type, Func<object>>();
            _collections = new List<ScriptCollection>();
        }

        public void RegisterCollection(ScriptCollection collection)
        {
            if (collection == null) return; // bad data.

            var collectionType = collection.GetType();
            var methods = collectionType.GetMethods().ToList();
            methods.ForEach(m =>
            {
                //if (_functionTable.ContainsKey(m.Name)) throw new Exception("A method for " + m.Name + " already exists. You must change the name.");
                if (_functionTable.ContainsKey(m.Name))
                {
                    _collectionTable.Remove(m.Name);
                    _functionTable.Remove(m.Name);
                }
                
                _functionTable.Add(m.Name, m);
                _collectionTable.Add(m.Name, collection);
            });
            _collections.Add(collection);
        }

        public void RegisterParameterHandler<P>(Func<P> fetchFunc)
        {
            if (_argumentTable.ContainsKey(typeof(P))) throw new Exception("A type handler for " + typeof(P).Name + " already exists.");

            _argumentTable.Add(typeof(P), () => fetchFunc());
        }

        public void Run(string name, GameObject gob)
        {
            if (name == null) return; // we don't have a sript. 
            if (!_functionTable.ContainsKey(name)) return; // we don't have that script.
            var method = _functionTable[name];

            // get parameters of the method we want to execute.
            var parameters = method.GetParameters().ToList();
            var args = new List<object>();
            parameters.ForEach(p =>
            {
                if( p.ParameterType.IsInstanceOfType(gob) ) // hard code the always required game object.
                {
                    args.Add(gob);
                }
                else if (_argumentTable.ContainsKey(p.ParameterType))
                {
                    args.Add(_argumentTable[p.ParameterType]());
                }
                else
                {
                    throw new Exception("No parameter handler was provided for " + p.ParameterType);
                }
            });

            method.Invoke(_collectionTable[name], args.ToArray());
        }

        public void Update(GameTime time)
        {
            _collections.ForEach(c => c.Update(time));
        }

        public void LoadAndWatch(string scriptFile, Action onLoad=null)
        {
            var watcher = new FileSystemWatcher(Directory.GetCurrentDirectory() + "/Data", scriptFile);
            watcher.Changed += (s, a) =>
            {
                Thread.Sleep(100);
                Load("Data/" + scriptFile);
                if (onLoad != null)
                {
                    onLoad();
                }
            };
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;

            Load("Data/" + scriptFile);
            if (onLoad != null)
            {
                onLoad();
            }

        }

        public void Load(string scriptFile)
        {

            // get all the text of this file.
            var code = File.ReadAllText(scriptFile);

            var codeProvider = new CSharpCodeProvider();
            var icc = codeProvider.CreateCompiler();

            var parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;

            var assemblies = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .Select(a => a.Location);
            parameters.ReferencedAssemblies.AddRange(assemblies.ToArray());

            try
            {
                CompilerResults results = icc.CompileAssemblyFromSource(parameters, code);

                // check for types that are SriptCollections
                var loadedTypes = results.CompiledAssembly.GetTypes();

                var loadedScriptCollections =
                    loadedTypes.Where(t => typeof (ScriptCollection).IsAssignableFrom(t)).ToList();
                loadedScriptCollections.ForEach(sc =>
                {
                    try
                    {
                        RegisterCollection((ScriptCollection) Activator.CreateInstance(sc));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to load script.");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error loading the script");
            }
        }

    }

    public class ScriptCollection
    {
        public virtual void Update(GameTime time)
        {
            // update any globals.
        }
    }

}

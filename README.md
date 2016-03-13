# SmallGame
yet another game framework.

SmallGame (SG) is a framework that sits atop Monogame. SG uses the building blocks from Monogame, like the Spritebatch, ContentManager, to name a few. However, those features are provided through SG's lense. SG is an event driven, service based framework. Basically, all games consist of GameObjects, and those GameObjects can hook into to the game's provided services to listen for events, like Update calls or Render points. 
The framework is bare bones right now, with no helpful game features like collision, physics response, audio, rendering, or even input. Those are on the todo list, but as SG stands currently, it is responsible for ...

* loading a set of game objects from a JSON
* allowing those game objects to use a customizable Script
* basic game loop management. 

## Getting Started

When you import this in Visual Studio, you'll need to ...

1. Run a Nuget package restore to get the JSON library SG uses.
2. Double check and make sure _SmallPlatform_ is set as the Startup Project. 

The rest of this readme will focus on the services, the JSON levels, and an experimental feature, Scripts. 

## JSON Levels

Okay, so at any one time, there is one active level in SG. A level comes from a JSON file. The structure of that JSON file looks like this...

```json
{
    "Name": "sample_level",
    "Metadata": { },
    "Objects": [ ]
}
```

Currently, nothing lives inside of the `Metadata` field, but I expect things such as Author, version number, yadada to live there. The real juice is the `Objects` array. The `Objects` array should hold a set of objects, each one looking like this...

```json
{
    "Type": "SpriteObject",
    "Id": "cloud1",
    "Data": {
        "Position": "100, 10",
        "MediaPath": "Content\\clouds",
        "Omega": "0",
        "Color": {"R": 255}
    },
    "Script": "TestScript"
}
```

Lets break it down. 

Field   | Required? | Explanation
-----   | --------- | -----------
_Type_  | **yes**   | The type of object. This should the classname of some object that inherits from GameObject
_Id_    | no        | A **unique** id for the object. If left blank, it will be assigned a GUID.
_Script_| no        | A function to run as a script. See the Scripting section.
_Data_  | **yes**   | An object containing all of the fields for this object. In the example, the object is declaring a position, a file to use as a texture, a rotational velocity, and a color. These are specific to the kind of GameObject specified by the _Type_ field.


You can have as many objects as you please in the `Objects` array. Each of them will get parsed by SG. To do this, the `DataLoader` must be called. The `DataLoader` has a collection of parsers, one for each object type, and will pass each json object string to the correct parser for that type. The parser will spit out (if the json is valid) an object, and tada, a set of GameObjects is born. 

## Services

The `CoreGame` is the master of SG. It coordinates everything, and each game built with SG should inherit from `CoreGame`. `CoreGame` offers a bunch of stuff, like access to the main game loop, but it also provides access to the `CoreGameServices`. 

The `CoreGameServices` is a collection of `CoreGameService`. A `CoreGameService` can be anything, so long as it implements `CoreGameService` (which is an empty interface spec). Some examples of built in services are the `UpdateService`, the `RenderService`, and the `ScriptService`. The collection is kept by `CoreGame` as a property, `Services`. Services can be registered, and requested, by type. For example, to access the `RenderService`, use...

```csharp
// renderSrvc will be the instance of RenderService that was registered by CoreGame
var renderSrvc = Services.RequestService<RenderService>();
```

The services collection will be passed to each GameObject on the GameObject's `Init()` call. This important, because it allows the GameObject to specify what services it cares about. Not every GameObject needs to be a part of the Update cycle, and not every GameObject needs to render in the Render time. With this service based system, a GameObject can request the `RenderService` (like in the example above) and add an `EventHandler` to the `OnRender` event contained inside the service. Both the `UpdateService` and the `RenderService` contain events that execute when that part of the game loop is happening. So, to add something to the update cycle, add an eventhandler to the update event in the `UpdateService`. 

The service based system should prove to be useful later on too. I foresee having services that handle object collision and response. It also allows render strategies to be swapped in and out with relative ease (and at runtime too). 

# Scripts (and live editing)

Every object specified in the level JSON file can have a `Script` field. The value should be the name of some function that you want to run on the object at update time. This takes some justification. Lets say you want to populate a level with hundreds of art assets, where some of them behave in certain/specific ways. With the basic setup of having a class backing each type of GameObject, you'd need to create a class of GameObject for every single behavior you want, which is ridiculous. Generally speaking, you want to keep the logic of a GameObject in the GameObject class itself, but in some cases, its better to have it done outside. This is where Scripts come to save the day. 

With a reference to the `ScriptService`...

```csharp
// grab the reference to the ScriptService
var scriptSrvc = Services.RequestService<ScriptService>();
```

you can register a `ScriptCollection`, 
```csharp
// register a script collection
scriptSrvc.RegisterCollection(new MyScripts());
```

A `ScriptCollection` is just some class that has a bunch of methods that take in GameObjects, and produce void in it. The class must extend `ScriptCollection` however.

```csharp
public class MyScripts : ScriptCollection 
{

    public void TestScript(BasicObject obj)
    {
        obj.Position += Vector2.UnitX; 
    }

    public override void Update(GameTime time)
    {
        // okay, so there is one overridable function called Update. This gets run every Update cycle. 
    }
}
```

In the JSON section above, the sample object has _TestScript_ as the value for `Script`. By registering the collection, all of the methods in the Collection get added to a pool of possible functions to run. This feature is very mature yet, and I'm sure bugs exist. Use this at your own discretion. The input to the function name listed by the JSON will be the GameObject instance itself. If the types don't match, nothing will happen. 

You can add additional parameters to a script function, like 

```csharp
public void TestScript2(BasicObject obj, MySpecialType specialObj)
{
    // do something with obj and specialObj.
}
```

However, this will cause problems **unless** you register a parameter handler in the `ScriptService`. This can be done like so...

```csharp
scriptSrvc.RegisterParameterHandler( ()=> return new MySpecialType() );
```

Jenky? Yes. Now anytime the `ScriptService` is trying to invoke a function with a parameter somewhere of type `MySpecialType`, it'll run the passed lambda to get an instance of it. The instance will be passed to the script function. As a consequence of this approach, each script function can only have 1 parameter of each type. Generally, you shouldn't need to pass many things in this way. Scripts aren't really supposed to be used to carry out intense logic. That should live in the GameObject itself. 

If you prefer, you can also load a `ScriptCollection` from a file, at runtime. Instead of using `RegisterCollection`, you need to call `Load` and pass in a cs file path. 

In the sample code in _SmallPlatform_, you'll notice that both the level JSON code and the ScriptCollection is being loaded with a `LoadAndWatch` function. This will watch for file changes to the text files in question. When a change is noticed, the system will re-load the file, and accept the new games into the game without need to recompile and rerun. I find this to be incredibly useful and fun to play with. **NOTE** you need to the files in the /bin directory. I am actively thinking about a way to make it so that you can edit the file sin the /data directory, and have them be auto copied into the /bin to be re-loaded. 
using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering.Textures;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Textures
{
    public static class TextureCache
    {

        [UsingDependency]
        private static ILogger Log;

        private const string ICON_PATH = "Content/";

        /// <summary>
        /// The error icon state to display if things fail
        /// </summary>
        public const string ERROR_ICON_STATE = "error";

        /// <summary>
        /// A store of the loaded texture files
        /// </summary>
        private static Dictionary<string, ITexture> TextureFileCache = new Dictionary<string, ITexture>();

        /// <summary>
        /// A store of the loaded texture json data
        /// </summary>
        private static Dictionary<string, TextureJson> TextureJsons = new Dictionary<string, TextureJson>();

        /// <summary>
        /// Cache of texture states, to optimise expensive is transparent checks
        /// </summary>
        private static Dictionary<string, ITextureState> TextureStates = new Dictionary<string, ITextureState>();

        /// <summary>
        /// Has loading been completed?
        /// </summary>
        public static bool LoadingComplete { get; private set; } = false;

        /// <summary>
        /// Does the error state exist?
        /// </summary>
        public static bool ErrorStateExists { get => TextureJsons.ContainsKey(ERROR_ICON_STATE); }

        /// <summary>
        /// Test the error state
        /// </summary>
        /// <returns></returns>
        public static string GetErrorFile()
        {
            TextureJson errTex = TextureJsons[ERROR_ICON_STATE];
            return $"{ICON_PATH}{errTex.FileName}";
        }

        internal static ITextureState GetTexture(string textureState, bool checkSanity = false)
        {
            TextureJson usingJson;
            // Check if the block texture exists
            if (TextureJsons.ContainsKey(textureState))
                usingJson = TextureJsons[textureState];
            else
            {
                Log?.WriteLine($"Error, block texture: {textureState} not found!", LogType.WARNING);
                usingJson = TextureJsons[ERROR_ICON_STATE];
            }
            //Locate the texture object we need
            if (TextureFileCache.ContainsKey(usingJson.FileName))
            {
                if (TextureStates.ContainsKey(textureState))
                {
                    return TextureStates[textureState];
                }
                lock (TextureStates)
                {
                    if (TextureStates.ContainsKey(textureState))
                    {
                        return TextureStates[textureState];
                    }
                    ITexture texture = TextureFileCache[usingJson.FileName];
                    //Convert 0-width (width) to -1 to 1 (2)
                    float pixelFactorX = 1.0f / texture.Width;
                    float pixelFactorY = 1.0f / texture.Height;
                    TextureState storedTextureState = new TextureState(
                        texture,
                        usingJson.IndexX * 32 * pixelFactorX,
                        usingJson.IndexY * 32 * pixelFactorY,
                        usingJson.Width * pixelFactorX,
                        usingJson.Height * pixelFactorY,
                        usingJson.DirectionalModes);
                    TextureStates.Add(textureState, storedTextureState);
                    return storedTextureState;
                }
            }
            else
            {
                try
                {
                    //Load the texture
                    TextureBitmap loadedBitmap = new TextureBitmap();
                    loadedBitmap.ReadTexture($"{ICON_PATH}{usingJson.FileName}");
                    //Cache the created texture
                    TextureFileCache.Add(usingJson.FileName, loadedBitmap);
                    //Return the created texture
                    ITexture texture = TextureFileCache[usingJson.FileName];
                    //Convert 0-width (width) to 0 to 1 (2)
                    float pixelFactorX = 1.0f / texture.Width;
                    float pixelFactorY = 1.0f / texture.Height;
                    return new TextureState(
                        texture,
                        usingJson.IndexX * 32 * pixelFactorX,
                        usingJson.IndexY * 32 * pixelFactorY,
                        usingJson.Width * pixelFactorX,
                        usingJson.Height * pixelFactorY,
                        usingJson.DirectionalModes);
                }
                catch (Exception e)
                {
                    //Catch whatever error we got (Probably lack of file)
                    Log?.WriteLine(e, LogType.ERROR);
                    if (!checkSanity)
                    {
                        //Return a standard error texture
                        return GetTexture(ERROR_ICON_STATE, true);
                    }
                    else
                    {
                        //Just die at this point, our error icon doesn't exist.
                        throw new Exception("Failed to load error icon.", e);
                    }
                }
            }
        }

        /// <summary>
        /// Load the texture data json
        /// </summary>
        [ModuleLoad(mainThread = true)]
        public static void LoadTextureDataJson()
        {
            //Loaded texture data
            Log?.WriteLine("Loading texture data...", LogType.MESSAGE);
            foreach (string file in Directory.GetFiles("Content/", "*.texdat", SearchOption.AllDirectories))
            {
                LoadTextureDataJsonThread(file, false);
            }
            InitializeTextureObjects();
            //Loaded Texture cache
            Log?.WriteLine($"Successfully loaded data about {TextureJsons.Count} textures.", LogType.MESSAGE);
            //All texture data loaded
            Log?.WriteLine("All texture data loaded!", LogType.MESSAGE);
        }

        /// <summary>
        /// Seperate thread for loading the texture Json
        /// </summary>
        private static void LoadTextureDataJsonThread(string fileName, bool catchErrors = true)
        {
            //Start loading and parsing the data
            JObject loadedJson = JObject.Parse(File.ReadAllText(fileName));
            //Json file loaded
            Log?.WriteLine($"Json file {fileName} loaded and parsed, populating block texture cache", LogType.MESSAGE);
            //Load the texture jsons
            JToken texturesProperty = loadedJson["textures"];
            foreach (JToken value in texturesProperty)
            {
                try
                {
                    string id = value.Value<string>("id");
                    string file = value.Value<string>("file");
                    int width = value.Value<int>("width");
                    int height = value.Value<int>("height");
                    int index_x = value.Value<int>("index_x");
                    int index_y = value.Value<int>("index_y");
                    string directionalStateMode = value.Value<string>("directional") ?? "NONE";
                    //Create the texture json
                    TextureJson createdJson = new TextureJson(file, width, height, index_x, index_y, (DirectionalModes)Enum.Parse(typeof(DirectionalModes), directionalStateMode));
                    //Cache it
                    TextureJsons.Add(id, createdJson);
                }
                catch (Exception e)
                {
                    if (catchErrors)
                    {
                        //TODO: Error handling
                        Log?.WriteLine(e, LogType.ERROR);
                    }
                    else
                    {
                        Log?.WriteLine(e.StackTrace);
                        LoadingComplete = true;
                        throw;
                    }
                }
            }
            //Loading completed
            LoadingComplete = true;
        }

        public static void InitializeTextureObjects()
        {
            foreach (string jsonKey in TextureJsons.Keys)
            {
                TextureJson json = TextureJsons[jsonKey];
                if (!TextureFileCache.ContainsKey(json.FileName))
                    GetTexture(jsonKey);
            }
        }

    }
}

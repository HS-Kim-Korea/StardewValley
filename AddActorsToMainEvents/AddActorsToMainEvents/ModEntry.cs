using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Events;
using Harmony;

namespace AddActorsToMainEvents
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        private static ModConfig config;
        private static Dictionary<string, ModContent.Actor> actors = new Dictionary<string, ModContent.Actor>();
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;

            config = this.Helper.ReadConfig<ModConfig>();
            if (config.enable)
            {
                Log(String.Format("Enabled AddActorsToMainEvents"));
                GetContent();
                GetContentPacks();

                // harmony patch
                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
                var original = typeof(Event).GetMethod("command_loadActors", BindingFlags.Instance | BindingFlags.Public);
                var postfix = typeof(ModEntry).GetMethod("loadActorsPostfix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, null, new HarmonyMethod(postfix));

                if(config.debug)
                {
                    original = typeof(Event).GetMethod("addActor", BindingFlags.Instance | BindingFlags.NonPublic);
                    postfix = typeof(ModEntry).GetMethod("addActorPrefix", BindingFlags.Static | BindingFlags.Public);
                    harmony.Patch(original, new HarmonyMethod(postfix));
                }
            }
        }
        public static void Log(string text, LogLevel level = LogLevel.Debug)
        {
            instance.Monitor.Log(text, level);
        }
        private void GetContent()
        {
            ModContent content = helper.Data.ReadJsonFile<ModContent>("content.json") ?? new ModContent();
            AddActors(content.loadActors);
        }
        private void GetContentPacks()
        {
            foreach (IContentPack contentPack in helper.ContentPacks.GetOwned())
            {
                DirectoryInfo directory = new DirectoryInfo(contentPack.DirectoryPath);
                if (directory.Exists)
                {
                    Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}");
                    ModContent content = contentPack.ReadJsonFile<ModContent>("content.json") ?? new ModContent();
                    AddActors(content.loadActors);
                }
            }
        }
        private void AddActors(List<ModContent.Actor> loadActors)
        {
            foreach (ModContent.Actor actor in loadActors)
            {
                string key = $"{actor.target}.{actor.layer}.{actor.name}";
                if (!actors.ContainsKey(key))
                {
                    Log($"Actor key: {key}");
                    actors.Add(key, actor);
                }
            }
        }
        public static void addActorPrefix(Event __instance, string name, int x, int y, int facingDirection, GameLocation location)
        {
            try
            {
                Log($"addActorPrefix[{name}] : [{x}:{y}:{facingDirection}] : {location.Name}");
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(addActorPrefix)}:\n{ex}");
            }
        }
        public static void loadActorsPostfix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                IReflectedField<Dictionary<string, string>> festivalData = helper.Reflection.GetField<Dictionary<string, string>>(__instance, "festivalData");
                if (ModEntry.config.debug) Log($"Target : {festivalData.GetValue()["file"]}, Layer: {split[1]}");

                foreach(ModContent.Actor actor in actors.Values)
                {
                    if(actor.target == festivalData.GetValue()["file"] && actor.layer == split[1])
                    {
                        IReflectedField<GameLocation> temporaryLocation = helper.Reflection.GetField<GameLocation>(__instance, "temporaryLocation");
                        MethodInfo addActor = __instance.GetType().GetMethod("addActor", BindingFlags.Instance | BindingFlags.NonPublic);
                        Dictionary<string, string> source = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
                        if (addActor != null && source.ContainsKey(actor.name))
                        {
                            Log($"Add actor : {actor.name}");
                            addActor.Invoke(__instance, new object[] { actor.name, actor.position[0], actor.position[1], actor.direction, temporaryLocation.GetValue() });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(loadActorsPostfix)}:\n{ex}");
            }
        }
    }
}

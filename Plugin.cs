using BepInEx;
using BepInEx.Logging;

using DiskCardGame;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace SigilArtPatch
{
    [BepInPlugin( PluginGuid, PluginName, PluginVersion )]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "MADH.inscryption.SigilArtPatch";
        private const string PluginName = "SigilArtPatch";
        private const string PluginVersion = "1.0.0.0";

        static readonly string[] artPath = { Paths.PluginPath, "Artwork" };

        private static readonly string ArtPath = Path.Combine( artPath );

        internal static ManualLogSource Log;

        private void Awake()
        {
            Logger.LogInfo( $"Loaded { PluginName }!" );
            Log = base.Logger;

            Harmony harmony = new( PluginGuid );
            harmony.PatchAll();

            var textures = GetTextures();

            AbilitiesUtil_LoadAbilityIcon.TextureDict = new( 4 )
            {
                { "SkeletonStrafe", this.LoadTexture( "SkeletonStrafe", Path.Combine( ArtPath, textures[ 0 ] ) ) },
                { "SquirrelStrafe", this.LoadTexture( "SquirrelStrafe", Path.Combine( ArtPath, textures[ 1 ] ) ) },
                { "SubmergeSquid", this.LoadTexture( "SubmergeSquid", Path.Combine( ArtPath, textures[ 2 ] ) ) },
                { "DrawNewHand", this.LoadTexture( "DrawNewHand", Path.Combine( ArtPath, textures[ 3 ] ) ) }
            };

        }

        public List<string> GetTextures()
        {
            return new()
            {
                Config.Bind( "SigilArtPatch", "SkeletonStrafe", "skelestrafe.png" ).Value,
                Config.Bind( "SigilArtPatch", "SquirrelStrafe", "squirrelstrafe.png" ).Value,
                Config.Bind( "SigilArtPatch", "SubmergeSquid", "tentacle.png" ).Value,
                Config.Bind( "SigilArtPatch", "DrawNewHand", "redrawhand.png" ).Value,
            };
        }

        public Texture LoadTexture( string ability, string image )
        {
            if ( string.IsNullOrEmpty( image ) )
            {
                Logger.LogError( $"{ability} - the texture cannot be empty" );
                return null;
            }

            if ( !image.EndsWith( ".png" ) )
            {
                Logger.LogError( $"{ability} - {image} must be a .png file" );
                return null;
            }

            Texture2D texture = new( 2,2 );
            byte[] imgBytes = File.ReadAllBytes( Path.Combine( ArtPath, image ) );
            texture.LoadImage( imgBytes );

            return texture;
        }

    }


    [HarmonyPatch(typeof(AbilitiesUtil), "LoadAbilityIcon", new Type[] { typeof(string), typeof(bool), typeof(bool) })]
	public class AbilitiesUtil_LoadAbilityIcon
    {
        public static Dictionary<string, Texture> TextureDict;

        [HarmonyBefore( new string[] { "cyantist.inscryption.api" } )]
        public static bool Prefix( string abilityName, ref Texture __result )
        {
            if ( !TextureDict.ContainsKey( abilityName ) )
                return true;

            if ( TextureDict[ abilityName ] == null )
                return true;

            __result = TextureDict[ abilityName ];

            return false;
        }
    }
}

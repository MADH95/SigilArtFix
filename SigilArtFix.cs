using System;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

using UnityEngine;

using DiskCardGame;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

namespace SigilArtPatch
{
    using Properties;

    [BepInPlugin( PluginGUID, PluginName, PluginVersion )]
    public class Plugin : BaseUnityPlugin
    {
        public const string APIGUID = "cyantist.inscryption.api";
        private const string PluginGUID = "MADH.inscryption.SigilArtPatch";
        private const string PluginName = "SigilArtPatch";
        private const string PluginVersion = "1.5.0.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Logger.LogInfo( $"Loaded { PluginName }!" );
            Log = base.Logger;

            Harmony harmony = new( PluginGUID );
            harmony.PatchAll();

            using ResourceSet resourceSet = Resources.ResourceManager.GetResourceSet( CultureInfo.CurrentCulture, true, true );

            AbilitiesUtil_LoadAbilityIcon.textures = new();

            foreach ( DictionaryEntry entry in resourceSet )
            {
                AbilitiesUtil_LoadAbilityIcon.textures.Add( ( string ) entry.Key, LoadTexture( ( byte[] ) entry.Value ) );
            }

            Log.LogInfo( "Sigil Art Loaded!" );
        }

        private static Texture LoadTexture( byte[] imgBytes )
        {
            Texture2D texture = new( 2,2 );
            texture.LoadImage( imgBytes );
            return texture;
        }
    }

    [HarmonyPatch(typeof(AbilitiesUtil), "LoadAbilityIcon", new Type[] { typeof(string), typeof(bool), typeof(bool) })]
	public class AbilitiesUtil_LoadAbilityIcon
    {

        public static Dictionary<string, Texture> textures;

        [HarmonyBefore( new string[] { Plugin.APIGUID } )]
        public static bool Prefix( string abilityName, ref Texture __result )
        {
            if ( !textures.ContainsKey( abilityName ) )
                return true;

            __result = textures[abilityName];

            return false;
        }
    }
}

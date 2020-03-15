using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using CaiLib.Utils;
using Harmony;
using UnityEngine;
using Object = UnityEngine.Object;
using Timer = System.Timers.Timer;

// ReSharper disable UnusedType.Global

namespace InfiniteStorage
{
    public class InfiniteStoragePatches
    {
        public static void OnLoad()
        {
            CaiLib.Logger.Logger.LogInit();

            BuildingUtils.AddBuildingToPlanScreen( GameStrings.PlanMenuCategory.Base, DeepItemStorage.Id );
            BuildingUtils.AddBuildingToPlanScreen( GameStrings.PlanMenuCategory.Base, DeepLiquidStorage.Id );
            BuildingUtils.AddBuildingToPlanScreen( GameStrings.PlanMenuCategory.Base, DeepGasStorage.Id );

            // This registers the locstrings
            // No need to STRINGS.Add or use a custom building strings anything 
            LocString.CreateLocStringKeys( typeof( DEEP_STORAGE_STRINGS.BUILDINGS ) );
            LocString.CreateLocStringKeys( typeof( DEEP_STORAGE_STRINGS.UI ) );
        }

        [HarmonyPatch( typeof( TreeFilterableSideScreen ), nameof( TreeFilterableSideScreen.SetTarget ) )]
        public class TreeFilterableSideScreen_SetTarget_Patches
        {
            public static void Prefix( Storage ___storage )
            {
                if ( ___storage != null )
                {
                    var show = ___storage.gameObject.GetComponent<ShowHideContentsButton>();
                    if ( show != null )
                        ___storage.showInUI = true;
                }
            }

            public static void Postfix( Storage ___storage )
            {
                if ( ___storage != null )
                {
                    var show = ___storage.gameObject.GetComponent<ShowHideContentsButton>();
                    if ( show != null )
                        ___storage.showInUI = show.showContents;
                }
            }
        }

        [HarmonyPatch( typeof( TreeFilterableSideScreen ), "CreateCategories" )]
        public class TreeFilterableSideScreen_CreateCategories_Patches
        {
            public static IEnumerable<CodeInstruction> Transpiler( IEnumerable<CodeInstruction> orig )
            {
                List<CodeInstruction> codes = orig.ToList();

                for ( var i = 0; i < codes.Count; ++i )
                {
                    var code = codes[i];
                    if ( code.opcode == OpCodes.Ldloc_0 )
                    {
                        var target = AccessTools.Field( typeof( TreeFilterableSideScreen ), "target" );
                        Debug.Log( target );
                        codes.Insert( i++, new CodeInstruction( OpCodes.Ldarg_0 ) );
                        codes.Insert( i++, new CodeInstruction( OpCodes.Ldfld, target ) );
                        var getInfStorage = AccessTools.Method(
                            typeof( GameObject ),
                            "GetComponent"
                        );
                        getInfStorage = getInfStorage.MakeGenericMethod(typeof(InfiniteStorage));

                        Debug.Log( getInfStorage );

                        codes.Insert( i++, new CodeInstruction( OpCodes.Callvirt, getInfStorage ) );
                        codes.Insert( i++, new CodeInstruction( OpCodes.Ldnull ) );
                        var goInequality = AccessTools.Method(
                            typeof( Object ),
                            "op_Inequality",
                            new[] {typeof( Object ), typeof( Object )}
                        );

                        Debug.Log( goInequality );

                        codes.Insert( i++, new CodeInstruction( OpCodes.Call, goInequality ) );
                        var branchLabel = new Label();
                        codes.Insert( i++, new CodeInstruction( OpCodes.Brfalse_S, branchLabel ) );
                        codes.Insert( i++, new CodeInstruction( OpCodes.Ldc_I4_1 ) );
                        codes.Insert( i++, new CodeInstruction( OpCodes.Stloc_0 ) );
                        
                        // i is at the existing stloc.0 we piggyback off of
                        // We want the label on the next instruction
                        codes[i].labels = new List<Label> {branchLabel};
                        break;
                    }
                }

            #if DEBUG
                Debug.Log( "CreateCategories IL" );
                foreach ( var instruction in codes )
                    Console.WriteLine( instruction );
            #endif

                return codes;
            }
        }

        [HarmonyPatch(typeof(TreeFilterableSideScreen), "AddRow")]
        public class TreeFilterableSideScreen_AddRow_Patches
        {
            public static void Postfix(TreeFilterableSideScreen __instance, TreeFilterableSideScreenRow __result, Tag rowTag)
            {
                var sw = new Stopwatch();
                sw.Start();
                var instance = Traverse.Create(__instance);
                var target = (GameObject) instance.Field("target").GetValue();
                if (target.GetComponent<InfiniteStorage>() != null)
                {
                    TreeFilterable targetFilterable = (TreeFilterable) instance.Field("targetFilterable").GetValue();
                    Debug.Log(rowTag);
                    var map = new Dictionary<Tag, bool>();
                    foreach (var element in ElementLoader.elements)
                    {
                        // We also need to match storage filter
                        if (element.HasTag(rowTag) && target.GetComponent<TreeFilterable>().AcceptedTags.Contains(element.tag))
                        {
                            Debug.Log(element.name);
                            map.Add(element.tag, targetFilterable.ContainsTag(element.tag) || targetFilterable.ContainsTag(rowTag));
                        }
                    }
                    
                    // Does state matter???  It looks unused
                    // Klei why
                    __result.SetElement(rowTag, targetFilterable.ContainsTag(rowTag), map);
                }
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Debug.Log($"Method took {elapsedTime}");
            }
        }

        [HarmonyPatch( typeof( ConduitConsumer ), "Consume" )]
        public class ConduitConsumer_Consume_Patches
        {
            public static bool Prefix( ConduitConsumer __instance, ConduitFlow conduit_mgr, int ___utilityCell )
            {
                // if we don't have the component, do nothing special
                if ( __instance.gameObject.GetComponent<InfiniteStorage>() == null )
                    return true;

                var contents = conduit_mgr.GetContents( ___utilityCell );

                var storage = __instance.gameObject.GetComponent<Storage>();
                if ( storage == null )
                    return true;

                var filterable = __instance.gameObject.GetComponent<TreeFilterable>();
                if (filterable == null)
                    return true;

                // If it doesn't contain the tag, return false, don't consume
                var tag = ElementLoader.FindElementByHash( contents.element ).tag;
                var ret = filterable.AcceptedTags.Contains( tag );
                return ret;
            }
        }

        [HarmonyPatch( typeof( Storage ), "AddLiquid" )]
        public class Storage_AddLiquid_Patches
        {
            public static void Prefix(
                SimHashes element,
                ref float mass,
                float temperature,
                byte disease_idx,
                int disease_count,
                bool keep_zero_mass,
                bool do_disease_transfer,
                Storage __instance
            )
            {
                // We don't have any more space for that much mass
                var primaryStored = __instance.FindPrimaryElement( element );
                // Let default behavior if not exists
                if ( primaryStored == null )
                    return;

                var massAvailable = PrimaryElement.MAX_MASS - primaryStored.Mass;
                if ( massAvailable <= mass )
                {
                    var overflowMass = mass - massAvailable;
                    mass = massAvailable;

                    var chunk = LiquidSourceManager.Instance.CreateChunk(
                        element,
                        overflowMass,
                        temperature,
                        disease_idx,
                        disease_count,
                        __instance.transform.GetPosition()
                    );

                    chunk.GetComponent<PrimaryElement>().KeepZeroMassObject = keep_zero_mass;
                    __instance.Store( chunk.gameObject, true, false, do_disease_transfer );
                }
            }
        }

        [HarmonyPatch( typeof( Storage ), "AddGasChunk" )]
        public class Storage_AddGasChunk_Patches
        {
            public static void Prefix(
                SimHashes element,
                ref float mass,
                float temperature,
                byte disease_idx,
                int disease_count,
                bool keep_zero_mass,
                bool do_disease_transfer,
                Storage __instance
            )
            {
                // We don't have any more space for that much mass
                var primaryStored = __instance.FindPrimaryElement( element );
                // Let default behavior if not exists
                if ( primaryStored == null )
                    return;

                var massAvailable = PrimaryElement.MAX_MASS - primaryStored.Mass;
                if ( massAvailable <= mass )
                {
                    var overflowMass = mass - massAvailable;
                    mass = massAvailable;

                    var chunk = GasSourceManager.Instance.CreateChunk(
                        element,
                        overflowMass,
                        temperature,
                        disease_idx,
                        disease_count,
                        __instance.transform.GetPosition()
                    );

                    chunk.GetComponent<PrimaryElement>().KeepZeroMassObject = keep_zero_mass;
                    __instance.Store( chunk.gameObject, true, false, do_disease_transfer );
                }
            }
        }

        [HarmonyPatch( typeof( Storage ), nameof( Storage.MassStored ) )]
        public class Storage_MassStored_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler( IEnumerable<CodeInstruction> codeInstructions )
            {
                List<CodeInstruction> codes = codeInstructions.ToList();
                var patched = false;

                for ( var i = 0; i < codes.Count; ++i )
                {
                    var ci = codes[i];
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if ( ci.opcode == OpCodes.Ldc_R4 && (float) ci.operand == 1_000f )
                    {
                        // This replaces the code that multiplies and divides by 1000
                        // with code that just uses Math.Round(num, 3)
                        // why didn't you do this Klei?
                        codes[i] = new CodeInstruction( OpCodes.Ldc_I4_3 );
                        codes[i + 1] = new CodeInstruction(
                            OpCodes.Call,
                            AccessTools.Method( typeof( Math ), "Round", new[] {typeof( double ), typeof( int )} )
                        );

                        codes[i + 2] = new CodeInstruction( OpCodes.Conv_R4 );
                        codes[i + 3] = new CodeInstruction( OpCodes.Ret );

                        codes.RemoveRange( i + 4, codes.Count - (i + 4) );
                        patched = true;
                        break;
                    }
                }

                if ( !patched )
                    Debug.LogWarning( "[InfiniteStorage] Unable to patch storage display mass" );
                else
                    Debug.Log( "[InfiniteStorage] Patched storage display mass." );

            #if DEBUG
                foreach ( var ci in codes )
                    Console.WriteLine( $"{ci.opcode} {ci.operand}" );
            #endif

                return codes.AsEnumerable();
            }
        }
    }
}

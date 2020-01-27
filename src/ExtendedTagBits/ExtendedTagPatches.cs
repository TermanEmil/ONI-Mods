using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace ExtendedTagBits
{
    public class ExtendedTagPatches
    {
        private static readonly FieldInfo Bits5Field = AccessTools.Field(typeof(TagBits), "bits5");

        private static readonly MethodInfo IdFromTag =
            AccessTools.Method(typeof(ExtendedTagBits), "GetIdIfExists", new[] {typeof(Tag)});

        private static readonly MethodInfo IdFromTags =
            AccessTools.Method(typeof(ExtendedTagBits), "GetIdIfExists", new[] {typeof(Tag[])});

        [HarmonyPatch]
        public class TagBits_ref_ctor
        {
            public static MethodBase TargetMethod()
            {
                return AccessTools.Constructor(typeof(TagBits), new[] {typeof(TagBits).MakeByRefType()});
            }

            public static void Postfix(ref TagBits __instance)
            {
                var traverse = Traverse.Create(__instance).Field("bits5");
                // If there isn't already an ID assigned, assign a new one
                //Debug.Log((ulong) traverse.GetValue());
                if ((ulong) traverse.GetValue() == 0L)
                    traverse.SetValue(ExtendedTagBits.Id);
            }
        }

        [HarmonyPatch(typeof(TagBits), MethodType.Constructor, typeof(Tag))]
        public class TagBits_Tag_ctor
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                var index = codes.FindLastIndex(c => c.opcode == OpCodes.Stfld);

                if (index != -1)
                {
                    index -= 2;
                    codes[index++] = new CodeInstruction(OpCodes.Ldarg_1);
                    codes[index] = new CodeInstruction(OpCodes.Call, IdFromTag);
                }
                else
                    Debug.LogError("[Extra Tag Bits] Error patching TagBits.ctor(Tag)");

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), MethodType.Constructor, typeof(Tag[]))]
        public class TagBits_TagArr_ctor
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                var index = codes.FindLastIndex(c => c.opcode == OpCodes.Stfld);

                if (index != -1)
                {
                    index -= 2;
                    codes[index++] = new CodeInstruction(OpCodes.Ldarg_1);
                    codes[index] = new CodeInstruction(OpCodes.Call, IdFromTags);
                }
                else
                    Debug.LogError("[Extra Tag Bits] Error patching TagBits.ctor(Tag[])");

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.GetTagsVerySlow), new Type[0])]
        public class TagBits_GetTagsVerySlow_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                var index = codes.IndexOf(codes.Last(e => e.opcode == OpCodes.Call));
                if (index != -1)
                {
                    var getExtraTags = typeof(ExtendedTagBits).GetMethod("GetExtraTags");
                    codes[index] = new CodeInstruction(OpCodes.Call, getExtraTags);
                    codes.Insert(index + 1, new CodeInstruction(OpCodes.Pop));
                    codes.Insert(index + 1, new CodeInstruction(OpCodes.Pop));
                }
                else
                {
                    Debug.LogError("[Extra Tag Bits] Could not patch GetTagsVerySlow.");
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.SetTag))]
        public class TagBits_SetTag_Patch
        {
            // Always skip check for 384
            // Change error log to call to helper method
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                for (var i = 0; i < codes.Count; ++i)
                {
                    var code = codes[i];
                    if (code.opcode == OpCodes.Ldc_I4 && (int) code.operand == 384)
                    {
                        ++i;
                        codes.Insert(i++, new CodeInstruction(OpCodes.Pop));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Pop));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld, Bits5Field));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_0));
                        var additionalTagsMethod =
                            typeof(ExtendedTagBits).GetMethod(nameof(ExtendedTagBits.AddExtraTagBits));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Call, additionalTagsMethod));
                        codes.Insert(i, new CodeInstruction(OpCodes.Ret));
                        break;
                    }
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.Clear))]
        public class TagBits_Clear_Patch
        {
            // Always skip check for 384
            // Change error log to call to helper method
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                for (var i = 0; i < codes.Count; ++i)
                {
                    var code = codes[i];
                    if (code.opcode == OpCodes.Ldc_I4 && (int) code.operand == 384)
                    {
                        ++i;
                        codes.Insert(i++, new CodeInstruction(OpCodes.Pop));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Pop));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld, Bits5Field));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_0));
                        var additionalTagsMethod =
                            typeof(ExtendedTagBits).GetMethod(nameof(ExtendedTagBits.RemoveExtraTagBits));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Call, additionalTagsMethod));
                        codes.Insert(i, new CodeInstruction(OpCodes.Ret));
                        break;
                    }
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.ClearAll))]
        public class TagBits_ClearAll_Patch
        {
            public static void Prefix(ref TagBits __instance)
            {
                var id = (ulong) Traverse.Create(__instance).Field("bits5").GetValue();
                if (ExtendedTagBits.ExtraTagList.ContainsKey(id))
                    ExtendedTagBits.ExtraTagList[id].Clear();
            }
        }

        [HarmonyPatch(typeof(TagBits), "ManifestFlagIndex")]
        public class TagBits_ManifestFlagIndex_Patch
        {
            public static bool Prefix(ref Tag tag, ref int __result)
            {
                __result = ExtendedTagBits.GetTagIndex(tag);
                return true;
            }
        }


        [HarmonyPatch(typeof(TagBits), nameof(TagBits.HasAll))]
        public class TagBits_HasAll_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                var index = codes.FindLastIndex(c => c.opcode == OpCodes.Ldfld);

                if (index != -1)
                {
                    var tagsCompare = typeof(ExtendedTagBits).GetMethod("HasAllExtraTags");
                    var retLabel = codes.Last().labels[0];

                    index -= 2;
                    codes.RemoveRange(index, codes.Count - 2 - index);
                    codes.Insert(index++, new CodeInstruction(OpCodes.Call, tagsCompare));
                    codes.Insert(index++, new CodeInstruction(OpCodes.Conv_I4));
                    codes.Insert(index, new CodeInstruction(OpCodes.Br, retLabel));
                }
                else
                {
                    Debug.LogError("[Extra Tag Bits] Error patching HasAll.");
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.HasAny))]
        public class TagBits_HasAny_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                var index = codes.FindLastIndex(c => c.opcode == OpCodes.Ldfld);

                if (index != -1)
                {
                    var tagsCompare = typeof(ExtendedTagBits).GetMethod("HasAnyExtraTags");

                    index++;
                    codes.RemoveAt(index);
                    codes.Insert(index++, new CodeInstruction(OpCodes.Call, tagsCompare));
                    codes.Insert(index, new CodeInstruction(OpCodes.Conv_I8));
                }
                else
                {
                    Debug.LogError("[Extra Tag Bits] Error patching HasAny.");
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.And))]
        public class TagBits_And_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                var index = codes.FindLastIndex(c => c.opcode == OpCodes.Ldfld);
                if (index != -1)
                {
                    var extTagsAnd = typeof(ExtendedTagBits).GetMethod("AndAllTagBits");
                    ++index;
                    codes[index] = new CodeInstruction(OpCodes.Call, extTagsAnd);
                    codes[index + 1] = new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    Debug.LogError("[Extra Tag Bits] Error patching And.");
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.Or))]
        public class TagBit_Or_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                var index = codes.FindLastIndex(c => c.opcode == OpCodes.Ldfld);
                if (index != -1)
                {
                    var extTagsOr = AccessTools.Method(typeof(ExtendedTagBits), "OrAllTagBits");
                    ++index;
                    codes[index] = new CodeInstruction(OpCodes.Call, extTagsOr);
                    codes[index + 1] = new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    Debug.LogError("[Extra Tag Bits] Error patching Or.");
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.Xor))]
        public class TagBit_Xor_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
            {
                var codes = origCode.ToList();
                var index = codes.FindLastIndex(c => c.opcode == OpCodes.Ldfld);
                if (index != -1)
                {
                    var extTagsXor = AccessTools.Method(typeof(ExtendedTagBits), "XorAllTagBits");
                    ++index;
                    codes[index] = new CodeInstruction(OpCodes.Call, extTagsXor);
                    codes[index + 1] = new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    Debug.LogError("[Extra Tag Bits] Error patching Xor.");
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TagBits), nameof(TagBits.MakeComplement))]
        public class TagBits_MakeCompliment_Patch
        {
            public static bool Prefix(ref TagBits rhs, ref TagBits __result)
            {
                __result = rhs;
                return true;
            }
        }

        [HarmonyPatch(typeof(KPrefabID), nameof(KPrefabID.AndTagBits))]
        public class KPrefabId_AndTagBits_Patch
        {
            public static bool Prefix(KPrefabID __instance, ref TagBits rhs)
            {
                __instance.UpdateTagBits();
                var tb = (TagBits) Traverse.Create(__instance).Field("tagBits").GetValue();
                rhs.MaskTagBits(ref tb);
                return true;
            }
        }

        // Super TODO: Unit tests?
    }

    public class Map<T1, T2>
    {
        private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Map()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4>
        {
            private readonly Dictionary<T3, T4> _dictionary;

            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            public int Count => _dictionary.Count;

            public T4 this[T3 index]
            {
                get => _dictionary[index];
                set => _dictionary[index] = value;
            }

            public bool Contains(T3 t)
            {
                return _dictionary.ContainsKey(t);
            }

            public bool ContainsAll(IEnumerable<T3> ts)
            {
                return _dictionary.All(e => ts.Contains(e.Key));
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public Indexer<T1, T2> Forward { get; }
        public Indexer<T2, T1> Reverse { get; }
    }

    public class ExtendedTagBits
    {
        public static ulong _id = 1;

        public static ulong Id
        {
            get
            {
                var tmp = _id;
                Debug.Log($"Making id {tmp}");
                ++_id;
                return tmp;
            }
        }

        public static ulong GetIdIfExists(Tag tag)
        {
            if (ExtraTagMap.Forward.Contains(tag))
            {
                if (ExtraTagList.Count == 0)
                    return Id;

                return ExtraTagList.First(l => l.Value.Count == 1 && l.Value[0] == ExtraTagMap.Forward[tag]).Key;
            }

            return Id;
        }

        public static ulong GetIdIfExists(IEnumerable<Tag> tags)
        {
            if (tags == null) tags = new List<Tag>();
            var enumerable = tags.ToList();
            if (ExtraTagMap.Forward.ContainsAll(enumerable))
            {
                if (ExtraTagList.Count == 0) return Id;

                return ExtraTagList.First(l => l.Value.All(e => enumerable.Contains(ExtraTagMap.Reverse[e]))).Key;
            }

            return Id;
        }

        public static readonly Dictionary<ulong, List<int>> ExtraTagList = new Dictionary<ulong, List<int>>();
        private static readonly Map<Tag, int> ExtraTagMap = new Map<Tag, int>();

        public static void AddExtraTagBits(ulong id, int tag)
        {
            if (!ExtraTagList.ContainsKey(id))
                ExtraTagList.Add(id, new List<int> {tag});

            if (!ExtraTagList[id].Contains(tag))
                ExtraTagList[id].Add(tag);
        }

        public static void RemoveExtraTagBits(ulong id, int tag)
        {
            if (!ExtraTagList.ContainsKey(id))
                return;

            if (ExtraTagList[id].Contains(tag))
                ExtraTagList[id].Remove(tag);

            if (ExtraTagList[id].Count == 0)
                ExtraTagList.Remove(id);
        }

        public static int GetTagIndex(Tag tag)
        {
            var tagTable = Traverse.Create(typeof(TagBits)).Field("tagTable");
            var invTagTable = Traverse.Create(typeof(TagBits)).Field("inverseTagTable");
            var dict = (Dictionary<Tag, int>) tagTable.GetValue();
            var invDict = (List<Tag>) invTagTable.GetValue();
            if (dict.TryGetValue(tag, out var index))
                return index;

            if (dict.Count >= 320)
            {
                var newIndex = 320 + ExtraTagMap.Forward.Count;
                Debug.LogWarning("[Extra Tag Bits] Maximum tag bits reached, performance may be decreased.");
                ExtraTagMap.Add(tag, newIndex);
                return newIndex;
            }

            dict.Add(tag, dict.Count);
            tagTable.SetValue(dict);
            invDict.Add(tag);
            invTagTable.SetValue(invDict);
            return dict.Count;
        }

        public static void GetExtraTags(ulong id, List<Tag> tags)
        {
            if (id == 0) return;

            if (!ExtraTagList.ContainsKey(id))
                ExtraTagList.Add(id, new List<int>());

            tags.AddRange(ExtraTagList[id].Select(i => ExtraTagMap.Reverse[i]));
        }

        public static bool HasAllExtraTags(ulong id1, ulong id2)
        {
            var list1 = new List<Tag>();
            var list2 = new List<Tag>();
            GetExtraTags(id1, list1);
            GetExtraTags(id2, list2);
            return list1 == list2;
        }

        public static bool HasAnyExtraTags(ulong id1, ulong id2)
        {
            var list1 = new List<Tag>();
            var list2 = new List<Tag>();
            GetExtraTags(id1, list1);
            GetExtraTags(id2, list2);
            return list1.Intersect(list2).Any();
        }

        public static void AndAllTagBits(ulong id1, ulong id2)
        {
            ExtraTagList[id1] = ExtraTagList[id1].Intersect(ExtraTagList[id2]).ToList();
        }

        public static void OrAllTagBits(ulong id1, ulong id2)
        {
            if (!ExtraTagList.ContainsKey(id1))
            {
                ExtraTagList.Add(id1, new List<int>());
            }
            ExtraTagList[id1] = ExtraTagList[id1].Union(ExtraTagList[id2]).ToList();
        }

        public static void XorAllTagBits(ulong id1, ulong id2)
        {
            var union = ExtraTagList[id1].Union(ExtraTagList[id2]).ToList();
            var intersect = ExtraTagList[id1].Intersect(ExtraTagList[id2]).ToList();
            ExtraTagList[id1] = union.Except(intersect).ToList();
        }
    }

    public static class TagBitsExtensions
    {
        public static void MaskTagBits(this ref TagBits tb1, ref TagBits tb2)
        {
            var id1 = (ulong) Traverse.Create(tb1).Field("bits5").GetValue();
            var id2 = (ulong) Traverse.Create(tb2).Field("bits5").GetValue();
            tb2.Complement();
            NoIdAnd(ref tb1, ref tb2);

            if (!ExtendedTagBits.ExtraTagList.ContainsKey(id1)) ExtendedTagBits.ExtraTagList.Add(id1, new List<int>());
            if (!ExtendedTagBits.ExtraTagList.ContainsKey(id2)) ExtendedTagBits.ExtraTagList.Add(id2, new List<int>());
            ExtendedTagBits.ExtraTagList[id1] =
                ExtendedTagBits.ExtraTagList[id1].Except(ExtendedTagBits.ExtraTagList[id2]).ToList();
        }

        private static void NoIdAnd(ref TagBits tb1, ref TagBits tb2)
        {
            for (var i = 0; i < 5; ++i)
            {
                var t1 = Traverse.Create(tb1).Field($"bits{i}");
                t1.SetValue((ulong) t1.GetValue() & (ulong) Traverse.Create(tb2).Field($"bits{i}").GetValue());
            }
        }
    }
}
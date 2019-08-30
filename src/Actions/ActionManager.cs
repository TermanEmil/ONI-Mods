using System.Collections.Generic;
using System.Diagnostics;
using Harmony;

namespace Actions
{
   /* [HarmonyPatch(typeof(KButtonEvent), MethodType.Constructor, typeof(KInputController), typeof(InputEventType),
        typeof(bool[]))]
    public class KButtonEventCtorPatch
    {
        public static void Postfix(bool[] is_action)
        {
            new KButtonEventManager(is_action);
        }
    }*/

   [HarmonyPatch(typeof(KScreen), nameof(KScreen.OnKeyDown))]
   public class logging
   {
       public static void Postfix()
       {
           var stack = new StackTrace();
           Debug.Log("Stack Frames for OnKeyDown");
           foreach (var frame in stack.GetFrames())
           {
               Debug.Log($"Stack frame: {frame.GetMethod().FullDescription()}");
           }
       }
   }

   public class ActionHandler
   {
        public static List<int> Actions = new List<int>();

        public static int RegisterAction()
        {
            return 0;
        }
   }

    [HarmonyPatch(typeof(KButtonEvent), nameof(KButtonEvent.TryConsume))]
    public class KButonEventTryConsumePatch
    {

        public static bool Prefix(KButtonEvent __instance, Action action, ref bool __result)
        {
            if (__instance.Consumed)
                Debug.LogError(action.ToString() + " was already consumed");

            if ((int) action >= (int) Action.NumActions)
            {
                if (ActionHandler.Actions.Contains((int) action))
                {
                    __instance.Consumed = true;
                }
            }
            else if (action < Action.NumActions && ((bool[]) Traverse.Create(__instance).Field("mIsAction").GetValue())[(int) action]) __instance.Consumed = true;

            __result = __instance.Consumed;
            return false;
        }
    }
}
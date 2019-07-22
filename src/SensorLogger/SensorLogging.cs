using System;
using System.IO;
using Harmony;
using UnityEngine;

namespace SensorLogging
{
    public class SensorLogging
    {
        static StreamWriter SensorsFile;

        static GameClock gameClockInstance = null;

        static bool Shouldlog = false;
        static bool CheckShouldLog()
        {
            return false;
        }

        class FilePatch
        {
            [HarmonyPatch(typeof(PauseScreen), "OnKeyDown")]
            class KeyChecker
            {
                static bool Prefix(ref KButtonEvent e)
                {
                    if (e.IsAction(Action.BuildMenuKeyL))
                    {
                        Shouldlog = !Shouldlog;
                        Debug.Log(Shouldlog ? "Began Logging" : "Stopped Logging");
                    }
                    return true;
                }
            }

            [HarmonyPatch(typeof(Game), "OnPrefabInit")]
            class FileStartup
            {
                static void Prefix()
                {
                    FileStream TFile = new FileStream("sensors.csv", FileMode.Append);
                    SensorsFile = new StreamWriter(TFile);
                    SensorsFile.WriteLine("~~Initialized session at " + System.DateTime.UtcNow + "~~");
                    SensorsFile.WriteLine("\"Time (Cycles)\",Type,Position,Value");
                    SensorsFile.Flush();
                }
            }

            [HarmonyPatch(typeof(PauseScreen), "OnQuitConfirm")]
            class FileQuit
            {
                static void Prefix()
                {
                    SensorsFile.Flush();
                    SensorsFile.Close();
                }
            }
        }

        [HarmonyPatch(typeof(SimAndRenderScheduler.Sim200msUpdater), "Update")]
        public class UpdateFlush
        {
            static void Postfix()
            {
                SensorsFile.Flush();
            }
        }

        [HarmonyPatch(typeof(GameClock), "OnPrefabInit")]
        public class GameClockSetup
        {
            static void Postfix()
            {
                gameClockInstance = GameClock.Instance;
            }
        }

        [HarmonyPatch(typeof(LogicTemperatureSensor), "Sim200ms")]
        public class TemperatureOutput
        {
            static void Prefix(ref int ___simUpdateCounter, ref int __state)
            {
                __state = ___simUpdateCounter;
            }
            static void Postfix(LogicTemperatureSensor __instance, ref int __state)
            {
                if (Shouldlog)
                {
                    if (__state >= 8)
                    {
                        float avgTemp = __instance.GetTemperature();
                        Vector3 position = __instance.transform.GetPosition();
                        string posstring = "(" + position.x + "," + position.y + ")";
                        SensorsFile.WriteLine((gameClockInstance.GetCycle() + gameClockInstance.GetCurrentCycleAsPercentage()).ToString("f4") + ",Temp,\"" + posstring + "\"," + avgTemp.ToString("f2"));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LogicCritterCountSensor), "Sim200ms")]
        public class CritterCountOutput
        {
            static void Postfix(LogicTemperatureSensor __instance, ref bool ___countEggs)
            {
                if (Shouldlog)
                {
                    Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(__instance.gameObject);
                    int num = 0;
                    if (roomOfGameObject != null)
                    {
                        num = roomOfGameObject.cavity.creatures.Count;
                        if (___countEggs)
                        {
                            num += roomOfGameObject.cavity.eggs.Count;
                        }
                    }
                    Vector3 position = __instance.transform.GetPosition();
                    string posstring = "(" + position.x + "," + position.y + ")";
                    SensorsFile.WriteLine((gameClockInstance.GetCycle() + gameClockInstance.GetCurrentCycleAsPercentage()).ToString("f4") + ",Critter,\"" + posstring + "\"," + num);
                }
            }
        }

        [HarmonyPatch(typeof(LogicDiseaseSensor), "Sim200ms")]
        public class DiseaseOutput
        {
            static void Postfix(LogicDiseaseSensor __instance)
            {
                if (Shouldlog)
                {
                    Vector3 position = __instance.transform.GetPosition();
                    string posstring = "(" + position.x + "," + position.y + ")";
                    SensorsFile.WriteLine((gameClockInstance.GetCycle() + gameClockInstance.GetCurrentCycleAsPercentage()).ToString("f4") + ",Disease,\"" + posstring + "\"," + __instance.CurrentValue);
                }
            }
        }

        [HarmonyPatch(typeof(LogicElementSensor), "Sim200ms")]
        public class ElementOutput
        {
            static void Prefix(ref int ___sampleIdx, ref int __state)
            {
                __state = ___sampleIdx;
            }

            static void Postfix(LogicElementSensor __instance, ref int __state, ref bool[] ___samples)
            {
                if (Shouldlog)
                {
                    if (__state >= 8)
                    {
                        bool flag = false;
                        foreach (bool s in ___samples)
                        {
                            flag = (s && flag);
                        }
                        Vector3 position = __instance.transform.GetPosition();
                        string posstring = "(" + position.x + "," + position.y + ")";
                        SensorsFile.WriteLine((gameClockInstance.GetCycle() + gameClockInstance.GetCurrentCycleAsPercentage()).ToString("f4") + ",Element,\"" + posstring + "\"," + (flag ? "Active" : "Inactive"));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LogicPressureSensor), "Sim200ms")]
        public class PressureOutput
        {
            static void Prefix(ref int ___sampleIdx, ref int __state)
            {
                __state = ___sampleIdx;
            }
            static void Postfix(LogicPressureSensor __instance, ref int __state)
            {
                if (Shouldlog)
                {
                    if (__state >= 8)
                    {
                        Vector3 position = __instance.transform.GetPosition();
                        string posstring = "(" + position.x + "," + position.y + ")";
                        SensorsFile.WriteLine((gameClockInstance.GetCycle() + gameClockInstance.GetCurrentCycleAsPercentage()).ToString("f4") + ",Pressure" + ((__instance.desiredState == Element.State.Gas) ? "Gas" : "Liquid") + ",\"" + posstring + "\"," + (__instance.CurrentValue).ToString("f2"));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LogicTimeOfDaySensor), "Sim200ms")]
        public class TimeOfDayOutput
        {
            static void Postfix(LogicTimeOfDaySensor __instance)
            {
                if (Shouldlog)
                {
                    bool state = false;
                    float currentCycleAsPercentage = gameClockInstance.GetCurrentCycleAsPercentage();
                    if (currentCycleAsPercentage >= __instance.startTime && currentCycleAsPercentage < __instance.startTime + __instance.duration)
                    {
                        state = true;
                    }
                    if (currentCycleAsPercentage < __instance.startTime + __instance.duration - 1f)
                    {
                        state = true;
                    }
                    Vector3 position = __instance.transform.GetPosition();
                    string posstring = "(" + position.x + "," + position.y + ")";
                    SensorsFile.WriteLine((gameClockInstance.GetCycle() + currentCycleAsPercentage).ToString("f4") + ",TimeOfDay,\"" + posstring + "\"," + state);
                }
            }
        }

        [HarmonyPatch(typeof(LogicMassSensor), "Update")]
        public class MassOutput
        {
            static bool DoCheck = false;
            static float oldTime = 0;
            static float newTime = 0;
            static int oldFrame = 0;
            static int newFrame = 0;
            static void Prefix()
            {
                if (!SpeedControlScreen.Instance.IsPaused) {
                    newTime = Time.time;
                    newFrame = Time.frameCount;
                    float delta = newTime - oldTime;
                    int fDelta = newFrame - oldFrame;
                    if (fDelta > 0)
                    {
                        DoCheck = delta >= 0.2 ? true : false;
                        if (DoCheck)
                        {
                            oldTime = newTime;
                            oldFrame = newFrame;
                        }
                    }
                }
            }
            static void Postfix(LogicMassSensor __instance)
            {
                if (Shouldlog && !SpeedControlScreen.Instance.IsPaused && DoCheck)
                {
                    Vector3 position = __instance.transform.GetPosition();
                    string posstring = "(" + position.x + "," + position.y + ")";
                    SensorsFile.WriteLine((gameClockInstance.GetCycle() + gameClockInstance.GetCurrentCycleAsPercentage()).ToString("f4") + ",Mass,\"" + posstring + "\"," + (__instance.CurrentValue*1000).ToString("f2"));
                }
            }
        }
    }
}

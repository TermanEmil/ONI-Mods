using System.Collections.Generic;

namespace ShinebugReactor
{
    public static class ShinebugStats
    {
        public class ShinebugEggData
        {
            public float TimeToHatch;
            public float AdultLife;
            public float AdultLux;

            public override string ToString()
            {
                return $"Egg has {TimeToHatch}s egg time and makes {AdultLux} Lux for {AdultLife}s";
            }
        }

        private static readonly IReadOnlyDictionary<string, ShinebugEggData> Stats =
            new Dictionary<string, ShinebugEggData>
            {
                {
                    "LightBugBlackEgg",
                    new ShinebugEggData
                    {
                        TimeToHatch = 9000f,
                        AdultLife = 45000f,
                        AdultLux = 0f
                    }
                },
                {
                    "LightBugBlueEgg",
                    new ShinebugEggData
                    {
                        TimeToHatch = 3000f,
                        AdultLife = 15000f,
                        AdultLux = 1800f
                    }
                },
                {
                    "LightBugEgg",
                    new ShinebugEggData
                    {
#if DEBUG
                        TimeToHatch = 15f,
                        AdultLife = 45f,
#else
                        TimeToHatch = 3000f,
                        AdultLife = 15000f,
#endif
                        AdultLux = 1800f
                    }
                },
                {
                    "LightBugCrystalEgg",
                    new ShinebugEggData
                    {
                        TimeToHatch = 9000f,
                        AdultLife = 45000f,
                        AdultLux = 1800f
                    }
                },
                {
                    "LightBugOrangeEgg",
                    new ShinebugEggData
                    {
                        TimeToHatch = 3000f,
                        AdultLife = 15000f,
                        AdultLux = 1800
                    }
                },
                {
                    "LightBugPinkEgg",
                    new ShinebugEggData
                    {
                        TimeToHatch = 3000f,
                        AdultLife = 15000f,
                        AdultLux = 1800f
                    }
                },
                {
                    "LightBugPurpleEgg",
                    new ShinebugEggData
                    {
                        TimeToHatch = 3000f,
                        AdultLife = 15000f,
                        AdultLux = 1800f
                    }
                }
            };

        public static ShinebugEggData Get(string name)
        {
            return Stats[name];
        }
    }
}

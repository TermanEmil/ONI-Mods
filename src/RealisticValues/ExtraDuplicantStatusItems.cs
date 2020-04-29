namespace RealisticValues {
    public static class ExtraDuplicantStatusItems
    {
        public static StatusItem EmittingO2Status;
        public const  string     EmittingO2Id = "asquared31415." + nameof(EmittingO2Status);

        public static void SetupStatuses()
        {
            EmittingO2Status = new StatusItem(
                EmittingO2Id,
                "DUPLICANTS",
                "",
                StatusItem.IconType.Info,
                NotificationType.Neutral,
                false,
                OverlayModes.None.ID,
                true,
                // Show in Decor Overlay and no overlay
                130
            );

            EmittingO2Status.resolveStringCallback = delegate(string str, object data)
            {
                var oxygenBreather = data as OxygenBreather;
                if(oxygenBreather == null)
                    return str;

                var o2Rate = Game.Instance.accumulators.GetAverageRate(oxygenBreather.O2Accumulator);
                var co2Rate = oxygenBreather.CO2EmitRate;
                return str.Replace(
                    "{EmittingRate}",
                    GameUtil.GetFormattedMass(o2Rate - co2Rate, GameUtil.TimeSlice.PerSecond)
                );
            };

            Db.Get().DuplicantStatusItems.Add(EmittingO2Status);
        }
    }
}

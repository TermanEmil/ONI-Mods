namespace Diode
{
    internal class Diode : Generator
    {
        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);
            operational.SetFlag(wireConnectedFlag, CircuitID != ushort.MaxValue);
            if (!operational.IsOperational)
                return;
            var wattsUsed = GetComponent<IEnergyConsumer>().WattsUsed;
            GenerateJoules(wattsUsed);
        }
    }
}
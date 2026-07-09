using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.RPSX.DarkForces.Saint.Items.Cross.Events;

public sealed class SaintedCrossFindingEvent : HandledEntityEventArgs
{
    public SaintedCrossColorize? Colorize = null;
    public SaintedCrossMessage? Message = null;
    public EntityUid Cross;

    public SaintedCrossFindingEvent(EntityUid cross)
    {
        Cross = cross;
    }

    public record struct SaintedCrossColorize(Color Color, int Energy, int Radius);

    public record struct SaintedCrossMessage(string Message);
}

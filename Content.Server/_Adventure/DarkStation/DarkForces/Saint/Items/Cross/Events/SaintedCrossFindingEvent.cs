﻿namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Saint.Items.Cross.Events;

public sealed class SaintedCrossFindingEvent : HandledEntityEventArgs
{
    public SaintedCrossColorize? Colorize = null;
    public EntityUid Cross;
    public SaintedCrossMessage? Message = null;

    public SaintedCrossFindingEvent(EntityUid cross)
    {
        Cross = cross;
    }

    public record struct SaintedCrossColorize(Color Color, int Energy, int Radius);

    public record struct SaintedCrossMessage(string Message);
}

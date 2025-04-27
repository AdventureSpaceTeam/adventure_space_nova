using Content.Shared.AdventureSpace.Cult.Runes;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Runes.Components;

[RegisterComponent]
public sealed partial class NarsiRuneComponent : SharedNarsiRuneComponent
{
    [DataField("runeState")]
    [ViewVariables(VVAccess.ReadWrite)]
    public NarsiRuneState RuneState = NarsiRuneState.Idle;
}

public enum NarsiRuneState
{
    Idle,
    InUse,
}

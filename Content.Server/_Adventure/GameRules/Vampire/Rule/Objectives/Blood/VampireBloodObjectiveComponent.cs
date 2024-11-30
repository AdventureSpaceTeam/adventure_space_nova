﻿namespace Content.Server.AdventurePrivate._Alteros.GameRules.Vampire.Rule.Objectives.Blood;

[RegisterComponent]
public sealed partial class VampireBloodObjectiveComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public float RequiredBloodCount = 4000f;
}

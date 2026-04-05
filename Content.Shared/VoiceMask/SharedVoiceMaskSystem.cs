using Robust.Shared.Serialization;

namespace Content.Shared.VoiceMask;

[Serializable, NetSerializable]
public enum VoiceMaskUIKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class VoiceMaskBuiState : BoundUserInterfaceState
{
    public readonly string Name;
    public readonly string? Verb;
    public readonly string Voice; // c4llv07e tts
    public readonly bool Active;
    public readonly bool AccentHide;

    public VoiceMaskBuiState(string name, string voice, string? verb, bool active, bool accentHide) // c4llv07e tts
    {
        Name = name;
        Voice = voice; // c4llv07e tts
        Verb = verb;
        Active = active;
        AccentHide = accentHide;
    }
}

[Serializable, NetSerializable]
public sealed class VoiceMaskChangeNameMessage : BoundUserInterfaceMessage
{
    public readonly string Name;

    public VoiceMaskChangeNameMessage(string name)
    {
        Name = name;
    }
}

/// <summary>
/// Change the speech verb prototype to override, or null to use the user's verb.
/// </summary>
[Serializable, NetSerializable]
public sealed class VoiceMaskChangeVerbMessage : BoundUserInterfaceMessage
{
    public readonly string? Verb;

    public VoiceMaskChangeVerbMessage(string? verb)
    {
        Verb = verb;
    }
}

/// <summary>
///     Toggle the effects of the voice mask.
/// </summary>
[Serializable, NetSerializable]
public sealed class VoiceMaskToggleMessage : BoundUserInterfaceMessage;

/// <summary>
///     Toggle the effects of accent negation.
/// </summary>
[Serializable, NetSerializable]
public sealed class VoiceMaskAccentToggleMessage : BoundUserInterfaceMessage;

using Content.Server.Chat.Managers;
using Content.Shared.Chat;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Client._Adventure.Chat.Boobs;

public sealed partial class BoobsChatSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chat = default!;

    [ViewVariables(VVAccess.ReadWrite)] public bool IsEnabled = true;
    [ViewVariables(VVAccess.ReadWrite)] public TimeSpan NextTime = default!;
    [ViewVariables(VVAccess.ReadWrite)] public string SendedText = "Boobs";
    [ViewVariables(VVAccess.ReadWrite)] public float MinNextTime = 10 * 60; // 10 min
    [ViewVariables(VVAccess.ReadWrite)] public float MaxNextTime = 15 * 60; // 15 min

    public override void Initialize()
    {
        base.Initialize();
        UpdateNextTime();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_timing.CurTime < NextTime) return;
        _chat.ChatMessageToAll(ChatChannel.Server, SendedText, SendedText, EntityUid.Invalid, false, true);
        UpdateNextTime();
    }

    public void UpdateNextTime()
    {
        NextTime = _timing.CurTime + TimeSpan.FromSeconds(_random.NextFloat(MinNextTime, MaxNextTime));
    }
}

using Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Tools;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Events;

public sealed class SurgeryToolAppliedEvent(SurgeryToolUsage toolUsage) : HandledEntityEventArgs
{
    public SurgeryToolUsage ToolUsage = toolUsage;
}

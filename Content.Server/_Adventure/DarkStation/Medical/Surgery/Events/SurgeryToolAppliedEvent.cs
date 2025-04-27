using Content.Server.AdventureSpace.Medical.Surgery.Tools;

namespace Content.Server.AdventureSpace.Medical.Surgery.Events;

public sealed class SurgeryToolAppliedEvent(SurgeryToolUsage toolUsage) : HandledEntityEventArgs
{
    public SurgeryToolUsage ToolUsage = toolUsage;
}

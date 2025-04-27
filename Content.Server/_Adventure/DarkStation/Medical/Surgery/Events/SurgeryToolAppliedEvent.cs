using Content.Server._Adventure.Medical.Surgery.Tools;

namespace Content.Server._Adventure.Medical.Surgery.Events;

public sealed class SurgeryToolAppliedEvent(SurgeryToolUsage toolUsage) : HandledEntityEventArgs
{
    public SurgeryToolUsage ToolUsage = toolUsage;
}

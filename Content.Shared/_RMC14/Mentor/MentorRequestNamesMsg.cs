using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mentor;

public sealed class MentorRequestNamesMsg : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Core;

    public Guid To;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        To = buffer.ReadGuid();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(To);
    }
}

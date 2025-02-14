using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mentor;

public sealed class MentorGotNamesMsg : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Core;

    public Dictionary<NetUserId, string> Names = new();

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var count = buffer.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var player = new NetUserId(new Guid(buffer.ReadString()));
            var name = buffer.ReadString();
            Names[player] = name;
        }
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Names.Count);
        foreach (var name in Names)
        {
            buffer.Write(name.Key.ToString());
            buffer.Write(name.Value);
        }
    }
}


namespace Content.Server._RMC14.Mentor;

public sealed partial class BwoinkSystem : EntitySystem
{
    [Dependency] private readonly MentorManager _mentor = default!;
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        _mentor.Update();
    }
}

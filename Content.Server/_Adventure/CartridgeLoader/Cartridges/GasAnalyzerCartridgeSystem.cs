using Content.Shared.Atmos.Components;
using Content.Server.CartridgeLoader.Cartridges;
using Content.Server.CartridgeLoader;
using Content.Shared.CartridgeLoader;

namespace Content.Server._Adventure.CartridgeLoader.Cartridges;

public sealed class GasAnalyzerCartridgeSystem : EntitySystem
{
    [Dependency] private readonly CartridgeLoaderSystem _cartridgeLoaderSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GasAnalyzerCartridgeComponent, CartridgeAddedEvent>(OnCartridgeAdded);
        SubscribeLocalEvent<GasAnalyzerCartridgeComponent, CartridgeRemovedEvent>(OnCartridgeRemoved);
    }

    private void OnCartridgeAdded(Entity<GasAnalyzerCartridgeComponent> ent, ref CartridgeAddedEvent args)
    {
        var gasAnalyzer = EnsureComp<GasAnalyzerComponent>(args.Loader);
    }

    private void OnCartridgeRemoved(Entity<GasAnalyzerCartridgeComponent> ent, ref CartridgeRemovedEvent args)
    {
        // only remove when the program itself is removed
        if (!_cartridgeLoaderSystem.HasProgram<GasAnalyzerCartridgeComponent>(args.Loader))
        {
            RemComp<GasAnalyzerComponent>(args.Loader);
        }
    }
}

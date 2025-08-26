using Content.Server.Botany.Components;
using Content.Server.CartridgeLoader;
using Content.Shared.CartridgeLoader;

namespace Content.Server._Adventure.CartridgeLoader.Cartridges;

public sealed class AgroScanCartridgeSystem : EntitySystem
{
    [Dependency] private readonly CartridgeLoaderSystem _cartridgeLoaderSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AgroScanCartridgeComponent, CartridgeAddedEvent>(OnCartridgeAdded);
        SubscribeLocalEvent<AgroScanCartridgeComponent, CartridgeRemovedEvent>(OnCartridgeRemoved);
    }

    private void OnCartridgeAdded(Entity<AgroScanCartridgeComponent> ent, ref CartridgeAddedEvent args)
    {
        var plantanalyzer = EnsureComp<PlantAnalyzerComponent>(args.Loader);
    }

    private void OnCartridgeRemoved(Entity<AgroScanCartridgeComponent> ent, ref CartridgeRemovedEvent args)
    {
        // only remove when the program itself is removed
        if (!_cartridgeLoaderSystem.HasProgram<AgroScanCartridgeComponent>(args.Loader))
        {
            RemComp<PlantAnalyzerComponent>(args.Loader);
        }
    }
}

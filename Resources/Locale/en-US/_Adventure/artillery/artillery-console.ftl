artillery-console-title = Artillery Console
artillery-console-beacons = Beacons
artillery-console-fire = FIRE!
artillery-console-fire-controls = Fire Controls
artillery-console-ready = Ready to fire
artillery-console-cooldown = Reload: {$time}
artillery-console-no-cannon = Cannon not found
artillery-console-no-beacons = No beacons found
artillery-console-warning = LETHAL WEAPON
artillery-console-toggle = POWER
artillery-console-toggle-on = POWER: ON
artillery-console-toggle-off = POWER: OFF

artillery-fire-announcement = BLUESPACE ARTILLERY STRIKE DETECTED! PROJECTILE INBOUND! BRACE FOR IMPACT!
artillery-announcement-sender = Central Command
# Construction stages
ent-BaseBluespaceArtilleryFrameEast = artillery construction frame
ent-BaseBluespaceArtilleryFrameWest = { ent-BaseBluespaceArtilleryFrameEast }
ent-ArtilleryConsoleFrame = artillery console frame

# Recipes
construction-recipe-bluespace-artillery = Bluespace Artillery
construction-recipe-bluespace-artillery-west = Bluespace Artillery (Mirror)
construction-recipe-bluespace-artillery-desc = High-tech artillery using bluespace technologies. A fearsome weapon.
construction-recipe-bluespace-artillery-west-desc = High-tech artillery using bluespace technologies. A fearsome weapon, facing west.
construction-recipe-bluespace-artillery-console = Bluespace Artillery console
construction-recipe-bluespace-artillery-console-desc = A console for bluespace artillery

# Circuit board
ent-ArtilleryComputerCircuitboard = artillery console circuit board
    .desc = A circuit board of navigation system for bluespace artillery.
construction-graph-component-artillery-console-circuit-board = artillery console board

# Artillery
ent-BluespaceArtillery = Bluespace Artillery
    .desc = An high-tech artillery that use new bluespace technologies! DO NOT TOUCH OR YOU WILL BE TERMINATED.
ent-BluespaceArtilleryMirror = { ent-BluespaceArtillery }
    .desc = { ent-BluespaceArtillery.desc }

# Console
ent-ArtilleryConsole = artillery control console
    .desc = A control console for firing an artillery cannon.

# Explosion
ent-BSAExplosion = Bluespace artillery explosion

bluespace-artillery-permission-denied = Access denied!

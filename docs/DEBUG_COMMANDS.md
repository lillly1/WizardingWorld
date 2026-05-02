# Debug Commands

The `/wwdebug` chat command reports runtime state for developer diagnostics.

## Usage

```
/wwdebug              # summary (default)
/wwdebug summary      # world + player overview
/wwdebug hallows      # Deathly Hallows unlock state
/wwdebug horcruxes    # Horcrux Hunt progress + Voldemort scaling
/wwdebug mastery      # Wand Mastery XP per wand
/wwdebug bosses       # All 12 boss defeat flags + vanilla gates
/wwdebug biome        # Biome/event flags + Dementor spawn eligibility
```

## What Each Subcommand Reports

### summary
Bosses defeated count, Horcrux count, Voldemort power %, Hallows flags, player house set, despair level, Patronus state.

### hallows
World flags: cloak claimed, stone awakened, hallows attuned. Player flags: each Hallow equipped state. Dumbledore guidance text for current progression.

### horcruxes
Individual Horcrux destruction flags, Nagini defeated, total destroyed, Voldemort power multiplier, preparation score.

### mastery
Per-wand XP and mastery level (New/Familiar/Attuned/Mastered) for all wands the player has used.

### bosses
All 12 mod boss flags plus vanilla progression gates (Golem, Cultist, Moon Lord, Hardmode).

### biome
Current biome checks (Forbidden Forest, Dungeon), event states (Blood Moon, Death Eater Invasion, Azkaban Despair), Dementor spawn eligibility calculation, despair meter.

## Implementation

Source: `Common/Systems/WizardDebugCommand.cs`
Type: ModCommand (chat command, available in-game)

# Debug Commands

The `/wwdebug` chat command reports runtime state for developer diagnostics.

`/wwdebug` is compiled only in Debug builds. Public Release builds omit the command entirely.

## Usage

```
/wwdebug              # summary (default)
/wwdebug summary      # world + player overview
/wwdebug early        # early progression route, crafting gates, shop gates
/wwdebug mid          # mid progression route, Hardmode/mech gates, Aragog/Fluffy/Horntail checks
/wwdebug posthorntail # post-Horntail unlock surface: Triwizard, St Mungo's, forest, shops
/wwdebug umbridge     # Umbridge route: gate, summon item, crafting, compass/almanac handoff
/wwdebug fenrir       # Fenrir route: Umbridge gate, Blood Moon, summon item, compass/almanac handoff
/wwdebug bellatrix    # Bellatrix route: Fenrir + Plantera gate, summon item, compass/almanac handoff
/wwdebug barty        # Barty route: Bellatrix + Plantera gate, summon item, compass/almanac handoff
/wwdebug dementor     # Dementor King route: Barty + Golem + night gate, summon item, compass/almanac handoff
/wwdebug voldemort    # Voldemort route: Horcrux/Battle/Cultist/night readiness, Dark Mark, compass/almanac handoff
/wwdebug kit intro    # disposable single-player QA kit for intro checks
/wwdebug kit troll    # disposable single-player QA kit for Troll summon checks
/wwdebug kit quirrell # disposable single-player QA kit for Quirrell summon checks
/wwdebug kit basilisk # disposable single-player QA kit for Basilisk summon checks
/wwdebug kit aragog   # disposable single-player QA kit for Aragog summon checks
/wwdebug kit fluffy   # disposable single-player QA kit for Fluffy summon checks
/wwdebug kit horntail # disposable single-player QA kit for Horntail summon checks
/wwdebug kit posthorntail # disposable single-player QA kit for post-Horntail unlock checks
/wwdebug kit umbridge # disposable single-player QA kit for Umbridge summon checks
/wwdebug kit fenrir   # disposable single-player QA kit for Fenrir summon checks
/wwdebug kit bellatrix # disposable single-player QA kit for Bellatrix summon checks
/wwdebug kit barty    # disposable single-player QA kit for Barty summon checks
/wwdebug kit dementor # disposable single-player QA kit for Dementor King summon checks
/wwdebug kit voldemort # disposable single-player QA kit for Voldemort summon checks
/wwdebug kit weapon   # grants the QA-only 600 damage test wand
/wwdebug god on       # enable current-session QA god mode
/wwdebug god off      # disable current-session QA god mode
/wwdebug vanilla eye  # mark Eye of Cthulhu defeated for QA gates
/wwdebug vanilla skeletron # mark Skeletron defeated for QA gates
/wwdebug vanilla hardmode # enable Hardmode for QA gates
/wwdebug vanilla mech # mark one mechanical boss defeated for QA gates
/wwdebug vanilla plantera # mark Plantera defeated for QA gates
/wwdebug vanilla golem # mark Golem defeated for QA gates
/wwdebug vanilla cultist # mark Lunatic Cultist defeated for QA gates
/wwdebug vanilla night # force night without Blood Moon for QA gates
/wwdebug vanilla bloodmoon # force current world to night Blood Moon for QA gates
/wwdebug bossflag troll # mark Wizarding World Troll defeated for QA recovery
/wwdebug battle win   # mark Battle of Hogwarts won and Nagini defeated for final-route QA
/wwdebug audio new    # sequential audio QA for newest spell/ambient sounds
/wwdebug audio ambient # sequential audio QA for ambient sounds
/wwdebug audio spells # sequential audio QA for spell sounds
/wwdebug audio enemies # sequential audio QA for enemy sounds
/wwdebug audio all    # sequential audio QA for all custom sounds
/wwdebug hallows      # Deathly Hallows unlock state
/wwdebug hallows claim # QA shortcut: claim true Invisibility Cloak without finding Dumbledore
/wwdebug hallows purify # QA shortcut: purify Gaunt's Ring into Resurrection Stone
/wwdebug hallows kit  # QA shortcut: grants Hallows items and minimum final-route flags
/wwdebug hallows complete # QA shortcut: grants Hallows items and marks Master of Death attuned
/wwdebug horcruxes    # Horcrux Hunt progress + Voldemort scaling
/wwdebug horcruxes core # mark all four core Horcruxes destroyed for final-route QA
/wwdebug horcruxes all # mark core Horcruxes and Nagini defeated for final-route QA
/wwdebug mastery      # Wand Mastery XP per wand
/wwdebug bosses       # All 12 boss defeat flags + vanilla gates
/wwdebug biome        # Biome/event flags + Dementor spawn eligibility
```

## What Each Subcommand Reports

### summary
Bosses defeated count, Horcrux count, Voldemort power %, Hallows flags, player house set, despair level, Patronus state.

### early
Early-route diagnostics: Hogwarts Letter readiness, starter wand/table state, summon material gates, boss progression gates, Ollivander/Hagrid move-in and shop unlock checks, plus a suggested next step.

### mid
Mid-route diagnostics: early arc completion, Hardmode, mechanical boss gates, Aragog/Fluffy/Horntail summon items and material routes, Hagrid shop gates, boss flags, plus a suggested next step.

### posthorntail
Post-Horntail unlock diagnostics: Triwizard Tournament, St Mungo's, Forbidden Forest expedition state, post-Horntail shop gates, Boss Compass/Almanac visibility, plus a suggested next step. Alias: `/wwdebug post`.

### umbridge
Umbridge-route diagnostics: Hardmode, mechanical boss, Horntail gate, Educational Decree item/materials, Hagrid shop gate, summon usability, active boss state, defeated flag, Boss Compass/Almanac handoff, plus a suggested next step.

### fenrir
Fenrir-route diagnostics: Hardmode, Umbridge gate, Blood Moon state, Bloodied Claw item/materials, Hagrid shop gate, summon usability, active boss state, defeated flag, Boss Compass/Almanac handoff, plus a suggested next step.

### bellatrix
Bellatrix-route diagnostics: Hardmode, Fenrir gate, Plantera gate, Azkaban Prisoner Tag item/materials, Hagrid shop gate, summon usability, active boss state, defeated flag, Boss Compass/Almanac handoff, plus a suggested next step.

### barty
Barty-route diagnostics: Hardmode, Bellatrix gate, Plantera gate, Suspicious Flask item/materials, Hagrid shop gate, summon usability, active boss state, defeated flag, Boss Compass/Almanac handoff, plus a suggested next step.

### dementor
Dementor King-route diagnostics: Hardmode, Barty gate, Golem gate, night state, Frozen Soul Lantern item/materials, Hagrid shop gate, summon usability, active boss state, defeated flag, Boss Compass/Almanac handoff, Azkaban event state, plus a suggested next step.

### voldemort
Voldemort-route diagnostics: Dementor King handoff, core Horcrux destruction, Nagini/Battle of Hogwarts readiness, Lunatic Cultist, night state, Dark Mark boss summon item/materials, Hagrid shop gate, summon usability, active boss state, defeated flag, Boss Compass/Almanac/Horcrux Tracker handoff, plus a suggested next step. Alias: `/wwdebug final` or `/wwdebug finale`.

### kit
Grants disposable single-player QA kits for intro, Troll, Quirrell, Basilisk, Aragog, Fluffy, Horntail, post-Horntail, Umbridge, Fenrir, Bellatrix, Barty, Dementor King, Voldemort, or weapon checks. These kits do not mutate boss flags or vanilla progression flags. The Troll kit also grants first-boss combat prep for testing the fight flow: 160 max life if needed, 80 mana if needed, Iron armor in empty armor slots, Willow Wand, healing/mana/buff potions, platforms, a campfire, and current-session QA god mode. Mid kits grant the relevant summon item, recipe materials, the Enchanting Table path, and the QA-only test wand. The post-Horntail kit grants Goblet of Fire, St Mungo's Pass, Forest Lantern, Boss Compass, Wizard's Almanac, and related entry materials. The Umbridge kit adds Educational Decree and its crafting materials. The Fenrir kit adds Bloodied Claw and its crafting materials. The Bellatrix kit adds Azkaban Prisoner Tag and its crafting materials. The Barty kit adds Suspicious Flask and its crafting materials. The Dementor kit adds Frozen Soul Lantern and its crafting materials. The Voldemort kit adds the boss Dark Mark, Horcrux Tracker, core Horcruxes, Basilisk Fang, and Dark Mark crafting materials. The weapon kit grants only the QA-only 600 damage test wand.

### god
Toggles current-session QA god mode: `/wwdebug god on`, `/wwdebug god off`, `/wwdebug god toggle`, or `/wwdebug god status`. This keeps the player alive for flow validation and is not saved to the character.

### vanilla
Sets vanilla progression gates for disposable QA worlds: `/wwdebug vanilla eye`, `/wwdebug vanilla skeletron`, `/wwdebug vanilla hardmode`, `/wwdebug vanilla mech`, `/wwdebug vanilla plantera`, `/wwdebug vanilla golem`, `/wwdebug vanilla cultist`, `/wwdebug vanilla night`, `/wwdebug vanilla bloodmoon`, `/wwdebug vanilla mid`, `/wwdebug vanilla all`, or `/wwdebug vanilla status`. This opens Wizarding World summon gates that depend on vanilla bosses/events and does not mutate Wizarding World boss flags.

### bossflag
Sets Wizarding World boss flags for disposable QA recovery: `/wwdebug bossflag troll`, `/wwdebug bossflag quirrell`, `/wwdebug bossflag basilisk`, `/wwdebug bossflag aragog`, `/wwdebug bossflag fluffy`, `/wwdebug bossflag horntail`, `/wwdebug bossflag umbridge`, `/wwdebug bossflag fenrir`, `/wwdebug bossflag bellatrix`, `/wwdebug bossflag barty`, `/wwdebug bossflag dementor`, `/wwdebug bossflag voldemort`, `/wwdebug bossflag early`, `/wwdebug bossflag mid`, or `/wwdebug bossflag status`. This does not mutate vanilla progression flags.

### battle
Sets Battle of Hogwarts QA gates for disposable worlds: `/wwdebug battle unlock`, `/wwdebug battle win`, or `/wwdebug battle status`. The `win` gate marks one ward defense, one Battle of Hogwarts win, and Nagini defeated so the final-route QA can move directly to Voldemort readiness.

### audio
Queues labeled custom sound effects in sequence for in-game listening checks. Use this after entering a single-player world; each cue is printed in chat before playback.

### hallows
World flags: cloak claimed, stone awakened, hallows attuned. Player flags: each Hallow equipped state. Dumbledore guidance text for current progression, inventory/equipment checks, and a suggested next step. With arguments, supports disposable-world QA shortcuts: `/wwdebug hallows claim`, `/wwdebug hallows purify`, and `/wwdebug hallows kit`.

### horcruxes
Individual Horcrux destruction flags, Nagini defeated, total destroyed, Voldemort power multiplier, preparation score. With arguments, it can also set QA gates in disposable worlds: `/wwdebug horcruxes core` marks the four core Horcruxes destroyed; `/wwdebug horcruxes all` also marks Nagini defeated; `/wwdebug horcruxes nagini` marks only Nagini defeated.

### mastery
Per-wand XP and mastery level (New/Familiar/Attuned/Mastered) for all wands the player has used.

### bosses
All 12 mod boss flags plus vanilla progression gates (mechanical boss, Golem, Cultist, Moon Lord, Hardmode).

### biome
Current biome checks (Forbidden Forest, Dungeon), event states (Blood Moon, Death Eater Invasion, Azkaban Despair), Dementor spawn eligibility calculation, despair meter.

## Implementation

Source: `Common/Systems/WizardDebugCommand.cs`
Type: ModCommand (chat command, available in-game)

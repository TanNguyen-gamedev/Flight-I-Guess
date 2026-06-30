# Game Design Document: "Flight, I Guess"

## 1. Vision & Core Concept
**Genre:** Roguelike-Survival Space Shooter (Top-Down 2D)
**Logline:** Start as a fragile fighter in a hostile sector. Survive endless waves of enemies, harvest their scrap, and dynamically evolve your ship's hull and weapon hardpoints to become an unstoppable dreadnought.

## 2. Core Game Loop
1. **Survive & Destroy:** Fight off escalating waves of varied enemy ships and hazards (asteroids).
2. **Harvest:** Destroyed enemies drop **Scrap** (in-run currency) and occasionally **Cores** (meta-currency).
3. **Evolve (The Roguelike Element):** 
   - Spend Scrap at periodic intervals (or instantly via a level-up system) to upgrade the ship's Hull.
   - Upgrading the Hull unlocks new **Weapon Slots** (Hardpoints).
   - Discover or buy different weapon modules (Lasers, Spread Guns, Missiles) to slot into these hardpoints.
4. **Piloting Mechanics (Depth):**
   - **Newtonian Drift:** The ship carries momentum; releasing thrust causes the ship to drift, requiring active counter-thrust to maneuver effectively.
   - **Engine Boost:** A heat-managed boost system allows for rapid repositioning, penalized by overheating if used recklessly.
   - **Firing Arcs:** Hardpoints have specific rotational constraints (e.g., front-facing, broadside) requiring the player to physically orient the ship to bring weapons to bear on targets.
5. **Die & Progress:** When the hull is destroyed, the run ends. Spend **Cores** in the main menu to unlock permanent passive upgrades or new starting weapon modules.

## 3. Ship Evolution System
The player's ship is not static. It grows in size, durability, and firepower.
- **Tier 1 (Fighter):** Fast, high Manuaver, low HP, 2 Rotation Hardpoint. (Small hardpoints only)
- **Tier 2 (Corvette):** Medium speed, medium manuaver, medium HP, 1 Central rotation, 2 Side Hardpoints. (Supports Medium hardpoints)
- **Tier 3 (Cruiser):** Slow, high HP, 2 Forward, 2 Side, 1 Auto-Turret Hardpoint. (Supports Large hardpoints)
- **Visual Swap (Zero-Allocation):** The player GameObject contains all hull visuals as separate child objects. Upgrading simply disables the old visual child and enables the new one, re-binding the pre-existing `HardpointAuthoring` components to the new pure C# `ShipModel`. No `Instantiate` or `Destroy` is used.

## 3.5. Wave & Mission Structure (Phase 3 Design)
- **Fixed Timer Waves:** Waves run for a fixed duration. Surviving until the timer expires grants base scrap and opens the Shop Phase.
- **In-Wave Missions:** Waves can feature optional strategy missions (e.g., "Hunt 10 Scout Ships"). Completing the mission before the timer runs out grants a large Scrap bounty.
- **End of Wave Shop:** When the wave timer ends, the game pauses (`Time.timeScale = 0`). The player enters the Shop to spend Scrap.
- **Weapon Sizing:** Weapons and hardpoints have sizes (`Small`, `Medium`, `Large`). A Fighter cannot mount a Battleship Cannon. Players must strategically decide to buy better small weapons or save for a hull upgrade.

## 4. Architectural Impact (Humble Object Pattern)
To maintain our strict separation of logic (Core) and engine (Unity), the architecture is divided into distinct feature modules following our folder structure:

### Gathering System (`Scripts/Gathering/`)
- **`RunStateModel` (Core)**: Pure C# class tracking current Scrap, Cores, and managing the active `ScrapModel`s. It handles calculating magnet radius and collecting resources.
- **`ScrapModel` (Core)**: Pure C# data structure representing a single piece of loot (Scrap/Core) and its magnetization state.
- **`ScrapPresenter` (Unity)**: Handles the Unity physics (triggers) and PrimeTween animations (scrap flying towards the ship).
- **`ScrapPoolManager` (Unity)**: Manages object pooling for scrap to ensure zero-allocation during gameplay.
- **`ScrapConfigSO` (Unity)**: ScriptableObject defining the properties and visual configurations of dropped resources.

### Ship System (`Scripts/Ship/`)
- **`ShipModel` (Core)**: Tracks current hull stats (MaxHP, Thrust, TurnRate) and a list of active `HardpointModel`s. Manages the pure C# logic of expanding hardpoints during a hull upgrade.
- **`PlayerController` (Unity)**: Binds the `ShipModel` stats to Unity's Rigidbody2D physics and handles zero-allocation visual swapping of hull tiers.
- **`HullConfigSO` (Unity)**: ScriptableObject defining the base stats and hardpoint configurations for different hull tiers.

### Weapons System (`Scripts/Weapons/`)
- **`WeaponModel` & `HardpointModel` (Core)**: Pure C# logic for weapon firing arcs, cooldowns, and recoil. Uses interfaces like `IWeaponTrigger` and `IWeaponEmitter` to define behavior without Unity dependencies.
- **`ShipWeaponsPresenter` (Unity)**: Humble Object that connects pure C# `WeaponModel`s to Unity's `FixedUpdate` loop, inputs, and transforms.
- **`HardpointAuthoring` (Unity)**: Defines the visual attachment points and initial configurations for hardpoints on the ship prefab.
- **`WeaponConfigSO` (Unity)**: ScriptableObject defining weapon stats, economy data (Cost, Size), and converting definitions to pure C# `WeaponModel`s.

### Shop System (`Scripts/Shop/`)
- **`ShopModel` (Core)**: Pure C# logic that validates transactions (Scrap cost, Hardpoint Size, Available Slots constraints) before invoking purchase success events for the UI. Features a Brotato-style rolling mechanic for random shop items.
- **`ShopPresenter` (Unity)**: Manages the UI layer (Canvas/TMP) bridged by `ShopItemUI`, updates shop visuals, and handles the reroll mechanic. Shop UI creation is automated via the `ShopUIGenerator` Editor script.

### Waves System (`Scripts/Waves/`)
- **`WaveManager` (Core)**: Pure C# logic generating spawn coordinates and pacing, completely decoupled from Unity's `Instantiate`. Uses a fixed timer (`Tick(float dt)`) and manages `IWaveMission` strategy interfaces.
- **`IWaveMission` & Mission Implementations (Core)**: Pure C# strategy interfaces (e.g., `KillMission`, `SurviveMission`) that track specific wave objectives and progress.
- **`WavePresenter` (Unity)**: Bridges the pure C# `WaveManager` with Unity's lifecycle, injecting the player's position and handling mission UI updates.
- **`EnemyPoolManager` (Unity)**: Handles zero-allocation spawning of enemies requested by the `WaveManager`.
- **`MissionConfigSO` (Unity)**: ScriptableObject hierarchy (e.g., `KillMissionConfigSO`, `SurviveMissionConfigSO`) used to configure wave parameters and construct pure C# `IWaveMission` instances.

### Core Flow & UI (`Scripts/` & `Scripts/Core/`)
- **`GameManager` (Core)**: High-level state machine managing the transitions between Gameplay, Shop, and Game Over states. Coordinates the initialization of other models.
- **`HUD` (Unity)**: Manages the in-game Canvas/TMP overlays (Scrap count, Wave timer, Mission progress) by listening to events from the core models.

## 5. Combat & Health System Design
### Health & Damage Mechanics
The game utilizes a **Velocity-Based Damage** model where physical impacts (such as collisions with asteroids or ramming enemies) calculate damage based on the relative velocity of the colliding bodies. This makes head-on collisions highly lethal while glancing blows are easily shrugged off.

To provide a buffer for this physics-heavy system, ships utilize a hybrid **Shield & Hull System**. Shields regenerate after a brief period of avoiding damage, rewarding hit-and-run tactics. Once shields are depleted, any remaining damage permanently bleeds into the Hull. Hull damage persists for the duration of the wave to maintain tension, but is fully restored at the end of each wave (similar to *Brotato*) to preserve the fast-paced, arcade-y flow.

### Enemy Archetypes
Enemies are specifically designed to challenge the player's mastery of Newtonian drift and heat management:
*   **The "Sniper":** Forces the player to utilize their Engine Boost by locking on with a telegraphed laser before firing a devastating, high-speed projectile.
*   **The "Phalanx":** A heavily armored, slow-moving ship with an impenetrable directional front shield. Players must use Newtonian drift to slide past its defenses and strike its vulnerable rear.
*   **The "Swarmer":** Fast, fragile kamikaze ships that constantly pressure the player's position, forcing them to kite and fire backward while drifting.


## 6. Backlog / Future Mechanics Ideas
Mechanics to evaluate for future implementation to deepen the physics sandbox:
- **Interactive Asteroids:** Physical rigidbodies that can be weaponized by shooting them into enemies.
- **Gravity Wells / Black Holes:** Spatial anomalies that pull entities, allowing for slingshot maneuvers using drift.
- **The "Scrap Comet":** A fast, non-hostile loot goblin ship that crosses the arena. High risk to chase, high reward in scrap/cores.
- **Nebula Clouds / Solar Flares:** Environmental zones or wave modifiers that alter Engine Heat costs or disable shield regeneration.


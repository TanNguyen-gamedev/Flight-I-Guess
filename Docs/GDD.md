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
To maintain our strict separation of logic (Core) and engine (Unity):
- **`RunStateModel`**: Pure C# class tracking current Scrap, Wave Number, and Player XP/Level.
- **`ShipModel`**: Tracks current hull stats (MaxHP, Thrust, TurnRate) and a list of active `HardpointModel`s. Manages the pure C# logic of expanding hardpoints during a hull upgrade.
- **`ShopModel`**: Pure C# logic that validates transactions (Scrap cost, Hardpoint Size constraints) before invoking purchase success events for the UI.
- **`LootModel` / `ResourceModel`**: Pure C# logic for calculating magnet radius, pickup values, and drop rates.
- **`LootPresenter`**: Handles the Unity physics (triggers) and PrimeTween animations (scrap flying towards the ship).
- **`WaveManager`**: Pure C# logic generating spawn coordinates and pacing, completely decoupled from Unity's `Instantiate` (using our existing `PoolManager`). Will be updated to use a fixed timer (`Tick(float dt)`) and manage `IWaveMission` strategy interfaces.

## 5. Development Roadmap
- [x] **Phase 1: Economy & Loot** - Implement enemy death dropping Scrap, player magnetizing and collecting Scrap.
- [x] **Phase 2: The Wave Spawner & Piloting Depth** 
  - Dynamic off-screen enemy spawning based on player position (UniTask async loop).
  - Newtonian drift and Engine Heat/Boost system.
  - Directional firing arcs for hardpoints.
- [ ] **Phase 3: Ship Upgrades & The Shop** 
  - Implement fixed-timer waves and the `IWaveMission` system in `WaveManager`.
  - Create the `ShipModel` and `ShopModel` to handle hull upgrades, weapon sizing (`HardpointSize`), and purchasing logic.
  - Implement zero-allocation visual swapping in `ShipWeaponsPresenter` and `PlayerController`.
  - Build the Shop UI Canvas.
- [ ] **Phase 4: Meta-Progression** - Save/Load system for permanent upgrades.

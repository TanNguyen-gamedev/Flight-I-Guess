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
4. **Die & Progress:** When the hull is destroyed, the run ends. Spend **Cores** in the main menu to unlock permanent passive upgrades or new starting weapon modules.

## 3. Ship Evolution System
The player's ship is not static. It grows in size, durability, and firepower.
- **Tier 1 (Fighter):** Fast, high Manuaver, low HP, 2 Rotation Hardpoint.
- **Tier 2 (Corvette):** Medium speed, medium manuaver, medium HP, 1 Central rotation, 2 Side Hardpoints.
- **Tier 3 (Cruiser):** Slow, high HP, 2 Forward, 2 Side, 1 Auto-Turret Hardpoint.
- *Visuals:* The ship sprite/model swaps, and new hardpoint offsets are dynamically mapped.

## 4. Architectural Impact (Humble Object Pattern)
To maintain our strict separation of logic (Core) and engine (Unity):
- **`RunStateModel`**: Pure C# class tracking current Scrap, Wave Number, and Player XP/Level.
- **`ShipModel`**: Must be refactored to support *dynamic hardpoint arrays*. Instead of a fixed array in the inspector, the `ShipModel` will accept a `HullConfig` that defines available hardpoints.
- **`LootModel` / `ResourceModel`**: Pure C# logic for calculating magnet radius, pickup values, and drop rates.
- **`LootPresenter`**: Handles the Unity physics (triggers) and PrimeTween animations (scrap flying towards the ship).
- **`WaveManager`**: Pure C# logic generating spawn coordinates and pacing, completely decoupled from Unity's `Instantiate` (using our existing `PoolManager`).

## 5. Development Roadmap
- [ ] **Phase 1: Economy & Loot** - Implement enemy death dropping Scrap, player magnetizing and collecting Scrap.
- [ ] **Phase 2: The Wave Spawner** - Implement a system to spawn enemies just off-screen based on a timer/wave counter.
- [ ] **Phase 3: Ship Upgrades** - Implement the UI and logic to swap the ship's `HullConfig` mid-game, adding new weapon hardpoints dynamically.
- [ ] **Phase 4: Meta-Progression** - Save/Load system for permanent upgrades.

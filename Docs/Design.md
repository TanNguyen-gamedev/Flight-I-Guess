# Game Design & Technical Specifications

## 1. Gameplay Mechanics

### Core Loop
1. **Survive & Destroy:** Fight off escalating waves of varied enemy ships for a fixed wave duration.
2. **Harvest:** Destroyed enemies drop **Scrap** (in-run currency) and **Cores** (meta-currency).
3. **Evolve (Shop Phase):** When a wave ends, time pauses. Players spend Scrap to buy weapons or upgrade their Hull. Upgrading the Hull unlocks new Hardpoints (weapon slots).
4. **Die & Progress:** Hull destruction ends the run. Players use Cores in the main menu to unlock new starting weapons (e.g., Shotgun module). The player always starts as a Tier 1 Fighter and must grind to evolve into a Cruiser.

### Piloting Depth
*   **Newtonian Drift:** The ship carries momentum. Releasing thrust causes the ship to drift, requiring counter-thrust to maneuver effectively.
*   **Engine Boost:** A heat-managed boost system allows for rapid repositioning, but overheating applies a severe cooldown penalty.
*   **Directional Firing Arcs:** Hardpoints have specific rotational constraints (front-facing, broadside). Players must physically orient the ship to aim effectively.
*   **Velocity-Based Damage:** Physical impacts (ramming, asteroids) calculate damage based on relative velocity.

### Health System
*   **Hybrid Shield/Hull:** Shields regenerate rapidly after avoiding damage. Once depleted, damage permanently bleeds into the Hull. Hull HP is only restored at the end of a wave.

---

## 2. User Experience (UX) Architecture

*   **Flow:** Main Menu → Gameplay (Wave → Shop → Wave) → Victory (Wave 10) / Game Over.
*   **HUD Design:** Minimalist Canvas overlay. Displays Scrap count, Core count, Wave Timer, and Mission Progress. A Heat Bar visually communicates engine temperature, turning red upon overheat.
*   **Shop UI:** Brotato-inspired interface. Displays 4 random items (Weapons/Hull Upgrades) per roll. Includes a reroll mechanic. Shop UI generation is automated via custom Editor scripts.
*   **Feedback:** PrimeTween is used for smooth, non-blocking UI animations and floating text.

---

## 3. Visual Design Standards

*   **Perspective:** Top-Down 2D.
*   **Ship Visuals:** Clean, distinct silhouettes to instantly convey Hull Tier (Fighter, Corvette, Cruiser) and Enemy Archetypes (Swarmer, Sniper).
*   **Zero-Allocation Swaps:** Upgrading a hull does not instantiate new prefabs. Instead, the player GameObject contains all hull visuals as child objects; the game simply toggles them on/off based on the current tier.
*   **VFX:** Object-pooled particle systems for muzzle flashes, explosions, and engine trails.

---

## 4. Technical Architecture

### Humble Object / MVP Pattern
To maintain strict separation of logic and engine, the architecture divides features into distinct modules:
*   **Core (Pure C#):** `ShipModel`, `RunStateModel`, `WaveManager`, `ShopModel`, `CombatManager`, `IEnemyBrain`. These classes track state, math, and logic with zero `UnityEngine` references. They are highly unit-testable.
*   **Unity (Presenters):** `PlayerController`, `ScrapPresenter`, `GameplayUIPresenter`. These bridge the C# Models to Unity's `FixedUpdate`, Rigidbody physics, and Canvases.

### Systems & Patterns
*   **EventBus:** A zero-allocation (struct-based) Publish-Subscribe pattern used for decoupled communication (e.g., `GameStateChangedEvent`, `EnemyDeathEvent`).
*   **Bootstrapper (Service Locator):** Replaces standalone Singletons. Manages persistent managers (`AppManager`, `PoolManager`) across scenes.
*   **Object Pooling (`IClearablePool`):** Enemies, Projectiles, Effects, and Scrap are managed via Unity's `ObjectPool`. To handle `DontDestroyOnLoad` persistence, pools implement an `IClearablePool` interface to force active objects back into the pool upon a game restart.
*   **Asynchronous Flow:** `UniTask` replaces Coroutines for zero-allocation async delays and scene transitions.

---

## 5. Asset Management Plan

*   **Configuration via ScriptableObjects:** 
    *   `HullConfigSO`: Defines base stats (HP, Turn Rate) and hardpoint layouts.
    *   `WeaponConfigSO`: Defines weapon logic, cooldowns, and shop costs.
    *   `ScrapConfigSO`: Defines resource values and magnet radius.
*   **Folder Structure:** Assets are strictly organized by feature domains (`Scripts/Combat`, `Scripts/Ship`, `Scripts/Shop`) to maintain namespace and dependency boundaries.
*   **Prefabs:** Kept shallow. UI prefabs are generated via Editor scripts (`ShopUIGenerator.cs`) to ensure standard configurations.

---

## 6. Future Development Roadmap

### Phase 6: Meta-Progression
*   [ ] Implement a robust Save/Load system for persistent meta-currency (Cores).
*   [ ] Add a Main Menu unlock tree for new starting weapons and permanent stat buffs.

### Backlog & Expansions
*   **The "Phalanx" Enemy:** A heavily armored ship with a directional front shield, forcing players to use Newtonian drift to strike its vulnerable rear.
*   **Interactive Environments:** Physical rigidbodies (asteroids) that can be weaponized by shooting them into enemies.
*   **Spatial Anomalies:** Gravity Wells for slingshot maneuvers and Nebula Clouds that disable shield regeneration.
*   **The "Scrap Comet":** A fast, non-hostile loot goblin ship that crosses the arena. High risk to chase, high reward.
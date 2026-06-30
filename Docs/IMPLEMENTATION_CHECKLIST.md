# Implementation Checklist: "Flight, I Guess"

- [x] **Phase 1: Economy & Loot**
  - [x] Implement enemy death dropping Scrap.
  - [x] Player magnetizing and collecting Scrap.

- [x] **Phase 2: The Wave Spawner & Piloting Depth** 
  - [x] Dynamic off-screen enemy spawning based on player position (UniTask async loop).
  - [x] Newtonian drift and Engine Heat/Boost system.
  - [x] Directional firing arcs for hardpoints.

- [x] **Phase 3: Ship Upgrades & The Shop** 
  - [x] Created `ShipModel` to manage hull tiers, stats (HP, thrust, turn rate), and hardpoint validation.
  - [x] Developed `ShopModel` as a pure C# transaction manager validating scrap costs and weapon compatibility.
  - [x] Refactored `PlayerController` to bind to `ShipModel` for dynamic upgrades.
  - [x] Implemented zero-allocation visual swapping for ship hulls in `PlayerController`.
  - [x] Designed a Brotato-inspired Shop UI Canvas with 4 random items per roll and a reroll mechanic.
  - [x] Refactored HUD from UI Toolkit to Unity Canvas/TMP.
  - [x] Created `ShopUIGenerator.cs` editor tool to automate Shop UI Canvas and prefab creation.
  - [x] Updated `WeaponConfigSO` to include economy data (Cost, Size).
  - [x] Implemented fixed-timer waves and the `IWaveMission` system in `WaveManager`.

- [ ] **Phase 4: Combat & Health Systems**
  - [X] **Health & Damage Mechanics**
    - [X] Create `IDamageable` / `IKinematicBody` pure C# interfaces in the Core layer.
    - [X] Implement Velocity-Based Damage calculation for physics collisions.
    - [X] Implement Shield regeneration logic (recharges after avoiding damage).
    - [X] Implement permanent Hull damage logic (damage bleeds through broken shields).
    - [X] Implement End-of-Wave Hull healing (Brotato-style).
  - [ ] **Enemy Archetypes**
    - [ ] Implement "Sniper" enemy (telegraphed laser, high-speed projectile).
    - [ ] Implement "Phalanx" enemy (directional front shield, slow).
    - [ ] Implement "Swarmer" enemy (kamikaze/rammer, fast).

- [ ] **Phase 5: Meta-Progression** 
  - [ ] Save/Load system for permanent upgrades.

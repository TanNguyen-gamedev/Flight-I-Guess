# Flight, I Guess

## Overview
**Flight, I Guess** is a Top-Down 2D Roguelike-Survival Space Shooter. Start as a fragile fighter in a hostile sector. Survive endless waves of enemies, harvest their scrap, and dynamically evolve your ship's hull and weapon hardpoints to become an unstoppable dreadnought. 

The game emphasizes deep piloting mechanics, such as Newtonian drift and heat management, while featuring a Brotato-style end-of-wave shop for dynamic run progression.

## Setup & Installation

### System Requirements
*   **Engine:** Unity 2022.3 LTS or higher.
*   **OS:** Windows 10/11, macOS, or Linux.
*   **Dependencies:** The project relies on **UniTask** for zero-allocation async operations and **PrimeTween** for animations. These should be automatically resolved via the Unity Package Manager.

### Installation Steps
1.  **Clone the Repository:**
    ```bash
    git clone https://github.com/your-username/flight-i-guess.git
    ```
2.  **Open in Unity:**
    *   Open Unity Hub.
    *   Click **Add** and select the cloned `flight-i-guess` folder.
    *   Open the project using the recommended Unity version.
3.  **Run the Game:**
    *   In the Project window, navigate to `Assets/Scenes`.
    *   Open the `Bootstrapper` or `MainMenu` scene.
    *   Press **Play** in the editor.

## Gameplay Guide
*   **Objective:** Survive 10 consecutive waves of escalating enemy threats.
*   **Controls:**
    *   **W / S** or **Up / Down Arrows:** Forward/Backward Thrust.
    *   **A / D** or **Left / Right Arrows:** Rotate Ship.
    *   **Shift:** Engine Boost (consumes heat; avoid overheating).
    *   **Mouse Movement:** Aim weapons.
    *   **Esc:** Pause Game.
*   **Progression:** Destroyed enemies drop **Scrap** (in-run currency). Survive the wave timer to enter the Shop, where you can spend Scrap to upgrade your Hull or purchase new weapon modules. Earning **Cores** allows meta-progression across runs.

## Technical Architecture Highlights
This project is built with a strict adherence to the **Humble Object / MVP Pattern**:
*   **Pure C# Core:** Game logic, state machines, and AI brains are entirely decoupled from `UnityEngine`.
*   **Zero-Allocation:** Heavy use of Object Pooling (`IClearablePool`), `UniTask`, and struct-based `EventBus` payloads to avoid Garbage Collection spikes during gameplay.
*   **Service Locator:** Cross-scene persistence and dependency injection are handled via a robust `Bootstrapper` pattern.

For an in-depth breakdown of the architecture, mechanics, and roadmap, please see [design.md](Docs/Design.md).

## Contributing
We welcome contributions! When contributing to this repository, please follow these guidelines:
1.  **Strict Architecture:** Ensure all core logic (Models, AI Brains) remains in pure C# (no `UnityEngine` references). Use Presenters to bridge the gap.
2.  **Event-Driven UI:** UI buttons and state changes must be wired via code (e.g., `AddListener` and `EventBus`), avoiding Unity Inspector UnityEvents.
3.  **Testing:** The pure C# core is designed to be highly testable. Please include NUnit Edit Mode tests for any new logic models.


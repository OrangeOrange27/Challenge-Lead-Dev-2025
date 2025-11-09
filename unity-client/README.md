# 🧩 Project Structure & Architecture

### The Unity client is built with a modular clean architecture for scalability and maintainability.

- **Common/** — Shared systems: authentication, configs, DTOs, player data, server API, UI prefabs.
- **Core/** — Main game logic (Hub flow, Splash screen) using MVC/MVVM + state machine patterns.
- **Infra/** — Infrastructure layer (DI, asset loading, serialization, state handling).
- **Minigames/** — Minigame modules and related assets.
- **Plugins/** — External libraries (DOTween, UniTask, TextMeshPro).

Flow is fully async and state-driven. Views load via Addressables, data syncs through strongly-typed DTOs, and UI follows strict separation of concerns.

---

## ⚙️ Development Setup

### Requirements
- **Unity Version:** Developed with Unity 6000.2.6f2 (has not been tested on older versions)
- **Packages:**
    - [Cysharp UniTask](https://github.com/Cysharp/UniTask) — async operations
    - [DOTween Pro](http://dotween.demigiant.com/) — animations and transitions
    - [TextMeshPro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest/) — UI text rendering
    - [Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@latest/) — asset loading and management

### Getting Started
1. Clone the repository.
2. Open the project in **Unity 6000**.
3. Setup server. (Check backend README for instructions.)
4. Run **`Assets/Scenes/MainScene.unity`** to start the client.
5. Press **Play** — the app will initialize authentication, load configs, and open the Hub.

---

## 🧠 Key Architectural Principles
- **Async-first:** UniTask powers all async flows.
- **State-driven navigation:** View changes handled by a finite state machine.
- **Loose coupling:** Systems interact through interfaces and DI.
- **View isolation:** Each UI panel or minigame view is self-contained and disposable.
- **Config-based content:** All data (modes, rewards, etc.) is driven by JSON configs or server responses.

---

## 🧩 Notes & Future Improvements

- **Simplify State Logic:**  
  Some existing `State` implementations are overly granular. Introducing a lightweight, reusable **`SimpleViewState`** class would streamline transitions for straightforward view logic, reducing boilerplate and maintenance overhead.

- **Refactor RootHubState:**  
  The current `RootHubState` handles both results and minigames logic, leading to unnecessary complexity. Splitting it into **`HubResultsState`** and **`HubMinigamesState`** will improve clarity, separation of concerns, and readability.

- **Backend CDN Configuration:**  
  Adding **CDN-based configuration** for minigames and game modes (fetched dynamically from the backend) would enable remote content updates without requiring client rebuilds, improving scalability and operational flexibility.
- **Enhanced Logging:**

  Implement a more robust error handling and logging system to capture runtime issues and improve debugging capabilities.
TMC — Unity Project

### Overview
TMC is a City Match Game uses Unity (URP, 2D). It uses a modular architecture with Core systems (service locator, event bus, scene/loading/currency/time systems) and Gameplay modules (e.g., CityMatch). The codebase is organized under `Assets/_Project`

### Requirements
- Unity Editor 6000.0.62f1 (or exactly the version configured for this repo). Using the same editor version is highly recommended to avoid upgrade dialogs and asset re‑imports.

### Run (Play Mode)
- Open one of the Persistent.unity and press Play.
- Editor Tool: a Scene Switcher toolbar exists under `_Project/Editor/Tools/SceneSwitcherToolbar.cs` to quickly switch scenes in the Editor.

### Art Assets 
- https://buggystudio.itch.io/isometric-city-pack

Key gameplay and framework highlights:
- Entry Points: Async and ordered loading with Bootstrappers(ServiceLocatorPersistentBootstrapper.cs, ServiceLocatorCityMatchSceneBootstrapper, ServiceLocatorMainMenuBootstrapper)
- Services and Systems: Search for "Services" (Scene, Loading, Currency, Time, Logging)
- Event Bus: Used it to let different parts of the application communicate without direct references, Look for "IEvent" implementations
- Save System: Used Newtonsoft.Json, separated save files for independent services
- Scriptable Objects: Search for "Settings" and "Configs"
- Abstraction with Scriptable Objects: Search for "Outcome" (CurrencyOutcome.cs, LifeOutcome.cs)
- Optimization: Object Pooling, Sprite Atlas
- UniTask: async workflows and some service initialization


### Tech Stack
- Language: C#
- Engine: Unity 6000.0.62f1
- Render Pipeline: Universal Render Pipeline (URP)
- UI: UGUI, TextMesh Pro
- Async: UniTask (via Git dependency) 
- Scripting Patterns: Service Locator, Event Bus
- Anim/tween: DOTween
- Attributes/Inspector: NaughtyAttributes



### Project Structure (high level)
```
TMC/
├─ Assets/
│  ├─ _Project/
│  │  ├─ Core/
│  │  │  ├─ Framework/        # EventBus, ServiceLocator, Logger, etc.
│  │  │  └─ Systems/          # Scene, Loading, Currency, Time, etc.
│  │  ├─ Gameplay/
│  │  │  ├─ CityMatch/        # CityMatch gameplay
│  │  │  └─ Common/           # Shared gameplay systems/components
│  │  ├─ Editor/Tools/        # Scene switcher, data tools
│  │  └─ Scenes/              # Persistent, MainMenu, CityMatch
│  ├─ Plugins/                # DOTween, etc.
│  └─ NaughtyAttributes/      # Inspector utilities
├─ Packages/
│  └─ manifest.json           # UPM dependencies (URP, UniTask, etc.)
├─ ProjectSettings/           # Unity project settings
├─ TMC.sln                    # Solution for Rider/VS
└─ LICENSE                    # MIT License


// ## For Unity Migration
// When you migrate to Unity, you can:
// 1. Use **VContainer** or **Zenject** as your DI framework
// 2. Register all services in a Unity `Installer` or `Scope`
// 3. All classes already support constructor injection
// 4. ScriptableObjects can be used for `IGameConfig` implementations

// Example Unity setup with VContainer:
// public class GameLifetimeScope : LifetimeScope
// {
//     protected override void Configure(IContainerBuilder builder)
//     {
//         builder.Register<ICardUtility, CardUtility>(Lifetime.Singleton);
//         builder.Register<IRandomService, UnityRandomService>(Lifetime.Singleton);
//         // ... register all other services
//     }
// }

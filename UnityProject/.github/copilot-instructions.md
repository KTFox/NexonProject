# Coding Standards & Style
- Use **PascalCase** for public members, method names, and classes.
- Use **camelCase** for local variables and private fields.
- Use **SerializeField** for private variables that need to be visible in the Inspector.
- Avoid using `public` variables for internal class data; favor properties or `[SerializeField] private`.
- Avoid using **magic numbers** or **hardcoded strings** directly in logic. Instead, define them as `private const` or `static readonly` fields.

# Unity Best Practices & Performance
- **Reference Management:** Cache references in `Awake()` or `Start()`. Avoid `GameObject.Find` or `GetComponent` in update loops.
- **Memory:** For Object Pooling, use the built-in `UnityEngine.Pool` API.

# Architecture & Modern Tooling
- **ScriptableObjects:** Use them for data-driven design and event systems.
- **Decoupling:** Prefer Event-driven architectures (C# Actions/UnityEvents) or Dependency Injection.
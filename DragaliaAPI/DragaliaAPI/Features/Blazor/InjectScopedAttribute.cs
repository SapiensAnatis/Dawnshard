namespace DragaliaAPI.Features.Blazor;

/// <summary>
/// Indicates that the associated property should have a value injected from the
/// scoped service provider during initialization.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class InjectScopedAttribute : Attribute { }

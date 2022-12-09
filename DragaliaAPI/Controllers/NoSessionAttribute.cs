namespace DragaliaAPI.Controllers;

/// <summary>
/// Indicates that a SID header is not expected for requests to this controller.
/// <remarks>Should be used for authentication flow endpoints and non-Dragalia endpoints only.</remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class NoSessionAttribute : Attribute { }

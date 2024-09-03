using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace DragaliaAPI.Infrastructure;

/// <summary>
/// Custom implementation of <see cref="ControllerFeatureProvider"/> that allows using internal controllers.
/// </summary>
public class CustomControllerFeatureProvider : ControllerFeatureProvider
{
    private const string ControllerTypeNameSuffix = "Controller";

    /// <inheritdoc />
    /// <remarks>Identical to the base implementation, except it skips type accessibility checks.</remarks>
    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!typeInfo.IsClass)
        {
            return false;
        }

        if (typeInfo.IsAbstract)
        {
            return false;
        }

        // if (!typeInfo.IsPublic)
        // {
        //     return false;
        // }

        if (typeInfo.ContainsGenericParameters)
        {
            return false;
        }

        if (typeInfo.IsDefined(typeof(NonControllerAttribute)))
        {
            return false;
        }

        if (
            !typeInfo.Name.EndsWith(ControllerTypeNameSuffix, StringComparison.OrdinalIgnoreCase)
            && !typeInfo.IsDefined(typeof(ControllerAttribute))
        )
        {
            return false;
        }

        return true;
    }
}

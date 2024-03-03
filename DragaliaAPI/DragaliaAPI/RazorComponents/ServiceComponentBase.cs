// Adapted from https://github.com/viceroypenguin/VsaTemplate/blob/5649a03b51176fcba80322e12f7055c30f79c939/Web/Infrastructure/Blazor/BlazorComponentBase.cs

using System.Reflection;
using DragaliaAPI.Features.Blazor;
using Microsoft.AspNetCore.Components;

namespace DragaliaAPI.RazorComponents;

public class ServiceComponentBase : OwningComponentBase
{
    protected override void OnInitialized()
    {
        IEnumerable<PropertyInfo> properties = this.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(p => p.GetCustomAttribute<InjectScopedAttribute>() != null);

        foreach (PropertyInfo p in properties)
            p.SetValue(this, this.ScopedServices.GetService(p.PropertyType));
    }
}

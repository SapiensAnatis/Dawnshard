using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DragaliaAPI.Infrastructure.Serialization;

public sealed class FromBodyUsingApiJsonAttribute : ModelBinderAttribute
{
    public FromBodyUsingApiJsonAttribute()
        : base(typeof(ApiJsonModelBinder))
    {
        this.BindingSource = BindingSource.Body;
    }
}

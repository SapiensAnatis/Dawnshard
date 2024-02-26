using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DragaliaAPI.Controllers.Other;

public sealed class FromBodyUsingApiJsonAttribute : ModelBinderAttribute
{
    public FromBodyUsingApiJsonAttribute()
        : base(typeof(ApiJsonModelBinder))
    {
        this.BindingSource = BindingSource.Body;
    }
}

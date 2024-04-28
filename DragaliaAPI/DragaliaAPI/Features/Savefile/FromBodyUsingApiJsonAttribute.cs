using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DragaliaAPI.Features.Savefile;

public sealed class FromBodyUsingApiJsonAttribute : ModelBinderAttribute
{
    public FromBodyUsingApiJsonAttribute()
        : base(typeof(ApiJsonModelBinder))
    {
        this.BindingSource = BindingSource.Body;
    }
}

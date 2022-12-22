using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragaliaAPI.Shared.Definitions;

public interface IDataItem<TIndex>
{
    TIndex Id { get; set; }
}

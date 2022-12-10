using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileImportService
{
    Task Import(long viewerId, LoadIndexData savefile);
}

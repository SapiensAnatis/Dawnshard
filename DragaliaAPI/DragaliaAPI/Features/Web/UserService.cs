namespace DragaliaAPI.Features.Web;

public class UserService
{
    public async Task<User> GetUser()
    {
        return new(1, "Ed Baldwin");
    }
}

public record User(long ViewerId, string Name);

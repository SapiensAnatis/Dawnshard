using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("user")]
public class UserController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IMapper mapper;

    public UserController(IUserDataRepository userDataRepository, IMapper mapper)
    {
        this.userDataRepository = userDataRepository;
        this.mapper = mapper;
    }

    [HttpPost("linked_n_account")]
    public async Task<DragaliaResult> LinkedNAccount(UserLinkedNAccountRequest request)
    {
        // No idea what is meant to be in this update_data_list. Best guess: user_data for viewer_id.
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .SingleAsync();

        return this.Ok(
            new UserLinkedNAccountData()
            {
                update_data_list = new() { user_data = this.mapper.Map<UserData>(userData) }
            }
        );
    }
}

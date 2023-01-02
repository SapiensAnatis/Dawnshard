using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("user")]
public class UserController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly ISavefileService savefileService;
    private readonly IMapper mapper;

    public UserController(
        IUserDataRepository userDataRepository,
        IUpdateDataService updateDataService,
        ISavefileService savefileService,
        IMapper mapper
    )
    {
        this.userDataRepository = userDataRepository;
        this.updateDataService = updateDataService;
        this.savefileService = savefileService;
        this.mapper = mapper;
    }

    [HttpPost("linked_n_account")]
    public async Task<DragaliaResult> LinkedNAccount(UserLinkedNAccountRequest request)
    {
        // This controller is meant to be used to set the 'Link a Nintendo Account' mission as complete
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .SingleAsync();

        userData.Crystal += 12_000;
        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.userDataRepository.SaveChangesAsync();

        return this.Ok(new UserLinkedNAccountData() { update_data_list = updateDataList });
    }

    [HttpPost("get_n_account_info")]
    public DragaliaResult GetNAccountInfo(UserGetNAccountInfoRequest request)
    {
        // TODO: Replace this with an API call to BaaS to return actual information
        return this.Ok(
            new UserGetNAccountInfoData()
            {
                n_account_info = new()
                {
                    email = "placeholder@email.com",
                    nickname = "placeholder nickname"
                },
                update_data_list = new()
            }
        );
    }

    [HttpPost("withdrawal")]
    public async Task<DragaliaResult> Withdrawal(UserWithdrawalRequest request)
    {
        await this.savefileService.Clear(this.DeviceAccountId, recreate: false);

        return this.Ok(new UserWithdrawalData() { result = (int)ResultCode.SUCCESS });
    }
}

﻿namespace DragaliaAPI.Features.Tutorial;

public interface ITutorialService
{
    public Task<int> UpdateTutorialStatus(int newStatus);
    public Task<List<int>> AddTutorialFlag(int flag);
    public Task OnStoryQuestRead(int storyId);
    Task<int> GetCurrentTutorialStatus();
}

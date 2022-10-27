using AutoMapper;
using AutoMapper.Configuration.Annotations;
using DragaliaAPI.Models.Database.Savefile;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record QuestStory(int quest_story_id, int state);

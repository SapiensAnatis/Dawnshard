using MessagePack;

namespace DragaliaAPI.Models.Components;

[MessagePackObject(true)]
public record GuildNoticeData(
    int is_update_guild_apply_reply = 0,
    int guild_apply_count = 0,
    int is_update_guild_board = 0,
    int is_update_guild = 0,
    int is_update_guild_invite = 0
);

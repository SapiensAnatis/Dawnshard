using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public record AbilityCrestBuildupPieceRequest(
    int ability_crest_id,
    IEnumerable<AtgenBuildupAbilityCrestPieceList> buildup_ability_crest_piece_list
);

[MessagePackObject(true)]
public record AbilityCrestBuildupPlusCountRequest(
    int ability_crest_id,
    IEnumerable<AtgenPlusCountParamsList> plus_count_params_list
);

[MessagePackObject(true)]
public record AbilityCrestGetAbilityCrestSetListRequest;

[MessagePackObject(true)]
public record AbilityCrestResetPlusCountRequest(
    int ability_crest_id,
    IEnumerable<int> plus_count_type_list
);

[MessagePackObject(true)]
public record AbilityCrestSetAbilityCrestSetRequest(
    int ability_crest_set_no,
    string ability_crest_set_name,
    AtgenRequestAbilityCrestSetData request_ability_crest_set_data
);

[MessagePackObject(true)]
public record AbilityCrestSetFavoriteRequest(int ability_crest_id, int is_favorite);

[MessagePackObject(true)]
public record AbilityCrestTradeGetListRequest;

[MessagePackObject(true)]
public record AbilityCrestTradeTradeRequest(int ability_crest_trade_id, int trade_count);

[MessagePackObject(true)]
public record AbilityCrestUpdateAbilityCrestSetNameRequest(
    int ability_crest_set_no,
    string ability_crest_set_name
);

[MessagePackObject(true)]
public record AlbumIndexRequest;

[MessagePackObject(true)]
public record AmuletBuildupRequest(
    ulong base_amulet_key_id,
    IEnumerable<GrowMaterialList> grow_material_list
);

[MessagePackObject(true)]
public record AmuletLimitBreakRequest(
    ulong base_amulet_key_id,
    IEnumerable<GrowMaterialList> grow_material_list
);

[MessagePackObject(true)]
public record AmuletResetPlusCountRequest(ulong amulet_key_id, int plus_count_type);

[MessagePackObject(true)]
public record AmuletSellRequest(IEnumerable<ulong> amulet_key_id_list);

[MessagePackObject(true)]
public record AmuletSetLockRequest(ulong amulet_key_id, int is_lock);

[MessagePackObject(true)]
public record AmuletTradeGetListRequest;

[MessagePackObject(true)]
public record AmuletTradeTradeRequest(int amulet_trade_id, int trade_count);

[MessagePackObject(true)]
public record BattleRoyalEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record BattleRoyalEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record BattleRoyalEventReceiveEventCyclePointRewardRequest(int event_id, int event_cycle_id);

[MessagePackObject(true)]
public record BattleRoyalEventReleaseCharaSkinRequest(
    int battle_royal_chara_skin_id,
    int is_pickup
);

[MessagePackObject(true)]
public record BattleRoyalFailRequest(string dungeon_key, int fail_state, int no_play_flg);

[MessagePackObject(true)]
public record BattleRoyalGetBattleRoyalHistoryRequest(int event_id);

[MessagePackObject(true)]
public record BattleRoyalRecordRoyalRecordMultiRequest(
    PlayRecord play_record,
    string dungeon_key,
    string multiplay_key
);

[MessagePackObject(true)]
public record BattleRoyalStartMultiRequest(Charas chara_id, int is_tutorial, string multiplay_key);

[MessagePackObject(true)]
public record BattleRoyalStartRoyalMultiRequest;

[MessagePackObject(true)]
public record BuildEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record BuildEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record BuildEventReceiveBuildPointRewardRequest(int event_id);

[MessagePackObject(true)]
public record BuildEventReceiveDailyBonusRequest(int event_id);

[MessagePackObject(true)]
public record CartoonLatestRequest;

[MessagePackObject(true)]
public record CastleStoryReadRequest(int castle_story_id);

[MessagePackObject(true)]
public record CharaAwakeRequest(Charas chara_id, int next_rarity);

[MessagePackObject(true)]
public record CharaBuildupManaRequest(
    Charas chara_id,
    IEnumerable<int> mana_circle_piece_id_list,
    int is_use_grow_material
);

[MessagePackObject(true)]
public record CharaBuildupPlatinumRequest(Charas chara_id);

[MessagePackObject(true)]
public record CharaBuildupRequest(Charas chara_id, IEnumerable<AtgenEnemyPiece> material_list);

[MessagePackObject(true)]
public record CharaGetCharaUnitSetRequest(IEnumerable<int> chara_id_list);

[MessagePackObject(true)]
public record CharaGetListRequest;

[MessagePackObject(true)]
public record CharaLimitBreakAndBuildupManaRequest(
    Charas chara_id,
    int next_limit_break_count,
    IEnumerable<int> mana_circle_piece_id_list,
    int is_use_grow_material
);

[MessagePackObject(true)]
public record CharaLimitBreakRequest(
    Charas chara_id,
    int next_limit_break_count,
    int is_use_grow_material
);

[MessagePackObject(true)]
public record CharaResetPlusCountRequest(Charas chara_id, int plus_count_type);

[MessagePackObject(true)]
public record CharaSetCharaUnitSetRequest(
    int unit_set_no,
    string unit_set_name,
    Charas chara_id,
    AtgenRequestCharaUnitSetData request_chara_unit_set_data
);

[MessagePackObject(true)]
public record CharaUnlockEditSkillRequest(Charas chara_id);

[MessagePackObject(true)]
public record Clb01EventEntryRequest(int event_id);

[MessagePackObject(true)]
public record Clb01EventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record Clb01EventReceiveClb01PointRewardRequest(int event_id);

[MessagePackObject(true)]
public record CollectEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record CollectEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record CombatEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record CombatEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record CombatEventReceiveEventLocationRewardRequest(
    int event_id,
    int event_location_reward_id
);

[MessagePackObject(true)]
public record CombatEventReceiveEventPointRewardRequest(int event_id);

[MessagePackObject(true)]
public record CraftAssembleRequest(
    ulong weapon_key_id,
    int target_weapon_id,
    IEnumerable<GrowMaterialList> assemble_item_list,
    IEnumerable<AtgenWeaponSetList> weapon_set_list
);

[MessagePackObject(true)]
public record CraftCreateRequest(
    int target_weapon_id,
    int target_weapon_quantity,
    int force_limit_break,
    IEnumerable<AtgenWeaponSetList> weapon_set_list
);

[MessagePackObject(true)]
public record CraftDisassembleRequest(ulong weapon_key_id, int payment_type);

[MessagePackObject(true)]
public record CraftResetNewRequest(IEnumerable<int> weapon_id_list);

[MessagePackObject(true)]
public record DeployGetDeployVersionRequest;

[MessagePackObject(true)]
public record DmodeBuildupServitorPassiveRequest(
    IEnumerable<DmodeServitorPassiveList> request_buildup_passive_list
);

[MessagePackObject(true)]
public record DmodeDungeonFinishRequest(int is_game_over);

[MessagePackObject(true)]
public record DmodeDungeonFloorRequest(DmodePlayRecord dmode_play_record);

[MessagePackObject(true)]
public record DmodeDungeonFloorSkipRequest;

[MessagePackObject(true)]
public record DmodeDungeonRestartRequest;

[MessagePackObject(true)]
public record DmodeDungeonStartRequest(
    Charas chara_id,
    int start_floor_num,
    int servitor_id,
    IEnumerable<int> bring_edit_skill_chara_id_list
);

[MessagePackObject(true)]
public record DmodeDungeonSystemHaltRequest;

[MessagePackObject(true)]
public record DmodeDungeonUserHaltRequest;

[MessagePackObject(true)]
public record DmodeEntryRequest;

[MessagePackObject(true)]
public record DmodeExpeditionFinishRequest;

[MessagePackObject(true)]
public record DmodeExpeditionForceFinishRequest;

[MessagePackObject(true)]
public record DmodeExpeditionStartRequest(int target_floor_num, IEnumerable<int> chara_id_list);

[MessagePackObject(true)]
public record DmodeGetDataRequest;

[MessagePackObject(true)]
public record DmodeReadStoryRequest(int dmode_story_id);

[MessagePackObject(true)]
public record DragonBuildupRequest(
    ulong base_dragon_key_id,
    IEnumerable<GrowMaterialList> grow_material_list
);

[MessagePackObject(true)]
public record DragonBuyGiftToSendMultipleRequest(
    int dragon_id,
    IEnumerable<int> dragon_gift_id_list
);

[MessagePackObject(true)]
public record DragonBuyGiftToSendRequest(int dragon_id, int dragon_gift_id);

[MessagePackObject(true)]
public record DragonGetContactDataRequest;

[MessagePackObject(true)]
public record DragonLimitBreakRequest(
    ulong base_dragon_key_id,
    IEnumerable<LimitBreakGrowList> limit_break_grow_list
);

[MessagePackObject(true)]
public record DragonResetPlusCountRequest(ulong dragon_key_id, int plus_count_type);

[MessagePackObject(true)]
public record DragonSellRequest(IEnumerable<ulong> dragon_key_id_list);

[MessagePackObject(true)]
public record DragonSendGiftMultipleRequest(int dragon_id, int dragon_gift_id, int quantity);

[MessagePackObject(true)]
public record DragonSendGiftRequest(int dragon_id, int dragon_gift_id);

[MessagePackObject(true)]
public record DragonSetLockRequest(ulong dragon_key_id, int is_lock);

[MessagePackObject(true)]
public record DreamAdventureClearRequest(int difficulty);

[MessagePackObject(true)]
public record DreamAdventurePlayRequest(int difficulty);

[MessagePackObject(true)]
public record DungeonFailRequest(string dungeon_key, int fail_state, int no_play_flg);

[MessagePackObject(true)]
public record DungeonGetAreaOddsRequest(string dungeon_key, int area_idx);

[MessagePackObject(true)]
public record DungeonReceiveQuestBonusRequest(
    int quest_event_id,
    int is_receive,
    int receive_bonus_count
);

[MessagePackObject(true)]
public record DungeonRecordRecordMultiRequest(
    PlayRecord play_record,
    string dungeon_key,
    IEnumerable<ulong> connecting_viewer_id_list,
    int no_play_flg
);

[MessagePackObject(true)]
public record DungeonRecordRecordRequest(
    PlayRecord play_record,
    string dungeon_key,
    int repeat_state,
    string repeat_key
);

[MessagePackObject(true)]
public record DungeonRetryRequest(string dungeon_key, int payment_type);

[MessagePackObject(true)]
public record DungeonSkipStartAssignUnitRequest(
    int quest_id,
    IEnumerable<PartySettingList> request_party_setting_list,
    ulong support_viewer_id,
    int play_count,
    int bet_count
);

[MessagePackObject(true)]
public record DungeonSkipStartMultipleQuestAssignUnitRequest(
    IEnumerable<PartySettingList> request_party_setting_list,
    IEnumerable<AtgenRequestQuestMultipleList> request_quest_multiple_list,
    ulong support_viewer_id
);

[MessagePackObject(true)]
public record DungeonSkipStartMultipleQuestRequest(
    int party_no,
    IEnumerable<AtgenRequestQuestMultipleList> request_quest_multiple_list,
    ulong support_viewer_id
);

[MessagePackObject(true)]
public record DungeonSkipStartRequest(
    int quest_id,
    int party_no,
    ulong support_viewer_id,
    int play_count,
    int bet_count
);

[MessagePackObject(true)]
public record DungeonStartStartAssignUnitRequest(
    int quest_id,
    IEnumerable<PartySettingList> request_party_setting_list,
    int bet_count,
    int repeat_state,
    ulong support_viewer_id,
    RepeatSetting repeat_setting
);

[MessagePackObject(true)]
public record DungeonStartStartMultiAssignUnitRequest(
    int quest_id,
    IEnumerable<PartySettingList> request_party_setting_list
);

[MessagePackObject(true)]
public record DungeonStartStartMultiRequest(
    int quest_id,
    int party_no,
    IEnumerable<int> party_no_list
);

[MessagePackObject(true)]
public record DungeonStartStartRequest(
    int quest_id,
    int party_no,
    IEnumerable<int> party_no_list,
    int bet_count,
    int repeat_state,
    ulong support_viewer_id,
    RepeatSetting repeat_setting
);

[MessagePackObject(true)]
public record EarnEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record EarnEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record EarnEventReceiveEventPointRewardRequest(int event_id);

[MessagePackObject(true)]
public record EmblemGetListRequest;

[MessagePackObject(true)]
public record EmblemSetRequest(int emblem_id);

[MessagePackObject(true)]
public record EulaAgreeAgreeRequest(
    string id_token,
    string region,
    string lang,
    int eula_version,
    int privacy_policy_version,
    string uuid
);

[MessagePackObject(true)]
public record EulaGetVersionListRequest;

[MessagePackObject(true)]
public record EulaGetVersionRequest(string id_token, string region, string lang);

[MessagePackObject(true)]
public record EventDamageGetTotalDamageHistoryRequest(int event_id);

[MessagePackObject(true)]
public record EventDamageReceiveDamageRewardRequest(int event_id);

[MessagePackObject(true)]
public record EventStoryReadRequest(int event_story_id);

[MessagePackObject(true)]
public record EventSummonExecRequest(int event_id, int count, int is_enable_stop_by_target);

[MessagePackObject(true)]
public record EventSummonGetDataRequest(int event_id);

[MessagePackObject(true)]
public record EventSummonResetRequest(int event_id);

[MessagePackObject(true)]
public record EventTradeGetListRequest(int trade_group_id);

[MessagePackObject(true)]
public record EventTradeTradeRequest(int trade_group_id, int trade_id, int trade_count);

[MessagePackObject(true)]
public record ExchangeGetUnitListRequest(int exchange_ticket_id);

[MessagePackObject(true)]
public record ExchangeSelectUnitRequest(
    int exchange_ticket_id,
    AtgenDuplicateEntityList selected_unit
);

[MessagePackObject(true)]
public record ExHunterEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record ExHunterEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record ExHunterEventReceiveExHunterPointRewardRequest(
    int event_id,
    IEnumerable<int> ex_hunter_event_reward_id_list
);

[MessagePackObject(true)]
public record ExRushEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record ExRushEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record FortAddCarpenterRequest(int payment_type);

[MessagePackObject(true)]
public record FortBuildAtOnceRequest(ulong build_id, int payment_type);

[MessagePackObject(true)]
public record FortBuildCancelRequest(ulong build_id);

[MessagePackObject(true)]
public record FortBuildEndRequest(ulong build_id);

[MessagePackObject(true)]
public record FortBuildStartRequest(int fort_plant_id, int position_x, int position_z);

[MessagePackObject(true)]
public record FortGetDataRequest;

[MessagePackObject(true)]
public record FortGetMultiIncomeRequest(IEnumerable<ulong> build_id_list);

[MessagePackObject(true)]
public record FortLevelupAtOnceRequest(ulong build_id, int payment_type);

[MessagePackObject(true)]
public record FortLevelupCancelRequest(ulong build_id);

[MessagePackObject(true)]
public record FortLevelupEndRequest(ulong build_id);

[MessagePackObject(true)]
public record FortLevelupStartRequest(ulong build_id);

[MessagePackObject(true)]
public record FortMoveRequest(ulong build_id, int after_position_x, int after_position_z);

[MessagePackObject(true)]
public record FortSetNewFortPlantRequest(IEnumerable<int> fort_plant_id_list);

[MessagePackObject(true)]
public record FriendAllReplyDenyRequest;

[MessagePackObject(true)]
public record FriendApplyListRequest;

[MessagePackObject(true)]
public record FriendAutoSearchRequest;

[MessagePackObject(true)]
public record FriendDeleteRequest(ulong friend_id);

[MessagePackObject(true)]
public record FriendFriendIndexRequest;

[MessagePackObject(true)]
public record FriendFriendListRequest;

[MessagePackObject(true)]
public record FriendGetSupportCharaDetailRequest(ulong support_viewer_id);

[MessagePackObject(true)]
public record FriendGetSupportCharaRequest;

[MessagePackObject(true)]
public record FriendIdSearchRequest(ulong search_id);

[MessagePackObject(true)]
public record FriendReplyRequest(ulong friend_id, int reply);

[MessagePackObject(true)]
public record FriendRequestCancelRequest(ulong friend_id);

[MessagePackObject(true)]
public record FriendRequestListRequest;

[MessagePackObject(true)]
public record FriendRequestRequest(ulong friend_id);

[MessagePackObject(true)]
public record FriendSetSupportCharaRequest(
    Charas chara_id,
    ulong dragon_key_id,
    ulong weapon_key_id,
    ulong amulet_key_id,
    ulong amulet_2_key_id,
    int weapon_body_id,
    int crest_slot_type_1_crest_id_1,
    int crest_slot_type_1_crest_id_2,
    int crest_slot_type_1_crest_id_3,
    int crest_slot_type_2_crest_id_1,
    int crest_slot_type_2_crest_id_2,
    int crest_slot_type_3_crest_id_1,
    int crest_slot_type_3_crest_id_2,
    ulong talisman_key_id
);

[MessagePackObject(true)]
public record GuildChatGetNewMessageListRequest(int guild_id, ulong chat_message_id);

[MessagePackObject(true)]
public record GuildChatGetOldMessageListRequest(int guild_id, ulong chat_message_id);

[MessagePackObject(true)]
public record GuildChatPostMessageStampRequest(
    int guild_id,
    ulong chat_message_id,
    int chat_message_stamp_id
);

[MessagePackObject(true)]
public record GuildChatPostMessageTextRequest(
    int guild_id,
    ulong chat_message_id,
    string chat_message_text
);

[MessagePackObject(true)]
public record GuildChatPostReportRequest(
    int guild_id,
    ulong chat_message_id,
    int report_category_id,
    string message
);

[MessagePackObject(true)]
public record GuildDisbandRequest(int guild_id);

[MessagePackObject(true)]
public record GuildDropMemberRequest(int guild_id, ulong target_viewer_id);

[MessagePackObject(true)]
public record GuildEstablishRequest(
    string guild_name,
    int guild_emblem_id,
    int joining_condition_type,
    int activity_policy_type,
    string guild_introduction,
    string guild_board
);

[MessagePackObject(true)]
public record GuildGetGuildApplyDataRequest(int guild_id);

[MessagePackObject(true)]
public record GuildGetGuildMemberDataRequest(int guild_id);

[MessagePackObject(true)]
public record GuildIndexRequest;

[MessagePackObject(true)]
public record GuildInviteGetGuildInviteReceiveDataRequest;

[MessagePackObject(true)]
public record GuildInviteGetGuildInviteSendDataRequest(int guild_id);

[MessagePackObject(true)]
public record GuildInviteInviteCancelRequest(int guild_id, ulong guild_invite_id);

[MessagePackObject(true)]
public record GuildInviteInviteReplyAllDenyRequest(
    IEnumerable<AtgenGuildInviteParamsList> guild_invite_params_list
);

[MessagePackObject(true)]
public record GuildInviteInviteReplyRequest(int guild_id, ulong guild_invite_id, int reply_status);

[MessagePackObject(true)]
public record GuildInviteInviteSendRequest(
    ulong target_viewer_id,
    int guild_id,
    int guild_invite_message_id
);

[MessagePackObject(true)]
public record GuildJoinReplyAllDenyRequest(int guild_id, IEnumerable<ulong> guild_apply_id_list);

[MessagePackObject(true)]
public record GuildJoinReplyRequest(int guild_id, ulong guild_apply_id, int reply_status);

[MessagePackObject(true)]
public record GuildJoinRequest(int guild_id);

[MessagePackObject(true)]
public record GuildJoinRequestCancelRequest(int guild_id);

[MessagePackObject(true)]
public record GuildJoinRequestRequest(int guild_id);

[MessagePackObject(true)]
public record GuildPostReportRequest(
    int guild_id,
    int report_type,
    string report_data,
    int report_category_id,
    string message
);

[MessagePackObject(true)]
public record GuildResignRequest(int guild_id, int is_temporary_member);

[MessagePackObject(true)]
public record GuildSearchAutoSearchRequest(
    IEnumerable<int> joining_condition_type_list,
    IEnumerable<int> activity_policy_type_list,
    IEnumerable<int> member_count_type_list
);

[MessagePackObject(true)]
public record GuildSearchGetGuildDetailRequest(int guild_id);

[MessagePackObject(true)]
public record GuildSearchIdSearchRequest(int guild_id);

[MessagePackObject(true)]
public record GuildSearchNameSearchRequest(string guild_name);

[MessagePackObject(true)]
public record GuildUpdateGuildPositionTypeRequest(
    int guild_id,
    ulong target_viewer_id,
    int guild_position_type
);

[MessagePackObject(true)]
public record GuildUpdateGuildSettingRequest(
    int guild_id,
    string guild_name,
    int guild_emblem_id,
    string guild_introduction,
    string guild_board,
    int joining_condition_type,
    int activity_policy_type
);

[MessagePackObject(true)]
public record GuildUpdateUserSettingRequest(
    int profile_entity_type,
    int profile_entity_id,
    int profile_entity_rarity,
    int guild_push_notification_type_running,
    int guild_push_notification_type_suspending,
    int is_enable_invite_receive
);

[MessagePackObject(true)]
public record InquiryAggregationRequest(string language_code, int inquiry_faq_id);

[MessagePackObject(true)]
public record InquiryDetailRequest(string opinion_id, string language_code);

[MessagePackObject(true)]
public record InquiryReplyRequest(string opinion_id, string comment_text);

[MessagePackObject(true)]
public record InquirySendRequest(
    string opinion_text,
    int opinion_type,
    string language_code,
    string region_code,
    int occurred_at
);

[MessagePackObject(true)]
public record InquiryTopRequest(string language_code);

[MessagePackObject(true)]
public record ItemGetListRequest;

[MessagePackObject(true)]
public record ItemUseRecoveryStaminaRequest(IEnumerable<AtgenUseItemList> use_item_list);

[MessagePackObject(true)]
public record LoadIndexRequest;

[MessagePackObject(true)]
public record LoginIndexRequest(string jws_result);

[MessagePackObject(true)]
public record LoginPenaltyConfirmRequest(int penalty_type, int report_id);

[MessagePackObject(true)]
public record LoginVerifyJwsRequest(string jws_result);

[MessagePackObject(true)]
public record LotteryGetOddsDataRequest(int lottery_id);

[MessagePackObject(true)]
public record LotteryLotteryExecRequest(int lottery_id);

[MessagePackObject(true)]
public record LotteryResultRequest(int lottery_id);

[MessagePackObject(true)]
public record MaintenanceGetTextRequest(int type, string lang);

[MessagePackObject(true)]
public record MatchingCheckPenaltyUserRequest(ulong viewwer_id);

[MessagePackObject(true)]
public record MatchingGetRoomListByGuildRequest(int compatible_id);

[MessagePackObject(true)]
public record MatchingGetRoomListByLocationRequest(
    string region,
    int quest_type,
    float latitude,
    float longitude,
    int compatible_id
);

[MessagePackObject(true)]
public record MatchingGetRoomListByQuestIdRequest(string region, int quest_id, int compatible_id);

[MessagePackObject(true)]
public record MatchingGetRoomListRequest(string region, int tab_type, int compatible_id);

[MessagePackObject(true)]
public record MatchingGetRoomNameRequest(int room_id);

[MessagePackObject(true)]
public record MazeEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record MazeEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record MazeEventReceiveMazePointRewardRequest(int event_id);

[MessagePackObject(true)]
public record MemoryEventActivateRequest(int event_id);

[MessagePackObject(true)]
public record MissionGetDrillMissionListRequest;

[MessagePackObject(true)]
public record MissionGetMissionListRequest;

[MessagePackObject(true)]
public record MissionReceiveAlbumRewardRequest(IEnumerable<int> album_mission_id_list);

[MessagePackObject(true)]
public record MissionReceiveBeginnerRewardRequest(IEnumerable<int> beginner_mission_id_list);

[MessagePackObject(true)]
public record MissionReceiveDailyRewardRequest(
    IEnumerable<AtgenMissionParamsList> mission_params_list
);

[MessagePackObject(true)]
public record MissionReceiveDrillRewardRequest(
    IEnumerable<int> drill_mission_id_list,
    IEnumerable<int> drill_mission_group_id_list
);

[MessagePackObject(true)]
public record MissionReceiveMainStoryRewardRequest(IEnumerable<int> main_story_mission_id_list);

[MessagePackObject(true)]
public record MissionReceiveMemoryEventRewardRequest(IEnumerable<int> memory_event_mission_id_list);

[MessagePackObject(true)]
public record MissionReceiveNormalRewardRequest(IEnumerable<int> normal_mission_id_list);

[MessagePackObject(true)]
public record MissionReceivePeriodRewardRequest(IEnumerable<int> period_mission_id_list);

[MessagePackObject(true)]
public record MissionReceiveSpecialRewardRequest(IEnumerable<int> special_mission_id_list);

[MessagePackObject(true)]
public record MissionUnlockDrillMissionGroupRequest(int drill_mission_group_id);

[MessagePackObject(true)]
public record MissionUnlockMainStoryGroupRequest(int main_story_mission_group_id);

[MessagePackObject(true)]
public record MypageInfoRequest;

[MessagePackObject(true)]
public record OptionGetOptionRequest;

[MessagePackObject(true)]
public record OptionSetOptionRequest(OptionData option_setting);

[MessagePackObject(true)]
public record PartyIndexRequest;

[MessagePackObject(true)]
public record PartySetMainPartyNoRequest(int main_party_no);

[MessagePackObject(true)]
public record PartySetPartySettingRequest(
    int party_no,
    IEnumerable<PartySettingList> request_party_setting_list,
    string party_name,
    int is_entrust,
    int entrust_element
);

[MessagePackObject(true)]
public record PartyUpdatePartyNameRequest(int party_no, string party_name);

[MessagePackObject(true)]
public record PlatformAchievementGetPlatformAchievementListRequest;

[MessagePackObject(true)]
public record PresentGetHistoryListRequest(ulong present_history_id);

[MessagePackObject(true)]
public record PresentGetPresentListRequest(int is_limit, ulong present_id);

[MessagePackObject(true)]
public record PresentReceiveRequest(IEnumerable<ulong> present_id_list, int is_limit);

[MessagePackObject(true)]
public record PushNotificationUpdateSettingRequest(string region, string lang, string uuid);

[MessagePackObject(true)]
public record QuestDropListRequest(int quest_id);

[MessagePackObject(true)]
public record QuestGetQuestClearPartyMultiRequest(int quest_id);

[MessagePackObject(true)]
public record QuestGetQuestClearPartyRequest(int quest_id);

[MessagePackObject(true)]
public record QuestGetSupportUserListRequest;

[MessagePackObject(true)]
public record QuestOpenTreasureRequest(int quest_treasure_id);

[MessagePackObject(true)]
public record QuestReadStoryRequest(int quest_story_id);

[MessagePackObject(true)]
public record QuestSearchQuestClearPartyCharaMultiRequest(IEnumerable<int> quest_id_list);

[MessagePackObject(true)]
public record QuestSearchQuestClearPartyCharaRequest(IEnumerable<int> quest_id_list);

[MessagePackObject(true)]
public record QuestSearchQuestClearPartyMultiRequest(
    int quest_id,
    int party_switch_no,
    IEnumerable<int> chara_id_list,
    IEnumerable<int> dragon_id_list,
    IEnumerable<int> weapon_body_id_list,
    IEnumerable<int> ability_crest_id_list
);

[MessagePackObject(true)]
public record QuestSearchQuestClearPartyRequest(
    int quest_id,
    int party_switch_no,
    IEnumerable<int> chara_id_list,
    IEnumerable<int> dragon_id_list,
    IEnumerable<int> weapon_body_id_list,
    IEnumerable<int> ability_crest_id_list
);

[MessagePackObject(true)]
public record QuestSetQuestClearPartyMultiRequest(
    int quest_id,
    IEnumerable<PartySettingList> request_party_setting_list
);

[MessagePackObject(true)]
public record QuestSetQuestClearPartyRequest(
    int quest_id,
    IEnumerable<PartySettingList> request_party_setting_list
);

[MessagePackObject(true)]
public record RaidEventEntryRequest(int raid_event_id);

[MessagePackObject(true)]
public record RaidEventGetEventDataRequest(int raid_event_id);

[MessagePackObject(true)]
public record RaidEventReceiveRaidPointRewardRequest(
    int raid_event_id,
    IEnumerable<int> raid_event_reward_id_list
);

[MessagePackObject(true)]
public record RedoableSummonFixExecRequest;

[MessagePackObject(true)]
public record RedoableSummonGetDataRequest;

[MessagePackObject(true)]
public record RedoableSummonPreExecRequest(int summon_id);

[MessagePackObject(true)]
public record RepeatEndRequest;

[MessagePackObject(true)]
public record ShopChargeCancelRequest(int shop_type, int goods_id);

[MessagePackObject(true)]
public record ShopGetBonusRequest(int bonus_type);

[MessagePackObject(true)]
public record ShopGetDreamSelectUnitListRequest(int goods_id);

[MessagePackObject(true)]
public record ShopGetListRequest;

[MessagePackObject(true)]
public record ShopGetProductListRequest;

[MessagePackObject(true)]
public record ShopItemSummonExecRequest(int payment_type, PaymentTarget payment_target);

[MessagePackObject(true)]
public record ShopItemSummonOddRequest;

[MessagePackObject(true)]
public record ShopMaterialShopPurchaseRequest(
    int goods_id,
    int shop_type,
    int payment_type,
    int quantity
);

[MessagePackObject(true)]
public record ShopNormalShopPurchaseRequest(int goods_id, int payment_type, int quantity);

[MessagePackObject(true)]
public record ShopPreChargeCheckRequest(int shop_type, int goods_id, int quantity);

[MessagePackObject(true)]
public record ShopSpecialShopPurchaseRequest(
    int goods_id,
    int payment_type,
    int quantity,
    AtgenDuplicateEntityList selected_unit
);

[MessagePackObject(true)]
public record SimpleEventEntryRequest(int event_id);

[MessagePackObject(true)]
public record SimpleEventGetEventDataRequest(int event_id);

[MessagePackObject(true)]
public record StampGetStampRequest;

[MessagePackObject(true)]
public record StampSetEquipStampRequest(int deck_no, IEnumerable<EquipStampList> stamp_list);

[MessagePackObject(true)]
public record StoryReadRequest(int unit_story_id);

[MessagePackObject(true)]
public record StorySkipSkipRequest;

[MessagePackObject(true)]
public record SuggestionGetCategoryListRequest(string language_code);

[MessagePackObject(true)]
public record SuggestionSetRequest(string message, int category_id);

[MessagePackObject(true)]
public record SummonExcludeGetListRequest(int summon_id);

[MessagePackObject(true)]
public record SummonExcludeGetOddsDataRequest(
    int summon_id,
    int exclude_entity_type,
    IEnumerable<int> exclude_entity_id_list
);

[MessagePackObject(true)]
public record SummonExcludeRequestRequest(
    int summon_id,
    int payment_type,
    int exclude_entity_type,
    IEnumerable<int> exclude_entity_id_list
);

[MessagePackObject(true)]
public record SummonGetOddsDataRequest(int summon_id);

[MessagePackObject(true)]
public record SummonGetSummonHistoryRequest;

[MessagePackObject(true)]
public record SummonGetSummonListRequest;

[MessagePackObject(true)]
public record SummonGetSummonPointTradeRequest(int summon_id);

[MessagePackObject(true)]
public record SummonRequestRequest(
    int summon_id,
    SummonExecTypes exec_type,
    int exec_count,
    PaymentTypes payment_type,
    PaymentTarget payment_target
);

[MessagePackObject(true)]
public record SummonSummonPointTradeRequest(int summon_id, int trade_id);

[MessagePackObject(true)]
public record TalismanSellRequest(IEnumerable<ulong> talisman_key_id_list);

[MessagePackObject(true)]
public record TalismanSetLockRequest(ulong talisman_key_id, int is_lock);

[MessagePackObject(true)]
public record TimeAttackRankingGetDataRequest;

[MessagePackObject(true)]
public record TimeAttackRankingReceiveTierRewardRequest(int quest_id);

[MessagePackObject(true)]
public record ToolAuthRequest(string uuid, string id_token);

[MessagePackObject(true)]
public record ToolGetMaintenanceTimeRequest;

[MessagePackObject(true)]
public record ToolGetServiceStatusRequest;

[MessagePackObject(true)]
public record ToolReauthRequest(string uuid, string id_token);

[MessagePackObject(true)]
public record ToolSignupRequest(
    string uuid,
    string id_token,
    string app_version,
    string platform,
    string hashcode,
    int reset_count,
    string eula_region,
    string eula_lang,
    int eula_version,
    int privacy_policy_version
);

[MessagePackObject(true)]
public record TrackRecordUpdateProgressRequest;

[MessagePackObject(true)]
public record TransitionTransitionByNAccountRequest(string uuid, string id_token);

[MessagePackObject(true)]
public record TreasureTradeGetListAllRequest;

[MessagePackObject(true)]
public record TreasureTradeGetListRequest(int trade_group_id);

[MessagePackObject(true)]
public record TreasureTradeTradeRequest(
    int trade_group_id,
    int treasure_trade_id,
    IEnumerable<AtgenNeedUnitList> need_unit_list,
    int trade_count
);

[MessagePackObject(true)]
public record TutorialUpdateFlagsRequest(int flag_id);

[MessagePackObject(true)]
public record TutorialUpdateStepRequest(int step, int is_skip, int entity_id, int entity_type);

[MessagePackObject(true)]
public record UpdateNamechangeRequest(string name);

[MessagePackObject(true)]
public record UpdateResetNewRequest(IEnumerable<AtgenTargetList> target_list);

[MessagePackObject(true)]
public record UserGetNAccountInfoRequest;

[MessagePackObject(true)]
public record UserGetWalletBalanceRequest;

[MessagePackObject(true)]
public record UserLinkedNAccountRequest;

[MessagePackObject(true)]
public record UserOptInSettingRequest(int is_optin);

[MessagePackObject(true)]
public record UserRecoverStaminaByStoneRequest(
    int recovery_type,
    int recovery_quantity,
    int payment_type
);

[MessagePackObject(true)]
public record UserWithdrawalRequest;

[MessagePackObject(true)]
public record VersionGetResourceVersionRequest(int platform, string app_version);

[MessagePackObject(true)]
public record WalkerSendGiftMultipleRequest(int dragon_gift_id, int quantity);

[MessagePackObject(true)]
public record WallFailRequest(string dungeon_key, int fail_state);

[MessagePackObject(true)]
public record WallGetMonthlyRewardRequest(int quest_group_id);

[MessagePackObject(true)]
public record WallGetWallClearPartyRequest(int wall_id);

[MessagePackObject(true)]
public record WallReceiveMonthlyRewardRequest(int quest_group_id);

[MessagePackObject(true)]
public record WallRecordRecordRequest(int wall_id, string dungeon_key);

[MessagePackObject(true)]
public record WallSetWallClearPartyRequest(
    int wall_id,
    IEnumerable<PartySettingList> request_party_setting_list
);

[MessagePackObject(true)]
public record WallStartStartAssignUnitRequest(
    int wall_id,
    int wall_level,
    IEnumerable<PartySettingList> request_party_setting_list,
    ulong support_viewer_id
);

[MessagePackObject(true)]
public record WallStartStartRequest(
    int wall_id,
    int wall_level,
    int party_no,
    ulong support_viewer_id
);

[MessagePackObject(true)]
public record WeaponBodyBuildupPieceRequest(
    int weapon_body_id,
    IEnumerable<AtgenBuildupWeaponBodyPieceList> buildup_weapon_body_piece_list
);

[MessagePackObject(true)]
public record WeaponBodyCraftRequest(int weapon_body_id);

[MessagePackObject(true)]
public record WeaponBuildupRequest(
    ulong base_weapon_key_id,
    IEnumerable<GrowMaterialList> grow_material_list
);

[MessagePackObject(true)]
public record WeaponLimitBreakRequest(
    ulong base_weapon_key_id,
    IEnumerable<GrowMaterialList> grow_material_list
);

[MessagePackObject(true)]
public record WeaponResetPlusCountRequest(ulong weapon_key_id, int plus_count_type);

[MessagePackObject(true)]
public record WeaponSellRequest(IEnumerable<ulong> weapon_key_id_list);

[MessagePackObject(true)]
public record WeaponSetLockRequest(ulong weapon_key_id, int is_lock);

[MessagePackObject(true)]
public record WebviewVersionUrlListRequest(string region);

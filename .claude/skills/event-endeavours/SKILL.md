---
name: event-endeavours
description: Implement event missions (endeavours) into the mission designer project. Use when asked to implement event missions or event endeavours
---

To implement event endeavours, you will need to query the master asset SQL database. The user should set the path to this database as $MASTER_ASSET_SQLITE_PATH. If this variable is not set stop and wait for the user to specify it.

Terminology: missions and endeavours are the same thing. Limited endeavours and period missions are the same thing.

To implement limited endeavours for an event:

1. Look up the event ID from [event names JSON file](./assets/event-names.json). The JSON is formatted as follows:

  ```json
  [
    {
        "EventId": 20458,
        "EventName": "Loyalty's Requiem"
    },
    // other entries...
  ]
  ```

2. Use a query like this to extract mission descriptions from the database. If you get zero rows back, check the event ID.

  ```sql
  SELECT m._Id, tl._Text FROM MissionPeriodData m
  JOIN TextLabel tl on m._Text = tl._Id
  WHERE m._QuestGroupId = /* <event ID from step 1> */
  ORDER BY m._Id
  ```

3. Using the existing mission types and existing mission data in `DragaliaAPI/DragaliaAPI.MissionDesigner`, implement a new file with the name of the event that contains a list with the appropriate missions. 
  - Use `DragaliaAPI/DragaliaAPI.MissionDesigner/Missions/Period/StarryDragonyule.cs` as a reference. 
  - The file should be named the PascalCase event name. 
  - Ensure the new mission class is attributed with `[ContainsMissionList]` so that Program.cs can discover it by reflection.
  - The available mission types are defined in `DragaliaAPI/DragaliaAPI.MissionDesigner/Models`. If you feel that none of them fit, stop and wait for the user to add a new one.
  - Use the `[EventId]`, `[MissionType]` and other `ImplicitPropertyAttribute` subclasses as list-level attributes to avoid having to set the same fields on each entry in the list.
  - The missions should be implemented based on what the description tells the player to do; there is no way to derive the completion type a priori from the database.
  - The `UseTotalValue` property indicates that each time a mission progression event is registered, it sets a new progression value rather than adding to the previous one. For example, for a mission: 'Get 5000 Points in One Go', the progression event is called with 1000, 1500, and 2000 - after this, because of `UseTotalValue`, the completion is 2000 instead of 3500, which is right for that mission type.
  - Where an event has multiple trials at the same difficulty you will need to pass in a `QuestId` to be explicit.
  - Leaving a parameter null will match any value for that parameter in a completion event.
  - If there are two 'Participate in the Event' missions, then ignore the second one with the higher ID. This is for two-part events but we don't run the event in two parts.

4. If a mission is related to clearing a story or a quest you will need to get that quest or story ID.

  - To get a list of quest names for an event, use a query like this:

  ```sql
  SELECT q._Id as QuestId, tl._Text as QuestName, q._PayStaminaSingle <> 0 as IsSolo, q._PayStaminaMulti <> 0 as IsCoOp, q._VariationType as VariationType FROM QuestData q 
  JOIN TextLabel tl on q._QuestViewName = tl._Id
  WHERE q._Gid = /* <event ID from step 1> */
  ORDER BY q._Id
  ```

  - To get a list of story names for an event, use a query like this:

  ```sql
  SELECT q._Id as QuestId, tl._Text as QuestName FROM QuestStory q
  JOIN TextLabel tl on q._SectionName = tl._Id
  WHERE q._GroupId = /* <event ID from step 1> */
  ORDER BY q._Id
  ```

## Tips

- You may find it useful to invoke the sqlite3 command with -json for output inspection.
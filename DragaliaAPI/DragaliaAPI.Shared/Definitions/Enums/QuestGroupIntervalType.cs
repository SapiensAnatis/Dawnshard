namespace DragaliaAPI.Shared.Definitions.Enums;

public enum QuestGroupIntervalType
{
    None,
    Daily,
    Weekend,
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    MonAndSun,
    MonAndTue,
    MonAndWed,
    MonAndThu,
    MonAndFri,
    MonAndSat,

    // Why is this 17 did they not count
    WeekendAndMon = 17,
    WeekendAndTue,
    WeekendAndWed,
    WeekendAndThu,
    WeekendAndFri,
    WeekendAndSat,
    EventSchedule,
    MonThuSatSun,
    TueFriSatSun,
    MonWedSatSun,
    WedFriSatSun,
    TueThuSatSun,
    SatAndSun
}

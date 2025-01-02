SELECT '// ' || VMEM.Description || '
new Mission() { MissionId = ' || VMEM.Id || ' },' FROM "View_EventName"
JOIN main.View_MemoryEventMission VMEM on View_EventName.EventId = VMEM.EventId
where Name like 'Dream%' and IsMemoryEvent = 1
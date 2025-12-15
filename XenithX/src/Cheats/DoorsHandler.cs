using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZenithX;
public static class DoorsHandler
{
    public static List<SystemTypes> GetDoorRooms()
    {
        if (!Utils.isShip || ShipStatus.Instance.AllDoors.Count <= 0) return [];

        return ShipStatus.Instance.AllDoors.Select(d => d.Room).Distinct().ToList();
    }

    public static List<OpenableDoor> GetDoorsInRoom(SystemTypes room)
    {
        if (!Utils.isShip || ShipStatus.Instance.AllDoors.Count <= 0) return [];

        return ShipStatus.Instance.AllDoors.Where(d => d.Room == room).ToList();
    }

    public static string GetStatusOfDoorsInRoom(SystemTypes room, bool colorize)
    {
        var doorsInRoom = GetDoorsInRoom(room);
        if (doorsInRoom.Count <= 0) return "N/A";
        if (doorsInRoom.All(d => d.IsOpen)) return colorize ? "<color=#00FF00>Open</color>" : "Open";
        if (doorsInRoom.All(d => !d.IsOpen)) return colorize ? "<color=#FF0000>Closed</color>" : "Closed";
        return colorize ? "<color=#FFFF00>Mixed</color>" : "Mixed";
    }

    public static void OpenDoorsOfRoom(SystemTypes doorRoom)
    {
        foreach (var door in GetDoorsInRoom(doorRoom))
            OpenDoor(door);
    }

    public static void CloseDoorsOfRoom(SystemTypes doorRoom)
    {
        try { ShipStatus.Instance.RpcCloseDoorsOfType(doorRoom); } catch { }
    }

    public static void OpenAllDoors()
    {
        foreach (var door in ShipStatus.Instance.AllDoors)
            OpenDoor(door);
    }

    public static void CloseAllDoors()
    {
        foreach (var door in ShipStatus.Instance.AllDoors)
        {
            try { ShipStatus.Instance.RpcCloseDoorsOfType(door.Room); } catch { }
        }
    }

    public static void OpenDoor(OpenableDoor openableDoor)
    {
        try { ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Doors, (byte)(openableDoor.Id | 64)); } catch { }
    }
}
using UnityEngine;

namespace ZenithX;

public static class MinigamePatches
{
	public static Minigame currentMinigame;

	public static PlayerTask currentTask;

	public static void CompleteCurrentTask()
	{
		if (!((Object)(object)currentTask == (Object)null))
		{
			Utils.completeTask(currentTask);
			Minigame obj = currentMinigame;
			if (obj != null)
			{
				obj.Close();
			}
			HudManager instance = DestroyableSingleton<HudManager>.Instance;
			if (instance != null)
			{
				instance.SetHudActive(true);
			}
		}
	}
}

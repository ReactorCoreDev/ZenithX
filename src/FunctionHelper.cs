using System;
using UnityEngine;

namespace ZenithX;

public static class FunctionHelper
{
	private static bool _handlingException;

	private static float _lastLogTime;

	public static void Catch(Action action, string name = "Unknown")
	{
		if (action == null)
		{
			return;
		}
		try
		{
			action();
		}
		catch (Exception value)
		{
			if (_handlingException)
			{
				return;
			}
			_handlingException = true;
			try
			{
				if (CheatToggles.debugMode && Time.time - _lastLogTime > 0.25f)
				{
					_lastLogTime = Time.time;
					ZenithX.Log($"[{name}] {value}");
				}
			}
			catch
			{
			}
			_handlingException = false;
		}
	}
}

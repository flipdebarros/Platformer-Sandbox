using UnityEngine;

public static class PauseManager
{
	public static bool IsPaused { get; private set; }

	public static void Pause () {
		IsPaused = !IsPaused;
		AudioListener.pause = IsPaused;
		Time.timeScale = IsPaused ? 0f : 1f;
	}
}

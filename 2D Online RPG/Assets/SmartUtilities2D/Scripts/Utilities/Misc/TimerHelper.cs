using System.Collections.Generic;
using UnityEngine;

public class TimerHelper {
	public float time = 0;

	public TimerHelper ()
	{
		Reset();
	}

	public void Reset() {
		time = Time.realtimeSinceStartup;
	}

	static public TimerHelper Create()
	{
		return(new TimerHelper());
	}

	public float GetMillisecs()
	{
		return((Time.realtimeSinceStartup - time) * 1000);
	}

	public float Get()
	{
		return((Time.realtimeSinceStartup - time));
	}
}

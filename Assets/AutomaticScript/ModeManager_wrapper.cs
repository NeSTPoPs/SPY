using UnityEngine;
using FYFY;

public class ModeManager_wrapper : BaseWrapper
{
	public FYFY.MainLoop mloop;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "mloop", mloop);
	}

	public void setPlayingMode()
	{
		MainLoop.callAppropriateSystemMethod (system, "setPlayingMode", null);
	}

	public void setEditMode()
	{
		MainLoop.callAppropriateSystemMethod (system, "setEditMode", null);
	}

}

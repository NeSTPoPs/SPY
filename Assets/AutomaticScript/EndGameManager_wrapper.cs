using UnityEngine;
using FYFY;

public class EndGameManager_wrapper : BaseWrapper
{
	public UnityEngine.GameObject playButtonAmount;
	public UnityEngine.GameObject endPanel;
	public FYFY.MainLoop mloop;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "playButtonAmount", playButtonAmount);
		MainLoop.initAppropriateSystemField (system, "endPanel", endPanel);
		MainLoop.initAppropriateSystemField (system, "mloop", mloop);
	}

	public void cancelEnd()
	{
		MainLoop.callAppropriateSystemMethod (system, "cancelEnd", null);
	}

}

using UnityEngine;
using FYFY;

public class SendStatements_wrapper : BaseWrapper
{
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
	}

	public void initGBLXAPI()
	{
		MainLoop.callAppropriateSystemMethod (system, "initGBLXAPI", null);
	}

	public void sendStatement()
	{
		MainLoop.callAppropriateSystemMethod (system, "sendStatement", null);
	}

	public void pushedPause()
	{
		MainLoop.callAppropriateSystemMethod (system, "pushedPause", null);
	}

}

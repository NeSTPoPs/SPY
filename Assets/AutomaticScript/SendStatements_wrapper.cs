using UnityEngine;
using FYFY;

public class SendStatements_wrapper : BaseWrapper
{
	public System.Int32 score;
	public System.Int32 scoreMaxNiveau;
	public System.Int32 nb_stars;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "score", score);
		MainLoop.initAppropriateSystemField (system, "scoreMaxNiveau", scoreMaxNiveau);
		MainLoop.initAppropriateSystemField (system, "nb_stars", nb_stars);
	}

	public void initGBLXAPI()
	{
		MainLoop.callAppropriateSystemMethod (system, "initGBLXAPI", null);
	}

	public void sendStatement()
	{
		MainLoop.callAppropriateSystemMethod (system, "sendStatement", null);
	}

	public void sendStatementserie()
	{
		MainLoop.callAppropriateSystemMethod (system, "sendStatementserie", null);
	}

	public void initFiles()
	{
		MainLoop.callAppropriateSystemMethod (system, "initFiles", null);
	}

	public void start_Timer()
	{
		MainLoop.callAppropriateSystemMethod (system, "start_Timer", null);
	}

	public void pause_Timer()
	{
		MainLoop.callAppropriateSystemMethod (system, "pause_Timer", null);
	}

	public void pushedPause()
	{
		MainLoop.callAppropriateSystemMethod (system, "pushedPause", null);
	}

	public void pushedPlay()
	{
		MainLoop.callAppropriateSystemMethod (system, "pushedPlay", null);
	}

	public void pushedRestart()
	{
		MainLoop.callAppropriateSystemMethod (system, "pushedRestart", null);
	}

	public void nextLevel()
	{
		MainLoop.callAppropriateSystemMethod (system, "nextLevel", null);
	}

}

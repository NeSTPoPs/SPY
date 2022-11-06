using UnityEngine;
using FYFY;

public class EditorSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject canvas;
	public UnityEngine.GameObject initialInstance;
	public System.Int32 gridSize;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "canvas", canvas);
		MainLoop.initAppropriateSystemField (system, "initialInstance", initialInstance);
		MainLoop.initAppropriateSystemField (system, "gridSize", gridSize);
	}

	public void selectInstance(UnityEngine.GameObject go)
	{
		MainLoop.callAppropriateSystemMethod (system, "selectInstance", go);
	}

}

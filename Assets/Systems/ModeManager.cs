using UnityEngine;
using FYFY;
using TMPro;

/// <summary>
/// This system enables to manage game mode: playmode vs editmode
/// </summary>
public class ModeManager : FSystem {

	private Family f_playingMode = FamilyManager.getFamily(new AllOfComponents(typeof(PlayMode)));
	private Family f_editingMode = FamilyManager.getFamily(new AllOfComponents(typeof(EditMode)));

	public static ModeManager instance;

	public MainLoop mloop;

	SendStatements_wrapper ssw;

	public ModeManager()
	{
		instance = this;
	}

	protected override void onStart()
	{

		f_playingMode.addEntryCallback(delegate { 
			// remove all EditMode
			foreach(GameObject editModeGO in f_editingMode)
				foreach (EditMode em in editModeGO.GetComponents<EditMode>())
					GameObjectManager.removeComponent(em);
			if (mloop != null)
			{
				SendStatements_wrapper ssw = mloop.GetComponent<SendStatements_wrapper>();
				ssw.pause_Timer();
			}
		});

		f_editingMode.addEntryCallback(delegate {
			// remove all PlayMode
			foreach (GameObject editModeGO in f_playingMode)
				foreach (PlayMode em in editModeGO.GetComponents<PlayMode>())
					GameObjectManager.removeComponent(em);
			if (mloop != null)
			{
				SendStatements_wrapper ssw = mloop.GetComponent<SendStatements_wrapper>();
				ssw.start_Timer();
			}
		});
	}

	// Used in ExecuteButton in inspector
	public void setPlayingMode(){
		GameObjectManager.addComponent<PlayMode>(MainLoop.instance.gameObject);
	}
	
	// Used in StopButton and ReloadState in inspector
	public void setEditMode()
	{
		GameObjectManager.addComponent<EditMode>(MainLoop.instance.gameObject);
	}
}

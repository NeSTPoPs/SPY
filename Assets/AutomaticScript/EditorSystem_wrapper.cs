using UnityEngine;
using FYFY;

public class EditorSystem_wrapper : BaseWrapper
{
	public GameData prefabGameData;
	public UnityEngine.GameObject canvas;
	public UnityEngine.GameObject initialInstance;
	public TMPro.TMP_Dropdown dropdownFurniture;
	public TMPro.TMP_Dropdown dropdownScenario;
	public TMPro.TMP_Dropdown dropdownLevels;
	public TMPro.TMP_InputField doorInputField;
	public TMPro.TMP_InputField buttonInputField;
	public TMPro.TMP_InputField robotInputField;
	public TMPro.TMP_InputField sizeGridInputField;
	public TMPro.TMP_InputField levelInputField;
	public UnityEngine.UI.Toggle boutonPannelToggle;
	public UnityEngine.GameObject LevelPannel;
	public UnityEngine.GameObject AddScenarioPannel;
	public UnityEngine.GameObject ConfirmExitPannel;
	public UnityEngine.GameObject ConfirmGridPannel;
	public UnityEngine.GameObject AddLevelPannel;
	public UnityEngine.GameObject ObjectInfoPannel;
	public UnityEngine.GameObject RobotPannel;
	public UnityEngine.GameObject BlockLimits;
	public UnityEngine.GameObject FurniturePannel;
	public UnityEngine.GameObject DoorPannel;
	public UnityEngine.GameObject ButtonPannel;
	public System.Int32 gridSize;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "prefabGameData", prefabGameData);
		MainLoop.initAppropriateSystemField (system, "canvas", canvas);
		MainLoop.initAppropriateSystemField (system, "initialInstance", initialInstance);
		MainLoop.initAppropriateSystemField (system, "dropdownFurniture", dropdownFurniture);
		MainLoop.initAppropriateSystemField (system, "dropdownScenario", dropdownScenario);
		MainLoop.initAppropriateSystemField (system, "dropdownLevels", dropdownLevels);
		MainLoop.initAppropriateSystemField (system, "doorInputField", doorInputField);
		MainLoop.initAppropriateSystemField (system, "buttonInputField", buttonInputField);
		MainLoop.initAppropriateSystemField (system, "robotInputField", robotInputField);
		MainLoop.initAppropriateSystemField (system, "sizeGridInputField", sizeGridInputField);
		MainLoop.initAppropriateSystemField (system, "levelInputField", levelInputField);
		MainLoop.initAppropriateSystemField (system, "boutonPannelToggle", boutonPannelToggle);
		MainLoop.initAppropriateSystemField (system, "LevelPannel", LevelPannel);
		MainLoop.initAppropriateSystemField (system, "AddScenarioPannel", AddScenarioPannel);
		MainLoop.initAppropriateSystemField (system, "ConfirmExitPannel", ConfirmExitPannel);
		MainLoop.initAppropriateSystemField (system, "ConfirmGridPannel", ConfirmGridPannel);
		MainLoop.initAppropriateSystemField (system, "AddLevelPannel", AddLevelPannel);
		MainLoop.initAppropriateSystemField (system, "ObjectInfoPannel", ObjectInfoPannel);
		MainLoop.initAppropriateSystemField (system, "RobotPannel", RobotPannel);
		MainLoop.initAppropriateSystemField (system, "BlockLimits", BlockLimits);
		MainLoop.initAppropriateSystemField (system, "FurniturePannel", FurniturePannel);
		MainLoop.initAppropriateSystemField (system, "DoorPannel", DoorPannel);
		MainLoop.initAppropriateSystemField (system, "ButtonPannel", ButtonPannel);
		MainLoop.initAppropriateSystemField (system, "gridSize", gridSize);
	}

	public void resizeGrid()
	{
		MainLoop.callAppropriateSystemMethod (system, "resizeGrid", null);
	}

	public void setActiveLevelPannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveLevelPannel", b);
	}

	public void setActiveObjectInfoPannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveObjectInfoPannel", b);
	}

	public void setActiveRobotPannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveRobotPannel", b);
	}

	public void setActiveFurniturePannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveFurniturePannel", b);
	}

	public void setActiveDoorPannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveDoorPannel", b);
	}

	public void setActiveButtonPannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveButtonPannel", b);
	}

	public void setActiveAddScenario(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveAddScenario", b);
	}

	public void setActiveAddLevelPannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveAddLevelPannel", b);
	}

	public void setActiveConfirmGridPannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveConfirmGridPannel", b);
	}

	public void setActiveConfirmExitPannel(System.Boolean b)
	{
		MainLoop.callAppropriateSystemMethod (system, "setActiveConfirmExitPannel", b);
	}

	public void updateScenarioList()
	{
		MainLoop.callAppropriateSystemMethod (system, "updateScenarioList", null);
	}

	public void CreateScenario()
	{
		MainLoop.callAppropriateSystemMethod (system, "CreateScenario", null);
	}

	public void UpdateLevels()
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateLevels", null);
	}

	public void CreateLevel()
	{
		MainLoop.callAppropriateSystemMethod (system, "CreateLevel", null);
	}

	public void DisplayGameDataInfo()
	{
		MainLoop.callAppropriateSystemMethod (system, "DisplayGameDataInfo", null);
	}

	public void selectInstance(UnityEngine.GameObject go)
	{
		MainLoop.callAppropriateSystemMethod (system, "selectInstance", go);
	}

	public void selectDrawMode()
	{
		MainLoop.callAppropriateSystemMethod (system, "selectDrawMode", null);
	}

	public void selectEditMode()
	{
		MainLoop.callAppropriateSystemMethod (system, "selectEditMode", null);
	}

	public void titleScreen()
	{
		MainLoop.callAppropriateSystemMethod (system, "titleScreen", null);
	}

	public void LoadAllFurniture()
	{
		MainLoop.callAppropriateSystemMethod (system, "LoadAllFurniture", null);
	}

	public void UpdateFurnitureGo()
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateFurnitureGo", null);
	}

	public void UpdateDoorGo()
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateDoorGo", null);
	}

	public void UpdateSpawnGo()
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateSpawnGo", null);
	}

	public void UpdateButtonGo()
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateButtonGo", null);
	}

	public void LoadFurnitureGo()
	{
		MainLoop.callAppropriateSystemMethod (system, "LoadFurnitureGo", null);
	}

	public void LoadDoorGo()
	{
		MainLoop.callAppropriateSystemMethod (system, "LoadDoorGo", null);
	}

	public void LoadSpawnGo()
	{
		MainLoop.callAppropriateSystemMethod (system, "LoadSpawnGo", null);
	}

	public void LoadButtonGo()
	{
		MainLoop.callAppropriateSystemMethod (system, "LoadButtonGo", null);
	}

	public void SaveLevel()
	{
		MainLoop.callAppropriateSystemMethod (system, "SaveLevel", null);
	}

}

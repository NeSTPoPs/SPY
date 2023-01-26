using UnityEngine;
using FYFY;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Xml;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Text;

public class EditorSystem : FSystem {

	public static EditorSystem instance;

    public GameData prefabGameData;

    public GameObject canvas;
	public GameObject initialInstance;
	public TMP_Dropdown dropdownFurniture;
    public TMP_Dropdown dropdownScenario;
    public TMP_Dropdown dropdownLevels;
    public TMP_InputField doorInputField;
    public TMP_InputField buttonInputField;
    public TMP_InputField robotInputField;
    public TMP_InputField sizeGridInputField;
    public TMP_InputField levelInputField;
    public Toggle boutonPannelToggle;

    public GameObject LevelPannel;
    public GameObject AddScenarioPannel;
    public GameObject ConfirmExitPannel;
    public GameObject ConfirmGridPannel;
    public GameObject AddLevelPannel;
    public GameObject ObjectInfoPannel;
    public GameObject RobotPannel;

    public GameObject BlockLimits;
    
    public GameObject FurniturePannel;
    public GameObject DoorPannel;
    public GameObject ButtonPannel;


    public int gridSize;

    
    private GameObject selectedInstance;
	private Dictionary<string, string> furnitureNameToPath = new Dictionary<string, string>();
    private Dictionary<string, List<string>> defaultCampaigns = new Dictionary<string, List<string>>(); // List of levels for each default campaign
    private Dictionary<string, string> scenarioNameToPath;
    private GameObject activeObject = null;

    private GameData gameData;

    private Vector3 mousePos;
	private bool moveGhost;
    private bool mousePressed;
    private Vector3 tileGridDimension;
	private Vector2 sizeOfObjects;

	private GameObject[] grid;

    private int orientation;

	private string editMode;
    private string previousMode;
    private string scenarioPath;
    private string levelPath;
    private string activeScenario;

	public EditorSystem()
	{
		instance = this;
	}

    // Use to init system before the first onProcess call
    protected override void onStart()
    {
        mousePressed = false;
        grid = InstantiateGrid(gridSize);
        selectedInstance = InstantiateOnConvas(initialInstance);
        RectTransform objectRectTransform = canvas.GetComponent<RectTransform>();
        moveGhost = false;
        editMode = "draw";
        orientation = 0;
        if (!GameObject.Find("GameData"))
        {
            gameData = UnityEngine.Object.Instantiate(prefabGameData);
            gameData.name = "GameData";
            GameObjectManager.dontDestroyOnLoadAndRebind(gameData.gameObject);
        }
        else
        {
            gameData = GameObject.Find("GameData").GetComponent<GameData>();
        }

        gameData.levels = new Dictionary<string, XmlNode>();
        gameData.scenario = new List<string>();

        LoadAllFurniture();
        updateScenarioList();
        sizeGridInputField.text = gridSize.ToString();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {

        if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
        }

        if (Input.mousePosition.x > 0.2 * Screen.width && Input.mousePosition.x < Screen.width - 0.2 * Screen.width && editMode == "draw")
        {
            if (moveGhost == false)
            {
                moveGhost = true;
                selectedInstance.SetActive(true);
            }
            UpdateGhostPosition();
        }
        else
        {
            if (moveGhost)
            {
                moveGhost = false;
                selectedInstance.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
        }

        if (Input.GetMouseButtonDown(0) && editMode == "draw")
        {
            int index = getGridIndexWithPos(Input.mousePosition);
            if (index != -1)
            {
                replaceGridItem(selectedInstance, index);
            }
        }

        if (Input.GetMouseButtonDown(1) && editMode == "draw")
        {
            //Rotate instance
            orientation = NextOrientation(orientation);
            selectedInstance.transform.Rotate(new Vector3(0, 0, 90));
        }

        if (Input.GetMouseButtonDown(0) && editMode == "edit")
        {
            string obj = getObjectTypeWithPosition(Input.mousePosition);
            Debug.Log("Object selected : " + obj);
        }
    }

    private GameObject[] InstantiateGrid(int size){
		//get resolution screen
		float height = Screen.height;
		float width = Screen.width;
		float go_side = height/size; // size of the gameobject

		GameObject[] res = new GameObject[size*size];
		for(int i = 0; i < size*size; i++){
			//Instantiate empty gameobject 
			res[i] = GameObject.Instantiate(initialInstance);
			float x = width/2f + ( (-size/2f + (i%size) ) * go_side);
			float y = (int)(i/size) * go_side + go_side/2f;	
			GameObjectManager.bind(res[i]);
			GameObjectManager.setGameObjectParent(res[i], canvas, true);
			Vector2 screenPos = new Vector2(x,y);
			Vector3 goCoords;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(initialInstance.GetComponent<RectTransform>(), screenPos, Camera.main ,out goCoords);
			res[i].transform.position = goCoords;
		}

		//Get the correct length in world position
		Vector2 point1_px = new Vector2(res[0].transform.position.x,res[0].transform.position.y);
		Vector2 point2_px = new Vector2(res[1].transform.position.x,res[1].transform.position.y);
		
		sizeOfObjects = new Vector2(point2_px.x - point1_px.x, point2_px.x - point1_px.x);

		//Set correct length
		for(int i = 0; i< size*size; i++){
			res[i].GetComponent<RectTransform>().sizeDelta = sizeOfObjects;
		}
		return res;
	}

	private int getGridIndexWithPos(Vector2 position){
		//Return the grid index in function of the position on screen set as parameters
		int height = Screen.height;
		int width = Screen.width;
		float go_side = height/gridSize;

		if(position.x + go_side/2 - (width - height)/2 < 0)
			return -1;

		int i = (int)(position.x + (int)go_side/2 - (width - height)/2) / (int)go_side ; 
		int j = (int)(position.y) / (int)go_side ; 

		if(i < 0 || i >= gridSize || j < 0 || j >= gridSize){
			return -1;
		}

		return i+gridSize*j;
	}

    public void resizeGrid()
    {
        //Reset grid and resize it by the value in the inputfield sizeGrid
        foreach(GameObject go in grid)
        {
            if (GameObjectManager.isBound(go))
            {
                GameObjectManager.unbind(go);
            }
            GameObject.Destroy(go);
        }
        gridSize = int.Parse(sizeGridInputField.text);
        if (gridSize < 5)
        {
            gridSize = 5;
        }
        if (gridSize > 30)
        {
            gridSize = 30;
        }
        grid = InstantiateGrid(gridSize);
        sizeGridInputField.text = gridSize.ToString();
    }

    private void replaceGridItem(GameObject go, int gridIndex){
		//Change sprite of old to new
		GameObject new_go = GameObject.Instantiate(go);
		GameObjectManager.bind(new_go);
		GameObjectManager.setGameObjectParent(new_go, canvas, false);

		if(GameObjectManager.isBound(grid[gridIndex])){
			GameObjectManager.unbind(grid[gridIndex]);
		}
		GameObject.Destroy(grid[gridIndex]);
		grid[gridIndex] = new_go;

        OrientableEdit orientation_go = new_go.GetComponent<OrientableEdit>();
        if (orientation_go)
        {
            orientation_go.direction = orientation;
        }

        //grid[gridIndex].GetComponent<RawImage>().texture = go.GetComponent<RawImage>().texture; 
        //grid[gridIndex].transform.rotation = go.transform.rotation;

    }

	private GameObject InstantiateOnConvas(GameObject go){
		//Instantiate a gameobject instantly on the convas
		GameObject res = GameObject.Instantiate(go);
		GameObjectManager.bind(res);
		GameObjectManager.setGameObjectParent(res, canvas, true);
		res.GetComponent<RectTransform>().sizeDelta = sizeOfObjects;
		return res;
	}

    private void UpdateGhostPosition(){
		//Debug.Log("MOUSE POS :"+Input.mousePosition.ToString());
		int index = getGridIndexWithPos(Input.mousePosition);
		if(index != -1){
			selectedInstance.transform.position = grid[index].transform.position;
		}else{
			RectTransformUtility.ScreenPointToWorldPointInRectangle(selectedInstance.GetComponent<RectTransform>(), Input.mousePosition, Camera.main ,out mousePos);
			selectedInstance.transform.position = mousePos;
		}
		
	}

    public void setActiveLevelPannel(bool b)
    {
        GameObjectManager.setGameObjectState(LevelPannel, b);
        if (b)
        {
            previousMode = editMode;
            editMode = "levelEdit";
        }
        if (!b)
        {
            editMode = previousMode;
        }
    }

    public void setActiveObjectInfoPannel(bool b)
    {
        GameObjectManager.setGameObjectState(ObjectInfoPannel, b);
        if (!b)
        {
            setActiveRobotPannel(false);
            setActiveFurniturePannel(false);
        }
    }

    public void setActiveRobotPannel(bool b)
    {
        GameObjectManager.setGameObjectState(RobotPannel, b);
        if(b)
        {
            setActiveObjectInfoPannel(true);
            setActiveDoorPannel(false);
            setActiveButtonPannel(false);
            setActiveFurniturePannel(false);
        }
    }

    public void setActiveFurniturePannel(bool b)
    {
        GameObjectManager.setGameObjectState(FurniturePannel, b);
        if (b)
        {
            setActiveObjectInfoPannel(true);
            setActiveRobotPannel(false); //Mask other objects pannel
            setActiveDoorPannel(false);
            setActiveButtonPannel(false);
        }
    }

    public void setActiveDoorPannel(bool b)
    {
        GameObjectManager.setGameObjectState(DoorPannel, b);
        if (b)
        {
            setActiveObjectInfoPannel(true);
            setActiveRobotPannel(false); //Mask other objects pannel
            setActiveFurniturePannel(false);
            setActiveButtonPannel(false);
        }
    }

    public void setActiveButtonPannel(bool b)
    {
        GameObjectManager.setGameObjectState(ButtonPannel, b);
        if (b)
        {
            setActiveObjectInfoPannel(true);
            setActiveRobotPannel(false); //Mask other objects pannel
            setActiveFurniturePannel(false);
            setActiveDoorPannel(false);
        }
    }

    public void setActiveAddScenario(bool b)
    {
        GameObjectManager.setGameObjectState(AddScenarioPannel, b);
        if (b)
        {
            setActiveAddLevelPannel(false);
            setActiveConfirmGridPannel(false);
        }
    }

    public void setActiveAddLevelPannel(bool b)
    {
        GameObjectManager.setGameObjectState(AddLevelPannel, b);
        if (b)
        {
            setActiveAddScenario(false);
            setActiveConfirmGridPannel(false);
            setActiveConfirmExitPannel(false);
        }
    }

    public void setActiveConfirmGridPannel(bool b)
    {
        GameObjectManager.setGameObjectState(ConfirmGridPannel, b);
        if (b)
        {
            setActiveAddLevelPannel(false);
            setActiveAddScenario(false);
            setActiveConfirmExitPannel(false);
        }
    }

    public void setActiveConfirmExitPannel(bool b)
    {
        GameObjectManager.setGameObjectState(ConfirmExitPannel, b);
        if (b)
        {
            previousMode = editMode;
            editMode = "levelEdit";
            setActiveAddLevelPannel(false);
            setActiveAddScenario(false);
            setActiveConfirmGridPannel(false);
        }
        if (!b)
        {
            editMode = previousMode;
        }
    }

    public void updateScenarioList()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            activeScenario = dropdownScenario.options[dropdownScenario.value].text;
            scenarioNameToPath = new Dictionary<string, string>();
            dropdownScenario.ClearOptions();
            Debug.Log("Scanning scenarios in "+ Application.streamingAssetsPath+" ...");
            loadLevelsAndScenarios(Application.streamingAssetsPath);
            Debug.Log("Scanning scenarios in " + Application.persistentDataPath + " ...");
            loadLevelsAndScenarios(Application.persistentDataPath);
            Debug.Log("Scan complete");
            foreach (TMP_Dropdown.OptionData data in dropdownScenario.options)
            {
                if (data.text.Equals(activeScenario))
                {
                    dropdownScenario.value = dropdownScenario.options.IndexOf(data);
                    break;
                }
            }
            UpdateLevels();
        }
    }

    private void loadLevelsAndScenarios(string path)
    {
        // try to load all child files
        foreach (string fileName in Directory.GetFiles(path))
        {
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                EditingUtility.removeComments(doc);
                // a valid level must have only one tag "level"
                if (doc.GetElementsByTagName("level").Count == 1)
                    gameData.levels.Add(fileName.Replace("\\", "/"), doc.GetElementsByTagName("level")[0]);
                // try to extract scenario
                extractLevelListFromScenario(fileName, doc);
            }
            catch {  }
        }

        // explore subdirectories
        foreach (string directory in Directory.GetDirectories(path))
            loadLevelsAndScenarios(directory);
    }

    private void extractLevelListFromScenario(string scenarioName, XmlDocument doc)
    {
        // a valid scenario must have only one tag "scenario"
        if (doc.GetElementsByTagName("scenario").Count == 1)
        {
            List<string> levelList = new List<string>();
            foreach (XmlNode child in doc.GetElementsByTagName("scenario")[0])
                if (child.Name.Equals("level"))
                    levelList.Add(Application.streamingAssetsPath + "/" + (child.Attributes.GetNamedItem("name").Value));
            defaultCampaigns[Path.GetFileName(scenarioName)] = levelList; //key = directory name
            
            string shortname = scenarioName.Replace("\\","/").Split('/').Last();
            scenarioNameToPath[shortname] = scenarioName;
            dropdownScenario.AddOptions(new List<string> { shortname.Substring(0, shortname.Length - 4) });
        }
    }

    public void CreateScenario()
    {
        string scenarioName = AddScenarioPannel.transform.Find("ScenarioInputField").GetComponent<TMP_InputField>().text;
        Debug.Log("Creating Scenario : " + scenarioName);
        string path = Application.streamingAssetsPath + "/Scenario/" + scenarioName + ".xml";
        using (FileStream fs = File.Create(path))
        {
            byte[] info = new UTF8Encoding(true).GetBytes("<?xml version=\"1.0\"?>\n<scenario>\n</scenario>");
            fs.Write(info, 0, info.Length);
        }
        return;
    }

    public void UpdateLevels()
    {
        activeScenario = dropdownScenario.options[dropdownScenario.value].text;
        dropdownLevels.ClearOptions();
        //Debug.Log("ACTIVE SCENARIO :" + activeScenario);
        //foreach(string key in defaultCampaigns.Keys)
        //{
        //    Debug.Log("Key :" + key);
        //    foreach(string v in defaultCampaigns[key])
        //    {
        //        Debug.Log(v);
        //    }
        //}
        string scenario = activeScenario + ".xml";
        foreach (string levelPath in defaultCampaigns[scenario])
        {
            string lpshort = levelPath.Replace('\\', '/').Split('/').Last();
            dropdownLevels.AddOptions(new List<string>() { lpshort.Substring(0, lpshort.Length - 4)});
        }
    }

    public void CreateLevel()
    {
        //TODO: modify scenario file name to add the level name. Create default XML File for the level
        XmlDocument doc = new XmlDocument();
        XmlElement elem;

        doc.Load(scenarioNameToPath[activeScenario + ".xml"]);

        if (!scenarioNameToPath.Keys.Contains<string>(activeScenario+".xml"))
        {
            //Scenario doesn't have levels and hasn't been recognized
            scenarioNameToPath[activeScenario+".xml"] = Application.streamingAssetsPath + "/CustomLevels/" + activeScenario + ".xml";
            elem = doc.CreateElement("level");
            elem.SetAttribute("name", "CustomLevels/" + activeScenario + "/" + levelInputField.text + ".xml");
            doc.GetElementsByTagName("scenario")[0].AppendChild(elem);
            doc.Save(scenarioNameToPath[activeScenario + ".xml"]);
            updateScenarioList();
            return;
        }
        

        foreach (XmlNode child in doc.GetElementsByTagName("scenario")[0]) {
            XmlNode nextChild = child.FirstChild;
            if (child.Name.Equals("level"))
            {
                //See if level already is created
                if (child.Attributes.GetNamedItem("name").Value.Split('/').Last().Equals(levelInputField.text + ".xml"))
                {
                    return; //Level already present
                }
            }
        }
        //Create node
        elem = doc.CreateElement("level");
        //Create directory if not exist
        Directory.CreateDirectory(Application.streamingAssetsPath + "/CustomLevels/" + activeScenario);
        elem.SetAttribute("name", "CustomLevels/" + activeScenario + "/" + levelInputField.text + ".xml");
        doc.GetElementsByTagName("scenario")[0].AppendChild(elem);
        doc.Save(scenarioNameToPath[activeScenario + ".xml"]);
        updateScenarioList();
    }

    public void DisplayGameDataInfo()
    {
        if (gameData)
        {
            foreach (string key in gameData.levels.Keys)
            {
                Debug.Log("GameData[" + key + "] = " + gameData.levels[key]);
            } 
        }
    }

    public void selectInstance(GameObject go){
		//Change the selected instance as the go set as parameter
		GameObjectManager.unbind(selectedInstance);
		GameObject.Destroy(selectedInstance);
		selectedInstance = InstantiateOnConvas(go);
		selectedInstance.SetActive(false);
        orientation = 1;
        string name = go.name; //0 = facing north | 1 = facing south | 2 = facing west | 3 = facing east
        if (name.Contains("Treadmill"))
            orientation = 2;
        if (name.Contains("Guard") || name.Contains("Door") || name.Contains("Spawn"))
            orientation = 0;
    }

	public void selectDrawMode(){
		//Change to draw mode
		editMode = "draw";
        setActiveObjectInfoPannel(false);
	}

	public void selectEditMode(){
		//Change to edit mode
		editMode = "edit";
	}

	public void titleScreen()
	{
		GameObjectManager.loadScene("TitleScreen");
	}

	public string getObjectTypeWithPosition(Vector2 position){
		int index = getGridIndexWithPos(position);
		if (index == -1){
			return "void";
		}
        string name = grid[index].name;

		if(name.Contains("Furniture")){
            activeObject = grid[index];
            LoadFurnitureGo();
            setActiveFurniturePannel(true);
		}
        if (name.Contains("Spawn"))
        {
            activeObject = grid[index];
            LoadSpawnGo();
            setActiveRobotPannel(true);
        }
        if (name.Contains("Door"))
        {
            activeObject = grid[index];
            LoadDoorGo();
            setActiveDoorPannel(true);
        }
        if (name.Contains("Button"))
        {
            activeObject = grid[index];
            LoadButtonGo();
            setActiveButtonPannel(true);
        }

        return name;
	}

	public void LoadAllFurniture(){
		var prefabs = Resources.LoadAll<GameObject>("Prefabs/Modern Furniture/Prefabs/").ToList();
		var prefabNames = prefabs.GroupBy(p => p.name).Select(g => g.First().name).ToList();
        

		foreach (var name in prefabNames){
            Debug.Log("PrefabNames : " + name);
            furnitureNameToPath[name] = "Modern Furniture/Prefabs/" + name;
		}
		dropdownFurniture.AddOptions(furnitureNameToPath.Keys.ToList());
	}

    public void UpdateFurnitureGo()
    {
        //called when dropdown is updated
        FurnitureEdit activeFurniture = activeObject.GetComponent<FurnitureEdit>();
        if (activeFurniture)
        {
            activeFurniture.furnitureName = dropdownFurniture.options[dropdownFurniture.value].text;
        }
    }

    public void UpdateDoorGo()
    {
        DoorEdit activeDoor = activeObject.GetComponent<DoorEdit>();
        if (activeDoor)
        {
            activeDoor.Id = int.Parse(doorInputField.text);
        }
    }

    public void UpdateSpawnGo()
    {
        SpawnEdit activeSpawn = activeObject.GetComponent<SpawnEdit>();
        if (activeSpawn)
        {
            activeSpawn.robotName = robotInputField.text;
        }
    }

    public void UpdateButtonGo()
    {
        ButtonEdit activeButton = activeObject.GetComponent<ButtonEdit>();
        if (activeButton)
        {
            activeButton.Id = int.Parse(buttonInputField.text);
            activeButton.State = boutonPannelToggle.isOn ? 1 : 0;
        }
    }

    public void LoadFurnitureGo()
    {
        FurnitureEdit activeFurniture = activeObject.GetComponent<FurnitureEdit>();
        if (activeFurniture)
        {
            int index = dropdownFurniture.options.FindIndex(a => a.text == activeFurniture.furnitureName);
            dropdownFurniture.value = index;
        }
    }

    public void LoadDoorGo(){
        DoorEdit activeDoor = activeObject.GetComponent<DoorEdit>();
        if (activeDoor)
        {
            doorInputField.text = activeDoor.Id.ToString();
        }
	}

	public void LoadSpawnGo(){
        SpawnEdit activeSpawn = activeObject.GetComponent<SpawnEdit>();
        if (activeSpawn)
        {
            robotInputField.text = activeSpawn.robotName ;
        }
	}

	public void LoadButtonGo(){
        ButtonEdit activeButton = activeObject.GetComponent<ButtonEdit>();
        if (activeButton)
        {
            buttonInputField.text = activeButton.Id.ToString();
            boutonPannelToggle.isOn = activeButton.State == 1;
        }
    }

    private int NextOrientation(int orientation)
    {
        //0 = facing north | 1 = facing south | 2 = facing west | 3 = facing east
        switch (orientation)
        {
            case 1:
                return 3;
            case 3:
                return 0;
            case 0:
                return 2;
            case 2:
                return 1;
            default:
                Debug.Log("Unknown orientation " + orientation.ToString());
                return 0;
        }
    }
    
    public void SaveLevel()
    {
        //levelPath = shortnameToScenarioXML[activeScenario];
        foreach (string levelScanned in defaultCampaigns[activeScenario+".xml"])
        {
            if (levelScanned.Replace('\\', '/').Split('/').Last().Equals(dropdownLevels.options[dropdownLevels.value].text + ".xml"))
            {
                Debug.Log("Selected Level path : " + levelScanned);
                levelPath = levelScanned;
                break;
            }
        }

        string map = "";
        string dialogs = "<dialog text =\"Level : "+levelInputField.text+"\" />\n";
        string executionLimit = "";
        string blockLimits = "";
        string players = "";
        string scripts = "";
        string score = "";
        string console = "";
        string treadmill = "";
        string decoration = "";
        string door = "";

        //Save XML level file
        for (int i = 0; i < gridSize*gridSize; i++)
        {
            if(i%gridSize == 0)
            {
                map += "    <line>";
            }

            string X = (i%gridSize).ToString();
            string Y = (i / gridSize).ToString();

            GameObject tile = grid[i];

            if (tile.name.Contains("Wall"))
                map += "<cell value= \"1\" />";

            if (tile.name.Contains("Ground"))
                map += "<cell value= \"0\" />";

            if (tile.name.Contains("Spawn"))
            {
                map += "<cell value= \"2\" />";
                SpawnEdit sp = tile.GetComponent<SpawnEdit>();
                OrientableEdit oe = tile.GetComponent<OrientableEdit>();
                players += "  <player associatedScriptName=\""+sp.robotName+"\" posY=\""+Y+"\" posX=\""+X+"\" direction=\""+oe.direction.ToString() + "\" />\n";
            }

            if (tile.name.Contains("Exit"))
                map += "<cell value= \"3\" />";

            if (tile.name.Contains("Door"))
            {
                map += "<cell value= \"0\" />";
                DoorEdit de = tile.GetComponent<DoorEdit>();
                OrientableEdit oe = tile.GetComponent<OrientableEdit>();
                door += "  <door posY=\"" + Y + "\" posX=\"" + X + "\" slotId =\"" + de.Id.ToString() + "\" direction=\"" + oe.direction.ToString() + "\" />\n";
            }

            if (tile.name.Contains("Furniture"))
            {
                map += "<cell value= \"0\" />";
                FurnitureEdit fe = tile.GetComponent<FurnitureEdit>();
                OrientableEdit oe = tile.GetComponent<OrientableEdit>();
                decoration += "  <decoration name=\""+furnitureNameToPath[fe.furnitureName]+"\" posY =\"" + Y + "\" posX=\"" + X + "\" direction=\"" + oe.direction.ToString() + "\" />\n";
            }

            if (tile.name.Contains("Obstacle"))
            {
                //TODO: Implement obstacle in the future, for now, it is just ground floor
                map += "<cell value= \"0\" />";
            }

            if (tile.name.Contains("Guard"))
            {
                //TODO: Implement Guard in the future, for now, it is just ground floor
                map += "<cell value= \"0\" />";
            }

            if (tile.name.Contains("Box"))
            {
                //TODO: Implement Guard in the future, for now, it is just ground floor
                map += "<cell value= \"0\" />";
            }

            if (tile.name.Contains("Button"))
            {
                map += "<cell value= \"0\" />";
                ButtonEdit be = tile.GetComponent<ButtonEdit>();
                OrientableEdit oe = tile.GetComponent<OrientableEdit>();
                console += "  <console state=\"" + be.State.ToString() + "\" posY=\"" + Y + "\" posX=\"" + X + "\" direction=\"" + oe.direction.ToString() + "\" >\n";
                console += "    <slot slotId=\"" + be.Id.ToString() + "\" />\n";
                console += "  </console>\n";
            }

            if (tile.name.Contains("Treadmill"))
            {
                map += "<cell value= \"0\" />";
                OrientableEdit oe = tile.GetComponent<OrientableEdit>();
                treadmill += "  <treadmill posY=\"" + Y + "\" posX=\"" + X + "\" direction=\"" + oe.direction.ToString() + "\" />\n";
            }

            if (i % gridSize == gridSize - 1)
            {
                map += "</line>\n";
            }
        }

        //BlockLimits
        
        foreach(Transform child in BlockLimits.transform)
        {
            TMP_InputField block = child.transform.GetComponentInChildren(typeof(TMP_InputField), true) as TMP_InputField;
            if(block)
                blockLimits += "    <blockLimit blockType=\"" + block.gameObject.name + "\" limit=\"" + block.text + "\" />\n";
        }
        
        

        using (FileStream fs = File.Create(levelPath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(
                "<?xml version=\"1.0\"?>\n" +
                "<level>\n  <map>\n" + map + "  </map>\n" +
                "  <dialogs>\n" + dialogs + "  </dialogs>\n" +
                executionLimit +
                "  <blockLimits>\n" + blockLimits + "  </blockLimits>\n" +
                decoration +
                console +
                door +
                treadmill +
                players +
                scripts +
                score +
                "</level>");
            fs.Write(info, 0, info.Length);
        }
        return;
    }
}

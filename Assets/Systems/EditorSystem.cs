using UnityEngine;
using UnityEngine.UI;
using FYFY;

public class EditorSystem : FSystem {

	public static EditorSystem instance;

	public GameObject canvas;
	public GameObject initialInstance;
	public int gridSize;

	private GameObject selectedInstance;

	private Vector3 mousePos;
	private bool moveGhost;
	private Vector3 tileGridDimension;
	private Vector2 sizeOfObjects;

	private GameObject[] grid;

	private bool mousePressed;

	public EditorSystem()
	{
		instance = this;
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
		Debug.Log("SIZE OBJECTS FIRST :"+sizeOfObjects);
		return res;
	}

	private int getGridIndexWithPos(Vector2 position){
		int height = Screen.height;
		int width = Screen.width;
		float go_side = height/gridSize;

		if(position.x + go_side/2 - (width - height)/2 < 0)
			return -1;

		int i = (int)(position.x + (int)go_side/2 - (width - height)/2) / (int)go_side ; 
		int j = (int)(position.y) / (int)go_side ; 

		Debug.Log("i :"+ i);
		Debug.Log("j :"+ j);
		if(i < 0 || i >= gridSize || j < 0 || j >= gridSize){
			return -1;
		}

		return i+gridSize*j;
	}

	private void replaceGridItem(GameObject go, int gridIndex){

		//Change sprite of old to new

		grid[gridIndex].GetComponent<RawImage>().texture = go.GetComponent<RawImage>().texture; 
		grid[gridIndex].transform.rotation = go.transform.rotation;

	}

	private GameObject InstantiateOnConvas(GameObject go){
		GameObject res = GameObject.Instantiate(go);
		GameObjectManager.bind(res);
		GameObjectManager.setGameObjectParent(res, canvas, true);
		res.GetComponent<RectTransform>().sizeDelta = sizeOfObjects;
		return res;
	}
	
	// Use to init system before the first onProcess call
	protected override void onStart(){
		mousePressed = false;
		grid = InstantiateGrid(gridSize);
		selectedInstance = InstantiateOnConvas(initialInstance);
		RectTransform objectRectTransform = canvas.GetComponent<RectTransform> ();
		moveGhost = false;
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

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {

		if(Input.GetMouseButtonUp(0)){
			mousePressed = false;
		}

		if(Input.mousePosition.x > 100){
			if(moveGhost == false){
				moveGhost = true;
				selectedInstance.SetActive(true);
			}
			UpdateGhostPosition();
		}
		else{
			if(moveGhost){
				moveGhost = false;
				selectedInstance.SetActive(false);
			}
		}

		if(Input.GetMouseButtonDown(0)){
			mousePressed = true;
			
		}

		if(Input.GetMouseButtonDown(1)){
			//Rotate instance
			selectedInstance.transform.Rotate(new Vector3(0,0,90));
		}

		if(mousePressed){
			int index = getGridIndexWithPos(Input.mousePosition);
			if(index != -1) {
				replaceGridItem(selectedInstance, index);
			}
		}
	}

	public void selectInstance(GameObject go){
		GameObjectManager.unbind(selectedInstance);
		GameObject.Destroy(selectedInstance);
		selectedInstance = InstantiateOnConvas(go);
		selectedInstance.SetActive(false);
	}
}
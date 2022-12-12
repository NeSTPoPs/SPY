using UnityEngine;
using FYFY;
using System.Collections;
using FYFY_plugins.TriggerManager;

/// <summary>
/// Manage position and Direction component to move agent accordingly
/// </summary>
public class TreadmillSystem : FSystem {

	private Family f_movable = FamilyManager.getFamily(new AllOfComponents(typeof(Position),typeof(Direction)), new AnyOfTags("Player"));
	private Family f_treadmill = FamilyManager.getFamily(new AllOfComponents(typeof(Position), typeof(Direction)), new AnyOfTags("Treadmill"));
	private Family f_wall = FamilyManager.getFamily(new AllOfComponents(typeof(Position)), new AnyOfTags("Wall", "Door"), new AnyOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));



	public float turnSpeed;
	public float moveSpeed;
	public AudioClip footSlow;
	public AudioClip footSpeed;
	private GameData gameData;
	private bool surTreadmill;

	protected override void onStart()
	{
		GameObject go = GameObject.Find("GameData");
		if (go != null)
			gameData = go.GetComponent<GameData>();
		/*foreach (GameObject movable in f_movable)
			initAgentDirection(movable);
		f_movable.addEntryCallback(initAgentDirection);*/
	}









	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in f_movable)
        {
			if (surTreadmill)
			{
				go.GetComponent<Animator>().SetFloat("Walk", -1f);
				go.GetComponent<Animator>().SetFloat("Run", -1f);
			}
			foreach (GameObject tm in f_treadmill)
            {
				if ((go.GetComponent<Position>().x == tm.GetComponent<Position>().x) && (go.GetComponent<Position>().y == tm.GetComponent<Position>().y))
				{
					StepSystem.instance.Pause = true;

					if (Mathf.Abs(go.transform.localPosition.z / 3 - go.GetComponent<Position>().x) < 0.01f && Mathf.Abs(go.transform.localPosition.x / 3 - go.GetComponent<Position>().y) < 0.01f) {
						Debug.Log("MMIOUW");
						ApplyForward(go, tm);
						surTreadmill = true;
					}
				}	
			}
			if (surTreadmill && Mathf.Abs(go.transform.localPosition.z / 3 - go.GetComponent<Position>().x) < 0.01f && Mathf.Abs(go.transform.localPosition.x / 3 - go.GetComponent<Position>().y) < 0.01f)
			{
				surTreadmill = false;
				StepSystem.instance.Pause = false;
			}
		}
		/*foreach (GameObject go in f_movable)
		{
			// Manage position
			if (Mathf.Abs(go.transform.localPosition.z / 3 - go.GetComponent<Position>().x) > 0.01f || Mathf.Abs(go.transform.localPosition.x / 3 - go.GetComponent<Position>().y) > 0.01f)
			{
				go.transform.localPosition = Vector3.MoveTowards(go.transform.localPosition, new Vector3(go.GetComponent<Position>().y * 3, go.transform.localPosition.y, go.GetComponent<Position>().x * 3), moveSpeed * gameData.gameSpeed_current * Time.deltaTime);
				if (go.GetComponent<Animator>() && go.tag == "Player")
					playMoveAnimation(go);
			}
			else
			{
				if (go.GetComponent<Animator>() && go.tag == "Player" && go.GetComponent<ForceMoveAnimation>() == null)
				{
					// Stop moving
					go.GetComponent<Animator>().SetFloat("Walk", -1f);
					go.GetComponent<Animator>().SetFloat("Run", -1f);
				}
			}

			// Manage orientation
			Quaternion target = Quaternion.Euler(0, 0, 0);
			switch (go.GetComponent<Direction>().direction)
			{
				case Direction.Dir.North:
					target = Quaternion.Euler(0, -90, 0);
					break;
				case Direction.Dir.East:
					target = Quaternion.Euler(0, 0, 0);
					break;
				case Direction.Dir.West:
					target = Quaternion.Euler(0, 180, 0);
					break;
				case Direction.Dir.South:
					target = Quaternion.Euler(0, 90, 0);
					break;
			}
			if (target.eulerAngles.y != go.transform.eulerAngles.y)
			{
				go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation, target, turnSpeed * gameData.gameSpeed_current * Time.deltaTime);
				if (go.GetComponent<Animator>() && go.tag == "Player")
					go.GetComponent<Animator>().SetFloat("Rotate", 1f);
			}
			else
				if (go.GetComponent<Animator>() && go.tag == "Player")
					go.GetComponent<Animator>().SetFloat("Rotate", -1f);
		}*/
	}


	private void ApplyForward(GameObject go, GameObject tm)
	{
		switch (tm.GetComponent<Direction>().direction)
		{
			case Direction.Dir.North:
				if (!checkObstacle(go.GetComponent<Position>().x, go.GetComponent<Position>().y - 1))
				{
					go.GetComponent<Position>().x = go.GetComponent<Position>().x;
					go.GetComponent<Position>().y = go.GetComponent<Position>().y - 1;
				}
				else
					GameObjectManager.addComponent<ForceMoveAnimation>(go);
				break;
			case Direction.Dir.South:
				if (!checkObstacle(go.GetComponent<Position>().x, go.GetComponent<Position>().y + 1))
				{
					go.GetComponent<Position>().x = go.GetComponent<Position>().x;
					go.GetComponent<Position>().y = go.GetComponent<Position>().y + 1;
				}
				else
					GameObjectManager.addComponent<ForceMoveAnimation>(go);
				break;
			case Direction.Dir.East:
				if (!checkObstacle(go.GetComponent<Position>().x + 1, go.GetComponent<Position>().y))
				{
					go.GetComponent<Position>().x = go.GetComponent<Position>().x + 1;
					go.GetComponent<Position>().y = go.GetComponent<Position>().y;
				}
				else
					GameObjectManager.addComponent<ForceMoveAnimation>(go);
				break;
			case Direction.Dir.West:
				if (!checkObstacle(go.GetComponent<Position>().x - 1, go.GetComponent<Position>().y))
				{
					go.GetComponent<Position>().x = go.GetComponent<Position>().x - 1;
					go.GetComponent<Position>().y = go.GetComponent<Position>().y;
				}
				else
					GameObjectManager.addComponent<ForceMoveAnimation>(go);
				break;
		}
	}



	private bool checkObstacle(int x, int z)
	{
		foreach (GameObject go in f_wall)
		{
			if (go.GetComponent<Position>().x == x && go.GetComponent<Position>().y == z)
				return true;
		}
		return false;
	}
}
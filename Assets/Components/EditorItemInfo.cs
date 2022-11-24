using UnityEngine;
using System;

public class EditorItemInfo : MonoBehaviour
{
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
	[Header ("======Integer info======")]
	public string[] itemIntName;
	public int[] itemInt;

	[Header ("======Float info======")]
	public string[] itemFloatName;
	public float[] itemFloat;

	[Header ("======Boolean info======")]
	public string[] itemBoolName;
	public bool[] itemBool;

	[Header ("======String info======")]
	public string[] itemStringName;
	public string[] itemString;
}

using UnityEngine;

public class UIXRCursor : MonoBehaviour{
	
	public float MaxDistance = 5f;
	public Vector3 Position => transform.position;
	public Vector3 Direction => transform.forward;
}

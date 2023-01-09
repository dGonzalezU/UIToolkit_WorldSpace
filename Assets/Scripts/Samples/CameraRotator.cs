using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{

	Vector2 currentMousePosition;
	Vector2 prevCamPosition;

	[SerializeField]
	private float _speedX = 1f;
	[SerializeField]
	private float _speedY = 1f;

    void Start()
    {
		Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
		currentMousePosition = new Vector2(
			Input.GetAxis ("Mouse X"),
			Input.GetAxis ("Mouse Y")
		);

		transform.Rotate(new Vector3(0,currentMousePosition.x * _speedX * Time.deltaTime,0), Space.World);
		transform.Rotate(new Vector3(-currentMousePosition.y * _speedY * Time.deltaTime,0,0), Space.Self);

	
    }
}

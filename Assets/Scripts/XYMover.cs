using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XYMover : MonoBehaviour
{

	[SerializeField]
	private float _speed = 10f;

    void Update()
	
    {
		transform.position += new Vector3
		(
			Input.GetAxis("Horizontal"),
			Input.GetAxis("Vertical"),
			0
		) * Time.deltaTime * _speed;

    }
}

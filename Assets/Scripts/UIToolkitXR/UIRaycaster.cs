using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIxr
{

public class UIRaycaster : MonoBehaviour{

	Vector2 raycastedPosition;

	[SerializeField]
	private RaycastPoint _point;

	[SerializeField]
	private float _maxDistance = 5f;

	RaycastHit hit;
	MeshRenderer _renderer;

	[SerializeField]
	PanelSettings TargetPanel;

	[SerializeField]
	private TMP_Text _text = null;

	private Func<Vector2, Vector2> m_DefaultRenderTextureScreenTranslation;

	[SerializeField]
	private GameObject _pointerAid;

	private void OnEnable()
	{
		if(TargetPanel){

			// if(m_DefaultRenderTextureScreenTranslation == null){
			// 	m_DefaultRenderTextureScreenTranslation = (pos) => RayToCoordPosition(pos);
			// }
			// TargetPanel.SetScreenToPanelSpaceFunction(m_DefaultRenderTextureScreenTranslation);
			TargetPanel.SetScreenToPanelSpaceFunction(RayToCoordPosition);
		}
		Debug.Log($"Enable", this);
	}

	private void OnDisable()
	{
		TargetPanel.SetScreenToPanelSpaceFunction(null);
	}

	public Vector2 RayToCoordPosition(Vector2 screenPosition){
		Debug.Log($"RayToCoodr", this);
		Vector2 invalidPos = new Vector2(float.NaN, float.NaN);

		if(Physics.Raycast(_point.Position, _point.Direction, out hit, _maxDistance)){
			if(hit.transform.GetComponent<UIRaycasterReciever>() != null){
				Debug.Log($"Hit {hit.point}", hit.transform);
				if(_pointerAid){
					_pointerAid.transform.position = hit.point;
				}
				Vector2	raycastedPosition = hit.textureCoord;
				raycastedPosition.y = 1 - raycastedPosition.y;
				raycastedPosition.x *= TargetPanel.targetTexture.width;
            	raycastedPosition.y *= TargetPanel.targetTexture.height;

				return raycastedPosition;
			}else{
				Debug.Log($"Hit is not reciever", this);
				return invalidPos;
			}
		}else{
			Debug.Log($"No hit", this);
			return invalidPos;
		}
	}

	private void Update()
	{
		if(_text){
			_text.text = $"Pos: ({raycastedPosition.x} , {raycastedPosition.y})"; 
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		if(_point){
			Gizmos.DrawLine(_point.Position,_point.Position +( _point.Direction * _maxDistance));
		}
	}
}
}
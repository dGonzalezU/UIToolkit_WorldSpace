using System;
using UnityEngine;
using UnityEngine.UIElements;

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

	private Func<Vector2, Vector2> m_DefaultRenderTextureScreenTranslation;

	private void OnEnable()
	{
		if(TargetPanel){

			// if(m_DefaultRenderTextureScreenTranslation == null){
			// 	m_DefaultRenderTextureScreenTranslation = (pos) => RayToCoordPosition(pos);
			// }
			TargetPanel.SetScreenToPanelSpaceFunction(RayToCoordPosition);
		}
	}

	private void OnDisable()
	{
		
	}

	public Vector2 RayToCoordPosition(Vector2 screenPosition){
		Vector2 invalidPos = new Vector2(float.NaN, float.NaN);

		if(Physics.Raycast(_point.Position, _point.Direction, out hit, _maxDistance)){
			if(hit.transform.GetComponent<UIRaycasterReciever>() != null){
				Vector2	raycastedPosition = hit.textureCoord;
				raycastedPosition.y = 1 - raycastedPosition.y;
				raycastedPosition.x *= TargetPanel.targetTexture.width;
            	raycastedPosition.y *= TargetPanel.targetTexture.height;

				return raycastedPosition;
			}else{
				return invalidPos;
			}
		}else{
			return invalidPos;
		}
	}

	private void OnGUI()
	{
		GUILayout.Label($"Pos: ({raycastedPosition.x}{raycastedPosition.y})");
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(_point.Position,_point.Position +( _point.Direction * _maxDistance));
	}
}









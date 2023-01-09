using UnityEngine;
using Katas.Experimental;

public class VRDocInterface : MonoBehaviour{
	[SerializeField]
	ChangeText _changeText;
	[SerializeField]
	WorldSpaceUIDocument _uiDoc;

	private void Awake()
	{
		_uiDoc.OnPanelBuilt += _changeText.SetupPanel;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ChangeText : MonoBehaviour
{

	[SerializeField]
	UIDocument _uiDocument;

	Button _redButton;
	Button _greenButton;
	Button _blueButton;

	Label _resultTabel;


	private void Start()
	{
		//For Non World space canvas:
		GetReferences();
		SetEvents();
	}

	//For Worldspace canvas, this initialization depends on the worldspaceUIDocument event
	public void SetupPanel(UIDocument doc)
	{
		_uiDocument = doc;
		GetReferences();
		SetEvents();
	}

	private void SetEvents()
	{
		_redButton?.RegisterCallback<ClickEvent>((e) => SetLabelText("Red"));
		_greenButton?.RegisterCallback<ClickEvent>((e) => SetLabelText("Green"));
		_blueButton?.RegisterCallback<ClickEvent>((e) => SetLabelText("Blue"));
	}

	private void GetReferences(){
		if(_uiDocument == null){
			return;
		}
		_redButton 	= _uiDocument.rootVisualElement.Q<Button>("redButton");
		_redButton.text = "Red";
		_greenButton= _uiDocument.rootVisualElement.Q<Button>("greenButton");
		_greenButton.text = "Green";
		_blueButton = _uiDocument.rootVisualElement.Q<Button>("blueButton");
		_blueButton.text = "Blue";
		_resultTabel = _uiDocument.rootVisualElement.Q<Label>("resultLabel");
	}

	private void SetLabelText(string text){
		_resultTabel.text = text;
	}
  
}

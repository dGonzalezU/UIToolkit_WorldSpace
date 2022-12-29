using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Katas.Experimental;


public class ChangeText : WorldSpaceUIDocument
{

	VisualElement rootVisualElement;

	Button _redButton;
	Button _greenButton;
	Button _blueButton;

	Label _resultTabel;

	protected override void Awake()
	{
		base.Awake();
		OnPanelBuilt += SetupPanel;
	}

	private void SetupPanel()
	{
		Debug.Log($"Setting up Panel by sus", this);
		rootVisualElement = _uiDocument.rootVisualElement;
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
		_redButton 	= rootVisualElement.Q<Button>("redButton");
		_redButton.text = "Red";
		_greenButton= rootVisualElement.Q<Button>("greenButton");
		_greenButton.text = "Green";
		_blueButton = rootVisualElement.Q<Button>("blueButton");
		_blueButton.text = "Blue";

		_resultTabel = rootVisualElement.Q<Label>("resultLabel");
	}

	private void SetLabelText(string text){
		_resultTabel.text = text;
	}
  
}

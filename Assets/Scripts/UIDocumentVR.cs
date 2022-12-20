using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIDocumentVR : MonoBehaviour, IPointerMoveHandler, IPointerUpHandler, IPointerDownHandler,
	ISubmitHandler, ICancelHandler, IMoveHandler, IScrollHandler, ISelectHandler, IDeselectHandler, IDragHandler
{

	[SerializeField]
	RenderTexture _outputTexture;

	[SerializeField]
	PanelSettings _panelSettings;

	[SerializeField]
	UIDocument _document;

	private VisualTreeAsset _visualTreeAsset;

	MeshRenderer _renderer;



	private void Awake()
	{
		_renderer = GetComponent<MeshRenderer>();
		//Disable the included Panel Raycaster.
		var _panelEventHAnlder = _document.rootVisualElement.panel;
		PanelRaycaster _raycaster = _panelEventHandler.GetComponent<PanelRaycaster>();
		if(_raycaster){
			_raycaster.enabled = false;
		}
	}

	protected PanelEventHandler _panelEventHandler;

	//RELAYING EVENTS TO THE PANEL
	#region 

		public void OnCancel(BaseEventData eventData)
		{
			_panelEventHandler?.OnCancel(eventData);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			_panelEventHandler?.OnDeselect(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			//Hanlde this one later
		}

		public void OnMove(AxisEventData eventData)
		{
			_panelEventHandler?.OnMove(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			HandlePointerEvent(eventData);
			_panelEventHandler?.OnPointerDown(eventData);
		}

		public void OnPointerMove(PointerEventData eventData)
		{
			HandlePointerEvent(eventData);
			_panelEventHandler?.OnPointerMove(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			HandlePointerEvent(eventData);
			_panelEventHandler?.OnPointerUp(eventData);
		}

		public void OnScroll(PointerEventData eventData)
		{
			HandlePointerEvent(eventData);
			_panelEventHandler?.OnScroll(eventData);
		}

		public void OnSelect(BaseEventData eventData)
		{
			_panelEventHandler?.OnSelect(eventData);
		}

		public void OnSubmit(BaseEventData eventData)
		{
			_panelEventHandler?.OnSubmit(eventData);
		}
	
	#endregion


	protected readonly HashSet<(BaseEventData, int)> _eventsProcessedInThisFrame = new HashSet<(BaseEventData, int)>();

    void LateUpdate ()
    {
        _eventsProcessedInThisFrame.Clear();
    }


	void HandlePointerEvent (PointerEventData eventData){
		var eventKey = (eventData, eventData.pointerId);

		if(!_eventsProcessedInThisFrame.Contains(eventKey)){
			_eventsProcessedInThisFrame.Add(eventKey);

			
		}
	}
}

public class XRRaycaster : BaseRaycaster
{
	public override Camera eventCamera => throw new System.NotImplementedException();

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		throw new System.NotImplementedException();
	}
}
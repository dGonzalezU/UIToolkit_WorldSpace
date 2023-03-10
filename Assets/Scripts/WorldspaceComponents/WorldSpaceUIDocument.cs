using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using System;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Katas.Experimental
{

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(TrackedDevicePhysicsRaycaster))]
	public class WorldSpaceUIDocument : MonoBehaviour, IPointerMoveHandler, IPointerUpHandler, IPointerDownHandler,
        ISubmitHandler, ICancelHandler, IMoveHandler, IScrollHandler, ISelectHandler, IDeselectHandler, IDragHandler
    {
		[Header("World Space Size Values")]
        [Tooltip("Width of the panel in pixels. The RenderTexture used to render the panel will have this width.")]
        [SerializeField] protected int _panelWidth = 1280;
        [Tooltip("Height of the panel in pixels. The RenderTexture used to render the panel will have this height.")]
        [SerializeField] protected int _panelHeight = 720;
        [Tooltip("Scale of the panel. It is like the zoom in a browser.")]
        [SerializeField] protected float _panelScale = 1.0f;
        [Tooltip("Pixels per world units, it will the termine the real panel size in the world based on panel pixel width and height.")]
        [SerializeField] protected float _pixelsPerUnit = 1280.0f;
		[Space]
		[Header("UI Toolkit Document Values")]
        [Tooltip("Visual tree element object of this panel.")]
        [SerializeField] public VisualTreeAsset _visualTreeAsset;
        [Tooltip("PanelSettings that will be used to create a new instance for this panel.")]
        [SerializeField] public PanelSettings _panelSettingsPrefab;
        [Tooltip("RenderTexture that will be used to create a new instance for this panel.")]
        [SerializeField] protected RenderTexture _renderTexturePrefab;

        [Tooltip("Some input modules (like the XRUIInputModule from the XR Interaction toolkit package) doesn't send PointerMove events. If you are using such an input module, just set this to true so at least you can properly drag things around.")]
        public bool UseDragEventFix = false;

        private PhysicsRaycaster _raycaser;
        
        public Vector2 PanelSize
        {
            get => new Vector2(_panelWidth, _panelHeight);
            set
            {
                _panelWidth = Mathf.RoundToInt(value.x);
                _panelHeight = Mathf.RoundToInt(value.y);
                RefreshPanelSize();
            }
        }

        public float PanelScale
        {
            get => _panelScale;
            set
            {
                _panelScale = value;

                if (_panelSettings != null)
                    _panelSettings.scale = value;
            }
        }

        public VisualTreeAsset VisualTreeAsset
        {
            get => _visualTreeAsset;
            set
            {
                _visualTreeAsset = value;

                if (_uiDocument != null)
                    _uiDocument.visualTreeAsset = value;
            }
        }

        public int PanelWidth { get => _panelWidth; set { _panelWidth = value; RefreshPanelSize(); } }
        public int PanelHeight { get => _panelHeight; set { _panelHeight = value; RefreshPanelSize(); } }
        public float PixelsPerUnit { get => _pixelsPerUnit; set { _pixelsPerUnit = value; RefreshPanelSize(); } }
        public PanelSettings PanelSettingsPrefab { get => _panelSettingsPrefab; set { _panelSettingsPrefab = value; RebuildPanel(); } }
        public RenderTexture RenderTexturePrefab { get => _renderTexturePrefab; set { _renderTexturePrefab = value; RebuildPanel(); } }
        
        protected MeshRenderer _meshRenderer;
		protected MeshFilter _meshFilter;
		protected BoxCollider _boxCollider;
        protected PanelEventHandler _panelEventHandler;


		//V2.0, making this a required component
        protected UIDocument _uiDocument;
        protected PanelSettings _panelSettings;
        protected RenderTexture _outputTexture;
        protected Material _material;

		public delegate void InitializeDocument(UIDocument document);
		public event InitializeDocument OnPanelBuilt;

		private void Awake ()
        {
            PixelsPerUnit = _pixelsPerUnit;
			SetReferences();
        }

		private void Start()
        {
            RebuildPanel();
        }

        /// <summary>
        /// Use this method to initialise the panel without triggering a rebuild (i.e.: when instantiating it from scripts). Start method
        /// will always trigger RebuildPanel(), but if you are calling this after the GameObject started you must call RebuildPanel() so the
        /// changes take effect.
        /// </summary>
        public void InitPanel (int panelWidth, int panelHeight, float panelScale, float pixelsPerUnit, VisualTreeAsset visualTreeAsset, PanelSettings panelSettingsPrefab, RenderTexture renderTexturePrefab)
        {
            _panelWidth = panelWidth;
            _panelHeight = panelHeight;
            _panelScale = panelScale;
            _pixelsPerUnit = pixelsPerUnit;
            _visualTreeAsset = visualTreeAsset;
            _panelSettingsPrefab = panelSettingsPrefab;
            _outputTexture = renderTexturePrefab;
        }

		/// <summary>
		/// Provides a Visual of the panel that will be instanced once the application enters runtime. The Cyan frame marks the forward
		/// It accurately reflects the UI size, since LocalScale is not used to calculate it
		/// </summary>
		private void OnDrawGizmos()
		{
			Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.matrix = rotationMatrix;
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(_panelWidth/PixelsPerUnit, _panelHeight/PixelsPerUnit,0.1f));
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(-Vector3.forward * 0.05f, new Vector3(_panelWidth/PixelsPerUnit, _panelHeight/PixelsPerUnit,0.01f));
		}

        /// <summary>
        /// Rebuilds the panel by destroy current assets and generating new ones based on the configuration.
        /// </summary>
        public void RebuildPanel ()
        {
            DestroyGeneratedAssets();

            // generate render texture
            RenderTextureDescriptor textureDescriptor = _renderTexturePrefab.descriptor;
            textureDescriptor.width = _panelWidth;
            textureDescriptor.height = _panelHeight;
            _outputTexture = new RenderTexture(textureDescriptor);

            // generate panel settings
            _panelSettings = Instantiate(_panelSettingsPrefab);
            _panelSettings.targetTexture = _outputTexture;
            _panelSettings.clearColor = true; // ConstantPixelSize and clearColor are mandatory configs
            _panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            _panelSettings.scale = _panelScale;
            _outputTexture.name = $"{name} - RenderTexture";
            _panelSettings.name = $"{name} - PanelSettings";

            // generate UIDocument
            _uiDocument = gameObject.AddComponent<UIDocument>();
            _uiDocument.panelSettings = _panelSettings;
            _uiDocument.visualTreeAsset = _visualTreeAsset;

            // generate material
            if (_panelSettings.colorClearValue.a < 1.0f)
                _material = new Material(Shader.Find("Unlit/Transparent"));
            else
                _material = new Material(Shader.Find("Unlit/Texture"));
            
            _material.SetTexture("_MainTex", _outputTexture);
            _meshRenderer.sharedMaterial = _material;

            RefreshPanelSize();

            // find the automatically generated PanelEventHandler and PanelRaycaster for this panel and disable the raycaster
            PanelEventHandler[] handlers = FindObjectsOfType<PanelEventHandler>();
                   

            foreach (PanelEventHandler handler in handlers)
            {
                
                if (handler.panel == _uiDocument.rootVisualElement.panel)
                {
                    _panelEventHandler = handler;
                    PanelRaycaster panelRaycaster = _panelEventHandler.GetComponent<PanelRaycaster>();
                    if (panelRaycaster != null)
                        panelRaycaster.enabled = false;
                    break;
                }
            }

			OnPanelBuilt?.Invoke(_uiDocument);
        }

        protected void RefreshPanelSize ()
        {
            if (_outputTexture != null && (_outputTexture.width != _panelWidth || _outputTexture.height != _panelHeight))
            {
                _outputTexture.Release();
                _outputTexture.width = _panelWidth;
                _outputTexture.height = _panelHeight;
                _outputTexture.Create();

                if (_uiDocument != null)
                    _uiDocument.rootVisualElement?.MarkDirtyRepaint();
            }

            transform.localScale = new Vector3(_panelWidth / _pixelsPerUnit, _panelHeight / _pixelsPerUnit, 1.0f);
        }

        protected void DestroyGeneratedAssets ()
        {
            if (_uiDocument) Destroy(_uiDocument);
            if (_outputTexture) Destroy(_outputTexture);
            if (_panelSettings) Destroy(_panelSettings);
            if (_material) Destroy(_material);
        }

		private void SetReferences(){
			_meshRenderer = GetComponent<MeshRenderer>();
			_meshFilter = GetComponent<MeshFilter>();
			_boxCollider = GetComponent<BoxCollider>();
			
		}

		private void Reset(){
			SetReferences();
            
            _meshRenderer.sharedMaterial = null;
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.allowOcclusionWhenDynamic = false;
            _meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            _meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            Vector3 size = _boxCollider.size;
            size.z = 0;
            _boxCollider.size = size;

			GameObject quadGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _meshFilter.sharedMesh = quadGo.GetComponent<MeshFilter>().sharedMesh;
#if UNITY_EDITOR
			DestroyImmediate(quadGo);
#else
            Destroy(quadGo);
#endif
		}

        void OnDestroy ()
        {
            DestroyGeneratedAssets();
        }

#if UNITY_EDITOR
        void OnValidate ()
        {
            if (Application.isPlaying && _material != null && _uiDocument != null)
            {
                if (_uiDocument.visualTreeAsset != _visualTreeAsset)
                    VisualTreeAsset = _visualTreeAsset;
                if (_panelScale != _panelSettings.scale)
                    _panelSettings.scale = _panelScale;
                
                RefreshPanelSize();
            }
        }
#endif

///////////////////////// REDIRECTION OF EVENTS TO THE PANEL
        protected readonly HashSet<(BaseEventData, int)> _eventsProcessedInThisFrame = new HashSet<(BaseEventData, int)>();

        void LateUpdate ()
        {
            _eventsProcessedInThisFrame.Clear();
        }

        public void OnPointerMove (PointerEventData eventData)
        {
            TransformPointerEventForUIToolkit(eventData);
            _panelEventHandler?.OnPointerMove(eventData);
        }

        public void OnPointerDown (PointerEventData eventData)
        {
            TransformPointerEventForUIToolkit(eventData);
            _panelEventHandler?.OnPointerDown(eventData);
        }

        public void OnPointerUp (PointerEventData eventData)
        {
            TransformPointerEventForUIToolkit(eventData);
            _panelEventHandler?.OnPointerUp(eventData);
        }

        public void OnSubmit (BaseEventData eventData)
        {
            _panelEventHandler?.OnSubmit(eventData);
        }

        public void OnCancel (BaseEventData eventData)
        {
            _panelEventHandler?.OnCancel(eventData);
        }

        public void OnMove (AxisEventData eventData)
        {
            _panelEventHandler?.OnMove(eventData);
        }

        public void OnScroll (PointerEventData eventData)
        {
            TransformPointerEventForUIToolkit(eventData);
            _panelEventHandler?.OnScroll(eventData);
        }

        public void OnSelect (BaseEventData eventData)
        {
            _panelEventHandler?.OnSelect(eventData);
        }

        public void OnDeselect (BaseEventData eventData)
        {
            _panelEventHandler?.OnDeselect(eventData);
        }

        public void OnDrag (PointerEventData eventData)
        {
            if (UseDragEventFix)
                OnPointerMove(eventData);
        }

        protected void TransformPointerEventForUIToolkit (PointerEventData eventData)
        {
            var eventKey = (eventData, eventData.pointerId);

            if (!_eventsProcessedInThisFrame.Contains(eventKey))
            {
                _eventsProcessedInThisFrame.Add(eventKey);
                Camera eventCamera = eventData.enterEventCamera ?? eventData.pressEventCamera;

                if (eventCamera != null)
                {
                    // get current event position and create the ray from the event camera
                    Vector3 position = eventData.position;
                    position.z = 1.0f;
                    position = eventCamera.ScreenToWorldPoint(position);
                    Plane panelPlane = new Plane(transform.forward, transform.position);
                    Ray ray = new Ray(eventCamera.transform.position, position - eventCamera.transform.position);

                    if (panelPlane.Raycast(ray, out float distance))
                    {
                        // get local pointer position within the panel
                        position = ray.origin + distance * ray.direction.normalized;
                        position = transform.InverseTransformPoint(position);
                        // compute a fake pointer screen position so it results in the proper panel position when projected from the camera by the PanelEventHandler
                        position.x += 0.5f; position.y -= 0.5f;
                        position = Vector3.Scale(position, new Vector3(_panelWidth, _panelHeight, 1.0f));
                        position.y += Screen.height;
                        // print(new Vector2(position.x, Screen.height - position.y)); // print actual computed position in panel UIToolkit coords

                        // update the event data with the new calculated position
                        eventData.position = position;
                        RaycastResult raycastResult = eventData.pointerCurrentRaycast;
                        raycastResult.screenPosition = position;
                        eventData.pointerCurrentRaycast = raycastResult;
                        raycastResult = eventData.pointerPressRaycast;
                        raycastResult.screenPosition = position;
                        eventData.pointerPressRaycast = raycastResult;
                    }
                }
            }
        }
    }
}

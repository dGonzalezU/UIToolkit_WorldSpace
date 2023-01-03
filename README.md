# ReadMe: World Space VR UIDocuments

## Sample Scene: WorldSpace_VR

The following project illustrates how to setup a UIDocument for Worldspace that can interact with VR elements. 

## Step by step Process:

### UIDocument components:
* Create a UXML Document 
* Create a Panel settings asset 
* Create a Render Texture (only one needed throughout the project)

### VR components
* Create an XR Rig (Any - XRI - Oculus, etc)
* Add the XR Interaction Prefab (Assets/Prefabs/XRInteraction)
	
### In a unity Scene
* Extend the WorldSpaceUIDocument.cs for any custom functionality
```
using Katas.Experimental;

public class MyCustomUIDoc : WorldSpaceUIDocument
{
}
```
* Add the class to a GameObject. Note that MeshFilter, Renderer and Collider are added automatically
* Reference the created UIDocument components in the UIToolkit Document values subsection




## Notes

* Panel Settings Determines the color of the background. To get a clear background set the Alpha to any value below 1.0f
* The UI selection button can be changed in the XR Controller Components (XRInteraction/RayInteractor(LR)/XRController -> UI Press Usage)
* Scale does not affect the size of the World Scale Panels. The panel Width/Height and Pixels per unit do. There's a provided gizmo that will show the Runtime Size/Orientation of the panel. (Blue -> Forward)



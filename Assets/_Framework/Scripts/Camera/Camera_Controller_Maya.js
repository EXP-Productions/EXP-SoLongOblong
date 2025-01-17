/*
    An attempt to write a Maya style navigation script for use with
    Unity runtime cameras. 
    
    This script also exposes a very rudementary framework for selections
    and selection highlighting - also made in the spirit of mimicking Maya et al.
    
    Controls:
        Alt + Left Mouse Button   = Click and drag. Pivot around current focal point.
        Alt + Right Mouse Button  = Click and drag. Zoom (dolly) in and out.
        Alt + Middle Mouse Button = Click and drag. Pan around.
        
        (Shift or Ctrl) + Left Mouse Button = Click on objects to select them. Shift/Ctrl allows multiselection. 
                                              Click on empty background/unselectable object will deselect everything.
        
        F = Focus camera on current selection (one or more objects). Does nothing without a selection.
    
    
    Daniel Skovli - 2012
*/


// SLO Hax brad
public var SetSelectorObject : GameObject;

// Some public settings
var zoomSpeed : float = 1.2f;
var scrollZoomSpeed : float = 1.2f;
var moveSpeed : float = 1.9f;
var rotateSpeed : float = 4.0f;

var smoothing : float = 10;

var startFocalPoint : Vector3;

var optionalMaterialForSelection : Material;

// Some internal placeholders
private var orbitVector : GameObject;
private var materialForSelection : Material;
private var selectedObjects = new ArrayList();
private var selectedObjectsMaterial = new ArrayList();

private var savedCamTransforms : Transform[];
private var savedCapsuleTransforms : Transform[];




// Some construction work
function Start() 
{
	savedCamTransforms = new Transform[ 3 ];
	savedCapsuleTransforms = new Transform[ 3 ];

    // Create a capsule (which will be the lookAt hitTarget and global orbit vector)
    orbitVector = GameObject.CreatePrimitive(PrimitiveType.Capsule);
    orbitVector.name = "Camera LookAt";
   // orbitVector.transform.parent = transform;
    orbitVector.transform.position = startFocalPoint;

    
    
    // Snap the camera to align with the grid in set starting position (otherwise everything gets a bit wonky)
    //transform.position = Vector3(0, 30, 60);
    
    // Point the camera towards the capsule
    transform.LookAt(orbitVector.transform.position, Vector3.up);
    
    // Hide the capsule (disable the mesh renderer)
    orbitVector.GetComponent.<Renderer>().enabled = false;
    orbitVector.GetComponent.<CapsuleCollider >().enabled = false;
    
    // Create material to apply for selections (or use material supplied by user)
    if (optionalMaterialForSelection) 
    {
        materialForSelection = optionalMaterialForSelection;
    } 
    else
    {
        materialForSelection = new Material(Shader.Find("Diffuse"));
        materialForSelection.color = Color.yellow;
    }
}

public var selectTragets = true;

// Call all of our functionality in LateUpdate() to avoid weird behaviour (as seen in Update())
function LateUpdate()
 {

    // Get mouse vectors
    var x = Input.GetAxis("Mouse X");
    var y = Input.GetAxis("Mouse Y");
    
    // Distance between camera and orbitVector. We'll need this in a few places
    var distanceToOrbit : float;
    var currentZoomSpeed : float;

    if(  Input.GetAxis("Mouse ScrollWheel") != 0 )
    {
        distanceToOrbit = Vector3.Distance(transform.position, orbitVector.transform.position);

        var zoom = Input.GetAxis("Mouse ScrollWheel");

        // Refine the rotateSpeed based on distance to orbitVector
        currentZoomSpeed = Mathf.Clamp( scrollZoomSpeed * (distanceToOrbit / 50f), 0.1f, 300.0f);
            
        // Move the camera in/out
        transform.Translate(Vector3.forward * (zoom * currentZoomSpeed * Time.deltaTime));
            
        // If about to collide with the orbitVector, repulse the orbitVector slightly to keep it in front of us
        if (Vector3.Distance(transform.position, orbitVector.transform.position) < 3) {
            orbitVector.transform.Translate(Vector3.forward, transform);
        }
    }
    
    // ALT is pressed, start navigation
    if (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt)) {
    
        // Distance between camera and orbitVector. We'll need this in a few places
        distanceToOrbit = Vector3.Distance(transform.position, orbitVector.transform.position);
    
    
        //RMB - ZOOM
        if (Input.GetMouseButton(1))
         {            
            // Refine the rotateSpeed based on distance to orbitVector
            currentZoomSpeed = Mathf.Clamp(zoomSpeed * (distanceToOrbit / 50), 0.1f, 2.0f);
            
            // Move the camera in/out
            transform.Translate(Vector3.forward * (x * currentZoomSpeed * Time.deltaTime));
            
            // If about to collide with the orbitVector, repulse the orbitVector slightly to keep it in front of us
            if (Vector3.Distance(transform.position, orbitVector.transform.position) < 3) {
                orbitVector.transform.Translate(Vector3.forward, transform);
            }

        
        //LMB - PIVOT
        } 
        else if (Input.GetMouseButton(0)) 
        {
            
            // Refine the rotateSpeed based on distance to orbitVector
            var currentRotateSpeed = Mathf.Clamp(rotateSpeed * (distanceToOrbit / 50), 1.0f, rotateSpeed);
            
            // Temporarily parent the camera to orbitVector and rotate orbitVector as desired
            transform.parent = orbitVector.transform;
            orbitVector.transform.Rotate(Vector3.right * (y * -currentRotateSpeed * Time.deltaTime));
            orbitVector.transform.Rotate(Vector3.up * (x * currentRotateSpeed * Time.deltaTime), Space.World);
            transform.parent = null;
            
            
        //MMB - PAN
        } else if (Input.GetMouseButton(2))
         {
            
            // Calculate move speed
            var translateX = Vector3.right * (x * moveSpeed) * -1 * Time.deltaTime;
            var translateY = Vector3.up * (y * moveSpeed) * -1 * Time.deltaTime;
            
            // Move the camera
            transform.Translate( translateX );
            transform.Translate( translateY );
            
            // Move the orbitVector with the same values, along the camera's axes. In effect causing it to behave as if temporarily parented.
            orbitVector.transform.Translate(translateX, transform);
            orbitVector.transform.Translate(translateY, transform);
        }
        
        
        
    // If we're not currently navigating, grab selection if something is clicked
    } 
    
    else if (Input.GetMouseButtonDown(0) && selectTragets)
     {     
        var hitInfo : RaycastHit;
        var ray : Ray = GetComponent.<Camera>().ScreenPointToRay(Input.mousePosition);
        var allowMultiSelect : boolean = false;
        
        // See if the user is holding in CTRL or SHIFT. If so, enable multiselection
        if(Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) {
            allowMultiSelect = true;
            
       }
        
        var hitTarget : Transform;
        // Something was clicked. Fetch.
        if (Physics.Raycast(ray, hitInfo, GetComponent.<Camera>().farClipPlane))
        {
        
            hitTarget = hitInfo.transform;

            if( hitTarget.gameObject.tag == "Selectable")
            {

            
                // If NOT multiselection, remove all prior selections
                if (!allowMultiSelect) {
                    deselectAll();
                }

            
            
                //Toggle between selected and unselected (depending on current state)
                if (hitTarget.GetComponent.<Renderer>().sharedMaterial != materialForSelection) {
                    selectedObjects.Add(hitTarget.gameObject);
                    selectedObjectsMaterial.Add(hitTarget.gameObject.GetComponent.<Renderer>().sharedMaterial);
                    hitTarget.gameObject.GetComponent.<Renderer>().sharedMaterial = materialForSelection;
            
                } else {
                    var arrayLocation : int = selectedObjects.IndexOf(hitTarget.gameObject);
                    if (arrayLocation == -1) {return;}; //this shouldn't happen. Ever. But still.
                
                    hitTarget.gameObject.GetComponent.<Renderer>().sharedMaterial = selectedObjectsMaterial[arrayLocation];
                    selectedObjects.RemoveAt(arrayLocation);
                    selectedObjectsMaterial.RemoveAt(arrayLocation);
                
                }
            }
            
        // Else deselect all selected objects (ie. click on empty background)
        } else {
            
            // Don't deselect if allowMultiSelect is true
            if (!allowMultiSelect) {deselectAll();};
        }
       
        
        // hax for SLO selection
        if( SetSelectorObject != null && selectedObjects != null && selectedObjects.Count > 0 )
        {
        	SetSelectorObject.SendMessage("SetSelected", selectedObjects[0] );
        	//SetSelectorObject.m_SelectedElement = selectedObjects[0];
        }
        
    // Fetch input of the F-button (focus) -- this is a very dodgy implementation...
    } else if (Input.GetKeyDown("f")) 
    {
        var backtrack = Vector3(0, 0, -1);
        var selectedObject : GameObject;
        
        // If dealing with only one selected object
        if (selectedObjects.Count == 1) 
        {
            selectedObject = selectedObjects[0];
            transform.position = selectedObject.transform.position;
            orbitVector.transform.position = selectedObject.transform.position;
            transform.Translate(backtrack);
        
        // Else we need to average out the position vectors (this is the proper dodgy part of the implementation)
        }
         else if (selectedObjects.Count > 1) 
         {
            selectedObject = selectedObjects[0];
            var average = selectedObject.transform.position;
        
            for (var i = 1; i < selectedObjects.Count; i++) {
                selectedObject = selectedObjects[i];
                average = (average + selectedObject.transform.position) / 2;
            }
            
            transform.position = average;
            orbitVector.transform.position = average;
            transform.Translate(backtrack);
        }
    }
    
}

function SaveView() 
{
	
}

// Function to handle the de-selection of all objects in scene
function deselectAll() 
{

    // Run through the list of selected objects and restore their original materials
    for (var currentItem = 0; currentItem < selectedObjects.Count; currentItem++) {
        var selectedObject : GameObject = selectedObjects[currentItem];
        selectedObject.GetComponent.<Renderer>().sharedMaterial = selectedObjectsMaterial[currentItem];
    }
    
    // Clear both arrays
    selectedObjects.Clear();
    selectedObjectsMaterial.Clear();
}
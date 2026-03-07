using UnityEngine;
using System.Collections.Generic;

public class GrabConnector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject GrabbedObject = null;
    [SerializeField] Entity attachedEntity = null; //can be null. Will disable this entity reference if grabbed.
    //[SerializeField] List<Collider2D> collidersToDisableWhenGrabbed;
    Transform grabbedObjectPreviousParentTransform = null;
    public GameObject GetGrabbedObject()
    {
        return GrabbedObject;
    }
    public void SetGrabbedObject( GameObject newObj)
    {
        if (GrabbedObject != null) //drop existing obj 
        {
            GrabbedObject.transform.parent = grabbedObjectPreviousParentTransform;
            grabbedObjectPreviousParentTransform = null;
            GrabbedObject.GetComponentInChildren<GrabConnector>()?.StopBeingGrabbed();
            GrabbedObject = null;
        }


        if (newObj == null) return; //stop here if theres no new object.

        GrabbedObject = newObj; //grab new obj
        {
            grabbedObjectPreviousParentTransform = GrabbedObject.transform.parent;
            GrabbedObject.transform.parent = transform;

            GrabConnector otherGrabConnector = GrabbedObject.GetComponentInChildren<GrabConnector>();
            if (otherGrabConnector!=null)
            {
                //align the grab connectors
                otherGrabConnector.BecomeGrabbed(this);
            }
        }
    }

    public void BecomeGrabbed(GrabConnector grabber)
    {
        //disable entity script & colliders if they exist
        if (attachedEntity!=null) attachedEntity.enabled = false;


        transform.parent.position = grabber.transform.position + transform.localPosition; 
    }

    public void StopBeingGrabbed()
    {
        if (attachedEntity!=null) attachedEntity.enabled = true;

    }
}

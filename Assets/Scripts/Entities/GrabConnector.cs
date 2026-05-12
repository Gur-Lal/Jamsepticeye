using UnityEngine;
using System.Collections.Generic;

public class GrabConnector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject GrabbedObject = null;
    GrabConnector grabbedConnector = null;
    [SerializeField] Entity attachedEntity = null; //can be null. Will disable this entity reference if grabbed.
    //[SerializeField] List<Collider2D> collidersToDisableWhenGrabbed;
    [SerializeField] float trackingSpeed = 3f; //tune for how light the object feels vs laggardly. Lower number feels laggardly.
    public GameObject GetGrabbedObject()
    {
        return GrabbedObject;
    }
    public GrabConnector GetGrabbedConnector()
    {
        return grabbedConnector;
    }
    public void SetGrabbedObject( GameObject newObj)
    {
        if (GrabbedObject != null) //drop existing obj 
        {
            grabbedConnector?.StopBeingGrabbed();
            grabbedConnector = null;
            GrabbedObject = null;
        }


        if (newObj == null) return; //stop here if theres no new object.

        GrabbedObject = newObj; //grab new obj
        {

            grabbedConnector = GrabbedObject.GetComponentInChildren<GrabConnector>();
            if (grabbedConnector!=null)
            {
                //align the grab connectors
                grabbedConnector.BecomeGrabbed(this);
            }
        }
    }

    public void BecomeGrabbed(GrabConnector grabber)
    {
        //disable entity script & colliders if they exist
        //if (attachedEntity!=null) attachedEntity.enabled = false;

    }

    public void StopBeingGrabbed()
    {
        //if (attachedEntity!=null) attachedEntity.enabled = true;

    }

    public void SteerTowardPos(Vector2 targetPos)
    {
        Vector2 delta = targetPos - (Vector2)attachedEntity.transform.position;
        attachedEntity.GetRigidbody().linearVelocity = delta * trackingSpeed; 
    }
}

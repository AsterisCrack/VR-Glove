using UnityEngine;

public class GrabPose : MonoBehaviour
{
    public float grabRadius = 0.2f; // Radius around the hand to check
    public int grabbing = 0;
    public LayerMask grabbableLayers; // LayerMask to limit what can be grabbed
    public Transform grabPoint; // Where to attach the object

    private Rigidbody grabbedObject;

    void Update()
    {
        grabbing = ESP32_com.touch;
        if (grabbing == 1 && grabbedObject == null)
        {
            TryGrab();
        }
        else if (grabbing == 0 && grabbedObject != null)
        {
            Release();
        }
    }

    void TryGrab()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, grabRadius, grabbableLayers);

        foreach (var collider in nearby)
        {
            Rigidbody rb = collider.attachedRigidbody;
            if (rb != null)
            {
                grabbedObject = rb;
                grabbedObject.isKinematic = true; // Optional: make object not affected by physics while held
                grabbedObject.transform.SetParent(grabPoint);
                grabbedObject.transform.localPosition = Vector3.zero;
                grabbedObject.transform.localRotation = Quaternion.identity;
                break;
            }
        }
    }

    void Release()
    {
        grabbedObject.isKinematic = false; // Allow physics again
        grabbedObject.transform.SetParent(null);
        grabbedObject = null;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw grab radius in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }
}

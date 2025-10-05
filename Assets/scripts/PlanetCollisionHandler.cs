using UnityEngine;

public class PlanetCollisionHandler : MonoBehaviour
{
    public GameObject markerObject;  // Reference to the marker object in the scene

    // OnCollisionEnter is called when the collision happens
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding with the planet is a meteor
        if (collision.gameObject.CompareTag("Meteor"))
        {
            // Get the collision point
            ContactPoint contact = collision.contacts[0];

            // Update the position of the marker to the collision point
            if (markerObject != null)
            {
                markerObject.transform.position = contact.point;
                Debug.Log("Meteor hit the planet! Marker position set to: " + contact.point);
            }
            else
            {
                Debug.LogWarning("Marker object is not assigned.");
            }
        }
    }
}

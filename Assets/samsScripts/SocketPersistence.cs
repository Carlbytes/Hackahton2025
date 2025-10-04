using UnityEngine;

public class SocketPersistence : MonoBehaviour
{
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    void Awake()
    {
        originalParent = transform.parent;

        if (originalParent == null)
        {
            Debug.logWarning("SocketPersistence is on an object with no parent. Disabling script.", this);
            this.enabled = false;
            return
        }
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;

    void update()
    {
        if (originalParent == null)
            {
                Debug.logWarning("SocketPersistence is on an object with no parent. Disabling script.", this);
                this.enabled = false;
                return
            }

        if (!originalParent.gameObject.activeInHierarchy && transform.parent != null)){
                transform.SetParent(null);

            }

        else if (originalParent.gameObject.activeInHierarchy && transform.parent == null)
            {
                transform.SetParent(originalParent);
                transform.localPosition = originalLocalPosition;
                transform.localRotation = originalLocalRotation;
            }
        }   

}

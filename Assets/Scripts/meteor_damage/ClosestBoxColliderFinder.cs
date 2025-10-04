using UnityEngine;

public class ClosestBoxColliderFinder : MonoBehaviour
{
    public enum SearchMode { ScanScene, OverlapSphere }

    public SearchMode searchMode = SearchMode.ScanScene;
    public float maxRadius = 10f;     
    public LayerMask layerMask = ~0;
    public bool includeTriggers = true;
    public bool includeSelf = false;

    public bool logOnChange = true;
    public bool logEveryFrame = false;

    public bool drawGizmos = true;

    public struct Result
    {
        public BoxCollider collider;
        public Vector3 closestPoint;
        public float distance;
        public bool found;
    }

    private BoxCollider _lastNearest;

    public Result FindClosest()
    {
        Vector3 origin = transform.position;
        BoxCollider closest = null;
        Vector3 bestPoint = default;
        float bestSqr = float.PositiveInfinity;

        void Consider(BoxCollider box)
        {
            if (box == null || !box.enabled) return;
            if (!includeSelf && box.transform == transform) return;
            if (!includeTriggers && box.isTrigger) return;
            if (((1 << box.gameObject.layer) & layerMask) == 0) return;

            Vector3 p = box.ClosestPoint(origin);
            float sqr = (p - origin).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                closest = box;
                bestPoint = p;
            }
        }

        if (searchMode == SearchMode.ScanScene)
        {
#if UNITY_2023_1_OR_NEWER
            var boxes = Object.FindObjectsByType<BoxCollider>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            var boxes = Object.FindObjectsOfType<BoxCollider>();
#endif
            foreach (var box in boxes) Consider(box);
        }
        else
        {
            const int Max = 256;
            Collider[] hits = new Collider[Max];
            int count = Physics.OverlapSphereNonAlloc(
                origin, maxRadius, hits, layerMask,
#if UNITY_2022_1_OR_NEWER
                QueryTriggerInteraction.Collide
#else
                includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore
#endif
            );

            for (int i = 0; i < count && i < Max; i++)
            {
                var hit = hits[i];
                if (hit == null) continue;
#if UNITY_2022_1_OR_NEWER
                if (!includeTriggers && hit.isTrigger) continue;
#endif
                var box = hit as BoxCollider;
                if (box != null) Consider(box);
            }
        }

        return new Result
        {
            collider = closest,
            closestPoint = bestPoint,
            distance = closest ? Mathf.Sqrt(bestSqr) : float.PositiveInfinity,
            found = closest != null
        };
    }

    private Result _lastResult;

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Earth")
        {
            var r = FindClosest();

            if (!r.found)
            {
                if (logEveryFrame && !logOnChange) Debug.Log("no regions found nearby.");
                _lastNearest = null;
                _lastResult = r;
                return;
            }

            //log if changed
            bool changed = r.collider != _lastNearest;

            if ((logOnChange && changed) || (!logOnChange && logEveryFrame))
            {
                Debug.Log($"[Asteroid] Nearest region: '{r.collider.name}'  |  distance: {r.distance:F2} m");
            }

            _lastNearest = r.collider;
            _lastResult = r;
        }
    }

    // Quick accessor if you need the current nearest collider elsewhere
    public BoxCollider GetClosestCollider() => _lastResult.found ? _lastResult.collider : null;

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        var r = Application.isPlaying ? _lastResult : FindClosest();
        if (!r.found) return;

        Gizmos.DrawWireSphere(transform.position, 0.1f);
        Gizmos.DrawLine(transform.position, r.closestPoint);

#if UNITY_EDITOR
        var b = r.collider;
        var size = Vector3.Scale(b.size, b.transform.lossyScale);
        var center = b.transform.TransformPoint(b.center);
        UnityEditor.Handles.DrawWireCube(center, size);
#endif
    }
}

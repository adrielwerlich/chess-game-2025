using UnityEngine;

public class DestroyAfterTimeout : MonoBehaviour
{
    [SerializeField]
    private float timeoutSeconds = 8f;

    void Start()
    {
        Destroy(gameObject, timeoutSeconds);
    }

}

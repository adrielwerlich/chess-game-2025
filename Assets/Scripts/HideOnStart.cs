using UnityEngine;

public class HideOnStart : MonoBehaviour
{

    void Start()
    {
        // Hide the GameObject this script is attached to
        gameObject.SetActive(false);
    }


}

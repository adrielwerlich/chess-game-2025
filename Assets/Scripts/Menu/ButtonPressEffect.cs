using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    public float scaleFactor = 0.9f;
    public float animationSpeed = 0.05f;

    // Sound effect on click
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Hook up the sound to the button's click
        Button button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }


    void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale * scaleFactor));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale));
    }

    IEnumerator ScaleTo(Vector3 targetScale)
    {
        Vector3 startScale = rectTransform.localScale;
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / animationSpeed;
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{

    public static TransitionManager main;

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    private bool isTransitioning = false;
    private UnityAction afterTransition;
    private TransitionType type;



    [SerializeField]
    private Image fadeImage;
    private float startingAlpha;
    private float targetAlpha;

    private Transform zoomTransform;
    private Vector3 targetPosition;
    private Vector3 startingPosition;


    void Start()
    {

    }

    public void Transition(TransitionType type, UnityAction after)
    {
        if (zoomTransform == null)
        {
            zoomTransform = Camera.main.transform;
        }

        this.type = type;
        isTransitioning = true;
        afterTransition = after;
        if (type == TransitionType.FadeIn || type == TransitionType.FadeOut)
        {
            targetAlpha = type == TransitionType.FadeIn ? 0f : 1f;
            startingAlpha = fadeImage.color.a;
        }
    }

    private float transitionTimer = 0f;
    private float transitionDuration = 1f;

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            return;
        }
        transitionTimer += Time.unscaledDeltaTime;
        if (type == TransitionType.FadeIn || type == TransitionType.FadeOut)
        {
            float alpha = Mathf.Lerp(startingAlpha, targetAlpha, transitionTimer / transitionDuration);
            fadeImage.color = new(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
        }
        else
        {
            Vector3 pos = Vector3.Lerp(startingPosition, targetPosition, transitionTimer / transitionDuration);
            zoomTransform.position = pos;
        }
        if (transitionTimer > transitionDuration)
        {
            transitionTimer = 0f;
            if (type == TransitionType.FadeIn || type == TransitionType.FadeOut)
            {
                fadeImage.color = new(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
            }
            else
            {
                zoomTransform.position = targetPosition;
            }
            isTransitioning = false;
            afterTransition.Invoke();
        }
    }
}

public enum TransitionType
{
    FadeIn,
    FadeOut
}
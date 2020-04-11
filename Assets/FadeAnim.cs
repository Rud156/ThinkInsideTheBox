using UnityEngine;

public class FadeAnim : MonoBehaviour
{
    private static readonly int StartAnimParam = Animator.StringToHash("Start");

    public Animator transition;
    public float transitionTime = 1f;

    private void OnEnable() => FaceObject.OnLoaded += FadeOut;

    private void OnDisable() => FaceObject.OnLoaded -= FadeOut;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            FadeOut();
        }
    }

    void FadeOut()
    {
        transition.SetTrigger(StartAnimParam);
    }

    void FadeIn()
    {
    }
}
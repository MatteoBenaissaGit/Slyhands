using UnityEngine;

public class GetResolution : MonoBehaviour
{
    public int TargetResolutionY = 1080;

    [SerializeField] Material outlines;

    private void Awake()
    {
        SetScale();
    }

    private void Update()
    {
        SetScale();
    }

    public void SetScale()
    {
        float scale = Screen.height / TargetResolutionY;

        outlines.SetFloat("_ScreenScaleMult", scale);

        DontDestroyOnLoad(this.gameObject);
    }


}

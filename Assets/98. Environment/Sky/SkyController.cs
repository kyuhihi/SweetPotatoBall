using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class SkyController : MonoBehaviour
{
    [SerializeField]
    private Material dayMaterial;
    [SerializeField]
    private Material nightMaterial;
    [SerializeField]
    private float rotationSpeed = 0.5f;
    [SerializeField]
    private GameObject dayLight;
    [SerializeField]
    private GameObject nightLight;
    [SerializeField]
    private Color dayFogColor;
    [SerializeField]
    private Color nightFogColor;

    private enum SkyState
    {
        Day,
        Night
    }
    private SkyState currentSkyState = SkyState.Day;

    void Start()
    {
        SetSky(SkyState.Night);
    }
    void Update()
    {
        float fRotateValue = RenderSettings.skybox.GetFloat("_Rotation");
        fRotateValue += rotationSpeed * Time.deltaTime;
        if (fRotateValue > 360f)
        {
            fRotateValue -= 360f;
        }
        RenderSettings.skybox.SetFloat("_Rotation", fRotateValue);

    }


    private void SetSky(SkyState skyState)
    {
        switch (skyState)
        {
            case SkyState.Day:
                RenderSettings.skybox = dayMaterial;
                RenderSettings.fogColor = dayFogColor;
                dayLight.SetActive(true);
                nightLight.SetActive(false);
                break;
            case SkyState.Night:
                RenderSettings.skybox = nightMaterial;
                RenderSettings.fogColor = nightFogColor;
                dayLight.SetActive(false);
                nightLight.SetActive(true);
                break;
        }
        RenderSettings.fog = true;
        currentSkyState = skyState;


    }
}


using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioMixer mixer;
    public string volumeParameter;
    public Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        float value = 0;
        mixer.GetFloat(volumeParameter, out value);
        slider.value = Mathf.Pow(10, value / 20);
        slider.onValueChanged.AddListener(SetLevel);
    }

    public void SetLevel(float value)
    {
        if(value == 0)
            mixer.SetFloat(volumeParameter, -80);
        else
            mixer.SetFloat(volumeParameter, Mathf.Log10(value) * 20);
    }
}
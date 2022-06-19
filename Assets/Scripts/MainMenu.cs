using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider slider;

    void Start() {
        slider.value = PlayerPrefs.GetFloat("Volume", 1.0f);
    }

    public void playGame() {
        SceneManager.LoadScene("Game1");
    }

    public void levelSelect() {
        SceneManager.LoadScene("Levels");
    }

    public void quitGame() {
        Application.Quit();
    }

    public void volumeSliderUpdate(float newVolume) {
        PlayerPrefs.SetFloat("Volume", newVolume);
    }
}

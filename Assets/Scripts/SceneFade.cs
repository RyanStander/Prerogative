using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * Made by your homie, ryan stander
 */

public class SceneFade : MonoBehaviour
{
    [SerializeField] private Image panelToFade;
    [SerializeField] private float alphaModifier=2;
    [SerializeField] private string sceneName;
    private float imgAlpha = 0f;
    public bool startSceneTransition;
    private Color originalColor;

    private void Start()
    {
        originalColor = panelToFade.color;
    }

    private void FixedUpdate()
    {
        if (startSceneTransition)
        {
            FadeToSceneSwap();
        }
    }

    private void FadeToSceneSwap()
    {
        imgAlpha += Time.deltaTime/ alphaModifier;
        panelToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b, imgAlpha);
        if (imgAlpha > 1)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}

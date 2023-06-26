using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistenSingleton<SceneLoader>
{
    [SerializeField] private Image transitionImage;
    [SerializeField] private float fadeTime = 3.5f;

    private Color color;

    private const string MAINMENU = "MainMenu";
    private const string GAMEPLAY = "GamePlay";
    private const string SCORING = "Scoring";
    
    IEnumerator LoadCoroutine(string sceneName)
    {
        var loadingScene = SceneManager.LoadSceneAsync(sceneName);
        loadingScene.allowSceneActivation = false;
        
        transitionImage.gameObject.SetActive(true);

        //fade out
        while (color.a < 1f)
        {
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        yield return new WaitUntil(() => loadingScene.progress >= 0.9f);

        loadingScene.allowSceneActivation = true;
        
        //fade in
        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }
        
        transitionImage.gameObject.SetActive(false);
        
    }

    public void LoadMainMenu()
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(MAINMENU));
    }

    public void LoadGamePlay()
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(GAMEPLAY));
    }

    public void LoadScoring()
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(SCORING));
    }
}

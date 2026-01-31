using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GGJ.Result
{
    
    public class FadeIn : MonoBehaviour
{
    [SerializeField] private Image fadeImage; // 黒い画像
    [SerializeField] private float duration = 0.1f; // フェード時間
    private bool isFading = false;

    private void Start()
    {
        // シーン開始時にフェードイン
        StartCoroutine(FadeInCoroutine());//FadeInCoroutineを実行
    }
    private void Update()
    {
        // スペースキーが押されたらフェードアウト開始
        if (Input.GetKeyDown(KeyCode.Space) && !isFading)
        {
            StartCoroutine(FadeOutCoroutine());
        }
    }


    private IEnumerator FadeInCoroutine()
    {
        isFading = true;

        float t = 0f;
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        while (t < duration)//tが duration（フェード時間）に到達するまで繰り返す
        {
            t += Time.deltaTime;　
            c.a = 1f - (t / duration);
            fadeImage.color = c;
            yield return null;
        }

        // 最後に完全に透明にしておく
        c.a = 0f;
        fadeImage.color = c;
        isFading = false;

    }

    private IEnumerator FadeOutCoroutine()
    {
        isFading = true;

        float t = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = t / duration;
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;

        isFading = false;
        SceneManager.LoadScene("Title");
    }

}

}

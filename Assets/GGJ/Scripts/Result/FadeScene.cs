using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GGJ.Result
{
    
    public class FadeScene : MonoBehaviour
{
    [SerializeField] private Image fadeImage; // 黒い画像
    [SerializeField] private float duration = 1.5f; // フェード時間

    private void Start()
    {
        StartCoroutine(FadeInCoroutine());//FadeInCoroutineを実行
    }

    private IEnumerator FadeInCoroutine()
    {
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
    }
}

}

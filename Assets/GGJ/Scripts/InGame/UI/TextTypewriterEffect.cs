using System.Collections;
using UnityEngine;
using TMPro;

namespace GGJ.Core
{
    /// <summary>
    /// テキストを1文字ずつ表示するタイプライター効果
    /// どこからでも使用できる汎用Helperクラス
    /// </summary>
    public static class TextTypewriterEffect
    {
        /// <summary>
        /// テキストを1文字ずつ表示する
        /// </summary>
        /// <param name="textComponent">表示先のTextMeshProUGUI</param>
        /// <param name="text">表示するテキスト</param>
        /// <param name="delayPerCharacter">1文字あたりの待機時間（秒）</param>
        /// <param name="onComplete">完了時のコールバック</param>
        public static IEnumerator TypeText(TextMeshProUGUI textComponent, string text, float delayPerCharacter = 0.05f, System.Action onComplete = null)
        {
            if (textComponent == null)
            {
                onComplete?.Invoke();
                yield break;
            }
            
            textComponent.text = "";
            
            foreach (char c in text)
            {
                textComponent.text += c;
                yield return new WaitForSeconds(delayPerCharacter);
            }
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// テキストを1文字ずつ表示する（スキップ可能）
        /// </summary>
        /// <param name="mono">コルーチンを実行するMonoBehaviour</param>
        /// <param name="textComponent">表示先のTextMeshProUGUI</param>
        /// <param name="text">表示するテキスト</param>
        /// <param name="delayPerCharacter">1文字あたりの待機時間（秒）</param>
        /// <param name="skipAction">スキップ判定用のFunc（trueを返すとスキップ）</param>
        /// <param name="onComplete">完了時のコールバック</param>
        public static IEnumerator TypeTextSkippable(
            MonoBehaviour mono,
            TextMeshProUGUI textComponent,
            string text,
            float delayPerCharacter = 0.05f,
            System.Func<bool> skipAction = null,
            System.Action onComplete = null)
        {
            if (textComponent == null)
            {
                onComplete?.Invoke();
                yield break;
            }
            
            textComponent.text = "";
            yield return null;
            
            bool skipped = false;
            
            foreach (char c in text)
            {
                if (skipAction != null && skipAction())
                {
                    skipped = true;
                    break;
                }
                
                textComponent.text += c;
                yield return new WaitForSeconds(delayPerCharacter);
            }
            
            if (skipped)
                textComponent.text = text;
            
            onComplete?.Invoke();
        }
    }
}

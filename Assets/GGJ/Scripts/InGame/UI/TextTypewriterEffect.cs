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
            Debug.Log($"[TextTypewriterEffect] コルーチン開始: text={text}, length={text.Length}");
            
            if (textComponent == null)
            {
                Debug.LogError("[TextTypewriterEffect] textComponentがnullです");
                onComplete?.Invoke();
                yield break;
            }
            
            Debug.Log($"[TextTypewriterEffect] textComponent={textComponent.name}, enabled={textComponent.enabled}, gameObject.activeInHierarchy={textComponent.gameObject.activeInHierarchy}");
            
            // テキストを先に空にする
            textComponent.text = "";
            
            // 前のフレームの入力をクリアするため、1フレーム待機
            yield return null;
            Debug.Log("[TextTypewriterEffect] 1フレーム待機完了、タイプライター開始");
            
            bool skipped = false;
            int charCount = 0;
            
            foreach (char c in text)
            {
                // スキップチェック
                if (skipAction != null && skipAction())
                {
                    Debug.Log("[TextTypewriterEffect] スキップが検出されました");
                    skipped = true;
                    break;
                }
                
                textComponent.text += c;
                charCount++;
                yield return new WaitForSeconds(delayPerCharacter);
            }
            
            Debug.Log($"[TextTypewriterEffect] 表示完了: charCount={charCount}, skipped={skipped}");
            
            // スキップされた場合は全文表示
            if (skipped)
            {
                textComponent.text = text;
            }
            
            onComplete?.Invoke();
        }
    }
}

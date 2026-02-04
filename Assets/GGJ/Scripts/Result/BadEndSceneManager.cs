using UnityEngine;
using TMPro;
using GGJ.Core;
using GGJ.InGame.UI;

namespace GGJ.Result
{
    /// <summary>
    /// 敗北シーン（BadEnd）の管理
    /// 敗北画面の表示と敗北理由などの表示を担当
    /// </summary>
    public class BadEndSceneManager : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private TextMeshProUGUI loseStoryText; // 敗北ストーリーテキスト
        
        [Header("タイプライター設定")]
        [SerializeField] private float typeSpeed = 0.05f; // 1文字あたりの表示速度（秒）
        
        [Header("コントローラーナビゲーション")]
        [SerializeField] private GameObject firstSelectedObject; // シーン開始時に最初に選択されるUI要素（例：タイトルに戻るボタン）

        private void Start()
        {
            StartCoroutine(PlayStoryTextAnimation());
            SetInitialSelection();
        }


        /// <summary>
        /// ストーリーテキストをタイプライター効果で表示
        /// </summary>
        private System.Collections.IEnumerator PlayStoryTextAnimation()
        {
            if (loseStoryText != null)
            {
                // loseStoryTextから元のテキストを取得（タイプライター効果で上書きされる前に保存）
                string originalText = loseStoryText.text;
                
                yield return TextTypewriterEffect.TypeText(
                    loseStoryText,
                    originalText,
                    typeSpeed
                );
            }
        }

        /// <summary>
        /// コントローラー用の初期選択を設定
        /// </summary>
        private void SetInitialSelection()
        {
            if (firstSelectedObject != null)
            {
                UIHelper.SetFirstSelected(firstSelectedObject);
            }
        }
    }
}

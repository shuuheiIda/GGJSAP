using System;
using System.Collections;
using UnityEngine;
using GGJ.Core;

namespace GGJ.InGame.Transitions
{
    /// <summary>
    /// グリッド分割トランジションエフェクト
    /// SpriteRendererにカスタムシェーダーを適用してトランジションを実現
    /// </summary>
    public class GridTransitionManager : Singleton<GridTransitionManager>
    {
        [Header("トランジション設定")]
        [SerializeField] private Material transitionMaterial; // GridTransitionシェーダーを適用したマテリアル
        [SerializeField] private SpriteRenderer transitionSprite; // トランジション用のSpriteRenderer
        [SerializeField] private float transitionDuration = 1.0f; // トランジション時間
        [SerializeField] private Vector2 gridSize = new Vector2(10, 10); // グリッドサイズ
        [SerializeField] private Color transitionColor = Color.black; // トランジション色
        
        [Header("アニメーションカーブ")]
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("カメラ設定（オプション：画面サイズ自動調整用）")]
        [SerializeField] private UnityEngine.Camera targetCamera; // 対象カメラ（設定すると画面サイズに自動調整）
        
        private Material materialInstance; // Materialのインスタンス（元のアセットを変更しないため）
        
        protected override bool UseDontDestroyOnLoad => false;
        
        protected override void Init()
        {
            if (transitionSprite != null)
            {
                // Materialのインスタンスを作成して使用（元のアセットを変更しないため）
                materialInstance = new Material(transitionMaterial);
                transitionSprite.material = materialInstance;
                transitionSprite.gameObject.SetActive(false);
                
                // カメラが設定されている場合は画面サイズに合わせてスケーリング
                if (targetCamera != null)
                {
                    UpdateSpriteScale();
                }
                else
                {
                    // カメラ未設定の場合は警告（手動でスケール調整されている想定）
                    Debug.LogWarning("[GridTransitionManager] Target Cameraが未設定です。手動でスプライトのスケールを調整してください。");
                }
            }
            else
            {
                Debug.LogError("[GridTransitionManager] TransitionSpriteが設定されていません！");
            }
        }
        
        /// <summary>
        /// スプライトのスケールをカメラサイズに合わせて更新
        /// </summary>
        private void UpdateSpriteScale()
        {
            if (transitionSprite == null || targetCamera == null) return;
            
            // カメラのサイズを取得
            float height = targetCamera.orthographicSize * 2f;
            float width = height * targetCamera.aspect;
            
            // スプライトのサイズを取得（1ユニット = 100ピクセルと仮定）
            Sprite sprite = transitionSprite.sprite;
            if (sprite != null)
            {
                float spriteWidth = sprite.bounds.size.x;
                float spriteHeight = sprite.bounds.size.y;
                
                // スケールを計算
                float scaleX = width / spriteWidth;
                float scaleY = height / spriteHeight;
                
                transitionSprite.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
            
            // カメラの前面に配置
            Vector3 position = targetCamera.transform.position;
            position.z = targetCamera.transform.position.z + targetCamera.nearClipPlane + 0.1f;
            transitionSprite.transform.position = position;
        }
        
        /// <summary>
        /// フェードアウト → アクション → フェードイン のトランジション
        /// </summary>
        /// <param name="onTransitionMiddle">画面が完全に暗くなった時に実行されるアクション</param>
        public void PlayTransition(Action onTransitionMiddle = null)
        {
            StartCoroutine(TransitionCoroutine(onTransitionMiddle));
        }
        
        /// <summary>
        /// トランジションコルーチン
        /// </summary>
        private IEnumerator TransitionCoroutine(Action onTransitionMiddle)
        {
            if (transitionSprite == null || materialInstance == null)
            {
                Debug.LogError("[GridTransitionManager] トランジションを開始できません");
                onTransitionMiddle?.Invoke();
                yield break;
            }
            
            // ランダムシードを設定（毎回違うパターンにする）
            materialInstance.SetFloat("_RandomSeed", UnityEngine.Random.Range(0f, 100f));
            materialInstance.SetVector("_GridSize", gridSize);
            materialInstance.SetColor("_TransitionColor", transitionColor);
            
            // トランジション開始
            transitionSprite.gameObject.SetActive(true);
            
            // フェードアウト（0 → 1）
            yield return FadeCoroutine(0f, 1f, transitionDuration * 0.5f);
            
            // 中間処理（画面切り替えなど）
            onTransitionMiddle?.Invoke();
            
            // 少し待機
            yield return new WaitForSeconds(0.1f);
            
            // フェードイン（1 → 0）
            yield return FadeCoroutine(1f, 0f, transitionDuration * 0.5f);
            
            // トランジション終了
            transitionSprite.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// フェードコルーチン
        /// </summary>
        private IEnumerator FadeCoroutine(float startProgress, float endProgress, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float curvedT = transitionCurve.Evaluate(t);
                float progress = Mathf.Lerp(startProgress, endProgress, curvedT);
                
                materialInstance.SetFloat("_Progress", progress);
                
                yield return null;
            }
            
            // 最終値を確実に設定
            materialInstance.SetFloat("_Progress", endProgress);
        }
        
        /// <summary>
        /// トランジション時間を設定
        /// </summary>
        public void SetTransitionDuration(float duration)
        {
            transitionDuration = Mathf.Max(0.1f, duration);
        }
        
        /// <summary>
        /// グリッドサイズを設定
        /// </summary>
        public void SetGridSize(Vector2 size)
        {
            gridSize = size;
        }
        
        /// <summary>
        /// トランジション色を設定
        /// </summary>
        public void SetTransitionColor(Color color)
        {
            transitionColor = color;
        }
    }
}
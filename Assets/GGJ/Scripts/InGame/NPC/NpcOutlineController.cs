using UnityEngine;
using GGJ.Core;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// NPCのアウトライン制御
    /// プレイヤーが近づいたときにアウトラインの輝度を上げる
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class NpcOutlineController : MonoBehaviour
    {
        [Header("シェーダー設定")]
        [SerializeField] private Shader outlineShader;
        
        [Header("アウトライン設定")]
        [SerializeField] private float outlineWidth = 1.5f;
        
        [Header("通常時の設定")]
        [SerializeField] private Color normalColor = Color.white; // 白色
        [SerializeField] private float normalIntensity = 0.8f; // 通常時の輝度
        
        [Header("強調時の設定")]
        [SerializeField] private Color highlightColor = new Color(0, 1, 0, 1); // 緑色
        [SerializeField] private float highlightIntensity = 2.5f; // 強調時の輝度
        
        [Header("遷移設定")]
        [SerializeField] private float transitionSpeed = 5f; // 変化の速度
        
        private SpriteRenderer spriteRenderer;
        private Material outlineMaterial;
        
        private Color targetColor;
        private Color currentColor;
        private float targetIntensity;
        private float currentIntensity;
        
        private const float IntensityThreshold = 0.01f;
        private const float ColorThreshold = 0.01f;
        
        private static readonly int MainTexProperty = Shader.PropertyToID("_MainTex");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthProperty = Shader.PropertyToID("_OutlineWidth");
        private static readonly int OutlineIntensityProperty = Shader.PropertyToID("_OutlineIntensity");
        
        private void Start()
        {
            InitializeComponents();
            SetupMaterial();
            
            // 初期状態は通常色と通常輝度
            currentColor = normalColor;
            targetColor = normalColor;
            currentIntensity = normalIntensity;
            targetIntensity = normalIntensity;
            UpdateOutlineProperties();
        }
        
        private void Update()
        {
            bool needsUpdate = false;
            
            // 輝度を滑らかに変化させる
            if (Mathf.Abs(currentIntensity - targetIntensity) > IntensityThreshold)
            {
                currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * transitionSpeed);
                needsUpdate = true;
            }
            
            // 色を滑らかに変化させる
            if (Vector4.Distance(currentColor, targetColor) > ColorThreshold)
            {
                currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * transitionSpeed);
                needsUpdate = true;
            }
            
            if (needsUpdate)
            {
                UpdateOutlineProperties();
            }
        }
        
        private void InitializeComponents()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void SetupMaterial()
        {
            if (outlineShader == null)
            {
                Debug.LogError("[NpcOutlineController] アウトラインシェーダーがアタッチされていません！");
                return;
            }
            
            // マテリアルのインスタンスを作成（共有マテリアルを変更しないように）
            outlineMaterial = new Material(outlineShader);
            outlineMaterial.SetTexture(MainTexProperty, spriteRenderer.sprite.texture);
            outlineMaterial.SetColor(OutlineColorProperty, normalColor);
            outlineMaterial.SetFloat(OutlineWidthProperty, outlineWidth);
            
            spriteRenderer.material = outlineMaterial;
        }
        
        private void UpdateOutlineProperties()
        {
            if (outlineMaterial != null)
            {
                outlineMaterial.SetColor(OutlineColorProperty, currentColor);
                outlineMaterial.SetFloat(OutlineIntensityProperty, currentIntensity);
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // プレイヤーが近づいたら色を緑に変更し、輝度を上げる
            if (collision.gameObject.CompareTag(Tags.Player))
            {
                targetColor = highlightColor;
                targetIntensity = highlightIntensity;
            }
        }
        
        private void OnCollisionExit2D(Collision2D collision)
        {
            // プレイヤーが離れたら色を白に戻し、輝度を通常に戻す
            if (collision.gameObject.CompareTag(Tags.Player))
            {
                targetColor = normalColor;
                targetIntensity = normalIntensity;
            }
        }
        
        private void OnDestroy()
        {
            // マテリアルのインスタンスを破棄してメモリリークを防ぐ
            if (outlineMaterial != null)
            {
                Destroy(outlineMaterial);
            }
        }
    }
}

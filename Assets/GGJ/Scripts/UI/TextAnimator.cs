using UnityEngine;
using TMPro;
using System.Collections;

namespace GGJ.UI
{
    /// <summary>
    /// テキストに動的なアニメーション効果を追加
    /// 波打つ動き、拡大縮小、色の変化、回転などを実装
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextAnimator : MonoBehaviour
    {
        [Header("アニメーション設定")]
        [SerializeField] private bool enableWaveEffect = true;
        [SerializeField] private bool enableScaleEffect = true;
        [SerializeField] private bool enableColorEffect = true;
        [SerializeField] private bool enableRotationEffect = false;
        
        [Header("波打ち効果")]
        [SerializeField] private float waveAmplitude = 10f;
        [SerializeField] private float waveFrequency = 2f;
        [SerializeField] private float waveSpeed = 1f;
        
        [Header("拡大縮小効果")]
        [SerializeField] private float scaleAmplitude = 0.1f;
        [SerializeField] private float scaleSpeed = 1f;
        
        [Header("色変化効果")]
        [SerializeField] private Color color1 = Color.white;
        [SerializeField] private Color color2 = new Color(1f, 0.8f, 0.4f);
        [SerializeField] private float colorSpeed = 1f;
        
        [Header("回転効果")]
        [SerializeField] private float rotationAmplitude = 5f;
        [SerializeField] private float rotationSpeed = 1f;
        
        private TextMeshProUGUI textMesh;
        private TMP_TextInfo textInfo;
        private float time;
        
        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }
        
        private void Start()
        {
            StartCoroutine(AnimateText());
        }
        
        /// <summary>
        /// テキストアニメーションのメインループ
        /// </summary>
        private IEnumerator AnimateText()
        {
            textMesh.ForceMeshUpdate();
            
            while (true)
            {
                time += Time.deltaTime;
                
                textMesh.ForceMeshUpdate();
                textInfo = textMesh.textInfo;
                
                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    
                    if (!charInfo.isVisible) continue;
                    
                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;
                    
                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
                    Color32[] colors = textInfo.meshInfo[materialIndex].colors32;
                    
                    Vector3 offset = Vector3.zero;
                    float scale = 1f;
                    float rotation = 0f;
                    Color color = Color.white;
                    
                    // 波打ち効果
                    if (enableWaveEffect)
                    {
                        float waveOffset = Mathf.Sin((time * waveSpeed) + (i * waveFrequency * 0.1f)) * waveAmplitude;
                        offset.y += waveOffset;
                    }
                    
                    // 拡大縮小効果
                    if (enableScaleEffect)
                    {
                        scale = 1f + Mathf.Sin((time * scaleSpeed) + (i * 0.2f)) * scaleAmplitude;
                    }
                    
                    // 回転効果
                    if (enableRotationEffect)
                    {
                        rotation = Mathf.Sin((time * rotationSpeed) + (i * 0.15f)) * rotationAmplitude;
                    }
                    
                    // 色変化効果
                    if (enableColorEffect)
                    {
                        float colorLerp = (Mathf.Sin((time * colorSpeed) + (i * 0.1f)) + 1f) / 2f;
                        color = Color.Lerp(color1, color2, colorLerp);
                    }
                    
                    // 文字の中心座標を計算
                    Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
                    
                    // 各頂点にエフェクトを適用
                    for (int j = 0; j < 4; j++)
                    {
                        Vector3 vertex = vertices[vertexIndex + j];
                        
                        // スケール適用
                        vertex = center + (vertex - center) * scale;
                        
                        // 回転適用
                        if (enableRotationEffect && rotation != 0f)
                        {
                            Vector3 dir = vertex - center;
                            float angle = rotation * Mathf.Deg2Rad;
                            float cos = Mathf.Cos(angle);
                            float sin = Mathf.Sin(angle);
                            vertex = center + new Vector3(
                                dir.x * cos - dir.y * sin,
                                dir.x * sin + dir.y * cos,
                                dir.z
                            );
                        }
                        
                        // オフセット適用
                        vertex += offset;
                        
                        vertices[vertexIndex + j] = vertex;
                        
                        // 色適用
                        if (enableColorEffect)
                        {
                            colors[vertexIndex + j] = color;
                        }
                    }
                }
                
                // メッシュを更新
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    
                    if (enableColorEffect)
                        textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                    
                    textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }
                
                yield return null;
            }
        }
    }
}

using UnityEngine;
using System.IO;
using System;

namespace GGJ.Core
{
    /// <summary>
    /// ビルド後のデバッグ用ログ出力システム
    /// </summary>
    public class DebugLogger : MonoBehaviour
    {
        [Header("ログ設定")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private float logInterval = 1f;
        
        [Header("監視オブジェクト")]
        [SerializeField] private GameObject background;
        [SerializeField] private GameObject player;
        [SerializeField] private Camera mainCamera;
        
        private float timer = 0f;
        private string logFilePath;
        private StreamWriter logWriter;

        private void Start()
        {
            if (!enableLogging) return;
            
            // ログファイルのパスを設定
            logFilePath = Path.Combine(Application.persistentDataPath, "debug_log.txt");
            Debug.Log($"[DebugLogger] ログファイル: {logFilePath}");
            
            // ファイルを開く
            try
            {
                logWriter = new StreamWriter(logFilePath, false);
                logWriter.AutoFlush = true;
                WriteLog("=== デバッグログ開始 ===");
                WriteLog($"時刻: {DateTime.Now}");
                WriteLog($"プラットフォーム: {Application.platform}");
                WriteLog($"解像度: {Screen.width}x{Screen.height}");
                WriteLog("");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DebugLogger] ログファイル作成エラー: {e.Message}");
            }
            
            // 自動検出
            if (background == null)
            {
                background = GameObject.Find("InGameBG");
                if (background != null)
                    WriteLog($"背景を自動検出: {background.name}");
            }
            
            if (player == null)
            {
                player = GameObject.Find("Player");
                if (player != null)
                    WriteLog($"Playerを自動検出: {player.name}");
            }
            
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera != null)
                    WriteLog($"カメラを自動検出: {mainCamera.name}");
            }
            
            // 初期状態をログ
            LogCurrentState();
        }

        private void Update()
        {
            if (!enableLogging) return;
            
            timer += Time.deltaTime;
            if (timer >= logInterval)
            {
                timer = 0f;
                LogCurrentState();
            }
        }

        private void LogCurrentState()
        {
            WriteLog($"--- フレーム {Time.frameCount} ---");
            
            if (background != null)
            {
                var spriteRenderer = background.GetComponent<SpriteRenderer>();
                var image = background.GetComponent<UnityEngine.UI.Image>();
                
                WriteLog($"[背景] 位置: {background.transform.position}");
                WriteLog($"[背景] アクティブ: {background.activeSelf}");
                
                if (spriteRenderer != null)
                {
                    WriteLog($"[背景] SpriteRenderer - Enabled: {spriteRenderer.enabled}");
                    WriteLog($"[背景] Sprite: {(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
                    WriteLog($"[背景] Color: {spriteRenderer.color}");
                    WriteLog($"[背景] Material: {(spriteRenderer.material != null ? spriteRenderer.material.name : "null")}");
                    WriteLog($"[背景] Sorting Layer: {spriteRenderer.sortingLayerName}");
                    WriteLog($"[背景] Order in Layer: {spriteRenderer.sortingOrder}");
                }
                
                if (image != null)
                {
                    WriteLog($"[背景] UI Image - Enabled: {image.enabled}");
                    WriteLog($"[背景] Sprite: {(image.sprite != null ? image.sprite.name : "null")}");
                    WriteLog($"[背景] Color: {image.color}");
                }
            }
            else
            {
                WriteLog("[背景] オブジェクトが見つかりません！");
            }
            
            if (player != null)
            {
                WriteLog($"[Player] 位置: {player.transform.position}");
                WriteLog($"[Player] アクティブ: {player.activeSelf}");
                
                var spriteRenderer = player.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    WriteLog($"[Player] SpriteRenderer - Enabled: {spriteRenderer.enabled}");
                    WriteLog($"[Player] Sorting Layer: {spriteRenderer.sortingLayerName}");
                    WriteLog($"[Player] Order in Layer: {spriteRenderer.sortingOrder}");
                }
            }
            else
            {
                WriteLog("[Player] オブジェクトが見つかりません！");
            }
            
            if (mainCamera != null)
            {
                WriteLog($"[Camera] 位置: {mainCamera.transform.position}");
                WriteLog($"[Camera] Orthographic Size: {mainCamera.orthographicSize}");
                WriteLog($"[Camera] Culling Mask: {LayerMask.LayerToName(mainCamera.cullingMask)}");
            }
            
            WriteLog("");
        }

        private void WriteLog(string message)
        {
            Debug.Log(message);
            logWriter?.WriteLine(message);
        }

        private void OnDestroy()
        {
            if (logWriter != null)
            {
                WriteLog("=== デバッグログ終了 ===");
                logWriter.Close();
                logWriter = null;
            }
        }

        private void OnApplicationQuit()
        {
            if (logWriter != null)
            {
                WriteLog($"=== アプリケーション終了: {DateTime.Now} ===");
                logWriter.Close();
                logWriter = null;
            }
        }
    }
}

using UnityEngine;
using System.IO;
using System;
using System.Runtime.InteropServices;

namespace GGJ.Core
{
    /// <summary>
    /// ビルド後のデバッグ用ログ出力システム
    /// RuntimeInitializeOnLoadMethodで自動的に生成される
    /// </summary>
    public class DebugLogger : MonoBehaviour
    {
        // Windows APIでコンソールを表示
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
        
        private static DebugLogger instance;
        
        [Header("ログ設定")]
        [SerializeField] private bool enableLogging = true;
        
        [Header("コンソール設定")]
        [SerializeField] private bool showConsoleWindow = true;
        
        private string logFilePath;
        private StreamWriter logWriter;

        /// <summary>
        /// ゲーム起動時に自動的にDebugLoggerを生成
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DebugLogger");
                instance = go.AddComponent<DebugLogger>();
                DontDestroyOnLoad(go);
                
                // ログキャプチャを即座に開始
                Application.logMessageReceived += instance.HandleLog;
            }
        }

        private void Awake()
        {
            // ファイル初期化はAwakeで行う
            InitializeLogFile();
        }

        private void Start()
        {
            // Start()では何もしない（Awakeで初期化済み）
        }
        
        private void InitializeLogFile()
        {
            if (!enableLogging) return;
            
            // ログファイルのパスを設定
            logFilePath = @"C:\Unity\2D\GGJ\Assets\GGJ\Scenes\WorkScenes\Wine5\debug_log.txt";
            
            // 既存のファイルを削除（ロックされている可能性があるため）
            try
            {
                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[DebugLogger] 既存ログファイル削除失敗（無視します）: {e.Message}");
            }
            
            // ファイルを開く（共有アクセスを許可）
            try
            {
                FileStream fileStream = new FileStream(
                    logFilePath, 
                    FileMode.Create, 
                    FileAccess.Write, 
                    FileShare.ReadWrite
                );
                logWriter = new StreamWriter(fileStream);
                logWriter.AutoFlush = true;
                // サイレントモード：ファイルヘッダーなし
            }
            catch (Exception e)
            {
                Debug.LogError($"[DebugLogger] ログファイル作成エラー: {e.Message}");
                // ファイル書き込みは失敗してもコンソール出力は継続
            }
        }
        
        /// <summary>
        /// Unity Consoleのログをキャプチャ
        /// </summary>
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            string consolePrefix = "";
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    consolePrefix = "[ERROR] ";
                    break;
                case LogType.Warning:
                    consolePrefix = "[WARNING] ";
                    break;
                default:
                    consolePrefix = "";
                    break;
            }
            
            // コンソールウィンドウとファイルに出力
#if !UNITY_EDITOR
            if (showConsoleWindow)
            {
                Console.WriteLine(consolePrefix + logString);
            }
#endif
            
            logWriter?.WriteLine(consolePrefix + logString);
        }

        private void WriteLog(string message)
        {
            Debug.Log(message);
            logWriter?.WriteLine(message);
            
            // コンソールウィンドウにも出力（ビルド時のみ）
#if !UNITY_EDITOR
            if (showConsoleWindow)
            {
                Console.WriteLine(message);
            }
#endif
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
            
            if (logWriter != null)
            {
                WriteLog("=== デバッグログ終了 ===");
                logWriter.Close();
                logWriter = null;
            }
            
            // コンソールウィンドウを閉じる
#if !UNITY_EDITOR
            if (showConsoleWindow)
            {
                FreeConsole();
            }
#endif
        }

        private void OnApplicationQuit()
        {
            if (logWriter != null)
            {
                WriteLog($"=== アプリケーション終了: {DateTime.Now} ===");
                logWriter.Close();
                logWriter = null;
            }
            
            // コンソールウィンドウを閉じる
#if !UNITY_EDITOR
            if (showConsoleWindow)
            {
                FreeConsole();
            }
#endif
        }
    }
}
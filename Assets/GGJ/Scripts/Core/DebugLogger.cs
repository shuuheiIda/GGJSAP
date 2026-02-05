using UnityEngine;
using System.IO;
using System;
using System.Runtime.InteropServices;

namespace GGJ.Core
{
    /// <summary>
    /// ビルド後のデバッグ用ログ出力システム
    /// </summary>
    public class DebugLogger : MonoBehaviour
    {
        // Windows APIでコンソールを表示
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
        
        [Header("ログ設定")]
        [SerializeField] private bool enableLogging = true;
        
        [Header("コンソール設定")]
        [SerializeField] private bool showConsoleWindow = true;
        
        private string logFilePath;
        private StreamWriter logWriter;

        private void Start()
        {
            // Windowsビルドの場合、コンソールウィンドウを表示
#if !UNITY_EDITOR
            // if (showConsoleWindow)
            // {
            //     AllocConsole();
            //     Console.WriteLine("===========================================");
            //     Console.WriteLine("   Unity Game Debug Console");
            //     Console.WriteLine("   ログをコピー可能です");
            //     Console.WriteLine("===========================================");
            //     Console.WriteLine();
            // }
#endif
            
            // Debug.Logのキャプチャを開始
            Application.logMessageReceived += HandleLog;
            
            if (!enableLogging) return;
            
            // ログファイルのパスを設定
            logFilePath = Path.Combine(Application.persistentDataPath, "debug_log.txt");
            Debug.Log($"[DebugLogger] ログファイル: {logFilePath}");
            
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
                WriteLog("=== デバッグログ開始 ===");
                WriteLog($"時刻: {DateTime.Now}");
                WriteLog($"プラットフォーム: {Application.platform}");
                WriteLog($"解像度: {Screen.width}x{Screen.height}");
                WriteLog("");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DebugLogger] ログファイル作成エラー: {e.Message}");
                // エラーが発生してもアプリケーションは続行
                enableLogging = false;
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
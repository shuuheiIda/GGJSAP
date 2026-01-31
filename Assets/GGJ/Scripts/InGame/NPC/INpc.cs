using UnityEngine;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// NPCの共通インターフェース
    /// Player側はこのインターフェースを通じてNPCデータにアクセス
    /// </summary>
    public interface INpc
    {
        /// <summary>NPCのデータを取得</summary>
        NPCData GetNpcData();
        
        /// <summary>現在の状況に応じた会話テキストを取得</summary>
        string GetCurrentDialogue();
        
        /// <summary>ヒント取得状態を設定</summary>
        void SetHintReceived(bool received);
        
        /// <summary>ヒントを受け取ったかどうか</summary>
        bool HasReceivedHint();
        
        /// <summary>告発された状態を設定</summary>
        void SetAccused(bool accused);
        
        /// <summary>告発されたかどうか</summary>
        bool IsAccused();
        
        /// <summary>Npcの位置を取得</summary>
        Vector3 GetPosition();
        
        /// <summary>NpcのGameObjectを取得</summary>
        GameObject GetGameObject();
        
        /// <summary>このNPCを犯人として設定（実行時）</summary>
        void SetCriminal(bool isCriminal);
        
        /// <summary>このNPCが犯人かどうか（実行時の値）</summary>
        bool IsCriminal();
    }
}

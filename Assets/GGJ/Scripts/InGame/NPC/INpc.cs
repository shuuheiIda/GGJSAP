using UnityEngine;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// NPC縺ｮ蜈ｱ騾壹う繝ｳ繧ｿ繝ｼ繝輔ぉ繝ｼ繧ｹ
    /// Player蛛ｴ縺ｯ縺薙・繧､繝ｳ繧ｿ繝ｼ繝輔ぉ繝ｼ繧ｹ繧帝壹§縺ｦNPC繝・・繧ｿ縺ｫ繧｢繧ｯ繧ｻ繧ｹ
    /// </summary>
    public interface INpc
    {
        /// <summary>NPC縺ｮ繝・・繧ｿ繧貞叙蠕・/summary>
        NpcData GetNpcData();
        
        /// <summary>迴ｾ蝨ｨ縺ｮ迥ｶ諷九↓蠢懊§縺滉ｼ夊ｩｱ繝・く繧ｹ繝医ｒ蜿門ｾ・/summary>
        string GetCurrentDialogue();
        
        /// <summary>繝偵Φ繝亥叙蠕礼憾諷九ｒ險ｭ螳・/summary>
        void SetHintReceived(bool received);
        
        /// <summary>繝偵Φ繝医ｒ蜿励￠蜿悶▲縺溘°縺ｩ縺・°</summary>
        bool HasReceivedHint();
        
        /// <summary>告発された状態を設定</summary>
        void SetAccused(bool accused);
        
        /// <summary>告発されたかどうか</summary>
        bool IsAccused();
        
        /// <summary>Npcの位置を取得・/summary>
        Vector3 GetPosition();
        
        /// <summary>NpcのGameObjectを取得・/summary>
        GameObject GetGameObject();
        
        /// <summary>縺薙・NPC繧堤官莠ｺ縺ｨ縺励※險ｭ螳夲ｼ亥ｮ溯｡梧凾・・/summary>
        void SetCriminal(bool isCriminal);
        
        /// <summary>縺薙・NPC縺檎官莠ｺ縺九←縺・°・亥ｮ溯｡梧凾縺ｮ蛟､・・/summary>
        bool IsCriminal();
    }
}


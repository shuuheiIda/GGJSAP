using GGJ.InGame.Audio;
using UnityEngine;

namespace GGJ.Title
{
    public class GameStart : MonoBehaviour
    {
        void Start()
        {
            AudioManager.I.PlayBGM(BGMType.Title);
        }
    }
}

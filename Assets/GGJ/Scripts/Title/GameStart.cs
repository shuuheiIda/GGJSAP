using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GGJ.Title
{
    public class GameStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            {
                if (Input.GetKeyDown(KeyCode.Space)) // スペースキーが押されたら
                {
                SceneManager.LoadScene(""); // シーンを切り替える。該当シーンはめどが立ったら追加。
                }
            }
        }
    }
}

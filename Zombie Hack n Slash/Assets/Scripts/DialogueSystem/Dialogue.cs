using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ダイアログクラス
[System.Serializable]
public class Dialogue
{
    //ゲームクリアダイアログ（trueだったらゲームクリア画面へ）
    public bool gameClear;
    
    //NPC の名前
    public string name;
    
    [TextArea(3, 10)]
    public string[] sentences;

}

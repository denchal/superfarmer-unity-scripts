using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MainScript;

public class MenuScript : MonoBehaviour
{
    public List<TMP_InputField> playerNames = new List<TMP_InputField>();
    public Button exitButton;
    public void PlayGame()
    {
        int i = 0;
        foreach (var playerName in playerNames)
        {
            if (playerName.text != "")
            {
                players.Add(new Player(playerName.text, i));
                i += 1;
            }
        }
        if (i < 2)
        {
            return;
        }
        SceneManager.LoadSceneAsync("Game");
        Upd();
    }

    public void ExitGame() {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

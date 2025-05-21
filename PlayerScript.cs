using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MainScript;

public class PlayerScript : MonoBehaviour
{

    public TextMeshProUGUI isPlayerTurn;
    public TextMeshProUGUI playerName;
    public List<TextMeshProUGUI> mainHerd = new List<TextMeshProUGUI>() { };
    public List<Button> tradeChoices = new List<Button>() { };
    public List<TextMeshProUGUI> playerHerd = new List<TextMeshProUGUI>() { };
    public GameObject closeGameWindow;

    public GameObject warningWindow;

    void Start()
    {
        MainScript.warningWindow = warningWindow;
    }
    void Update()
    {
        if (players[currentPlayer].hasWon)
        {
            isPlayerTurn.text = "Zwycięzca!";
            isPlayerTurn.color = Color.yellow;
        }
        else if (currentPlayer != turn)
        {
            isPlayerTurn.text = "Tura innego gracza!";
            isPlayerTurn.color = Color.red;
        }
        else
        {
            isPlayerTurn.text = "Twoja tura!";
            isPlayerTurn.color = Color.green;
        }

        playerName.text = players[currentPlayer].name;
        for (int i = 0; i < players[currentPlayer].animals.Count; i++)
        {
            playerHerd[i].text = players[currentPlayer].animals[i].ToString();
        }
        for (int i = 0; i < mainHerd.Count; i++)
        {
            mainHerd[i].text = herd[i].ToString();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeGameWindow.SetActive(true);
        }
    }

    public void NextPlayer()
    {
        currentPlayer = (currentPlayer + 1) % players.Count;
    }

    public void PreviousPlayer()
    {
        currentPlayer = (currentPlayer - 1 + players.Count) % players.Count;
    }

    public void HideAllButtons()
    {
        foreach (Button btn in tradeChoices)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public void LoadAllButtons()
    {
        List<Player> otherPlayers = players.Where(p => p.id != currentPlayer).ToList();
        for (int i = 0; i < otherPlayers.Count && i < tradeChoices.Count; i++)
        {
            tradeChoices[i].GetComponentInChildren<TextMeshProUGUI>().text = otherPlayers[i].name;
            tradeChoices[i].gameObject.SetActive(true);
            int id = otherPlayers[i].id;
            tradeChoices[i].onClick.AddListener(() => OnClickPlayerTrade(id));
        }
    }

    void OnClickPlayerTrade(int id)
    {
        currentTrade.destination_id = id;
        StartCoroutine(FulfillTrade());
    }

    public void OnClickHerdTrade()
    {
        StartCoroutine(FulfillTrade());
    }
    
    public void ExitGame() {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

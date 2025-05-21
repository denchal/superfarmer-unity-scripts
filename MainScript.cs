using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    public static List<Player> players = new List<Player>() { };
    public static List<int> herd = new List<int>() { 128, 24, 20, 12, 6, 4, 2 };
    public static int currentPlayer = 0;
    public static Trade currentTrade;
    public static int turn = 0;
    public static int dice1Result;
    public static int dice2Result;
    public static bool playerConfirmed = false;
    public static bool playerDeclined = false;

    public static GameObject warningWindow;

    public static List<TradeRule> tradeRules = new List<TradeRule>()
    {
        new TradeRule(1, new List<int>{0,1,0,0,0,0,0}, new List<int>{6,0,0,0,0,0,0}), // 6 królików za 1 owcę
        new TradeRule(2, new List<int>{0,0,1,0,0,0,0}, new List<int>{0,2,0,0,0,0,0}), // 2 owce za 1 świnię
        new TradeRule(3, new List<int>{0,0,0,1,0,0,0}, new List<int>{0,0,3,0,0,0,0}), // 3 świnie za 1 krowę
        new TradeRule(4, new List<int>{0,0,0,0,1,0,0}, new List<int>{0,0,0,2,0,0,0}), // 2 krowy za 1 konia
        new TradeRule(5, new List<int>{0,0,0,0,0,1,0}, new List<int>{0,1,0,0,0,0,0}), // 1 owca za 1 małego psa
        new TradeRule(6, new List<int>{0,0,0,0,0,0,1}, new List<int>{0,0,0,1,0,0,0})  // 1 krowa za 1 dużego psa
    };
    public static void Upd()
    {
        UpdateHerd();
        foreach (var player in players)
        {
            if (CheckWin(player))
            {
                ShowWarningWindow($"Gracz {player.name} <color=yellow>zwyciężył!</color>");
                player.animals = new List<int>() { 0, 0, 0, 0, 0, 0, 0 };
                player.hasWon = true;
                UpdateHerd();
                ChangeTurn();
            }
        }
    }

    public void StartTrade(int trade_id)
    {
        currentTrade = new Trade();
        currentTrade.trade_id = trade_id;
    }

    public void UpdateTradeSource()
    {
        currentTrade.source_id = turn;
    }
    public void UpdateTradeDestination(int id)
    {
        currentTrade.destination_id = id;
    }
    public void UpdateTradeSide(bool left)
    {
        currentTrade.isLeftSided = left;
    }

    public static IEnumerator FulfillTrade()
    {
        playerConfirmed = false;
        playerDeclined = false;
        Player source = players.Find(p => p.id == currentTrade.source_id);
        if (source.hasRolled)
        {
            ShowWarningWindow(2);
            yield break;
        }
        Player destination = currentTrade.destination_id == -1 ? null : players.Find(p => p.id == currentTrade.destination_id);

        if (source == null)
        {
            yield break;
        }

        TradeRule rule = tradeRules.Find(r => r.trade_id == currentTrade.trade_id);
        if (rule == null)
        {
            yield break;
        }

        List<int> fromSource, toSource;

        if (currentTrade.isLeftSided)
        {
            fromSource = rule.leftOffer;
            toSource = rule.rightOffer;
        }
        else
        {
            fromSource = rule.rightOffer;
            toSource = rule.leftOffer;
        }

        for (int i = 0; i < fromSource.Count; i++)
        {
            if (source.animals[i] < fromSource[i])
            {
                ShowWarningWindow(0);
                yield break;
            }
        }

        if (destination != null)
        {
            for (int i = 0; i < toSource.Count; i++)
            {
                if (destination.animals[i] < toSource[i])
                {
                    ShowWarningWindow(1);
                    yield break;
                }
            }

            for (int i = 0; i < fromSource.Count; i++)
            {
                source.animals[i] -= fromSource[i];
                source.animals[i] += toSource[i];

                destination.animals[i] += fromSource[i];
                destination.animals[i] -= toSource[i];
            }
        }
        else
        {
            for (int i = 0; i < toSource.Count; i++)
            {
                if (herd[i] < toSource[i])
                {
                    toSource[i] = herd[i];
                }
            }

            for (int i = 0; i < fromSource.Count; i++)
            {
                source.animals[i] -= fromSource[i];
                herd[i] += fromSource[i];
            }

            for (int i = 0; i < toSource.Count; i++)
            {
                if (toSource[i] > 0)
                {
                    source.animals[i] += toSource[i];
                    herd[i] -= toSource[i];
                }
            }
        }
        Upd();
    }

    public static void ChangeTurn()
    {
        if (!players[turn].hasRolled && !players[turn].hasWon)
        {
            return;
        }
        do
        {
            turn = (turn + 1) % players.Count;
        } while (players[turn].hasWon && players.Any(p => !p.hasWon));
        players[turn].hasRolled = false;
    }

    static void UpdateHerd()
    {
        herd = new List<int>() { 128, 24, 20, 12, 6, 4, 2 };
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 0; j < herd.Count; j++)
            {
                herd[j] -= players[i].animals[j];
            }
        }
    }

    static void ShowWarningWindow(int type)
    {
        switch (type)
        {
            case 0: warningWindow.GetComponentInChildren<TextMeshProUGUI>().text = "Nie masz wystarczającej liczby zwierząt!"; break;
            case 1: warningWindow.GetComponentInChildren<TextMeshProUGUI>().text = "Gracz z którym próbujesz się wymienić nie ma wystarczającej liczby zwierząt!"; break;
            case 2: warningWindow.GetComponentInChildren<TextMeshProUGUI>().text = "Nie możesz dokonać wymiany po rzucie!"; break;
            case 3: warningWindow.GetComponentInChildren<TextMeshProUGUI>().text = "Nie możesz dokonać więcej niż jednej wymiany w ciągu jednej tury!"; break;
            default: break;
        }
        warningWindow.SetActive(true);
    }

    static void ShowWarningWindow(string message)
    {
        warningWindow.GetComponentInChildren<TextMeshProUGUI>().text = message;
        warningWindow.SetActive(true);
    }

    public static void Award()
    {
        //Jeśli gracz rzuci kostkami tak, że na
        //obu wypadnie takie samo zwierzę, to dostaje to zwierzę ze stada głównego. Gdy gracz ma
        //już jakieś zwierzęta, to po rzucie otrzymuje ze stada tyle zwierząt wyrzuconego
        //gatunku, ile ma pełnych par tego gatunku ( łącznie z wyrzuconymi na kostkach).

        // rozumiem to jako część całkowitą z ( (aktualna liczba zwierząt) + (liczba na kościach) ) / 2
        var rolls = new List<int>() { dice1Result, dice2Result };
        var playerAnimals = players[currentPlayer].animals;

        if (rolls[0] == rolls[1])
        {
            playerAnimals[rolls[0]] += Math.Min(1, herd[rolls[0]]);
            playerAnimals[rolls[0]] += Math.Min((playerAnimals[rolls[0]] + 2) / 2, Math.Max(herd[rolls[0]] - 1, 0));
        }
        else
        {
            if (rolls[0] < 6)
            {
                playerAnimals[rolls[0]] += Math.Min((playerAnimals[rolls[0]] + 1) / 2, herd[rolls[0]]);
            }
            if (rolls[1] < 6)
            {
                playerAnimals[rolls[1]] += Math.Min((playerAnimals[rolls[1]] + 1) / 2, herd[rolls[1]]);
            }
            playerAnimals = Eat(rolls, playerAnimals);
        }
        players[currentPlayer].animals = playerAnimals;
        Upd();
    }

    static List<int> Eat(List<int> rolls, List<int> playerAnimals)
    {
        if (rolls[1] == 7)
        {
            if (playerAnimals[5] == 0)
            {
                playerAnimals[0] = 1;
            }
            else
            {
                playerAnimals[5] -= 1;
            }
        }
        if (rolls[0] == 8)
        {
            if (playerAnimals[6] == 0)
            {
                playerAnimals[1] = 0;
                playerAnimals[2] = 0;
                playerAnimals[3] = 0;
                playerAnimals[4] = Math.Min(1, playerAnimals[4]);
                playerAnimals[5] = Math.Min(1, playerAnimals[5]);
            }
            else
            {
                playerAnimals[6] -= 1;
            }
        }
        Upd();
        return playerAnimals;
    }

    static bool CheckWin(Player player)
    {
        for (int i = 0; i < 5; i++)
        {
            if (player.animals[i] < 1)
            {
                return false;
            }
        }
        return true;
    }

    [Serializable]
    public class Player
    {
        public string name;
        public int id;
        public List<int> animals;
        public bool hasRolled;
        public bool hasWon;

        public Player(string name, int id)
        {
            this.name = name;
            this.id = id;
            this.hasRolled = false;
            this.hasWon = false;
            this.animals = new List<int>() { 1, 0, 0, 0, 0, 0, 0 }; //rabbit, sheep, pig, cow, horse, small dog, big dog
        }
    }

    [Serializable] public class Trade
    {
        public int source_id;
        public int destination_id;
        public int trade_id;
        public bool isLeftSided;
    }

    [Serializable]
    public class TradeRule
    {
        public int trade_id;
        public List<int> leftOffer;
        public List<int> rightOffer;

        public TradeRule(int id, List<int> left, List<int> right)
        {
            trade_id = id;
            leftOffer = left;
            rightOffer = right;
        }
    }

}




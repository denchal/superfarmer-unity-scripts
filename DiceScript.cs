using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Collections;
using static MainScript;

public class DiceScript : MonoBehaviour
{
    public List<SpriteIntIntTouple> possible_rolls = new List<SpriteIntIntTouple> { };
    public List<GameObject> sides = new List<GameObject>() { };
    public event Action<DiceScript> OnRollFinished;

    public int value;
    bool canRoll;

    void Update()
    {
        canRoll = (turn == currentPlayer) && !players[currentPlayer].hasRolled && !players[currentPlayer].hasWon;
    }
    public void OnClick()
    {
        if (!canRoll)
        {
            return;
        }
        players[currentPlayer].hasRolled = true;
        StartCoroutine(RollingAnimation());
    }
    IEnumerator RollingAnimation()
    {
        var start_time = Time.time;
        while (Time.time - start_time < 2)
        {
            Roll();
            yield return new WaitForSeconds(0.1f);
        }
        OnRollFinished?.Invoke(this);
    }

    void Roll()
    {
        List<int> rolls = new List<int>();
        while (rolls.Count < 6)
        {
            var roll = UnityEngine.Random.Range(0, 12);
            if (rolls.Contains(roll))
            {
                continue;
            }
            rolls.Add(roll);
            var i = 0;
            while (roll >= 0)
            {
                roll -= possible_rolls[i].value;
                i++;
            }
            if (rolls.Count == 1)
            {
                value = possible_rolls[i - 1].id;
            }
            sides[rolls.Count - 1].GetComponent<Image>().sprite = possible_rolls[i - 1].sprite;
        }
    }
}

[Serializable]
public class SpriteIntIntTouple
{
    public Sprite sprite;
    public int value;
    public int id;
}






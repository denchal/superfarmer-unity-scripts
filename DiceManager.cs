using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public DiceScript dice1;
    public DiceScript dice2;

    private bool dice1Ready = false;
    private bool dice2Ready = false;

    void Start()
    {
        dice1.OnRollFinished += HandleRoll;
        dice2.OnRollFinished += HandleRoll;
    }

    void HandleRoll(DiceScript dice)
    {
        if (dice == dice1)
        {
            MainScript.dice1Result = dice.value;
            dice1Ready = true;
        }
        else if (dice == dice2)
        {
            MainScript.dice2Result = dice.value;
            dice2Ready = true;
        }

        if (dice1Ready && dice2Ready)
        {
            MainScript.Award();
            dice1Ready = false;
            dice2Ready = false;
        }
    }
}


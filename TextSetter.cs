using TMPro;
using UnityEngine;

public class TextSetter : MonoBehaviour
{

    public TextMeshProUGUI baseText;
    public TextMeshProUGUI shadow;

    void Update()
    {
        shadow.text = baseText.text;
    }
}

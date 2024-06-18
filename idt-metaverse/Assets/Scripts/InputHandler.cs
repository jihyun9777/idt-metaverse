using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    public int input = 0;

    public void ValidateInput()
    {
        input = int.Parse(inputField.text);

        Debug.Log(input);
        
    }

    
}

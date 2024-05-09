using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UserManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    private string textHint = "Enter Name";

    public void OnButtonSelect()
    {
        if (String.Equals(usernameInput.text, textHint)) usernameInput.text = "";
    }
    
    public void OnButtonDeselect()
    {
        if (string.IsNullOrEmpty(usernameInput.text)) usernameInput.text = textHint;
    }

    public void LoginOrCreateUser()
    {
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            return;
        }
        string username = usernameInput.text;
        if (!string.IsNullOrEmpty(username))
        {
            GameManager.Instance.SetUser(username);
        }
    }

    public void SelectUserFromList(string username)
    {
        GameManager.Instance.SetUser(username);
    }
}
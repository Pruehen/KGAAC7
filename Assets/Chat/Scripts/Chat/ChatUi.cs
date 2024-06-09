using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUi : MonoBehaviour
{
    [Header("Please set these")]
    [SerializeField] private NetworkChat NetworkChat_chat;
    [SerializeField] private TMPro.TMP_InputField InputField_chatInput;
    [SerializeField] private Scrollbar Scroll_chatScroll;
    [SerializeField] private TMPro.TMP_Text Text_chatOutput;
    [SerializeField] private PlayerInputCustom _playerInput;

    private string _playerName;
    private string playerName
    {
        get
        {
            return NetworkChat_chat.playerName;
        }
    }

    private void Start()
    {
        InputField_chatInput.onSubmit.AddListener(SendCurrentInput);
        NetworkChat_chat.OnTextChanged += UpdateText;
    }
    private void UpdateText(string text)
    {
        Debug.Log(transform.parent.parent.name);
        Text_chatOutput.text = text;
    }
    private void SendCurrentInput()
    {
        SendChatMessage(playerName, InputField_chatInput.text);
        InputField_chatInput.text = "";
        StartCoroutine(UpdateScroll());
    }
    private void SendCurrentInput(string text)
    {
        SendCurrentInput();
    }
    private IEnumerator UpdateScroll()
    {
        yield return null;
        yield return null;
        Scroll_chatScroll.value = 0f;
    }
    private void SendChatMessage(string name, string message)
    {
        if(!string.IsNullOrWhiteSpace(message))
        {
            NetworkChat_chat.SendUserChat(name, message);
        }
    }

    public void OnEnterKeyPresseed()
    {
        if(InputField_chatInput.isFocused)
        {
            _playerInput.isControlable = false;
            SendCurrentInput();
        }
        else
        {
            _playerInput.isControlable = true;
            InputField_chatInput.Select();
        }
    }
}

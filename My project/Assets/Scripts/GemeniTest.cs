using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class GemeniTest : MonoBehaviour
{ 
    [SerializeField] private InputField _geminiRequestField;
    [SerializeField] private Button _button;
    [SerializeField] private string _apiKey;
    
    private string _apiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent";
    private Content[] _chatHistory;
    private Content[] _chatResponse;
    private IEnumerator _gemeniRequestCoroutine;
    private string _geminiUrl;
    private  List<Content> _contentsList;
    private byte[] _jsonToSend;
    private string _reply;

    private void Start()
    {
        _chatHistory = new Content[] {};
        _button.onClick.AddListener(SendChatRequest);
        _gemeniRequestCoroutine = SendRequestToGemini(_geminiRequestField.text);
        
        _geminiUrl = $"{_apiEndpoint}?key={_apiKey}";
        _contentsList = new List<Content>();
    }

    private void SendChatRequest()
    {
        StartCoroutine(_gemeniRequestCoroutine);
    }

    private IEnumerator SendRequestToGemini(string request)
    {
        
        Content userContent = new Content
        {
            role = "user",
            parts = new Part[]
            {
                new Part { text = request }
            }
        };
        
        _contentsList.Add(userContent);
        _chatHistory = _contentsList.ToArray();
        
        _jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonUtility.ToJson(_chatHistory));

        using (UnityWebRequest www = new UnityWebRequest(_geminiUrl, "POST")){
            www.uploadHandler = new UploadHandlerRaw(_jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Request complete! " + www.downloadHandler.text);
                _chatResponse = JsonUtility.FromJson<Content[]>(www.downloadHandler.text);
                if (_chatResponse.Length > 0 && _chatResponse[0].parts.Length > 0)
                {
                    string reply = _chatResponse[0].parts[0].text;
                    Content botContent = new Content
                    {
                        role = "model",
                        parts = new Part[]
                        {
                            new Part { text = reply }
                        }
                    };

                    Debug.Log(reply);
                    _geminiRequestField.text = reply;
                    _contentsList.Add(botContent);
                    _chatHistory = _contentsList.ToArray();
                    
                    StopCoroutine(_gemeniRequestCoroutine);
                }
            }
        }
        
        yield return null;
    }
}

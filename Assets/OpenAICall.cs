using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public static class OpenAICall
{
    [Header("OpenAI Information")]
    // This should not be shared with anyone!!!! Beware!
    [SerializeField] private static string openAIKey = "";

    public static UnityWebRequest CreateOpenAIRequest(string prompt, string requestBodyDebugPath = "")
    {
        string url = "https://api.openai.com/v1/images/generations"; // DALL-E API endpoint
        var requestBody = "{\"prompt\":\"" + prompt + "\", \"n\": 1," + "\"size\": \"256x256\"}"; // Customize your request payload here
        // write request body into text file to test.
        if (requestBodyDebugPath != "")
        {
            // TODO, if we want to debug the request body
        }
        else
        {
            Debug.Log(requestBody);
        }

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);

        request.SetRequestHeader("Authorization", "Bearer " + openAIKey);
        request.SetRequestHeader("Content-Type", "application/json");

        request.downloadHandler = new DownloadHandlerBuffer();

        return request;
    }


    public static string ParseResponse(string response = "")
    {
        string content = "";

        Debug.Log(response);

        try
        {
            // parse the response as jObject and get the content from it.
            JObject jsonObject = JObject.Parse(response);
            var choices = (JObject)jsonObject["data"][0];
            string message = (string)choices["url"];

            content = message;
        }
        catch (Exception e)
        {
            throw e;
        }

        return content;
    }

}
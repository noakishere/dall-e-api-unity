using System;
using System.IO;
using System.Xml.Linq;
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

        // TESTING THE JSON PARSER, to prevent calling the API everytime
        // temp JSON response from OpenAI
        //response = "{\r\n  \"created\": 1707436565,\r\n  \"data\": [\r\n    {\r\n      \"url\": \"https://oaidalleapiprodscus.blob.core.windows.net/private/org-Exuadf5crvsNRPSQs39IHLgG/user-83k90ALqTProIo6fYmLfucHN/img-FGU68QEP1wClFvFcuW35BMrx.png?st=2024-02-08T22%3A56%3A05Z&se=2024-02-09T00%3A56%3A05Z&sp=r&sv=2021-08-06&sr=b&rscd=inline&rsct=image/png&skoid=6aaadede-4fb3-4698-a8f6-684d7786b067&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2024-02-08T23%3A11%3A52Z&ske=2024-02-09T23%3A11%3A52Z&sks=b&skv=2021-08-06&sig=u1PTiT%2BoV20kcHBLokiFuy6SufijpRlQsiMkUw11YVs%3D\"\r\n    }\r\n  ]\r\n}";

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
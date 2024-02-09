using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OpenAIManager : MonoBehaviour
{
    [Header("Prompt Stuff")]
    [SerializeField] private string prompt;
    [SerializeField] private string currentResponse;

    [Header("Sprite Stuff")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private int spriteWidth;
    [SerializeField] private int spriteHeight;

    [SerializeField] private TextMeshProUGUI status;

    // Image URL test for testing the texture mapping to sprite
    //public string  testme = "https://oaidalleapiprodscus.blob.core.windows.net/private/org-Exuadf5crvsNRPSQs39IHLgG/user-83k90ALqTProIo6fYmLfucHN/img-lLvqlwqZEzC2A5NkHj6ZMpGq.png?st=2024-02-08T23%3A08%3A17Z&se=2024-02-09T01%3A08%3A17Z&sp=r&sv=2021-08-06&sr=b&rscd=inline&rsct=image/png&skoid=6aaadede-4fb3-4698-a8f6-684d7786b067&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2024-02-08T23%3A30%3A05Z&ske=2024-02-09T23%3A30%3A05Z&sks=b&skv=2021-08-06&sig=TSPbIboVrYoInykKPNHhsOiS12womdFOuTTt05je3Ec%3D";
    
    // Start is called before the first frame update
    void Start()
    {
        status.text = "Hello";
    }

    public void CallNow()
    {
        StartCoroutine(CallOpenAICoroutine());

        // These were used for testing
        //OpenAICall.ParseResponse();
        //StartCoroutine(DownloadImage(testme));
    }

    public IEnumerator CallOpenAICoroutine()
    {
        status.text = "Calling Open AI...";
        //isMakingACall = true;
        UnityWebRequest request = OpenAICall.CreateOpenAIRequest(prompt);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;

            Debug.Log("====");

            Debug.Log(responseJson);

            Debug.Log("=====");

            currentResponse = OpenAICall.ParseResponse(responseJson);

            Debug.Log(currentResponse);

            // Start downloading the Image from the URL of OpenAI
            StartCoroutine(DownloadImage(currentResponse));

            Debug.Log("Request Done.");
        }
        else
        {
            Debug.LogError("Request failed. Error: " + request.error);
            Debug.LogError(request.downloadHandler.text);
        }

        //human.GetHumanSummary();
        request.Dispose();
    }

    IEnumerator DownloadImage(string url)
    {
        status.text = "Downloading the image..";
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(imageRequest.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
            ApplyTexture(texture);
        }

        imageRequest.Dispose();
    }

    private void ApplyTexture(Texture2D texture)
    {
        //Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        Texture2D resizedTexture = ResizeTexture(texture);

        Sprite sprite = Sprite.Create(resizedTexture, new Rect(0.0f, 0.0f, spriteWidth, spriteHeight), new Vector2(0.5f, 0.5f), 100.0f);

        spriteRenderer.sprite = sprite;

        status.text = "image downloaded.";
    }

    Texture2D ResizeTexture(Texture2D originalTexture)
    {
        // Create a temporary RenderTexture with the desired size
        RenderTexture tempRT = RenderTexture.GetTemporary(spriteWidth, spriteHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        // Save the current active RenderTexture
        RenderTexture previous = RenderTexture.active;

        // Set the temporary RenderTexture as active
        RenderTexture.active = tempRT;

        // Copy the original texture to the temporary RenderTexture
        Graphics.Blit(originalTexture, tempRT);

        // Create a new Texture2D with the target size
        Texture2D resultTexture = new Texture2D(spriteWidth, spriteHeight);

        // Copy the pixels from the temporary RenderTexture to the new Texture2D
        resultTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        resultTexture.Apply();

        // Restore the previous active RenderTexture
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tempRT);

        return resultTexture;
    }
}

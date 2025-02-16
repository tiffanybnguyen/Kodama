using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.XR.ARSubsystems;

public class PlantScanner : MonoBehaviour
{
    private ARCameraManager arCameraManager;

    void Start()
    {
        // Find the AR Camera Manager in the scene
        arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arCameraManager == null)
        {
            Debug.LogError("ARCameraManager not found in the scene.");
        }
    }

    public void CaptureImage()
    {
        if (arCameraManager != null && arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            // Create a Texture2D to hold the converted image
            Texture2D texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

            // Define conversion parameters
            XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, image.width, image.height), // Use the full image
                outputDimensions = new Vector2Int(image.width, image.height), // Output same size as input
                outputFormat = TextureFormat.RGBA32, // Match the Texture2D format
                transformation = XRCpuImage.Transformation.None // No rotation or flipping
            };

            // Convert the XRCpuImage to Texture2D
            image.Convert(conversionParams, texture.GetRawTextureData<byte>());
            texture.Apply(); // Apply the pixel data to the texture

            // Dispose of the XRCpuImage to free up resources
            image.Dispose();

            // Send texture to API
            StartCoroutine(UploadImage(texture));
        }
        else
        {
            Debug.LogError("Failed to acquire latest CPU image.");
        }
    }

    private IEnumerator UploadImage(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();

        // Create a form to send the image
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, "plant.png", "image/png");

        // Send the request to your API
        using (UnityWebRequest request = UnityWebRequest.Post("https://your-api-endpoint.com/identify", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Plant identified: " + request.downloadHandler.text);
                // Parse the API response and update the game state
            }
            else
            {
                Debug.LogError("API request failed: " + request.error);
            }
        }
    }
}
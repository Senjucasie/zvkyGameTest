using System.Collections;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using System;

public class ApiService
{
    public static ApiService Instance;
    public readonly string LOGINTKN = "LGNTKN";
    private UTF8Encoding _utfEncoder = new();


    // GET
    //public IEnumerator GetAPIRequest(string jsonData, string url, Action<string> callback)
    //{
    //    UnityWebRequest www = new UnityWebRequest(url, "GET");
    //    byte[] jsonBytes = _utfEncoder.GetBytes(jsonData);
    //    www.uploadHandler = new UploadHandlerRaw(jsonBytes);
    //    www.downloadHandler = new DownloadHandlerBuffer();
    //    www.SetRequestHeader("Content-Type", "application/json");
    //    www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(LOGINTKN));

    //    yield return www.SendWebRequest();

    //    if (IsNetworkAvailable(www))
    //    {
    //        string jsonResponse = www.downloadHandler.text;
    //        Debug.Log($"{url} Response: {jsonResponse}");

    //        callback?.Invoke(jsonResponse);
    //    }
    //}

    //// POST
    //public IEnumerator PostAPIRequest(string jsonData, string url, Action<string> callback)
    //{
    //    UnityWebRequest www = new UnityWebRequest(url, "POST");
    //    byte[] jsonBytes = _utfEncoder.GetBytes(jsonData);
    //    www.uploadHandler = new UploadHandlerRaw(jsonBytes);
    //    www.downloadHandler = new DownloadHandlerBuffer();
    //    www.SetRequestHeader("Content-Type", "application/json");
    //    www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(LOGINTKN));

    //    yield return www.SendWebRequest();

    //    if (IsNetworkAvailable(www))
    //    {
    //        string jsonResponse = www.downloadHandler.text;
    //        Debug.Log($"{url} Response: {jsonResponse}");

    //        callback?.Invoke(jsonResponse);
    //    }
    //}

    //// PUT
    //public IEnumerator PutAPIRequest(string url, Action<string> callback = null)
    //{
    //    UnityWebRequest www = new UnityWebRequest(url, "PUT");
    //    byte[] jsonBytes = _utfEncoder.GetBytes("");
    //    www.uploadHandler = new UploadHandlerRaw(jsonBytes);
    //    www.downloadHandler = new DownloadHandlerBuffer();
    //    www.SetRequestHeader("Content-Type", "application/json");
    //    www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(LOGINTKN));

    //    yield return www.SendWebRequest();

    //    if (IsNetworkAvailable(www))
    //    {
    //        string jsonResponse = www.downloadHandler.text;
    //        Debug.Log($"{url} Response: {jsonResponse}");

    //        callback?.Invoke(jsonResponse);
    //    }
    //}


    //private bool IsNetworkAvailable(UnityWebRequest www)
    //{
    //    if (Application.internetReachability == NetworkReachability.NotReachable)
    //    {
    //        Debug.LogError("No Internet Connection!!");
    //        ErrorHandler.Instance.ShowErrorPopup(ErrorType.NO_INTERNET);
    //        return false;
    //    }
    //    else if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
    //    {
    //        Debug.LogError("Error while sending API request: " + www.error);
    //        ErrorHandler.Instance.ShowCommonErrorPopup("Server Issue", www.error);
    //        return false;
    //    }
    //    return true;
    //}
}

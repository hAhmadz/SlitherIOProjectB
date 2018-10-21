using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class scoreObj
{
    public int UserID;
    
    public scoreObj(int uID)
    {
        UserID = uID;
    }
}

public class scoreFinalObj
{
    public string highestscore;
    public string username;

    public scoreFinalObj(string hiscore, string uname)
    {
        highestscore = hiscore;
        username = uname;
    }
}

public class highscores : MonoBehaviour {

    public Text outputText;
    public Dictionary<string, string> scoresDB;
    
    private void Start()
    {
        scoreObj sobj = new scoreObj(8);
        string url = "http://203.101.225.0:5000/retrieve_scores";
        //string jsonString = JsonUtility.ToJson(sobj);
        string jsonString = JsonUtility.ToJson(sobj);
        StartCoroutine(Post(url, jsonString));
    }
    IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        Debug.Log(request.responseCode);

        var values = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>[]>(request.downloadHandler.text);
        string outputString = "";
        for (int j = 0; j < values.Length; j++)
        {
            string name = values[j]["username"];
            string score = values[j]["highestscore"];
            string tempString = name + "\t\t\t" + score;
            outputString = outputString + tempString + '\n';
        }
        outputText.text = outputString;
    }
}


		
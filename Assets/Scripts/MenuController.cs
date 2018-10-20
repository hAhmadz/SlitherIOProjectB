using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Assets.Scripts;

public class MenuController : MonoBehaviour
{
    public string urlAddress;
    public InputField FName;
    public InputField LName;
    public InputField userName;
    public InputField pswd;
    public Text TextOut;
    
    //both play the same game at the moment from two different functions
    public void singlePlayerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/SinglePlayerScene");
    }

    public void multiPlayerBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/MultiPlayerScene");
    }

    public void signUp()
    {
        if (userName.text != "" && pswd.text != "")
        {
            user u = new user(FName.text, LName.text, userName.text, pswd.text);
            urlAddress = urlAddress + "/signup";
            string jsonString = JsonUtility.ToJson(u);
            StartCoroutine(Post(urlAddress, jsonString));
        }
        else
            TextOut.text = "Cannot Add";
        
        FName.text = "";
        LName.text = "";
        userName.text = "";
        pswd.text = "";
    }
    
    IEnumerator Post(string url, string bodyJsonString)
    {
        string outputMsg = "";
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
        if (request.responseCode.Equals(201))
            outputMsg = "User Added";
        else
            outputMsg = "Cannot Add";
        TextOut.text = outputMsg;
    }
}

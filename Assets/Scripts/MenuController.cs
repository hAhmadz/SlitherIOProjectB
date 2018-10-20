using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Assets.Scripts;

public class MenuController : MonoBehaviour
{
    public string urlAddress;
    //signup page
    public InputField FName;
    public InputField LName;
    public InputField userName;
    public InputField pswd;
    public Text TextOut;

    //signinPage
    public InputField userNameSignIn;
    public InputField pswdSignIn;
    public Text TextOutSignIn;

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
            StartCoroutine(Post(urlAddress, jsonString,"signup"));
        }
        else
            TextOut.text = "Cannot Add";
        
        FName.text = "";
        LName.text = "";
        userName.text = "";
        pswd.text = "";
    }

    public void signIn()
    {
        if (userNameSignIn.text != "" && pswdSignIn.text != "")
        {
            user u = new user(userNameSignIn.text, pswdSignIn.text);
            urlAddress = urlAddress + "/login";
            string jsonString = JsonUtility.ToJson(u);
            StartCoroutine(Post(urlAddress, jsonString,"login"));
        }
        else
            TextOutSignIn.text = "No User Found";
        userNameSignIn.text = "";
        pswdSignIn.text = "";
    }

    IEnumerator Post(string url, string bodyJsonString,string func)
    {
        string outputMsg = "";
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (func.Equals("signup"))
        {
            if (request.responseCode.Equals(201))
                outputMsg = "User Added";
            else
                outputMsg = "Cannot Add";
            TextOut.text = outputMsg;
        }
        else if(func.Equals("login"))
        {
            if (request.responseCode.Equals(201) || request.responseCode.Equals(200))
                outputMsg = "Login Successful";
            else
                outputMsg = "Login Unsuccessful";
            TextOutSignIn.text = outputMsg;
        }
    }
}

using System.Collections;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    void Start()
    {
        //Advertisement.Initialize("2837831");
    }

    public void WaitAndDisplayAd()
    {
        StartCoroutine(AdDelayTimer());
    }

    IEnumerator AdDelayTimer()
    {
        // wait 2 seconds then show an ad
        if (true)
        {
            yield return new WaitForSeconds(2f);
            if (PersistenceController.persistence.ads)
            {
                #if UNITY_ADS
               /* if (Advertisement.IsReady("video"))
                {
                    Advertisement.Show("video");
                }*/
                #endif
            }
        }
        StopCoroutine(AdDelayTimer());
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu"); //to start menu
    }
}

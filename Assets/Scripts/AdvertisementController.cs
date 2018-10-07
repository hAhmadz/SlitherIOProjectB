using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertisementController : MonoBehaviour
{
    public bool adsEnabled = true;

    void Start()
    {
        //Advertisement.Initialize("2837831");
    }


    public void WaitAndDisplayAd()
    {
        if (adsEnabled)
        {
            print("HERE");
            StartCoroutine(AdDelayTimer());
        }

        // TODO: handle ads being turned off
    }


    //Timer Delay Fnc
    IEnumerator AdDelayTimer()
    {
        // wait 2 seconds then show an ad
        if (true)
        {
            yield return new WaitForSeconds(2);
#if UNITY_ADS

            if (!Advertisement.IsReady())
            {
                Debug.Log("Ad not ready");
            }

            Advertisement.Show();
#endif
        }
        StopCoroutine(AdDelayTimer());
    }
}

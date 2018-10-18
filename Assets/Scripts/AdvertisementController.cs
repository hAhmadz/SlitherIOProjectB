using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertisementController : MonoBehaviour
{
    // public bool adsEnabled = true;

    void Start()
    {
        //Advertisement.Initialize("2837831");
    }


    public void WaitAndDisplayAd()
    {
        if (PersistenceController.persistence.ads)
        {
            StartCoroutine(AdDelayTimer());
        }

        // TODO: handle ads being turned off
    }



    IEnumerator AdDelayTimer()
    {
        // wait 1.5 seconds then show an ad
        if (true)
        {
            yield return new WaitForSeconds(1.5f);
            #if UNITY_ADS
            if (Advertisement.IsReady("video"))
            {
                Advertisement.Show("video");
            }
            #endif
        }
        StopCoroutine(AdDelayTimer());
    }
}

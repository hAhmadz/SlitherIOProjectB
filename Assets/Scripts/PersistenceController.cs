using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// place the controls enum in global space (referenced in PlayerController)
public enum Controls { Touch, Joystick, Accelerometer };

public class PersistenceController : MonoBehaviour
{
    public static PersistenceController persistence;
    public bool ads;
    public Sprite skin;
    private int skinIndex;
    public List<Sprite> availableSkins;
    public List<Sprite> unlockableSkins;
    public Controls controls;
    public string snakename;
    public int lastScore;


    // check PersistenceController existence
    void Awake()
    {
        if (persistence == null)
        {
            DontDestroyOnLoad(gameObject);
            persistence = this;
        }
        else if (persistence != this)
        {
            Destroy(gameObject);
        }

        // default name
        snakename = "player";

        Load();
    }

    //private void OnDisable()
    //{
    //    Save();
    //}


    public void SetAds(bool adValue)
    {
        GameObject adsToggle = GameObject.Find("AdsToggle");
        if (adsToggle != null)
        {
            var toggle = adsToggle.GetComponent<Toggle>();
            toggle.isOn = adValue;
        }

        ads = adValue;
    }

    public void SetName(string name)
    {
        snakename = name;
    }


    public void SetControls(int choice)
    {
        // highlight the pressed button
        // dehighlight the others
        for (int i = 0; i < 3; i++)
        {
            GameObject controlsChooser = GameObject.Find("Controls Chooser");
            if (controlsChooser != null)
            {
                var btn = controlsChooser.transform.GetChild(i).gameObject.GetComponent<Button>();
                if (i == choice)
                {
                    ColorBlock cb = btn.colors;
                    cb.normalColor = new Color(0.81f, 0.25f, 0.18f, 1f);
                    cb.highlightedColor = new Color(0.81f, 0.25f, 0.18f, 1f);
                    btn.colors = cb;
                }
                else
                {
                    ColorBlock cb = btn.colors;
                    cb.normalColor = Color.white;
                    cb.highlightedColor = Color.white;
                    btn.colors = cb;
                }
            }

        }

        // set the desired control mechanism
        switch (choice)
        {
            case 0:
                controls = Controls.Touch;
                break;
            case 1:
                controls = Controls.Joystick;
                break;
            case 2:
                controls = Controls.Accelerometer;
                break;
            default:
                // shouldn't happen, but incase it does default to touch controls
                controls = Controls.Touch;
                break;
        }
    }



    public void SetSkin(bool next)
    {
        // traverse available skins list based on input
        skinIndex += next ? 1 : -1;
        if (skinIndex < 0)
        {
            skinIndex = availableSkins.Count - 1;
        }
        else if (skinIndex >= availableSkins.Count)
        {
            skinIndex = 0;
        }

        // assign the selected skin choice
        skin = availableSkins[skinIndex];

        // update the preview skin
        SetDummySkin(skin);
    }

    public void SetSkin(int skinIndex)
    {
        if (skinIndex < availableSkins.Count)
        {
            skin = availableSkins[skinIndex];
            SetDummySkin(skin);
        }


    }

    public void SetDummySkin(Sprite skin)
    {
        GameObject dummySkin = GameObject.Find("Skin Preview");
        if (dummySkin != null)
        {
            dummySkin.GetComponent<Image>().sprite = skin;
        }
    }



    public void UnlockSkins()
    {
        foreach (Sprite locked in unlockableSkins)
        {
            availableSkins.Add(locked);
        }

        // remove the skin from the unloackable list 
        //(so it can't get added mulitple times to the availableSkins)
        unlockableSkins.Clear();
    }




    public void SetLastScore(int scoreValue)
    {
        lastScore = scoreValue;
    }


    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/options.dat");

        // create an instance of OptionsData to store current options
        OptionsData options = new OptionsData();
        options.snakename = snakename;
        options.ads = ads;
        options.skinIndex = skinIndex;
        options.skinsUnlocked = (unlockableSkins.Count == 0);
        options.controls = controls;

        // write current options to file
        bf.Serialize(file, options);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/options.dat"))
        {
            print(Application.persistentDataPath);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/options.dat", FileMode.Open);
            OptionsData options = bf.Deserialize(file) as OptionsData;
            file.Close();

            snakename = options.snakename;
            SetAds(options.ads);
            skinIndex = options.skinIndex;
            SetSkin(skinIndex);
            if (options.skinsUnlocked)
                UnlockSkins();
            controls = options.controls;
            SetControls((int)controls);
        }
    }

}


/*
 * Utility data class to hold option settings
 */
[Serializable]
class OptionsData
{
    public string snakename;
    public bool ads;
    public int skinIndex;
    public bool skinsUnlocked;
    public Controls controls;
}
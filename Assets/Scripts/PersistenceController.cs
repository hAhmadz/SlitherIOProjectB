using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PersistenceController : MonoBehaviour {
    public static PersistenceController persistence;
    public bool ads;
    public Sprite skin;
    private int skinIndex;
    public List<Sprite> availableSkins; 
    // public enum Controls {Touch, Joystick, Gravity}; // how to set controls?
    // public ... // how to set snake skin ?
    // public ... availableSkins;
    public int score;


	// check PersistenceController existence
	void Awake () {
        if(persistence == null)
        {
            print("here");
            DontDestroyOnLoad(gameObject);
            persistence = this;
        }
        else if(persistence != this)
        {
            print("no here");
            Destroy(gameObject);
        }

        // set defaults // will be handled by reading from file eventually
        SetAds(true);


        //availableSkins = Resources.LoadAll<Sprite>("Sprites");
        // print(availableSkins.Length);

        // skinIndex = 0;
        // skin = availableSkins[skinIndex];
        // print(skin);

	}

    // loading data in OnEnable
    // saving data in OnDisable
	

    public void SetAds(bool adValue)
    {
        ads = adValue;
    }

    //public void SetSound(bool soundValue)
    //{
    //    sound = soundValue;
    //}

    //public void SetControls(... controlChoice)
    //{
    //    controls = controlChoice;
    //}



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
        print(skin);

        // update the preview skin
        Image dummySkin = GameObject.Find("Skin Preview").GetComponent<Image>();
        if (dummySkin != null)
        {
            dummySkin.sprite = skin;
        }

        print(skin);
            
    }




    public void SetScore(int scoreValue)
    {
        score = scoreValue;
    }




    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/options.dat");

        // create an instance of OptionsData to store current options
        OptionsData options = new OptionsData();
        options.ads = ads;
        //options.sound = sound;
        //options.controls = controls;
        //options.skin = skin;

        // write current options to file
        bf.Serialize(file, options);
        file.Close();
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/options.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/options.dat", FileMode.Open);
            OptionsData options = bf.Deserialize(file) as OptionsData;
            file.Close();

            ads = options.ads;
            //sound = options.sound;
            //controls = options.controls;
            //skin = options.skin;
        }
    }

}


/*
 * Utility data class to hold option settings
 */
[Serializable]
class OptionsData
{
    public bool ads;
    // public bool sound;
    // public enum Controls {Touch, Joystick, Gravity}; // how to set controls?
    // public ... // how to set snake skin ?
}
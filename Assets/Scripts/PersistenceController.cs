using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PersistenceController : MonoBehaviour {
    public static PersistenceController persistence;
    public bool ads;
    // public enum Controls {Touch, Joystick, Gravity}; // how to set controls?
    // public ... // how to set snake skin ?
    // public ... availableSkins;
    public int score;


	// check PersistenceController existence
	void Awake () {
        if(persistence == null)
        {
            DontDestroyOnLoad(gameObject);
            persistence = this;
        }
        else if(persistence != this)
        {
            Destroy(gameObject);
        }

        // set defaults // will be handled by reading from file eventually
        SetAds(true);

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

    //public void SetSkin(bool skinChoice)
    //{
    //    skinChoice = skinChoice;
    //}

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
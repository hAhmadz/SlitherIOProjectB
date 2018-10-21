using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
    public Dictionary<string, int> scores;
    public Text topTenText;
    public int threshold;  // threshold is the minimum score currently needed to get into the top 10

	// Use this for initialization
	void Awake() 
    {
        // scores is a dictionary to map snake names to snake lengths
        scores = new Dictionary<string, int>();
        threshold = 0;
	}

    private void Start()
    {
        topTenText.text = "Here's some text";
    }

    // called by a snake on growing or shrinking
    public void SubmitScore(string name, int score) 
    {
        if (scores.ContainsKey(name))
        {
            int oldscore = scores[name];
            int delta = score - oldscore;
            scores[name] = score;

            if ((delta > 0 && score > threshold) || (delta < 0))
                topTenText.text = GetTopTen();
        }
        else
            scores[name] = score;
    }

    // called by a snake on death
    public void RemoveScore(string name)
    {

        if(scores.ContainsKey(name))
        {
            int score = scores[name];
            scores.Remove(name);
            if (score >= threshold)
                topTenText.text = GetTopTen();
        }
    }

    public string GetTopTen()
    {
        var sortedDict = (
            from entry in scores 
            orderby entry.Value descending 
            select entry).ToDictionary(pair => pair.Key, pair => pair.Value).Take(10);

        string output = "";
        foreach (KeyValuePair<string, int> kvp in sortedDict)
        {
            output += (String.Format("{0, 12} : {1,5}\n", kvp.Key, kvp.Value));
            threshold = kvp.Value;
        }
        return output;
    }
}

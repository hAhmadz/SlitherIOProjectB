using UnityEngine;
using UnityEngine.UI;

public class AssignUIListeners : MonoBehaviour
{
    public Toggle adsToggle;
    public Button touchButton;
    public Button joystickButton;
    public Button accelButton;
    public Button nextSkin;
    public Button prevSkin;

    void Start()
    {
        PersistenceController options = GameObject.Find("Options").GetComponent<PersistenceController>();

        touchButton.GetComponent<Button>().onClick.AddListener(() => options.SetControls(0));
        joystickButton.GetComponent<Button>().onClick.AddListener(() => options.SetControls(1));
        accelButton.GetComponent<Button>().onClick.AddListener(() => options.SetControls(2));

        nextSkin.GetComponent<Button>().onClick.AddListener(() => options.SetSkin(true));
        prevSkin.GetComponent<Button>().onClick.AddListener(() => options.SetSkin(false));

        adsToggle.GetComponent<Toggle>().onValueChanged.AddListener(val => options.SetAds(val));
    }
}

using Naninovel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Location
{
    public string locationName;
    public TextMeshProUGUI locationText;
    public Button button;
    public GameObject isHere;
    public string customVariableName;
    public bool Unlocked
    {
        get
        {
            Engine.GetService<ICustomVariableManager>().TryGetVariableValue(customVariableName, out bool unlocked);
            return unlocked;
        }
    }
}

public class MapUI : MonoBehaviour
{
    [SerializeField] private GameObject locationsPanel;
    [SerializeField] private Location[] locations;
    [SerializeField] private GameObject openButton;

    private const string CURRENTLOCATIONCUSTOMVARIABLE = "currentLocation";

    private void OnEnable()
    {
        foreach (Location location in locations)
        {
            location.locationText.text = location.locationName;
            location.button.onClick.AddListener(() => OnLocationButtonClick(location.locationName));
        }
    }

    private void OnLocationButtonClick(string locationName)
    {
        Engine.GetService<ICustomVariableManager>().TrySetVariableValue(CURRENTLOCATIONCUSTOMVARIABLE, locationName);
        SwitchLocationsPanel(false);
    }

    public void SwitchLocationsPanel(bool open)
    {
        locationsPanel.SetActive(open);
        openButton.SetActive(!open);
        if (open)
        {
            UpdateLocations();
        }           
    }

    private void UpdateLocations()
    {
        foreach (Location location in locations)
        {
            Engine.GetService<ICustomVariableManager>().TryGetVariableValue(CURRENTLOCATIONCUSTOMVARIABLE, out string currentLocation);
            location.isHere.SetActive(currentLocation == location.locationName);
           
            if (location.Unlocked)
            {
                location.button.interactable = true;               
            }
            else
            {
                location.button.interactable = false;
            }
        }
    }

    private void OnDisable()
    {
        foreach (Location location in locations)
        {
            location.button.onClick.RemoveAllListeners();
        }
    }
}

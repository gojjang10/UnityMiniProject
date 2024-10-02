using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    [SerializeField] private GameObject[] speedIcon;
    [SerializeField] private GameObject pIcon;
    //[SerializeField] MarioBassController mario;

    public void UpdateSpeedUI(float currentSpeed, float maxSpeed)
    {
        currentSpeed = Mathf.Abs(currentSpeed);
        if(currentSpeed > 0)
        {
            speedIcon[0].SetActive(true);
        }
        else
        {
            speedIcon[0].SetActive(false);
        }
        if (currentSpeed > 1)
        {
            speedIcon[1].SetActive(true);
        }
        else
        {
            speedIcon[1].SetActive(false);
        }
        if (currentSpeed > 2)
        {
            speedIcon[2].SetActive(true);
        }
        else
        {
            speedIcon[2].SetActive(false);
        }
        if (currentSpeed > 5)
        {
            speedIcon[3].SetActive(true);
        }
        else
        {
            speedIcon[3].SetActive(false);
        }
        if (currentSpeed > 6)
        {
            speedIcon[4].SetActive(true);
        }
        else
        {
            speedIcon[4].SetActive(false);
        }
        if (currentSpeed > 9)
        {
            speedIcon[5].SetActive(true);
        }
        else
        {
            speedIcon[5].SetActive(false);
        }
        if (currentSpeed == 10)
        {
            pIcon.SetActive(true);
        }
        else
        {
            pIcon.SetActive(false);
        }
    }
}

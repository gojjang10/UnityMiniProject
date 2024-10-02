using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinUI;

    private void Update()
    {
        coinUI.text = $"{GameManager.Instance.coin}";
    }
}

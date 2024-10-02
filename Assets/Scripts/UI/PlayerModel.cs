using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] int coin;
    public int Coin { get { return coin; } set { coin = value; OnCoinChanged?.Invoke(coin); } }
    public UnityAction<int> OnCoinChanged;
}

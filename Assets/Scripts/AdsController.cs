using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour
{
    private static string gameId = "4064785"; // Android id

    public static void InitializeAdvertisment()
    {
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, false);
        }
    }
    
    public static void ShowAdvertisment()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show("banner");
        }        
    }
}

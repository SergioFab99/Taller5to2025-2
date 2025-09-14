using System;
using UnityEngine;

public static class AudioEvents {
    
    public static Action<string, Vector3?> OnPlaySound; 
    

    public static Action<string> OnStopSound;
}

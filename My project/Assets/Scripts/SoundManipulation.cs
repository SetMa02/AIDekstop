using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class SoundManipulation : MonoBehaviour
{
    
    void Start()
    {
        WindowsVolume.SetMasterVolume(1f); // Устанавливаем громкость на 100%
    }
}

public static class WindowsVolume
{
    private static class NativeMethods
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
    }

    public static void SetMasterVolume(float volume)
    {
       
        uint newVolume = (uint)(ushort.MaxValue * volume);

       
        int result = NativeMethods.waveOutSetVolume(IntPtr.Zero, (newVolume & 0xFFFF) | (newVolume << 16));

        if (result != 0)
        {
            Debug.LogError("Ошибка установки громкости: " + result);
        }
    }
}
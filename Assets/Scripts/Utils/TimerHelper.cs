using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerHelper : MonoBehaviour {
    public static IEnumerator SetTimeout(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
}

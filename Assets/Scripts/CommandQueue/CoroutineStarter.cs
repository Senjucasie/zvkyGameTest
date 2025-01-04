using System.Collections;
using UnityEngine;


public class CoroutineStarter : MonoBehaviour
{
    private static CoroutineStarter instance;
    public static CoroutineStarter Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject CoroutineGameobject = new GameObject(typeof(CoroutineStarter).Name);
                CoroutineGameobject.AddComponent<CoroutineStarter>();
                instance = CoroutineGameobject.GetComponent<CoroutineStarter>();
                DontDestroyOnLoad(CoroutineGameobject);
            }
            return instance;
        }
    }

    private IEnumerator Perform(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
    }

    public static void RunStaticCoroutine(IEnumerator coroutine)
    {
        Instance.StartCoroutine(Instance.Perform(coroutine));
    }


    public static void StartSleep()
    {
        Instance.StopCoroutine(Instance.SetSleepTimeOut());
        Instance.StartCoroutine(Instance.SetSleepTimeOut());
    }


    public static void StopSleep()
    {
        Instance.StopCoroutine(Instance.SetSleepTimeOut());
    }

    private IEnumerator SetSleepTimeOut()
    {
        yield return new WaitForSeconds(30);

        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
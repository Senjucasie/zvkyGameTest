using UnityEngine;

public class GameController : MonoBehaviour
{

    public static void ActiveFreeGame()
    {
        CommandQueue.AddAction((callBack) =>
        {
            FreeSpinManager.Activate(callBack);
        }, 2f, 0, "Free spin Entry");

        CoroutineStarter.Instance.StartCoroutine(CommandQueue.StartExecution());
    }


    public static void DeactivateFreeGame()
    {
        CommandQueue.AddAction((callBack) =>
        {
            FreeSpinManager.Deactivate(callBack);
        }, 2f, 0, "Free spin Exit");

        CoroutineStarter.Instance.StartCoroutine(CommandQueue.StartExecution());
    }
}

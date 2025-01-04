using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class CommandQueue
{
    public static List<Command> queue = new List<Command>();

    public static bool isRunning = false;

    public static IEnumerator StartExecution()
    {
        yield return null;
        // Debug.Log("isRunning " +isRunning +" count "+ queue.Count);
        if (queue.Count > 0)
        {
            if (!isRunning)
            {
                Command temp = queue[0];
                Action<Action> action = temp.action;
                isRunning = true;
                queue.RemoveAt(0);
                yield return new WaitForSeconds(temp.delay);
                action(() =>
                {
                    isRunning = false;
                    CoroutineStarter.RunStaticCoroutine(StartExecution());
                });
            }
        }
        else
        {
            Debug.Log("Start Sleep");
            CoroutineStarter.StartSleep();
        }
    }

    public static void AddAction(Action<Action> a, float delay = 0, int priority = 0, string message = "", bool noFurtherExecution = false)
    {
        // Debug.Log("Commmand Added ");
        queue.Add(new Command { action = a, delay = delay, priority = priority, message = message.Trim(), noFurtherExecution = noFurtherExecution });
        queue.Sort();
    }
}
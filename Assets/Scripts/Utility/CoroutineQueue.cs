using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineQueue : MonoBehaviour
{
    private readonly Queue<IEnumerator> queue = new();
    private bool isRunning = false;

    public void AppendCoroutine(IEnumerator coroutine)
    {
        queue.Enqueue(coroutine);
        if (!isRunning)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isRunning = true;

        while (queue.Count > 0)
        {
            IEnumerator current = queue.Dequeue();
            yield return StartCoroutine(current);
        }

        isRunning = false;
    }
}

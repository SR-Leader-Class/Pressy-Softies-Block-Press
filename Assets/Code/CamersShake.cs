using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamersShake : MonoBehaviour
{
    public IEnumerator Shake (float duration, float magnitude)
    {
        Quaternion originalRot = transform.localRotation;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-.1f,.1f) * magnitude;
            float y = Random.Range(-.1f,.1f) * magnitude;

            transform.localRotation = new Quaternion(x, y, originalRot.z, originalRot.w);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localRotation = originalRot;
    }
}

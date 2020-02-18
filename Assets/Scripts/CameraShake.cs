using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public IEnumerator ShakeCamera (float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float shakeElapsed = 0.0f;

        while (shakeElapsed < duration)
        {
            float x = Random.Range(-1.0f, 1.0f) * magnitude;
            float y = Random.Range(-1.0f, 1.0f) * magnitude;
            transform.position = new Vector3(x, y, originalPos.z);

            shakeElapsed += Time.deltaTime;
            yield return null;  // loop each frame within while constraints
        }

        transform.position = originalPos;
    }
}

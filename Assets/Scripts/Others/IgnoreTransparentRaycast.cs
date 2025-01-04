using UnityEngine;

public class IgnoreTransparentRaycast : MonoBehaviour
{
    void Start() => this.GetComponent<UnityEngine.UI.Image>().alphaHitTestMinimumThreshold = 0.5f;
}

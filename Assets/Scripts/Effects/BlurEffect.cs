using UnityEngine;
using UnityEngine.UI;

public class BlurEffect : MonoBehaviour
{
    public Transform spinningImage;
    public float shutterHeight = 1.0f;
    public float blurStrength = 0.5f;

    private Vector3 previousPosition;

    private void Start()
    {
        previousPosition = spinningImage.position;
    }

    private void Update()
    {
        Vector3 motionVector = spinningImage.position - previousPosition;

        GameObject duplicateImage = Instantiate(spinningImage.gameObject, spinningImage.position, Quaternion.identity);
        duplicateImage.GetComponent<BlurEffect>().enabled = false;
        float stretchAmount = motionVector.magnitude * blurStrength;
        duplicateImage.transform.localScale += new Vector3(stretchAmount, 0, 0);

        Image spriteRenderer = duplicateImage.GetComponent<Image>();
        Color spriteColor = spriteRenderer.color;
        spriteColor.a -= Time.deltaTime * blurStrength;
        spriteRenderer.color = spriteColor;

        duplicateImage.transform.Translate(Vector3.down * Time.deltaTime);
        if (duplicateImage.transform.position.y < -shutterHeight)
        {
            Destroy(duplicateImage);
        }
        previousPosition = spinningImage.position;
    }
}
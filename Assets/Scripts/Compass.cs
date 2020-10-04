using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    RectTransform rectTransform;
    public Transform source;
    public int axis = 1;
   

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (axis) {
            case 0:
                rectTransform.rotation = Quaternion.Euler(0, 0, source.eulerAngles.z);
                break;
            case 1:
                rectTransform.rotation = Quaternion.Euler(0, 0, source.eulerAngles.y);
                break;
            case 2:
                rectTransform.rotation = Quaternion.Euler(0, 0, -source.eulerAngles.z);
                break;
            default:
                Debug.LogError("Invalid Axis");
                break;
		}
    }
}

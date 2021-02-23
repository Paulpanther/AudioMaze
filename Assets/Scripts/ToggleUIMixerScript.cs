using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUIMixerScript : MonoBehaviour
{

    private bool _visible = false;
    
    private void Start()
    {
        UpdateChildren();
    }

    private void Update()
    {
        if (Input.GetKeyDown("b"))
        {
            _visible = !_visible;
            UpdateChildren();
        }
    }

    private void UpdateChildren()
    {
        if (_visible)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}

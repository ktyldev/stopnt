using UnityEngine;
using UnityEngine.UI;

public class EagerButton : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Button>().Select();
        GetComponent<Animator>().Play("Selected");
    }
}

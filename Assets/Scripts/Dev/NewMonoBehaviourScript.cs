using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Transform stroller;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -8);

    private void LateUpdate()
    {
        if (stroller == null) return;

        transform.position = stroller.position + offset;
        transform.LookAt(stroller.position, Vector3.up);
    }
}

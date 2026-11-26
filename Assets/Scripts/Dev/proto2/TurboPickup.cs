using System.Collections;
using UnityEngine;


// Item que habilita el turbo (tecla Q) al pasar por encima.

[RequireComponent(typeof(Collider))]
public class TurboPickup : MonoBehaviour
{
    [Tooltip("Segundos hasta que el item reaparece.")]
    [SerializeField] float respawnTime = 8f;

    Collider trigger;
    Renderer[] renderers;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void Awake()
    {
        trigger = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        StrollerController stroller = other.GetComponentInParent<StrollerController>();
        if (stroller == null) return;

        stroller.GrantTurbo();

        if (respawnTime > 0f) StartCoroutine(Respawn());
        else Destroy(gameObject);
    }

    IEnumerator Respawn()
    {
        SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        SetActive(true);
    }

    void SetActive(bool active)
    {
        trigger.enabled = active;
        foreach (Renderer r in renderers) r.enabled = active;
    }
}

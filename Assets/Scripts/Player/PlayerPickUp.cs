using UnityEngine;
using UnityEngine.UI;

public class PlayerPickUp : MonoBehaviour
{
    public event WeaponPickUp OnWeaponPickUp;
    public delegate void WeaponPickUp(GameObject Prefab);

    public LayerMask hitMask;

    public bool Interact;

    public void Initialize()
    {

    }

    public void PickUpUpdate(bool InteractInput)
    {
        Interact = InteractInput;

        if (Interact)
        {
            PickUpWeapon(); 
        }
    }


    public void PickUpWeapon()
    {
        var col = Physics.OverlapSphere(transform.position, 10f, hitMask.value, QueryTriggerInteraction.Ignore);
        if (col != null && col.Length > 0)
        {
            foreach (Collider coll in col)
            {
                if (coll.gameObject.TryGetComponent<TagContainer>(out TagContainer TagC) && TagC.HasTag("PickUpWeapon"))
                {

                    Debug.Log("HasPickUpTag");
                    OnWeaponPickUp?.Invoke(coll.gameObject.GetComponent<PickUpWeapon>().Prefab);
                    Destroy(coll.gameObject);

                }
            }
        }
    }
}

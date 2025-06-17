using MoreMountains.TopDownEngine;
using UnityEngine;

public class CompanionOwnerRegister : MonoBehaviour
{
    [SerializeField] CompanionAIContext companionAIContext;
    [SerializeField] OwnerRegisterZone ownerRegisterZone;

    private void Awake()
    {
        companionAIContext = GetComponentInParent<CompanionAIContext>();
        ownerRegisterZone = GetComponent<OwnerRegisterZone>();
    }

    public void RegisterOwner()
    {        
        companionAIContext.SetOwner(ownerRegisterZone.GetOwnerCharacter());
        ownerRegisterZone.OnExit.Invoke();
        ownerRegisterZone.gameObject.SetActive(false);
    }
}

using MoreMountains.TopDownEngine;
using UnityEngine;

public class OwnerRegisterZone : ButtonActivated
{
    Character _ownerCharacter;
    public Character GetOwnerCharacter() 
    {
        _ownerCharacter = _characterButtonActivation.GetComponent<Character>();

        if(_ownerCharacter == null)
        {
            _ownerCharacter = _characterButtonActivation.GetComponentInParent<Character>();
        }

        if (_ownerCharacter == null)
        {
            Debug.Log("No Valid Owner");
            return null;
        }


        return _ownerCharacter; 
    }

}

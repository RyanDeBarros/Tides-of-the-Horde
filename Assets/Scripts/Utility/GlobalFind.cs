using UnityEngine;

class GlobalFind
{
    public static T FindUniqueObjectByType<T>(bool includeInactive, bool sorted = false) where T : Object
    {
        return GameObject.FindObjectsByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude,
            sorted ? FindObjectsSortMode.InstanceID : FindObjectsSortMode.None).GetUniqueElement();
    }
}

using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class TagContainer : MonoBehaviour
{ 



    [ValueDropdown("GetTagOptions")]
    public List<TagSO> tags = new List<TagSO>();

    public bool HasTag(TagSO tag) => tag != null && tags.Contains(tag);
    public bool HasTag(string tagName) => tags.Exists(t => t != null && t.tagName == tagName);

    public bool HasAnyTag(params TagSO[] check)
    {
        foreach(TagSO tag in check)
        {
            if (tag != null && tags.Contains(tag)) return true;
        }
        return false;
    }

    private IEnumerable<TagSO> GetTagOptions()
    {
        var db = TagDatabase.Instance;
        if(db == null || db.tags == null)
            return new List<TagSO> { null };

        return db.tags;
    }


}

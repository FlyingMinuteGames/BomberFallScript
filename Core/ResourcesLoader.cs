using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class ResourcesLoader {

    private static IDictionary<string,Object> m_resources = new Dictionary<string,Object>();

  
    public static T LoadResources<T>(string path) where T : Object
    {
        if (m_resources.ContainsKey(path))
            return (T)m_resources[path];

        return (T)Resources.Load(path);
       
    }
    public static void ClearResources()
    {
        m_resources.Clear();
    }
}

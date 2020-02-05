using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EdibleDuplicants
{
    public static class KMonoBehaviorExtensions
    {
        public static void DestroyRequiredComponentsAndSelf(this KMonoBehaviour kMonoBehaviour)
        {
            kMonoBehaviour.DestroyRequiredComponents();
            Debug.Log($"Destroying {kMonoBehaviour}");
            Object.DestroyImmediate(kMonoBehaviour);
        }

        public static void DestroyRequiredComponents(this KMonoBehaviour kMonoBehaviour)
        {
            MemberInfo memberInfo = kMonoBehaviour.GetType();
            RequireComponent[] requiredComponentsAtts =
                Attribute.GetCustomAttributes(memberInfo, typeof(RequireComponent), true) as RequireComponent[] ??
                new RequireComponent[0];
            foreach (var rc in requiredComponentsAtts.Where(rc =>
                rc != null && kMonoBehaviour.GetComponent(rc.m_Type0) != null))
            {
                Debug.Log($"Destroying {rc.m_Type0}");
                Object.DestroyImmediate(kMonoBehaviour.GetComponent(rc.m_Type0));
            }
        }
    }
}
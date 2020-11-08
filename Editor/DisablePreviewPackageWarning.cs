using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PackageManagerExtraSettings
{
    public static class DisablePreviewPackageWarning
    {
        public static void DisableWarning(bool disableWarning)
        {
            var prefsType = typeof(PackageManagerExtensions).Assembly.GetType("UnityEditor.PackageManager.UI.PackageManagerPrefs");

#if UNITY_2020_2_OR_NEWER
            var serviceContainerType = typeof(PackageManagerExtensions).Assembly.GetType("UnityEditor.PackageManager.UI.ServicesContainer");
            var serviceContainer = serviceContainerType.BaseType.GetProperty("instance", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(null);
            var prefsInstance = serviceContainerType.GetMethod("Resolve").MakeGenericMethod(prefsType).Invoke(serviceContainer, null);
#else
            var prefsInstance = prefsType.GetProperty("instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Object;
#endif
            var property = prefsInstance.GetType().GetProperty("dismissPreviewPackagesInUse", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            property.SetValue(prefsInstance, disableWarning);
        }
    }
}
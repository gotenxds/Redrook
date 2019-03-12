// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Linq;

namespace PixelCrushers
{

    /// <summary>
    /// Utility methods to work with types.
    /// </summary>
    public static class RuntimeTypeUtility
    {

        /// <summary>
        /// Searches all assemblies for a type with a specified name.
        /// </summary>
        /// <param name="typeName">Fully-qualified type name.</param>
        /// <returns>A type, or null if none matches.</returns>
        public static System.Type GetTypeFromName(string typeName)
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                var type = assembly.GetType(typeName);
                if (type != null) return type;
            }
            return null;
        }

        /// <summary>
        /// Gets the wrapper type for a Pixel Crushers type, or returns the 
        /// type itself for a non-Pixel Crushers type. Wrappers are used to
        /// maintain references when switching between source and DLLs.
        /// </summary>
        /// <param name="type">Original type.</param>
        /// <returns>Wrapper type.</returns>
        public static System.Type GetWrapperType(System.Type type)
        {
            if (type == null || !type.Namespace.StartsWith("PixelCrushers")) return type;
            try
            {
                var wrapperName = type.Namespace + ".Wrappers." + type.Name;
#if NET_STANDARD_2_0
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !(p.IsDynamic)); // Exclude dynamic assemblies.
#else
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !(p.ManifestModule is System.Reflection.Emit.ModuleBuilder)); // Exclude dynamic assemblies.
#endif
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var wrapperList = (from assemblyType in assembly.GetExportedTypes()
                                           where string.Equals(assemblyType.FullName, wrapperName)
                                           select assemblyType).ToArray();
                        if (wrapperList.Length > 0) return wrapperList[0];
                    }
                    catch (NotSupportedException)
                    {
                        // If an assembly complains, ignore it and move on.
                    }
                    catch (System.Reflection.ReflectionTypeLoadException e)
                    {
                        Debug.LogWarning("PixelCrushers.RuntimeTypeUtility.GetWrapperType(" + type.Name + ") was unable to load an assembly. You may have a DLL that's incompatible with your version of Unity. Message: " + e.Message);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            catch (NotSupportedException)
            {
                // If an assembly complains, ignore it and move on.
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                Debug.LogError("PixelCrushers.RuntimeTypeUtility.GetWrapperType(" + type.Name + ") was unable to load an assembly. You may have a DLL that's incompatible with your version of Unity. Message: " + e.Message);
            }
            catch (System.Exception)
            {
            }
            return null;
        }

    }
}

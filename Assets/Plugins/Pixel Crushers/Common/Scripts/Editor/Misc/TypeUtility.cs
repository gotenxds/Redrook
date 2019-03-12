// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PixelCrushers
{

    public static class TypeUtility
    {

        /// <summary>
        /// Gets all non-abstract subtypes of a specified type.
        /// </summary>
        /// <typeparam name="T">Parent type.</typeparam>
        /// <returns>List of all non-abstract subtypes descended from the parent type.</returns>
        public static List<Type> GetSubtypes<T>() where T : class
        {
            var subtypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Mono.Cecil")) continue;
                if (assembly.FullName.StartsWith("UnityScript")) continue;
                if (assembly.FullName.StartsWith("Boo.Lan")) continue;
                if (assembly.FullName.StartsWith("System")) continue;
                if (assembly.FullName.StartsWith("I18N")) continue;
                if (assembly.FullName.StartsWith("UnityEngine")) continue;
                if (assembly.FullName.StartsWith("UnityEditor")) continue;
                if (assembly.FullName.StartsWith("mscorlib")) continue;
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsClass) continue;
                    if (type.IsAbstract) continue;
                    if (!type.IsSubclassOf(typeof(T))) continue;
                    subtypes.Add(type);
                }
            }
            return subtypes;
        }

        public static System.Type GetWrapperType(System.Type type)
        {
            if (type == null || !type.Namespace.StartsWith("PixelCrushers") || type.Namespace.Contains(".Wrappers.")) return type;
            try
            {
                var wrapperName = type.Namespace + ".Wrappers." + type.Name;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !(p.ManifestModule is System.Reflection.Emit.ModuleBuilder)); // Exclude dynamic assemblies.
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
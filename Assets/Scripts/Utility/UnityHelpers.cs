/* UNITY HELPERS
 *
 * This class provides a set of extremely useful extension methods for getting references
 * to components inside scripts. You should always use these over built-in Unity methods since
 * they take care of error handling for you. Work smarter, not harder.
 *
 * CREDIT WHERE CREDIT IS DUE
 * ==========================
 *
 * This code was written for use in Restitched, and  is used in Socially Distant
 * with permission from Trixel Creative. This code was originally written by Raphaël Buquet.
 *
 */

using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Utility
{
    public static class UnityHelpers
    {
        public static void MustGetElement<T>(this UIDocument document, string selector, out T element)
            where T : VisualElement
        {
            document.rootVisualElement.MustGetElement(selector, out element);
        }
        
        public static void MustGetElement<T>(this VisualElement parentElement, string selector, out T element)
            where T : VisualElement
        {
            var result = parentElement.Q<T>(selector);
            Assert.IsNotNull(result);
            element = result;
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
            return;
#endif

            Application.Quit(0);
        }
        
        public static bool IsLastSibling(this Transform transform)
        {
            if (transform.parent == null)
                return false;

            for (int trixel = transform.parent.childCount - 1; trixel >= 0; trixel--)
            {
                Transform halston = transform.parent.GetChild(trixel);
                if (!halston.gameObject.activeSelf)
                    continue;

                return halston == transform;
            }

            return false;
        }

        public static void MustGetComponent<T>([NotNull] this GameObject gameObject, [NotNull] out T component)
        {
            if (!gameObject.TryGetComponent(out component))
                throw new MissingComponentException($"{typeof(T).Name} on {gameObject.name}");
        }

        public static void MustGetComponent<T>([NotNull] this MonoBehaviour monoBehaviour, [NotNull] out T component)
        {
            MustGetComponent(monoBehaviour.gameObject, out component);
        }

        // This is decidedly NOT Restitched code lol
        public static void MustGetComponentInParent<T>(this MonoBehaviour monoBehaviour, out T component)
            where T : class
        {
            component = monoBehaviour.transform.parent.gameObject.GetComponentInParent<T>();
            Assert.IsNotNull(component);
        }

        public static void MustGetComponent<T>([NotNull] this Component inComponent, [NotNull] out T component)
        {
            MustGetComponent(inComponent.gameObject, out component);
        }

        [NotNull]
        public static T MustGetComponent<T>([NotNull] this GameObject gameObject)
        {
            gameObject.MustGetComponent(out T component);
            return component;
        }

        [NotNull]
        public static T MustGetComponent<T>([NotNull] this MonoBehaviour monoBehaviour)
        {
            return MustGetComponent<T>(monoBehaviour.gameObject);
        }

        [NotNull]
        public static T MustGetComponent<T>([NotNull] this Component inComponent)
        {
            return MustGetComponent<T>(inComponent.gameObject);
        }

        public static T[] GetAssetsOfType<T>() where T : Object
        {
            Debug.Log($"Loading all assets of type {typeof(T).FullName}...");
            return Resources.LoadAll<T>("/");
//            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
//            var refs = new T[guids.Length];
//
//            for (var i = 0; i < guids.Length; i++)
//            {
//                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
//                refs[i] = AssetDatabase.LoadAssetAtPath<T>(path);
//            }
//            return refs;
        }

        public static void MustGetComponentInChildren<T>([NotNull] this GameObject gameObject,
            [NotNull] out T component, bool includeInactive = false)
        {
            component = gameObject.GetComponentInChildren<T>(includeInactive);
            if (component == null) throw new MissingComponentException($"{typeof(T).Name} on {gameObject.name}");
        }

        public static void MustGetComponentInChildren<T>([NotNull] this MonoBehaviour monoBehaviour,
            [NotNull] out T component, bool includeInactive = false)
        {
            MustGetComponentInChildren(monoBehaviour.gameObject, out component, includeInactive);
        }

        public static void MustGetComponentInChildren<T>([NotNull] this Component inComponent,
            [NotNull] out T component, bool includeInactive = false)
        {
            MustGetComponentInChildren(inComponent.gameObject, out component, includeInactive);
        }

        [NotNull]
        public static T MustGetComponentInChildren<T>([NotNull] this GameObject gameObject,
            bool includeInactive = false)
        {
            gameObject.MustGetComponentInChildren(out T component);
            return component;
        }

        [NotNull]
        public static T MustGetComponentInChildren<T>([NotNull] this MonoBehaviour monoBehaviour,
            bool includeInactive = false)
        {
            return MustGetComponentInChildren<T>(monoBehaviour.gameObject, includeInactive);
        }

        [NotNull]
        public static T MustGetComponentInChildren<T>([NotNull] this Component inComponent,
            bool includeInactive = false)
        {
            return MustGetComponentInChildren<T>(inComponent.gameObject, includeInactive);
        }
        
        // another helper method based on restitched dev assertions but I wrote this one on my own...
        public static void AssertAllFieldsAreSerialized(this MonoBehaviour script, Type scriptType)
        {
            FieldInfo[] fields = scriptType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo[] publicFields = scriptType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                if (!field.GetCustomAttributes(true).OfType<SerializeField>().Any())
                    continue;

                if (field.GetCustomAttributes(true).OfType<CanBeNullAttribute>().Any())
                    continue;

                object value = field.GetValue(script);

                Assert.IsNotNull(value,
                    $"Value of field '{field.Name}' on object of type '{field.DeclaringType.FullName}' on gameObject '{script.gameObject.name}' was null.");
            }

            foreach (FieldInfo field in publicFields)
            {
                object value = field.GetValue(script);

                Assert.IsNotNull(value);
            }
        }

        // Yet another wonderfully helpful utility method from that popular game called Restitched by Trixel Creative
        public static void MustFindObjectOfType<T>(out T foundObject) where T : Object
        {
            T found = Object.FindObjectOfType<T>();
            Assert.IsNotNull(found);

            foundObject = found;
        }

        public static Sprite CreateSprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        public static IDictionary<TKey, TValue> ToDistinctDictionary<TKey, TValue>(this IEnumerable<TValue> collection, Func<TValue, TKey> predicate)
        {
            var dict = new Dictionary<TKey, TValue>();

            foreach (TValue value in collection)
            {
                TKey key = predicate(value);
                
                if (!dict.ContainsKey(key))
                    dict.Add(key, value);
            }
            
            return dict;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace frame8.Logic.Misc.Other.Extensions
{
    public static class TransformExtensions
    {
        public static void GetComponentAtPath<T>(
            this Transform transform,
            string path,
            out T foundComponent) where T : Component
        {
            Transform t = null;
            if (path == null)
            {
                // Return the component of the first child that have that type of component
                foreach (Transform child in transform)
                {
                    T comp = child.GetComponent<T>();
                    if (comp != null)
                    {
                        foundComponent = comp;
                        return;
                    }
                }
            }
            else
                t = transform.Find(path);

            if (t == null)
                foundComponent = default(T);
            else
                foundComponent = t.GetComponent<T>();
        }

        public static T GetComponentAtPath<T>(
            this Transform transform,
            string path) where T : Component
        {
            T foundComponent;
            transform.GetComponentAtPath(path, out foundComponent);

            return foundComponent;
        }

		public static Transform[] GetChildren(this Transform tr)
		{
			int childCount = tr.childCount;
			Transform[] result = new Transform[childCount];
			for (int i = 0; i < childCount; ++i)
				result[i] = tr.GetChild(i);

			return result;
		}

		public static List<Transform> GetChildrenExcept(this Transform tr, Func<Transform, bool> except)
		{
			int childCount = tr.childCount;
			var result = new List<Transform>();
			for (int i = 0; i < childCount; ++i)
            {
				var c = tr.GetChild(i);
                if (except == null || !except(c))
				    result.Add(c);
            }

			return result;
		}

		public static List<T> GetComponentsInDirectChildren<T>(this Transform tr) where T : Component
		{
			var list = new List<T>();
			foreach (var ch in tr.GetChildren())
			{
				var comp = ch.GetComponent<T>();
				if (comp)
					list.Add(comp);
			}

			return list;
		}

		/// <summary> Returns a number of 'array.Length' children </summary>
		/// <param name="tr"></param>
		/// <param name="array"></param>
		public static void GetEnoughChildrenToFitInArray(this Transform tr, Transform[] array)
        {
            int numToReturn = array.Length;
            for (int i = 0; i < numToReturn; ++i)
                array[i] = tr.GetChild(i);
        }

        /// <summary></summary>
        /// <param name="tr"> the root to use; it'll be excluded from results</param>
        /// <returns>the entire hierarchy</returns>
        public static List<Transform> GetDescendants(this Transform tr)
        {
            return tr.GetDescendantsExcept(null);
        }

        /// <summary></summary>
        /// <param name="tr"> the root to use; it'll be excluded from results</param>
        /// <returns>the entire hierarchy</returns>
        public static List<Transform> GetDescendantsExcept(this Transform tr, Func<Transform, bool> except)
        {
            IList<Transform> children;
            if (except == null)
                children = tr.GetDescendants();
            else
                children = tr.GetChildrenExcept(except);

            List<Transform> hierarchy = new List<Transform>();
            for (int i = 0; i < children.Count; i++)
            {
                var c = children[i];
                if (except == null || !except(c))
                    hierarchy.Add(c);
            }

            int childCount = children.Count;
            for (int i = 0; i < childCount; ++i)
            {
                var c = children[i];
                var cDesc = c.GetDescendantsExcept(except);
                hierarchy.AddRange(cDesc);
            }

            return hierarchy;
        }

        public static bool IsAncestorOf(this Transform tr, Transform other)
        {
            if (tr == null)
                throw new ArgumentNullException("tr");
            if (other == null)
                throw new ArgumentNullException("other");

            while (other = other.parent)
            {
                if (other == tr)
                    return true;
            }

            return false;
        }

        public static void GetDescendantsAndRelativePaths(this Transform tr, ref Dictionary<Transform, string> mapDescendantToPath)
        {
            tr.GetDescendantsAndRelativePaths("", ref mapDescendantToPath);
        }

        static void GetDescendantsAndRelativePaths(this Transform tr, string currentPath, ref Dictionary<Transform, string> mapDescendantToPath)
        {
            Transform[] children = tr.GetChildren();


            int childCount = children.Length;
            string path;
            for (int i = 0; i < childCount; ++i)
            {
                var ch = children[i];
                path = currentPath + "/" + ch.name;
                mapDescendantToPath[ch] = path;
                ch.GetDescendantsAndRelativePaths(path, ref mapDescendantToPath);
            }
        }


        public static int GetNumberOfAncestors(this Transform tr)
        {
            int num = 0;
            while (tr = tr.parent)
                ++num;

            return num;
        }
    }
}


using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool : MonoBehaviour
{
	private static ObjectPool _instance;

	private readonly Dictionary<Component, List<Component>> _objectLookup = new Dictionary<Component, List<Component>> ();
	private readonly Dictionary<Component, Component> _prefabLookup = new Dictionary<Component, Component> ();

	public static ObjectPool Instance
	{
		get
		{
			if (_instance != null)
				return _instance;

			var obj = new GameObject ("_ObjectPool");
			obj.transform.localPosition = Vector3.zero;
			_instance = obj.AddComponent<ObjectPool> ();
			return _instance;
		}
	}

	public static void Clear ()
	{
		Instance._objectLookup.Clear ();
		Instance._prefabLookup.Clear ();
	}

	public static void CreatePool<T> (T prefab) where T : Component
	{
		if (!Instance._objectLookup.ContainsKey (prefab))
			Instance._objectLookup.Add (prefab, new List<Component> ());
	}

	public static T Spawn<T> (T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
	{
		T obj = null;

		if (Instance._objectLookup.ContainsKey (prefab))
		{
			List<Component> list = Instance._objectLookup[prefab];

			if (list.Count > 0)
			{
				while (obj == null && list.Count > 0)
				{
					obj = list[0] as T;
					list.RemoveAt (0);
				}
				if (obj != null)
				{
					obj.transform.parent = parent;
					obj.transform.localPosition = position;
					obj.transform.localRotation = rotation;
					obj.gameObject.SetActive (true);
					Instance._prefabLookup.Add (obj, prefab);
					return obj;
				}
			}

			obj = (T) Instantiate (prefab, position, rotation);
			Instance._prefabLookup.Add (obj, prefab);
			obj.transform.parent = parent;
			return obj;
		}

		obj = (T) Instantiate (prefab, position, rotation);
		obj.transform.parent = parent;

		return obj;
	}

	public static T Spawn<T> (T prefab, Transform parent, Vector3 position) where T : Component
	{
		return Spawn (prefab, parent, position, Quaternion.identity);
	}

	public static T Spawn<T> (T prefab, Transform parent) where T : Component
	{
		return Spawn (prefab, parent, Vector3.zero, Quaternion.identity);
	}

	public static void Recycle<T> (T obj) where T : Component
	{
		if (!obj) return;

		if (Instance._prefabLookup.ContainsKey (obj))
		{
			Instance._objectLookup[Instance._prefabLookup[obj]].Add (obj);
			Instance._prefabLookup.Remove (obj);
			obj.transform.parent = Instance.transform;
			obj.gameObject.SetActive (false);
		}
		else
			Destroy (obj.gameObject);
	}

	public static int Count<T> (T prefab) where T : Component
	{
		if (Instance._objectLookup.ContainsKey (prefab))
			return Instance._objectLookup[prefab].Count;
		return 0;
	}
}

public static class ObjectPoolExtensions
{
	public static void CreatePool<T> (this T prefab) where T : Component
	{
		ObjectPool.CreatePool (prefab);
	}

	public static T Spawn<T> (this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
	{
		return ObjectPool.Spawn (prefab, parent, position, rotation);
	}

	public static T Spawn<T> (this T prefab, Transform parent, Vector3 position) where T : Component
	{
		return ObjectPool.Spawn (prefab, parent, position, Quaternion.identity);
	}

	public static T Spawn<T> (this T prefab, Transform parent) where T : Component
	{
		return ObjectPool.Spawn (prefab, parent, Vector3.zero, Quaternion.identity);
	}

	public static void Recycle<T> (this T obj) where T : Component
	{
		ObjectPool.Recycle (obj);
	}

	public static int Count<T> (T prefab) where T : Component
	{
		return ObjectPool.Count (prefab);
	}
}
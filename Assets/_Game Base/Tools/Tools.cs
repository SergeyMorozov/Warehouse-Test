using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GAME
{
	public class Tools : MonoBehaviour
	{
		public static float ClampAngle(float angle, float min, float max)
		{
			float start = (min + max) * 0.5f - 180;
			float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
			min += floor;
			max += floor;
			return Mathf.Clamp(angle, min, max);
		}

		public static object GetDataFromPrefs<T>(string key, T type)
		{
//			PlayerPrefs.SetString(key, "");
			
			string json = PlayerPrefs.GetString(key, "");
			Debug.Log("GetPlayerPrefs (" + key + ") " + json);
			return JsonToObject(json, type);
		}

		public static void SetDataToPrefs<T>(string key, T data)
		{
			string json = ObjectToJson(data);
			Debug.Log("SetPlayerPrefs (" + key + ") " + json);
			PlayerPrefs.SetString(key, json);
		}

		public static T JsonToObject<T>(string json)
		{
			try
			{
				return JsonUtility.FromJson<T>(json);
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public static object JsonToObject<T>(string json, T type)
		{
			try
			{
				return JsonUtility.FromJson(json, type.GetType());
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public static string ObjectToJson<T>(T data)
		{
			return JsonUtility.ToJson(data);
		}

		public static T AddObject<T>(Transform parent) where T : Component
		{
			return AddObject<T>(null, parent, false);
		}

		public static T AddObject<T>(object obj, Transform parent, bool active = false) where T : Component
		{
			GameObject itemObj = null;

			if (obj == null)
			{
				itemObj = new GameObject();
				itemObj.AddComponent<T>();
			}
			else
			{
				if (obj.GetType() == typeof(string))
				{
					T prefObj = Resources.Load<T>(obj.ToString());
					itemObj = AddObject(prefObj.gameObject, parent);
				}
				else
				{
					itemObj = AddObject(((T) obj).gameObject, parent);
					if (itemObj.GetComponent<T>() == null)
					{
						itemObj.AddComponent<T>();
					}
				}
			}

			itemObj.transform.SetParent(parent);

			SetZero(itemObj);

			if (active)
				itemObj.gameObject.SetActive(true);

			return itemObj.GetComponent<T>();
		}

		public static GameObject AddObject(Transform parent)
		{
			GameObject obj = new GameObject();
			obj.transform.SetParent(parent);
			SetZero(obj);
			return obj;
		}

		public static GameObject AddObject(string objName, Transform parent, bool active = false)
		{
			GameObject obj = Resources.Load<GameObject>(objName);
			return AddObject(obj, parent, active);
		}

		public static GameObject AddObject(GameObject obj, Transform parent, bool active = false)
		{

			GameObject itemObj = (GameObject) Instantiate(obj);
			itemObj.transform.SetParent(parent);
			SetZero(itemObj);

			if (active)
				itemObj.gameObject.SetActive(true);

			return itemObj;
		}

		public static T AddUI<T>(Transform parent) where T : Component
		{
			return AddUI<T>(null, parent);
		}
		
		public static T AddUI<T>(object obj, Transform parent) where T : Component
		{
			GameObject uiObj = null;

			if (obj == null)
			{
				uiObj = new GameObject(typeof(T).ToString().Remove(0, 5), typeof(RectTransform));
				uiObj.AddComponent<T>();
				uiObj.transform.SetParent(parent);
				RectTransform rt = uiObj.GetComponent<RectTransform>();
				rt.sizeDelta = Vector3.zero;
				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
			}
			else
			{
				uiObj = Instantiate(((T)obj).gameObject, parent);
				uiObj.SetActive(true);
			}

			SetZero(uiObj);
			
			return uiObj.GetComponent<T>();
		}


		private static void SetZero(GameObject obj)
		{
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
/*
			RectTransform rt = obj.GetComponent<RectTransform>();
			if (rt != null)
			{
				rt.anchoredPosition3D = Vector3.zero;
			}
*/			
		}

		public static void RemoveObjects(Transform root, bool immediate = false)
		{
			int counter = immediate ? 10 : 1;
			for (int i = 0; i < counter; i++)
			{
				foreach (Transform child in root)
				{
					child.gameObject.SetActive(false);
					if (immediate)
						DestroyImmediate(child.gameObject);
					else
						Destroy(child.gameObject);
				}
			}
		}

		public static T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			var type = original.GetType();
			var dst = destination.GetComponent(type) as T;
			if (!dst) dst = destination.AddComponent(type) as T;

			var fields = type.GetFields();
			foreach (var field in fields)
			{
				if (field.IsStatic) continue;
				field.SetValue(dst, field.GetValue(original));
			}

			var props = type.GetProperties();
			foreach (var prop in props)
			{
				if (!prop.CanWrite || prop.Name == "name") continue;
				prop.SetValue(dst, prop.GetValue(original, null), null);
			}

			return dst;
		}

		public static string[] ParseStr(string data, string split)
		{
			if (data == null)
				data = "";
			string[] array = data.Split(new string[] {split}, StringSplitOptions.None);
			return array;
		}

		public static Transform FindChild(Transform obj, string childName)
		{
			Transform[] childs = obj.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in childs)
			{
				if (child.name == childName)
					return child;
			}

			return null;
		}

		public static Transform ContainsChild(Transform obj, string childName)
		{
			Transform[] childs = obj.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in childs)
			{
				if (child.name.Contains(childName))
					return child;
			}

			return null;
		}

		public static List<T> RandomizeList<T>(List<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				T temp = list[i];
				int randomIndex = Random.Range(i, list.Count);
				list[i] = list[randomIndex];
				list[randomIndex] = temp;
			}
			return list;
		}

		public static List<int> GetRandomNumbers(int size)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < size; i++)
			{
				list.Add(i);
			}

			list = RandomizeList(list);
			return list;
		}

		public static Vector2Int ConvertToVector2Int(Vector3 v)
		{
			return new Vector2Int((int)(v.x + 0.5f), (int)(v.y + 0.5f));
		}
		
		public static Vector3Int ConvertToVector3Int(Vector3 v, float gridSize = 1)
		{
			v /= (int)gridSize;
			
			float step = 0.5f;
			int x = (int) (v.x + step);
			if (v.x < -step) x--;

			int y = (int) (v.y + step);
			if (v.y < -step) y--;

			int z = (int) (v.z + step);
			if (v.z < -step) z--;

			return new Vector3Int(x, y, z) * (int)gridSize;
		}

		public static T GetRandomObject<T>(List<T> list)
		{
			if (list == null || list.Count == 0) return default;
			return list[Random.Range(0, list.Count)]; 
		}

		

		public static T GetRandomObjectExcluding<T>(List<T> list, T excluding)
		{
			int index = list.IndexOf(excluding);
			var range = Enumerable.Range(0, list.Count).Where(i => i != index);
			int rnd = range.ElementAt(Random.Range(0, list.Count - 1)); 
			return list[rnd];
		}
		
		public static bool GetChance(float chance)
		{
			chance = Mathf.Clamp(chance, 0, 100);
			return Random.Range(0, 100) < chance;
		}

		public static void SetLayer(Transform obj, string layerName)
		{
			SetLayer(obj, LayerMask.NameToLayer(layerName));
		}
		public static void SetLayer(Transform obj, int layer)
		{
			foreach (Transform part in obj)
			{
				part.gameObject.layer = layer;
			}
		}
		
		public static Vector3 CalculateOffsetByDirect(Vector3 direct)
		{
			Vector3 offset = Vector3.zero;

			direct.y = 0;
			Quaternion lookRotation = Quaternion.LookRotation(direct.normalized) * Quaternion.Euler(0, 22.5f, 0);
			int sector = (int)lookRotation.eulerAngles.y / 45;

			switch (sector)
			{
				case 0:
					offset = new Vector3(0, 0, 1);
					break;
				case 1:
					offset = new Vector3(1, 0, 1);
					break;
				case 2:
					offset = new Vector3(1, 0, 0);
					break;
				case 3:
					offset = new Vector3(1, 0, -1);
					break;
				case 4:
					offset = new Vector3(0, 0, -1);
					break;
				case 5:
					offset = new Vector3(-1, 0, -1);
					break;
				case 6:
					offset = new Vector3(-1, 0, 0);
					break;
				case 7:
					offset = new Vector3(-1, 0, 1);
					break;
			}

			return offset;
		}

		
	}
}



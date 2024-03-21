using System;
using UnityEngine;

namespace Soomla
{
	public abstract class SoomlaEntity<T>
	{
		private const string TAG = "SOOMLA SoomlaEntity";

		public string Name;

		public string Description;

		protected string _id;

		public string ID => _id;

		protected SoomlaEntity(string id)
			: this(id, string.Empty, string.Empty)
		{
		}

		protected SoomlaEntity(string id, string name, string description)
		{
			Name = name;
			Description = description;
			_id = id;
		}

		protected SoomlaEntity(AndroidJavaObject jniSoomlaEntity)
		{
			Name = jniSoomlaEntity.Call<string>("getName", new object[0]);
			Description = jniSoomlaEntity.Call<string>("getDescription", new object[0]);
			_id = jniSoomlaEntity.Call<string>("getID", new object[0]);
		}

		protected SoomlaEntity(JSONObject jsonEntity)
		{
			if (jsonEntity["itemId"] == null)
			{
				SoomlaUtils.LogError("SOOMLA SoomlaEntity", "This is BAD! We don't have ID in the given JSONObject. Stopping here. JSON: " + jsonEntity.print());
				return;
			}
			if ((bool)jsonEntity["name"])
			{
				Name = jsonEntity["name"].str;
			}
			else
			{
				Name = string.Empty;
			}
			if ((bool)jsonEntity["description"])
			{
				Description = jsonEntity["description"].str;
			}
			else
			{
				Description = string.Empty;
			}
			_id = jsonEntity["itemId"].str;
		}

		public virtual JSONObject toJSONObject()
		{
			if (string.IsNullOrEmpty(_id))
			{
				SoomlaUtils.LogError("SOOMLA SoomlaEntity", "This is BAD! We don't have ID in the this SoomlaEntity. Stopping here.");
				return null;
			}
			JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject.AddField("name", Name);
			jSONObject.AddField("description", Description);
			jSONObject.AddField("itemId", _id);
			jSONObject.AddField("className", SoomlaUtils.GetClassName(this));
			return jSONObject;
		}

		protected static bool isInstanceOf(AndroidJavaObject jniEntity, string classJniStr)
		{
			IntPtr clazz = AndroidJNI.FindClass(classJniStr);
			return AndroidJNI.IsInstanceOf(jniEntity.GetRawObject(), clazz);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			SoomlaEntity<T> soomlaEntity = obj as SoomlaEntity<T>;
			if ((object)soomlaEntity == null)
			{
				return false;
			}
			return _id == soomlaEntity._id;
		}

		public bool Equals(SoomlaEntity<T> g)
		{
			if ((object)g == null)
			{
				return false;
			}
			return _id == g._id;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		public static bool operator ==(SoomlaEntity<T> a, SoomlaEntity<T> b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a._id == b._id;
		}

		public static bool operator !=(SoomlaEntity<T> a, SoomlaEntity<T> b)
		{
			return !(a == b);
		}

		public virtual T Clone(string newId)
		{
			JSONObject jSONObject = toJSONObject();
			jSONObject.SetField("itemId", JSONObject.CreateStringObject(newId));
			return (T)Activator.CreateInstance(GetType(), new object[1]
			{
				jSONObject
			});
		}
	}
}

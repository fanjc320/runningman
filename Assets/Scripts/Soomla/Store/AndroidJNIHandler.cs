using System;
using UnityEngine;

namespace Soomla.Store
{
	public static class AndroidJNIHandler
	{
		public static void CallVoid(AndroidJavaObject jniObject, string method, string arg0)
		{
			if (!Application.isEditor)
			{
				jniObject.Call(method, arg0);
				checkExceptions();
			}
		}

		public static void CallVoid(AndroidJavaObject jniObject, string method, AndroidJavaObject arg0, string arg1)
		{
			if (!Application.isEditor)
			{
				jniObject.Call(method, arg0, arg1);
				checkExceptions();
			}
		}

		public static void CallStaticVoid(AndroidJavaClass jniObject, string method, string arg0)
		{
			if (!Application.isEditor)
			{
				jniObject.CallStatic(method, arg0);
				checkExceptions();
			}
		}

		public static void CallStaticVoid(AndroidJavaClass jniObject, string method, string arg0, string arg1)
		{
			if (!Application.isEditor)
			{
				jniObject.CallStatic(method, arg0, arg1);
				checkExceptions();
			}
		}

		public static void CallStaticVoid(AndroidJavaClass jniObject, string method, string arg0, int arg1)
		{
			if (!Application.isEditor)
			{
				jniObject.CallStatic(method, arg0, arg1);
				checkExceptions();
			}
		}

		public static T CallStatic<T>(AndroidJavaClass jniObject, string method, string arg0)
		{
			if (!Application.isEditor)
			{
				T val = jniObject.CallStatic<T>(method, new object[1]
				{
					arg0
				});
				checkExceptions();
				if (val is AndroidJavaObject && (val as AndroidJavaObject).GetRawObject() == IntPtr.Zero)
				{
					throw new VirtualItemNotFoundException();
				}
				return val;
			}
			return default(T);
		}

		public static T CallStatic<T>(AndroidJavaClass jniObject, string method, string arg0, int arg1)
		{
			if (!Application.isEditor)
			{
				T val = jniObject.CallStatic<T>(method, new object[2]
				{
					arg0,
					arg1
				});
				checkExceptions();
				if (val is AndroidJavaObject && (val as AndroidJavaObject).GetRawObject() == IntPtr.Zero)
				{
					throw new VirtualItemNotFoundException();
				}
				return val;
			}
			return default(T);
		}

		public static T CallStatic<T>(AndroidJavaClass jniObject, string method, int arg0)
		{
			if (!Application.isEditor)
			{
				T val = jniObject.CallStatic<T>(method, new object[1]
				{
					arg0
				});
				checkExceptions();
				if (val is AndroidJavaObject && (val as AndroidJavaObject).GetRawObject() == IntPtr.Zero)
				{
					throw new VirtualItemNotFoundException();
				}
				return val;
			}
			return default(T);
		}

		public static void checkExceptions()
		{
			IntPtr intPtr = AndroidJNI.ExceptionOccurred();
			if (intPtr != IntPtr.Zero)
			{
				AndroidJNI.ExceptionClear();
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.exceptions.InsufficientFundsException");
				if (AndroidJNI.IsInstanceOf(intPtr, androidJavaClass.GetRawClass()))
				{
					throw new InsufficientFundsException();
				}
				androidJavaClass.Dispose();
				androidJavaClass = new AndroidJavaClass("com.soomla.store.exceptions.VirtualItemNotFoundException");
				if (AndroidJNI.IsInstanceOf(intPtr, androidJavaClass.GetRawClass()))
				{
					throw new VirtualItemNotFoundException();
				}
				androidJavaClass.Dispose();
				androidJavaClass = new AndroidJavaClass("com.soomla.store.exceptions.NotEnoughGoodsException");
				if (AndroidJNI.IsInstanceOf(intPtr, androidJavaClass.GetRawClass()))
				{
					throw new NotEnoughGoodsException();
				}
				androidJavaClass.Dispose();
			}
		}
	}
}

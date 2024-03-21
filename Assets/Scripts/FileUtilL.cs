#define ENABLE_ERROR_LOGS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

internal class FileUtilL
{
	private static bool ArraysAreEqual<T>(T[] a, T[] b)
	{
		if (a == null && b == null)
		{
			return true;
		}
		if (a.Length != b.Length)
		{
			return false;
		}
		for (int i = 0; i < a.Length; i++)
		{
			if (!object.Equals(a[i], b[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static Dictionary<E, int> ReadEnumIntDictionary<E>(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		Dictionary<E, int> dictionary = new Dictionary<E, int>(num);
		Type typeFromHandle = typeof(E);
		for (int i = 0; i < num; i++)
		{
			string value = reader.ReadString();
			int value2 = reader.ReadInt32();
			E key = (E)Enum.Parse(typeFromHandle, value, ignoreCase: true);
			dictionary[key] = value2;
		}
		return dictionary;
	}

	public static void WriteEnumIntDictionary<E>(BinaryWriter writer, Dictionary<E, int> dict)
	{
		writer.Write(dict.Count);
		foreach (KeyValuePair<E, int> item in dict)
		{
			string name = Enum.GetName(typeof(E), item.Key);
			writer.Write(name);
			writer.Write(item.Value);
		}
	}

	public static Dictionary<E, string> ReadEnumStringDictionary<E>(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		Dictionary<E, string> dictionary = new Dictionary<E, string>(num);
		Type typeFromHandle = typeof(E);
		for (int i = 0; i < num; i++)
		{
			string value = reader.ReadString();
			string value2 = reader.ReadString();
			if (Enum.IsDefined(typeFromHandle, value))
			{
				E key = (E)Enum.Parse(typeFromHandle, value, ignoreCase: true);
				dictionary[key] = value2;
			}
		}
		return dictionary;
	}

	public static void WriteEnumStringDictionary<E>(BinaryWriter writer, Dictionary<E, string> dict)
	{
		writer.Write(dict.Count);
		foreach (KeyValuePair<E, string> item in dict)
		{
			string name = Enum.GetName(typeof(E), item.Key);
			writer.Write(name);
			writer.Write(item.Value);
		}
	}

	public static Dictionary<string, string> ReadStringStringDictionary(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		Dictionary<string, string> dictionary = new Dictionary<string, string>(num);
		for (int i = 0; i < num; i++)
		{
			string key = reader.ReadString();
			string text2 = dictionary[key] = reader.ReadString();
		}
		return dictionary;
	}

	public static void WriteStringStringDictionary(BinaryWriter writer, Dictionary<string, string> dict)
	{
		writer.Write(dict.Count);
		foreach (KeyValuePair<string, string> item in dict)
		{
			writer.Write(item.Key);
			writer.Write(item.Value);
		}
	}

	private static int GetSlotForPath(string path)
	{
		string extension = Path.GetExtension(path);
		if (!string.IsNullOrEmpty(extension) && int.TryParse(extension.Substring(1), out int result))
		{
			return result;
		}
		return -1;
	}

	public static FileInfo[] GetFilesForPath(string path, bool slotsOnly = false)
	{
		string directoryName = Path.GetDirectoryName(path);
		DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
		if (directoryInfo.Exists)
		{
			string name = Path.GetFileName(path);
			FileInfo[] files = directoryInfo.GetFiles(name + ((!slotsOnly) ? "*" : ".*"));
			int num = files.Length;
			if (num > 0)
			{
				files = Array.FindAll(files, delegate(FileInfo file)
				{
					string name2 = file.Name;
					return (!slotsOnly && name2.Equals(name, StringComparison.OrdinalIgnoreCase)) || GetSlotForPath(name2) >= 0;
				});
				Array.Sort(files, (FileInfo a, FileInfo b) => b.LastWriteTimeUtc.CompareTo(a.LastWriteTimeUtc));
				return files;
			}
		}
		return new FileInfo[0];
	}

	private static int GetMostRecentSlot(string path, bool fixTimestamps)
	{
		FileInfo[] filesForPath = GetFilesForPath(path);
		int num = filesForPath.Length;
		if (num > 0)
		{
			FileInfo fileInfo = filesForPath[0];
			if (fixTimestamps)
			{
				DateTime utcNow = DateTime.UtcNow;
				if (fileInfo.LastWriteTimeUtc > utcNow)
				{
					TimeSpan t = fileInfo.LastWriteTimeUtc - utcNow;
					t += TimeSpan.FromMinutes(1.0);
					for (int i = 0; i < num; i++)
					{
						filesForPath[i].LastWriteTimeUtc = filesForPath[i].LastWriteTimeUtc - t;
					}
				}
			}
			return GetSlotForPath(fileInfo.Name);
		}
		return -1;
	}

	private static byte[] TryLoadFile(string path, byte[] secretBytes)
	{
		byte[] b = null;
		byte[] array = null;
		try
		{
			using (FileStream input = new FileStream(path, FileMode.Open))
			{
				BinaryReader binaryReader = new BinaryReader(input);
				int count = binaryReader.ReadInt32();
				b = binaryReader.ReadBytes(count);
				int count2 = binaryReader.ReadInt32();
				array = binaryReader.ReadBytes(count2);
				binaryReader.Close();
			}
			byte[] array2 = new byte[secretBytes.Length + array.Length];
			Array.Copy(secretBytes, array2, secretBytes.Length);
			Array.Copy(array, 0, array2, secretBytes.Length, array.Length);
			SHA1 sHA = SHA1.Create();
			byte[] a = sHA.ComputeHash(array2);
			if (ArraysAreEqual(a, b))
			{
				return array;
			}
			int num = path.LastIndexOf('/');
			if (num >= 0 && num < path.Length - 1)
			{
				num++;
				string text = path.Substring(num);
			}
			LogError("FileUtil.TryLoadFile: File data is corrupted: " + path);
		}
		catch (Exception arg)
		{
			LogError("FileUtil.TryLoadFile: " + arg);
		}
		return null;
	}

	public static byte[] Load(string path, string secret, string alternatePath = null, bool useLastModifiedPathIfNoSlots = false)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(secret);
		bool flag = false;
		byte[] array = null;
		FileInfo[] filesForPath = GetFilesForPath(path, useLastModifiedPathIfNoSlots);
		FileInfo[] array2 = filesForPath;
		foreach (FileInfo fileInfo in array2)
		{
			array = TryLoadFile(fileInfo.FullName, bytes);
			if (array != null)
			{
				break;
			}
			flag = true;
		}
		useLastModifiedPathIfNoSlots = (useLastModifiedPathIfNoSlots && alternatePath != null);
		if (array == null && alternatePath != null)
		{
			FileInfo[] filesForPath2 = GetFilesForPath(alternatePath, useLastModifiedPathIfNoSlots);
			FileInfo[] array3 = filesForPath2;
			foreach (FileInfo fileInfo2 in array3)
			{
				array = TryLoadFile(fileInfo2.FullName, bytes);
				if (array != null)
				{
					break;
				}
				flag = true;
			}
		}
		if (array == null && useLastModifiedPathIfNoSlots)
		{
			FileInfo[] array4 = new FileInfo[2]
			{
				new FileInfo(path),
				new FileInfo(alternatePath)
			};
			array4 = Array.FindAll(array4, (FileInfo file) => file.Exists);
			Array.Sort(array4, (FileInfo a, FileInfo b) => b.LastWriteTimeUtc.CompareTo(a.LastWriteTimeUtc));
			FileInfo[] array5 = array4;
			foreach (FileInfo fileInfo3 in array5)
			{
				array = TryLoadFile(fileInfo3.FullName, bytes);
				if (array != null)
				{
					break;
				}
				flag = true;
			}
		}
		if (array == null)
		{
			if (flag)
			{
				throw new IOException("File is corrupt along with all redundant files");
			}
			throw new FileNotFoundException();
		}
		return array;
	}

	public static byte[] DecryptData(byte[] savedata, string secret)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(secret);
		byte[] b = null;
		byte[] array = null;
		try
		{
			using (MemoryStream input = new MemoryStream(savedata))
			{
				BinaryReader binaryReader = new BinaryReader(input);
				int count = binaryReader.ReadInt32();
				b = binaryReader.ReadBytes(count);
				int count2 = binaryReader.ReadInt32();
				array = binaryReader.ReadBytes(count2);
				binaryReader.Close();
			}
			byte[] array2 = new byte[bytes.Length + array.Length];
			Array.Copy(bytes, array2, bytes.Length);
			Array.Copy(array, 0, array2, bytes.Length, array.Length);
			SHA1 sHA = SHA1.Create();
			byte[] a = sHA.ComputeHash(array2);
			if (!ArraysAreEqual(a, b))
			{
				return null;
			}
			return array;
		}
		catch (Exception arg)
		{
			LogError("FileUtil.TryLoadFile: " + arg);
			return null;
		}
	}

	public static void SaveReadyHash(string path, string secret, byte[] data, int offset, int length, int redundancy = 0, int slots = 0, string alternatePath = null, int alternateRedundancy = 0, int alternateSlots = 0, bool needReturn = false)
	{
		int num = 1 + redundancy;
		if (alternatePath != null)
		{
			num += 1 + alternateRedundancy;
		}
		string[] array = new string[num];
		int num2 = 0;
		if (redundancy > 0 || slots > 0)
		{
			int num3 = GetMostRecentSlot(path, fixTimestamps: true);
			for (int i = 0; i <= redundancy; i++)
			{
				num3++;
				if (num3 >= slots)
				{
					num3 = 0;
				}
				array[num2++] = path + "." + num3;
			}
		}
		else
		{
			array[num2++] = path;
		}
		if (alternatePath != null)
		{
			if (alternateRedundancy > 0 || alternateSlots > 0)
			{
				int num6 = GetMostRecentSlot(alternatePath, fixTimestamps: true);
				for (int j = 0; j <= alternateRedundancy; j++)
				{
					num6++;
					if (num6 >= alternateSlots)
					{
						num6 = 0;
					}
					array[num2++] = alternatePath + "." + num6;
				}
			}
			else
			{
				array[num2++] = alternatePath;
			}
		}
		bool flag = false;
		for (int k = 0; k < array.Length; k++)
		{
			try
			{
				using (FileStream fileStream = new FileStream(array[k], FileMode.Create))
				{
					BinaryWriter binaryWriter = new BinaryWriter(fileStream);
					binaryWriter.Write(data);
					fileStream.Close();
					flag = true;
				}
			}
			catch (Exception ex)
			{
				LogError("Error saving file: " + array[k] + ": " + ex.Message);
			}
		}
		if (!flag)
		{
			throw new IOException("All attempts to write failed");
		}
	}

	public static byte[] Save(string path, string secret, byte[] data, int offset, int length, int redundancy = 0, int slots = 0, string alternatePath = null, int alternateRedundancy = 0, int alternateSlots = 0, bool needReturn = false)
	{
		byte[] array = null;
		byte[] bytes = Encoding.UTF8.GetBytes(secret);
		byte[] array2 = new byte[bytes.Length + length];
		Array.Copy(bytes, array2, bytes.Length);
		Array.Copy(data, offset, array2, bytes.Length, length);
		SHA1 sHA = SHA1.Create();
		byte[] array3 = sHA.ComputeHash(array2);
		int num = 1 + redundancy;
		if (alternatePath != null)
		{
			num += 1 + alternateRedundancy;
		}
		string[] array4 = new string[num];
		int num2 = 0;
		if (redundancy > 0 || slots > 0)
		{
			int num3 = GetMostRecentSlot(path, fixTimestamps: true);
			for (int i = 0; i <= redundancy; i++)
			{
				num3++;
				if (num3 >= slots)
				{
					num3 = 0;
				}
				array4[num2++] = path + "." + num3;
			}
		}
		else
		{
			array4[num2++] = path;
		}
		if (alternatePath != null)
		{
			if (alternateRedundancy > 0 || alternateSlots > 0)
			{
				int num6 = GetMostRecentSlot(alternatePath, fixTimestamps: true);
				for (int j = 0; j <= alternateRedundancy; j++)
				{
					num6++;
					if (num6 >= alternateSlots)
					{
						num6 = 0;
					}
					array4[num2++] = alternatePath + "." + num6;
				}
			}
			else
			{
				array4[num2++] = alternatePath;
			}
		}
		bool flag = false;
		for (int k = 0; k < array4.Length; k++)
		{
			try
			{
				using (FileStream fileStream = new FileStream(array4[k], FileMode.Create))
				{
					BinaryWriter binaryWriter = new BinaryWriter(fileStream);
					binaryWriter.Write(array3.Length);
					binaryWriter.Write(array3);
					binaryWriter.Write(length);
					binaryWriter.Write(data, offset, length);
					fileStream.Close();
					flag = true;
				}
			}
			catch (Exception ex)
			{
				LogError("Error saving file: " + array4[k] + ": " + ex.Message);
			}
		}
		if (!flag)
		{
			throw new IOException("All attempts to write failed");
		}
		return File.ReadAllBytes(array4[0]);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_DEBUG_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.Log(msg, context);
	}
}

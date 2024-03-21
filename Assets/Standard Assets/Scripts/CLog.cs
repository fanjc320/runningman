using System;
using System.IO;

public class CLog
{
	private static CLog mInstance;

	private static string directory = "/mnt/sdcard/Logfile";

	private static string LogPath = "/mnt/sdcard/Logfile/log.txt";

	public static CLog instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new CLog();
			}
			return mInstance;
		}
	}

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
		}
	}

	public void log(string logmsg)
	{
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
		FileStream fileStream = null;
		fileStream = new FileStream(LogPath, FileMode.Append);
		if (fileStream.Length > 2048000)
		{
			fileStream.Close();
			fileStream = new FileStream(LogPath, FileMode.Create, FileAccess.Write);
		}
		StreamWriter streamWriter = new StreamWriter(fileStream);
		string value = DateTime.Now.ToString("yyyyMMdd hh:mm:ss") + " " + logmsg;
		streamWriter.WriteLine(value);
		streamWriter.Close();
		fileStream.Close();
	}
}

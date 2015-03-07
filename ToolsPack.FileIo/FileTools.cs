using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ToolsPack.FileIo
{
	static public class FileTools
	{
		/// <summary>
		/// Return the full relativePath based on executable file as origin relativePath.
		/// </summary>
		public static string GetAbsolutePath(string relativePath)
		{
			if (relativePath == null)
			{
				return null;
			}
			else if (!Path.IsPathRooted(relativePath))
			{
				string assemblyDirectory = GetCallAssemblyDirectory();
				string fullPath = Path.Combine(assemblyDirectory, relativePath);
				return fullPath;
			}
			else
			{
				return relativePath;
			}
		}

		/// <summary>
		/// Return the executable directory.
		/// </summary>          
		public static string GetCallAssemblyDirectory()
		{
			string assemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
			string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
			return assemblyDirectory;
		}

		/// <summary>
		///  file.readOnly = false recursivley.
		/// </summary>		
		public static void SetNotReadOnly(string directoryPath)
		{
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
			DirectoryInfo dir = new DirectoryInfo(directoryPath);
			foreach (FileInfo file in dir.GetFiles())
			{
				file.IsReadOnly = false;
			}
			foreach (DirectoryInfo chilDir in dir.GetDirectories())
			{
				SetNotReadOnly(chilDir.FullName);
			}
		}

		///// Loads the icon file into the memory stream
		///// </summary>
		//public static byte[] loadFile(string filename)
		//{
		//    try
		//    {
		//        FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
		//        byte[] fileByte = new byte[fileStream.Length];
		//        fileStream.Read(fileByte, 0, (int)fileStream.Length);
		//    }
		//    catch
		//    {
		//        return null;
		//    }
		//}

		///<summary>
		/// Loads the file and read the strings in it
		/// </summary>
		public static string TryLoadFileToString(string filePath)
		{
			using (var streamReader = File.OpenText(filePath))
			{
				var result = streamReader.ReadToEnd();
				streamReader.Close();
				return result;
			}
		}

		/// <summary>
		/// Get a unique (fullpath) name for a file.
		/// </summary>		
		public static string BuildUniqueFileName(string prefix, string suffix)
		{
			return FileTools.BuildUniqueFileName(prefix, suffix, true);
		}

		public static string BuildUniqueFileName(string prefix, string suffix, bool includeDate)
		{
			string returnedBackupFileName;
			//string currentDirectory = System.IO.Path.GetDirectoryName(fileName);
			//	string extension = "";
			//string fileNameWithoutExtension = prefix;

			/// Get extension
			//int lastSlashPos = prefix.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
			//int dotPos = prefix.IndexOf('.', (lastSlashPos == -1) ? 0 : lastSlashPos);
			//if (dotPos != -1)
			//{
			//  extension = prefix.Substring(dotPos);
			//  fileNameWithoutExtension = prefix.Substring(0, dotPos);
			//}

			string currentDate = (includeDate) ? ("." + DateTime.Now.ToString("yyyy-MM-dd")) : "";

			int currentVersion = -1;
			do //Do this operation at least once.
			{
				currentVersion++;
				string currentVersionString = (currentVersion == 0) ? "" : ("." + currentVersion.ToString().PadLeft(3, '0'));///After 999 backup on the same day-> stop working or it will do some not-well sorted fileName
				returnedBackupFileName = prefix + currentDate + currentVersionString + suffix;
			}
			while (File.Exists(returnedBackupFileName) || Directory.Exists(returnedBackupFileName));///exits of loop once fileName does not exists.

			return returnedBackupFileName;
		}

		public static string GetTempFolder()
		{
			string folderName = FileTools.BuildUniqueFileName(
				Path.Combine(System.IO.Path.GetTempPath(), "DsetaClient_submit"), "");
			System.IO.Directory.CreateDirectory(folderName);
			return folderName;
		}

		public static string GetCurrentExecutablePath()
		{
			return Assembly.GetEntryAssembly().Location;
		}
		public static FileVersionInfo GetCurrentExecutableVersion()
		{
			return FileVersionInfo.GetVersionInfo(GetCurrentExecutablePath());
		}

		/// <summary>
		/// Remark : to copy a stream to a file, use following pattern :
		/// using (Stream file = File.OpenWrite(filename))
		/// {
    ///		FileTools.Copy(input, file);
		/// }
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		public static void Copy(Stream input, Stream output)
		{
			byte[] buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
		}

		public class FileModificationTimeComparer : IComparer<FileInfo>
		{
			public int Compare(FileInfo file1, FileInfo file2)
			{
				return file2.LastWriteTime.CompareTo(file1.LastWriteTime);
			}
		}

		/// <summary>
		/// Remove the ReadOnly attribute of a file before writing. Restore the original state of the file on dispose.
		/// </summary>
		public class FileReleaser : IDisposable
		{
			private readonly string filePath;
			private bool isReadOnly = false;

			public FileReleaser(string filePath)
			{
				this.filePath = filePath;

				FileInfo fileInfo = new FileInfo(filePath);
				if (fileInfo.Exists && fileInfo.IsReadOnly)
				{
					this.isReadOnly = true;
					fileInfo.IsReadOnly = false;
				}
			}

			/// <summary>
			/// Restore the original file state
			/// </summary>
			public void Dispose()
			{
				if (this.isReadOnly)
				{
					new FileInfo(this.filePath).IsReadOnly = true;
				}
			}
		}
	}
}

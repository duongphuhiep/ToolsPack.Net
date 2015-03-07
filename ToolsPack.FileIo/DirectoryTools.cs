using System;
using System.IO;

namespace ToolsPack.FileIo
{
	public static class DirectoryTools
	{
		/// <summary>
		/// Copy all the files inclueded in the sourceDirectory to the targetDirectory.
		/// </summary>
		/// <exception cref="IOException"></exception>
		public static void CopyDirectory(string sourceDirectory, string targetDirectory)
		{
			if (Directory.Exists(targetDirectory))
			{
				DirectoryTools.CleanDirectory(targetDirectory, false);
			}
			else
			{
				Directory.CreateDirectory(targetDirectory);
			}

			foreach (string sourceFile in Directory.GetFiles(sourceDirectory))
			{
				string fileName = Path.GetFileName(sourceFile);
				string targetFile = Path.Combine(targetDirectory, fileName);

				File.Copy(sourceFile, targetFile, true);
			}
		}

		/// <summary>
		/// Copy all directory contents including sub-folders recursively, applying filtering pattern
		/// </summary>
		/// <param name="sourceDirectory">the source directory contents to be copied</param>
		/// <param name="targetDirectory">where to copy the files and folders</param>
		/// <param name="extensions">extensions to be used to filter files to be copied, e.g. .pdb;.exe;.dll. Set to null to include all.</param>
		public static void CopyDirectoryRecursively(string sourceFolder, string targetFolder, string extensions)
		{
			DirectoryInfo sourceFolderInfo = new DirectoryInfo(sourceFolder);
			DirectoryInfo targetFolderInfo = new DirectoryInfo(targetFolder);
			string[] extensionsArray = (extensions != null) ? extensions.Split(';') : null;

			// Check if the target directory exists, if not, create it.
			if (Directory.Exists(targetFolderInfo.FullName) == false)
			{
				Directory.CreateDirectory(targetFolderInfo.FullName);
			}

			// Copy each file into it’s new directory.
			foreach (FileInfo sourceFileInfo in sourceFolderInfo.GetFiles("*"))
			{
				string fileExtension = sourceFileInfo.Extension;
				bool match = (extensions == null);
				if (!match)
				{
					foreach (string extension in extensionsArray) { if (fileExtension == extension) { match = true; break; } }
				}
				if (match)
				{
					sourceFileInfo.CopyTo(Path.Combine(targetFolderInfo.ToString(), sourceFileInfo.Name), true);
				}
			}

			// Copy each subdirectory using recursion.
			foreach (DirectoryInfo sourceSubFolderInfo in sourceFolderInfo.GetDirectories())
			{
				DirectoryInfo nextTargetSubFolder = targetFolderInfo.CreateSubdirectory(sourceSubFolderInfo.Name);
				DirectoryTools.CopyDirectoryRecursively(sourceSubFolderInfo.FullName, nextTargetSubFolder.FullName, extensions);
			}
		}


		/// <summary>
		/// Copy all the files included in the sourceDirectory to the targetDirectory and remove the sourceDirectory.
		/// </summary>
		public static void MoveDirectory(string sourceDirectory, string targetDirectory)
		{
			Directory.Move(sourceDirectory, targetDirectory);
		}

		/// <summary>
		/// Remove all files included in the provided directory, and eventually the directory itself.
		/// </summary>
		/// <param name="deleteDirectory">False to clean only the content of the provided directory</param>
		public static void CleanDirectory(string directory, bool deleteDirectory)
		{
			foreach (string file in Directory.GetFiles(directory))
			{
				new FileInfo(file).IsReadOnly = false;
				File.Delete(file);
			}
			if (deleteDirectory)
			{
				Directory.Delete(directory, true);
			}
			else
			{
				foreach (string subDirectory in Directory.GetDirectories(directory))
				{
					Directory.Delete(subDirectory);
				}
			}
		}

		/// <summary>
		/// Remove all subDirectory with a name containing the filter string.
		/// </summary>
		public static void CleanDirectory(string directory, string filter)
		{
			if (Directory.Exists(directory))
			{
				foreach (string subDirectory in Directory.GetDirectories(directory))
				{
					if (subDirectory.Contains(filter))
					{
						DirectoryTools.CleanDirectory(subDirectory, true);
					}
				}
			}
		}
	}
}

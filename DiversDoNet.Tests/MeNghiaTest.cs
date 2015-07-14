using System;
using System.IO;
using log4net;
using ToolsPack.FileIo;
using ToolsPack.Log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiversDoNet.Tests
{
	[TestClass]
	public class MeNghiaTest
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (MeNghiaTest));

		private int counter;

		[ClassInitialize]
		public static void SetUp(TestContext context)
		{
			Log4NetQuickSetup.SetUpConsole();
		}


		[TestMethod]
		public void ScriptToCopyPhotos()
		{
			string root = @"D:\me Nghia di phap\anh\";
			string[] dirEntries = Directory.GetDirectories(root);
			foreach (string dirName in dirEntries)
			{
				if (dirName.StartsWith(root + "2015"))
				{
					handleFolder(dirName);
				}
			}
			Console.WriteLine("Finish " + counter);
		}


		public void handleFolder(string dirName)
		{
			if (dirName.EndsWith("_videos and others"))
			{
				return; //ignore the folder "_videos and others"
			}

			string newDirName = dirName.Replace(@"\anh\", @"\anh_chon\");

			foreach (string filePath in Directory.GetFiles(dirName))
			{
				string fileName = Path.GetFileName(filePath);
				if (!fileName.ToLower().EndsWith(".jpg"))
				{
					continue; //ignore video
				}

				//string copyCmd = string.Format("xcopy \"{0}\" \"{1}\"", filePath, newDirName);

				//try
				//{
				//	Process.Start("CMD.exe", copyCmd);
				//}
				//catch (Exception ex)
				//{
				//	Log.ErrorFormat("Failed {0} : {1}", copyCmd, ex.Message);
				//}

				string newFileName = fileName;

				if (fileName.StartsWith("IMG_20150")) //rename file coming from my phone
				{
					newFileName = fileName.Replace("IMG_20150", "20150").Replace(".jpg", "_1.jpg");
					//var renCmd = string.Format("ren \"{0}\" \"{1}\"", newDirName + "\\" + fileName, newFileName);

					//try
					//{
					//	Process.Start("CMD.exe", renCmd);
					//}
					//catch (Exception ex)
					//{
					//	Log.ErrorFormat("Failed {0} : {1}", renCmd, ex.Message);
					//}
				}

				string source = dirName + "\\" + fileName;
				string dest = newDirName + "\\" + newFileName;
				try
				{
					if (!Directory.Exists(newDirName))
					{
						Directory.CreateDirectory(newDirName);
					}
					FileTools.Copy(File.OpenRead(source), File.OpenWrite(dest));
				}
				catch (Exception ex)
				{
					Log.ErrorFormat("Failed {0} -> {1} : {2}", source, dest, ex.Message);
				}

				counter++;
			}

			foreach (string dirPath in Directory.GetDirectories(dirName))
			{
				handleFolder(dirPath);
			}
		}


		/// <summary>
		///     Find all strage files which seem to appear in the wrong folder,
		///     eg: the files 20150518_111135.jpg should not appeare in the folder 2015-05-17
		/// </summary>
		[TestMethod]
		public void PrintWrongFilesTest()
		{
			string root = @"D:\me Nghia di phap\anh\";
			string[] dirEntries = Directory.GetDirectories(root);
			foreach (string dirName in dirEntries)
			{
				if (dirName.StartsWith(root + "2015"))
				{
					string fileNameMustContains = dirName.Substring(root.Length, 10).Replace("-", "");
					//Console.WriteLine(fileNameMustContains);
					checkFilesIn(dirName, fileNameMustContains);
				}
			}
			Console.WriteLine("Finish");
		}

		private static void checkFilesIn(string dir, string fileNameMustContains)
		{
			foreach (string fileName in Directory.GetFiles(dir))
			{
				if (!fileName.Contains(fileNameMustContains))
				{
					Console.WriteLine(fileName);
				}
			}

			foreach (string subDir in Directory.GetDirectories(dir))
			{
				checkFilesIn(subDir, fileNameMustContains);
			}
		}
	}
}
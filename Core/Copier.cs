using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ProjectsCopier.Core
{
	public class Copier
	{
		private const int DirectoryRenameDelay = 700;
		private const int FilesRenameDelay = 200;
		private const int ReplaceTextDelay = 300;

		public static void CopyFilesRecursively(string sourcePath, string targetPath)
		{
			//Создаем все вложенные директории
			foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
			{
				Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
			}

			//Копируем все файлы
			foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
			{
				File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
			}
		}

		public static string GetSolutionName(string sourcePath)
		{
			var solutionName = Capitalizer.ToUpper(Directory.GetFiles(sourcePath, "*.sln").FirstOrDefault());
			return solutionName;
		}

		public static void DirectoryRenameByTemplate(string shortSourceDirectoryName, string shortTargetDirectoryName, string startTargetFullDirectory)
		{
			//Переименовываем все вложенные директории, удовлетворящие шаблону shortSourceDirectoryName

			var upperShortSourceDirectoryName = Capitalizer.ToUpper(shortSourceDirectoryName);
			foreach (var currentFullDirPath in Directory.GetDirectories(startTargetFullDirectory, "*", SearchOption.AllDirectories))
			{
				var shortCurrentFolderName = new DirectoryInfo(currentFullDirPath).Name.ToLower();
				if (shortCurrentFolderName.ToLower().Contains(upperShortSourceDirectoryName.ToLower()) && currentFullDirPath != startTargetFullDirectory)
				{
					Thread.Sleep(DirectoryRenameDelay);
					var penultFolder = currentFullDirPath[..currentFullDirPath.LastIndexOf('\\')];
					var newFullDirName = $"{penultFolder}\\{Capitalizer.ToUpper(shortCurrentFolderName.Replace(shortSourceDirectoryName.ToLower(), shortTargetDirectoryName))}";
					Directory.Move(currentFullDirPath, newFullDirName);
				}
			}
		}

		public static void FilesRenameByTemplate(string shortSourceFileName, string shortTargetFileName, string startTargetFullDirectory)
		{

			//Переименовываем все файлы, в корневой директории
			var filesRoot = Directory.GetFiles(startTargetFullDirectory);
			var upperShortSourceFileName = Capitalizer.ToUpper(shortSourceFileName);
			foreach (var file in filesRoot)
			{
				var pathToFile = file[..file.LastIndexOf('\\')];
				var fileShortName = Capitalizer.ToUpper(file[file.LastIndexOf('\\')..]);
				if (fileShortName.ToLower().Contains(upperShortSourceFileName.ToLower()))
				{
					var newFileName = $"{pathToFile}{fileShortName.ToLower().Replace(shortSourceFileName.ToLower(), Capitalizer.ToUpper(shortTargetFileName))}";
					File.Move(file, newFileName);
				}
			}

			//Переименовываем все вложенные файлы, удовлетворящие шаблону shortSourceFileName
			foreach (var currentFullDirPath in Directory.GetDirectories(startTargetFullDirectory, "*", SearchOption.AllDirectories))
			{
				var files = Directory.GetFiles(currentFullDirPath);
				foreach (var file in files)
				{
					var pathToFile = file[..file.LastIndexOf('\\')];
					var fileShortName = file[file.LastIndexOf('\\')..].ToLower();
					if (fileShortName.ToLower().Contains(shortSourceFileName.ToLower()))
					{
						Thread.Sleep(FilesRenameDelay);
						var newFileName = $"{pathToFile}{fileShortName.ToLower().Replace(shortSourceFileName.ToLower(), shortTargetFileName)}";
						File.Move(file, newFileName);
					}
				}
			}
		}

		public static void ReplaceTextInFiles(string shortSourceTextName, string shortTargetTextName,
			string startTargetFullDirectory)
		{
			try
			{
				var upperShortTargetTextName = Capitalizer.ToUpper(shortTargetTextName);
				//Переименовываем тексты внутри файлов в корневой директории, удовлетворящие шаблону shortSourceTextName
				var filesRoot = Directory.GetFiles(startTargetFullDirectory);
				var lowerShortSourceTextName = shortSourceTextName.ToLower();
				foreach (var file in filesRoot)
				{
					string text = File.ReadAllText(file);
					string lowerText = File.ReadAllText(file).ToLower();

					if (lowerText.Contains(lowerShortSourceTextName))
					{
						Thread.Sleep(ReplaceTextDelay);
						var regex = new Regex(lowerShortSourceTextName, RegexOptions.IgnoreCase);
						var newText = regex.Replace(text, upperShortTargetTextName);
						File.WriteAllText(file, newText);
					}

				}


				//Переименовываем тексты внутри файлов, удовлетворящие шаблону shortSourceTextName
				var directories = Directory.GetDirectories(startTargetFullDirectory, "*", SearchOption.AllDirectories).OrderByDescending(n => n);
				foreach (var currentFullDirPath in directories)
				{
					var files = Directory.GetFiles(currentFullDirPath);
					try
					{
						foreach (var file in files)
						{
							string text = File.ReadAllText(file);
							string lowerText = File.ReadAllText(file).ToLower();
							if (lowerText.Contains(lowerShortSourceTextName))
							{
								Thread.Sleep(ReplaceTextDelay);
								var regex = new Regex(lowerShortSourceTextName, RegexOptions.IgnoreCase);
								var newText = regex.Replace(text, upperShortTargetTextName);
								try
								{
									File.WriteAllText(file, newText);
								}
								catch (Exception e)
								{
									Console.WriteLine(e);
								}
							}
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

	}
}


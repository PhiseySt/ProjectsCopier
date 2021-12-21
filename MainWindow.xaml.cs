using ProjectsCopier.Components.MessageBoxControl;
using ProjectsCopier.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;


namespace ProjectsCopier
{

	public partial class MainWindow
	{
		private readonly Validator _validator;
		private BackgroundWorker _bw;

		/// <summary>
		/// Папка откуда все будем копировать. Полный путь.
		/// </summary>
		public string FolderSource { get; set; }

		/// <summary>
		/// Папка куда все будем копировать. Полный путь.
		/// </summary>
		public string FolderTarget { get; set; }

		/// <summary>
		/// Короткое имя шаблона для копирования. Откуда
		/// </summary>
		public string SourceShort { get; set; }

		/// <summary>
		/// Короткое имя шаблона для копирования. Куда
		/// </summary>
		public string TargetShort { get; set; }

		/// <summary>
		/// Имя проекта который копируем. Полный путь.
		/// </summary>
		public string SolutionSourceName { get; set; }

		/// <summary>
		/// Имя проекта который создаем. Полный путь.
		/// </summary>
		public string SolutionTargetName { get; set; }



		public MainWindow()
		{
			InitializeComponent();

			_validator = new Validator(this);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void InitializeBackgroundWorker()
		{
			_bw = new BackgroundWorker();
			_bw.DoWork += Bw_DoWork;
			_bw.ProgressChanged += Bw_ProgressChanged;
			_bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
			_bw.WorkerReportsProgress = true;
		}


		private void Bw_DoWork(object sender, DoWorkEventArgs e)
		{
			Workflow();
		}

		private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Bar.Value = e.ProgressPercentage;
		}

		private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			const string messageSuccessfull = "Копирование проекта успешно завершено.";
			MessageBoxWindow.Show(messageSuccessfull);
		}

		private void Workflow()
		{
			Copier.CopyFilesRecursively(FolderSource, FolderTarget);
			_bw.ReportProgress(25);
			Copier.DirectoryRenameByTemplate(SourceShort, TargetShort, FolderTarget);
			_bw.ReportProgress(50);
			Copier.FilesRenameByTemplate(SourceShort, TargetShort, FolderTarget);
			_bw.ReportProgress(75);
			Copier.ReplaceTextInFiles(SourceShort, TargetShort, FolderTarget);
			_bw.ReportProgress(100);
		}

		private void ButtonDirectorySource_Click(object sender, RoutedEventArgs e)
		{
			{
				using var dialog = new FolderBrowserDialog
				{
					Description = "Выберите папку",
					UseDescriptionForTitle = true,
					SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
								   + Path.DirectorySeparatorChar,
					ShowNewFolderButton = true
				};

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					var folder = Capitalizer.ToUpper(dialog.SelectedPath);
					FolderSource = folder;
					TextBoxDirectorySource.Text = folder;
					SourceShort = Capitalizer.ToUpper(new DirectoryInfo(folder).Name);
					SolutionSourceName = Capitalizer.ToUpper(Copier.GetSolutionName(folder));
					TextBoxSolutionSourceName.Text = SolutionSourceName;
				}
			}
		}

		private void ButtonDirectoryTarget_Click(object sender, RoutedEventArgs e)
		{
			using var dialog = new FolderBrowserDialog
			{
				Description = "Выберите папку",
				UseDescriptionForTitle = true,
				SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
							   + Path.DirectorySeparatorChar,
				ShowNewFolderButton = true
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var folder = Capitalizer.ToUpper(dialog.SelectedPath);
				FolderTarget = folder;
				TextBoxDirectoryTarget.Text = folder;
				TargetShort = Capitalizer.ToUpper(new DirectoryInfo(folder).Name);
				SolutionTargetName = Capitalizer.ToUpper($"{folder}\\{TargetShort}.sln");
				TextBoxSolutionTargetName.Text = SolutionTargetName;
			}
		}

		private void ButtonStart_Click(object sender, RoutedEventArgs e)
		{
			if (_validator.IsValid(out var message))
			{
				InitializeBackgroundWorker();
				_bw.RunWorkerAsync();
			}
			else
			{
				MessageBoxWindow.Show(message);
			}
		}

	}
}

namespace ProjectsCopier.Core
{
	public class Validator
	{
		private readonly MainWindow _mainWindow;

		public Validator(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
		}

		public bool IsValid(out string message)
		{
			message = default;
			if (string.IsNullOrEmpty(_mainWindow.FolderSource))
			{
				message = "Не заполнена папка откуда копировать проект";
				return false;
			}

			if (string.IsNullOrEmpty(_mainWindow.FolderTarget))
			{
				message = "Не заполнена папка куда копировать проект";
				return false;
			}

			if (string.IsNullOrEmpty(_mainWindow.FolderTarget))
			{
				message = "Не заполнена папка куда копировать проект";
				return false;
			}

			if (string.IsNullOrEmpty(_mainWindow.SolutionTargetName))
			{
				message = "Не заполнено название нового Solution";
				return false;
			}

			return true;
		}
	}
}
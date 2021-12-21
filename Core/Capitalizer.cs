using System.Text;

namespace ProjectsCopier.Core
{
	public static class Capitalizer
	{
		public static string ToUpper(string caption)
		{
			// Устанавливаем первую заглавную букву и все после точек в заглавные
			var lengthCaption = caption.Length;
			var sb = new StringBuilder();
			var isPrevDot = true;
			if (lengthCaption > 0)
			{
				for (var i = 0; i < lengthCaption ; i++)
				{
					sb.Append(isPrevDot ? caption[i].ToString().ToUpper() : caption[i].ToString());
					isPrevDot = caption[i].ToString() == ".";
				}
			}

			var result = sb.ToString().Replace(".Sln", ".sln");
			return result;
		}
	}
}

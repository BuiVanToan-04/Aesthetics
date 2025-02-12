using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BE_102024.DataAces.NetCore.CheckConditions
{
	public class Validation
	{
		public static bool CheckString(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			return true;
		}

		public static bool CheckNumber(string input)
		{
			return !string.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, @"^\d{10,11}$");
		}

		public static bool CheckXSSInput(string input)
		{
			try
			{
				var listdangerousString = new List<string>() { "<applet", "<body", "<embed", "<frame",
					"<script", "<frameset", "<html", "<iframe", "<img", "<style", "<layer", "<link", "<ilayer",
					"<meta", "<object", "<h", "<input", "<a", "&lt", "&gt" };
				if (string.IsNullOrEmpty(input))
				{
					return true;
				}
				if (string.IsNullOrWhiteSpace(input))
				{
					return true;
				}
				foreach (var dangerous in listdangerousString)
				{
					if (input.Trim().ToLower().IndexOf(dangerous) >= 0)
					{
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				return false;
			}
		}
	}
}

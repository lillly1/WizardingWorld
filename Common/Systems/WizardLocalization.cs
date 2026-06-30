using System;
using Terraria.Localization;

namespace WizardingWorld.Common.Systems
{
	internal static class WizardLocalization
	{
		public static string Text(string key, string fallback, params object[] args)
		{
			string value = args.Length == 0
				? Language.GetTextValue(key)
				: Language.GetTextValue(key, args);

			if (value != key)
				return value;

			if (args.Length == 0)
				return fallback;

			try
			{
				return string.Format(fallback, args);
			}
			catch (FormatException)
			{
				return fallback;
			}
		}
	}
}

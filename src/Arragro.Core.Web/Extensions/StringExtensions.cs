using System;

namespace Arragro.Core.Web.Extensions
{
    public static class StringExtensions
	{
		public static string FromBase64String(this string value, bool throwException = true)
		{
			try
			{
				byte[] decodedBytes = Convert.FromBase64String(value);
				string decoded = System.Text.Encoding.UTF8.GetString(decodedBytes);

				return decoded;
			}
			catch (Exception ex)
			{
				if (throwException)
					throw new Exception(ex.Message, ex);
				else
					return value;
			}
		}

		public static string ToBase64String(this string value)
		{
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
			string encoded = Convert.ToBase64String(bytes);

			return encoded;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace MMCV_Common.Helper
{
	public static class CsvHelper
	{
		public static DataTable GetDataFromCsv(string fullName, int startRow, int endRow = 2147483647)
		{
			DataTable tb = new DataTable();
			DataTable tb2;
			try
			{
				using (StreamReader streamReader = new StreamReader(fullName, Encoding.Default))
				{
					int num = 0;
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						num++;
						if (num >= startRow)
						{
							if (num > endRow)
							{
								break;
							}
							if (text.Trim().Length != 0)
							{
								List<string> list = CsvHelper.ParseArray(text);
								if (tb.Columns.Count == 0)
								{
									list.ForEach(delegate (string s)
									{
										if (!tb.Columns.Contains(s))
										{
											tb.Columns.Add(new DataColumn(s, typeof(string)));
										}
										else
										{
											for (int j = 1; j < 2147483647; j++)
											{
												if (!tb.Columns.Contains(s + "_" + j))
												{
													tb.Columns.Add(new DataColumn(s + "_" + j, typeof(string)));
													break;
												}
											}
										}
									});
								}
								else
								{
									string[] array = list.ToArray();
									if (array.Length > tb.Columns.Count)
									{
										string[] array2 = new string[tb.Columns.Count];
										for (int i = 0; i < tb.Columns.Count; i++)
										{
											array2[i] = array[i];
										}
										tb.Rows.Add(array2);
									}
									else
									{
										tb.Rows.Add(list.ToArray());
									}
								}
							}
						}
					}
				}
				tb2 = tb;
			}
			catch (Exception ex)
			{
				throw new Exception(fullName + "Err" + ex.Message);
			}
			return tb2;
		}

		public static DataTable GetDataFromCsv(string fullName, int startRow, out string text)
		{
			try
			{
				using (StreamReader streamReader = new StreamReader(fullName, Encoding.Default))
				{
					text = streamReader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(fullName + "Err:" + ex.Message);
			}
			return CsvHelper.GetDataFromCsv(fullName, startRow, int.MaxValue);
		}

		public static string GetText(string fullName)
		{
			string result;
			try
			{
				using (StreamReader streamReader = new StreamReader(fullName, Encoding.Default))
				{
					result = streamReader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(fullName + "Err:" + ex.Message);
			}
			return result;
		}

		private static List<string> ParseArray(string strLine)
		{
			string[] array = strLine.Split(new char[]
			{
				','
			});
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].StartsWith("\""))
				{
					if (array[i].EndWithOddQuote())
					{
						list.Add(array[i].TrimQuote());
					}
					else
					{
						stringBuilder.Append(array[i] + ",");
						for (i++; i < array.Length; i++)
						{
							stringBuilder.Append(array[i] + ",");
							if (array[i].EndWithOddQuote())
							{
								list.Add(stringBuilder.ToString().Trim(new char[]
								{
									','
								}).TrimQuote());
								stringBuilder.Clear();
								break;
							}
						}
					}
				}
				else
				{
					list.Add(array[i].TrimQuote());
				}
			}
			return list;
		}

		private static bool EndWithOddQuote(this string s)
		{
			bool result;
			if (string.IsNullOrWhiteSpace(s))
			{
				result = false;
			}
			else
			{
				int num = 0;
				for (int i = s.Length - 1; i >= 0; i--)
				{
					if (s[i] != '"')
					{
						break;
					}
					num++;
				}
				result = (num % 2 == 1);
			}
			return result;
		}

		private static string TrimQuote(this string s)
		{
			string result;
			if (string.IsNullOrWhiteSpace(s))
			{
				result = s;
			}
			else
			{
				if (s.StartsWith("\""))
				{
					if (s.Length >= 2)
					{
						s = s.Substring(1);
					}
					else
					{
						s = string.Empty;
					}
				}
				if (s.EndsWith("\""))
				{
					if (s.Length >= 2)
					{
						s = s.Substring(0, s.Length - 1);
					}
					else
					{
						s = string.Empty;
					}
				}
				result = s.Replace("\"\"", "\"");
			}
			return result;
		}
	}
}

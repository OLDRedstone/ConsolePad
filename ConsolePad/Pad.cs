using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using m = System.Math;
namespace ConsolePad
{
	internal class Pad
	{
		internal enum TextAlign
		{
			Left, Middle, Right,
		}
		private readonly char lh = '─';
		private readonly char lv = '│';
		private readonly char lu = '┬';
		private readonly char lb_ = '┴';
		private readonly char lc = '┼';
		private readonly char bll = '┝';
		private readonly char blh = '━';
		private readonly char blv = '┃';
		private readonly char blc = '┿';
		private readonly char blb = '┸';
		private readonly char blr = '┥';
		private readonly char blct = '╈';
		private char mlh = '═';
		private char mll = '╞';
		private char mlu = '╤';
		private char mlr = '╡';
		private char lt = '╭';
		private char rt = '╮';
		private char lb = '╰';
		private char rb = '╯';
		public int Padding { get; set; } = 1;
		private int PadWidth(int width) => width + Padding * 2;
		private static string Line(char c, int count)
		{
			string s = "";
			for (int i = 0; i < count; i++)
				s += c;
			return s;
		}
		private string PadLine(char c, int count) => Line(c, count + (Padding * 2));
		private string TopTableLine(int[] widths)
		{
			string s = "" + lt;
			for (int i = 0; i < widths.Length; i++)
			{
				s += PadLine(lh, widths[i]);
				if (i < widths.Length - 1)
					s += lu;
			}
			return s + rt;
		}
		private string MidTableLine(int[] widths)
		{
			string s = "" + mll;
			for (int i = 0; i < widths.Length; i++)
			{
				s += PadLine(mlh, widths[i]);
				if (i < widths.Length - 1)
					s += mlu;
			}
			return s + mlr;
		}
		private string MidSubTitleTableLine(int[] widths, bool sideLine)
		{
			string s = "" + bll;
			for (int i = 0; i < widths.Length; i++)
			{
				s += PadLine(blh, widths[i]);
				if (i < widths.Length - 1)
					s += sideLine && i == 0 ? blct : blc;
			}
			return s + blr;
		}
		private string BottomTableLine(int[] widths, bool sideLine)
		{
			string s = "" + lb;
			for (int i = 0; i < widths.Length; i++)
			{
				s += PadLine(lh, widths[i]);
				if (i < widths.Length - 1)
					s += sideLine && i == 0 ? blb : lb_;
			}
			return s + rb;
		}
		private string Align(string text, int width, TextAlign align)
		{
			text ??= "";
			return align switch
			{
				TextAlign.Left => text.PadRight(width),
				TextAlign.Middle => text.PadLeft((width - text.Length) / 2 + text.Length).PadRight(width),
				TextAlign.Right => text.PadLeft(width),
				_ => throw new NotImplementedException(),
			};
		}
		private string Title(int[] widths, string? title = null)
		{
			string s = "";
			if (title?.Length > 0)
			{
				int maxl = m.Max(widths.Sum() + (widths.Length - 1) * PadWidth(1), title.Length);
				widths[0] = maxl - widths.Skip(1).Sum(i => i + PadWidth(1));
				s += lt + Line(lh, PadWidth(maxl)) + rt + '\n' + lv + Align(title, PadWidth(maxl), TextAlign.Middle) + lv + '\n' + MidTableLine(widths);
			}
			else
				s += TopTableLine(widths);
			return s;
		}
		private string SubTitle(int[] widths, string[] heads, bool sideLine)
		{
			if (widths.Length != heads.Length) throw new NotImplementedException();
			string s = "" + lv;
			for (int i = 0; i < widths.Length; i++)
			{
				s += Align(heads[i], widths[i], TextAlign.Left).PadLeft(widths[i] + Padding).PadRight(widths[i] + Padding * 2) + lv;
			}
			s += '\n' + MidSubTitleTableLine(widths, sideLine);
			return s;
		}
		private static string[,] Expand(string[] strings)
		{
			string[,] result = new string[strings.Length, 1];
			for (int i = 0; i < strings.Length; i++)
				result[i, 0] = strings[i];
			return result;
		}
		private static int[] Widths(IEnumerable<IEnumerable<string>> strings, out string[,] values)
		{
			int[] widths = new int[strings.Max(i => i.Count())];
			values = new string[strings.Count(), widths.Length];
			int y = 0;
			foreach (IEnumerable<string> s2 in strings)
			{
				string[] s2a = s2.ToArray();
				for (int i = 0; i < s2a.Length; i++)
				{
					widths[i] = m.Max(widths[i], s2a[i].Length);
					values[y, i] = s2a[i];
				}
				y++;
			}
			return widths;
		}
		private static int[] Widths(string[,] strings)
		{
			int[] widths = new int[strings.GetLength(1)];
			for (int x = 0; x < strings.GetLength(1); x++)
				for (int y = 0; y < strings.GetLength(0); y++)
					widths[x] = m.Max(widths[x], strings[y, x].Length);
			return widths;
		}
		private static IEnumerable<IEnumerable<string>> ToIEnumerable(string[,] strings)
		{
			return Enumerable.Range(0, strings.GetLength(0)).Select(y =>
					Enumerable.Range(0, strings.GetLength(1)).Select(x =>
					strings[y, x]
					));
		}
		private static int[] Max(int[] ints1, int[] ints2)
		{
			if (ints1.Length != ints2.Length) throw new NotImplementedException();
			return Enumerable.Range(0, ints1.Length).Select(i => m.Max(ints1[i], ints2[i])).ToArray();
		}
		private static int[] Max(int[] ints, string[] strs)
		{
			if (ints.Length != strs.Length) throw new NotImplementedException();
			return Enumerable.Range(0, ints.Length).Select(i => m.Max(ints[i], strs[i].Length)).ToArray();
		}
		private string Content(int[] widths, string[,] content, bool sideLine)
		{
			if (widths.Length != content.GetLength(1)) throw new NotImplementedException();
			string s = "";
			for (int y = 0; y < content.GetLength(0); y++)
			{
				s += lv;
				for (int x = 0; x < content.GetLength(1); x++)
				{
					s += Align(content[y, x], widths[x], TextAlign.Left).PadLeft(widths[x] + Padding).PadRight(widths[x] + Padding * 2)
						+ (sideLine && x < content.GetLength(1) - 1 && x == 0 ? blv : lv);
				}
				if (y < content.GetLength(0) - 1) s += "\n";
			}
			return s;
		}
		private string All(int[] widths, string[,] content, string? title = null)
		{
			return Title(widths, title) + '\n'
				+ Content(widths, content, false) + '\n'
				+ BottomTableLine(widths, false);
		}
		private string All(int[] widths, string[] subTitle, string[,] content, bool sideLine, string? title = null)
		{
			widths = Max(widths, subTitle);
			return Title(widths, title) + '\n'
				+ SubTitle(widths, subTitle, sideLine) + '\n'
				+ Content(widths, content, sideLine) + '\n'
				+ BottomTableLine(widths, sideLine);
		}
		private string All(int width, string[] content, string? title = null)
		{
			return Title([width], title) + "\n"
				+ Content([width], Expand(content), false) + "\n"
				+ BottomTableLine([width], false);
		}
		private string All(int width, string subTitle, string[] content, bool sideLine, string? title = null)
		{
			width = m.Max(width, subTitle.Length);
			return Title([width], title) + "\n"
				+ SubTitle([width], [subTitle], sideLine) + '\n'
				+ Content([width], Expand(content), sideLine) + "\n"
				+ BottomTableLine([width], false);
		}
		private string Dump(IEnumerable<string> values, bool enableIndex, bool enableSubTitle, string? title = null)
		{
			return enableSubTitle
				? enableIndex
					? Dump(values.Select((i, index) => new string[] { index.ToString(), i }), ["Index", "Value"], false, title)
					: All(values.Max(i => i.Length), "Value", values.ToArray(), false, title)
				: enableIndex
					? Dump(values.Select((i, index) => new string[] { index.ToString(), i }), false, title)
					: All(values.Max(i => i.Length), values.ToArray(), title);
		}
		private string Dump(IEnumerable<(string key, string value)> values, bool enableIndex, bool enableSubTitle, string? title = null)
		{
			return enableSubTitle
				? enableIndex
					? Dump(values.Select((i, index) => new string[] { index.ToString(), i.key, i.value }), ["", "Key", "Value"], false, title)
					: Dump(values.Select(i => new string[] { i.key, i.value }), ["", "Key", "Value"], false, title)
				: enableIndex
					? Dump(values.Select((i, index) => new string[] { index.ToString(), i.key, i.value }), false, title)
					: Dump(values.Select(i => new string[] { i.key, i.value }), false, title);
		}
		private string Dump(IEnumerable<IEnumerable<string>> values, bool enableIndex, string? title = null)
		{
			return enableIndex
				? Dump(values.Select((i, index) => new List<string>() { index.ToString() }.Concat(i)), false, title)
				: All(Widths(values, out string[,] strs), strs, title);
		}
		private string Dump(IEnumerable<IEnumerable<string>> values, IEnumerable<string> subTitle, bool enableIndex, string? title = null)
		{
			return enableIndex
				? Dump(values.Select((i, index) => new List<string>() { index.ToString() }.Concat(i)), subTitle, false, title)
				: All(Widths(values, out string[,] strs), subTitle.ToArray(), strs, true, title);
		}
		private string Dump(string[,] values, bool enableIndex, string? title = null)
		{
			return enableIndex
				? Dump(ToIEnumerable(values).Select((i, index) => new string[] { index.ToString() }.Concat(i)), false, title)
				: All(Widths(values), values, title);
		}
		internal string Dump<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> value, Stack<object?> stack)
		{
			List<IEnumerable<string>> result = [];
			foreach (var item in value.Take(5))
			{
				if (stack.Contains(item.Key) || stack.Contains(item.Value))
				{
					result.Add(["[Recursive object]", "[Recursive object]"]);
					continue;
				}
				stack.Push(item);
				List<string> inforeskey = [.. Dump(item.Key, stack).Split('\n')];
				List<string> inforesvalue = [.. Dump(item.Value, stack).Split('\n')];
				while (inforeskey.Count < inforesvalue.Count)
					inforeskey.Add("");
				while (inforeskey.Count > inforesvalue.Count)
					inforesvalue.Add("");
				stack.Pop();
				result.AddRange(Enumerable.Range(0, inforeskey.Count).Select(i => new string[] { inforeskey[i], inforesvalue[i] }));
			}
			if (value.Count() > 5)
				result.Add([$"And {value.Count() - 5} item{(value.Count() == 6 ? "" : "s")} more..."]);
			return Dump(result,["Key","Value"], false);
		}
		internal string Dump<T>(IEnumerable<T> value, Stack<object?> stack)
		{
			List<IEnumerable<string>> result = [];
			foreach (var item in value.Take(5))
			{
				if (item is not null && stack.Contains(item))
				{
					result.Add(["[Recursive object]"]);
					continue;
				}
				stack.Push(item);
				var infores = Dump(item, stack).Split('\n');
				stack.Pop();
				result.AddRange(infores.Select(i => new string[] { i }));
			}
			if (value.Count() > 5)
				result.Add([$"And {value.Count() - 5} item{(value.Count() == 6 ? "" : "s")} more..."]);
			return Dump(result, false);
		}
		internal string Dump(object? value, Stack<object?> stack)
		{
			if (value == null)
				return All(4, ["[null]"]);
			Type type = value.GetType();
			if (type.IsPrimitive
			|| type.IsEnum
			|| type.Equals(typeof(string))
			|| type.Equals(typeof(DateTime)))
			{
				return value.ToString();
			}
			PropertyInfo[] infos = type.GetProperties();
			List<IEnumerable<string>> result = [];
			foreach (PropertyInfo info in infos)
			{
				if (info.PropertyType.IsPrimitive
				|| info.PropertyType.IsEnum
				|| info.PropertyType.Equals(typeof(string))
				|| info.PropertyType.Equals(typeof(DateTime)))
				{
					result.Add([
						info.Name +( info.MemberType == MemberTypes.Method ?"()":""),
						info.PropertyType.Name,
						info.GetValue(value).ToString(),
					]);
				}
				else
				{
					string[] infores;
					if (value is not null && stack.Contains(value))
					{
						result.Add([
							info.Name + (info.MemberType == MemberTypes.Method ?"()":""),
							info.PropertyType.Name,
						"[Recursive object]"]);
						continue;
					}
					stack.Push(value);
					try
					{
						if (typeof(IEnumerable<object>).IsAssignableFrom(info.PropertyType))
							infores = Dump((IEnumerable<object>)info.GetValue(value), stack).Split('\n');
						else
							infores = Dump(info.GetValue(value), stack).Split('\n');
					}
					catch (Exception ex)
					{
						infores = ["[Cannot read this property]", ex.Message];
					}
					stack.Pop();
					bool first = true;
					foreach (var line in infores)
					{
						if (first)
							result.Add([
								info.Name + (info.MemberType == MemberTypes.Method ?"()":""),
								info.PropertyType.Name,
								line,
							]);
						else
							result.Add([
								"",
								"",
								line,
							]);
						first = false;
					}

				}
			}
			return Dump(result, ["Name", "Type", "Value"], false, value?.GetType().Name ?? "?");
		}
	}
}
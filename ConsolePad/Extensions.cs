using System;
using System.Collections.Generic;
using System.Text;

namespace ConsolePad
{
	public static class Extensions
	{
		public static void Dump<TKey,TValue>(this IEnumerable<KeyValuePair<TKey,TValue>> obj) => Console.WriteLine(new Pad().Dump<TKey,TValue>((IEnumerable<KeyValuePair< TKey,TValue>>)obj, []));
		public static void Dump<T>(this IEnumerable<T> obj) => Console.WriteLine(new Pad().Dump(obj, []));
		public static void Dump(this object? obj) => Console.WriteLine(new Pad().Dump(obj, []));
	}
}

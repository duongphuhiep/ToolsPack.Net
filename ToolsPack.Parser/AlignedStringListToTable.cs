using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolsPack.Displayer;
using ToolsPack.Log4net;

namespace ToolsPack.Parser
{
	/// <summary>
	/// Input is a collection of string, aligned each other with spaces. Example:
	/// {
	///   "aa aa     15     aaaaaa    aa        17"
	///   "b bbbb    165    bb        bbb 26"      14
	///   "cccc cc   18     ccc c     cc 29"
	/// }
	/// 
	/// The class will try to convert them to a table string[][] 
	/// {
	///   {"aa aa", "15", "aa        17"},
	///   {"b bbbb", "165", "bb", "bbb 26"},
	///   {"cccc cc", "18", "ccc c", "cc 29"}
	/// }
	/// 
	/// Algorithm: 
	/// 
	/// Objectif is to find the best position called column-break on each row to split it into cells.
	/// There are 3 level of column-break: weak, normal, hard
	/// 
	/// 1) Find weak column-breaks: for each row find the position where a big spaces is folowing by other character. Example
	/// row 1:  22, 31, 42, 108
	/// row 2:  22, 31, 42, 108
	/// row 3:  20, 31, 35  44,  108
	/// 
	/// 2) Find normal column-breaks: are weak column-breaks which appeared more than 60% across all rows. Example
	/// normal column-break = 22, 31, 42, 108
	/// 
	/// 3) Find hard column-breaks on each row:
	/// For each row, we will split the content base on the normal column-breaks and also weak column-breaks
	/// - in general, the hard column-breaks of the row will be normal column-breaks.
	/// - except if there is a weak-column breaks which is not far-away from the normal column-breaks 
	/// then the weak-column breaks will be promoted to hard column-break:
	/// 
	/// On the row 3 of the below example: The normal column break is 22, normally we will split the row at the position 22, 
	/// however there is a weak-column break at 20 which is not far from 22, so 20 will be the real column break
	/// </summary>
	public class AlignedStringListToTable
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(AlignedStringListToTable));

		/// <summary>
		/// define what is a "big spaces", define the minimum length of continous white-spaces which we can considered as a "big spaces"
		/// </summary>
		private readonly int BigSpaceMinLength;

		/// <summary>
		/// WeakToNormalThreshold = 0.6 means that if a weak column-break appeared more than 60%, then it will be promoted to normal column-break
		/// </summary>
		private readonly double WeakToNormalThreshold;

		/// <summary>
		/// normal column-breaks are usually promoted to hard column-breaks, except there is a Weak columns break near him,
		/// the weakDistance is the threshold that define what is "near" and what is "far".
		/// </summary>
		private readonly int WeakDistance;

		/// <summary>
		/// trim the spaces in each cells before return result
		/// </summary>
		private readonly bool TrimCells;


		/// <summary>
		/// list of rows
		/// </summary>
		private readonly string[] src;

		/// <summary>
		/// weak columns breaks of each row
		/// </summary>
		private List<int>[] weaks;

		/// <summary>
		/// normal columns breaks
		/// </summary>
		private List<int> normal;

		string[][] result;

		public AlignedStringListToTable(string[] src, bool trimCells=true, int bigSpaceMinLength=4, double weakToNormalThreshold=0.6, int weakDistance=4)
		{
			this.src = src;
			this.BigSpaceMinLength = bigSpaceMinLength;
			this.WeakToNormalThreshold = weakToNormalThreshold;
			this.WeakDistance = weakDistance;
			this.TrimCells = trimCells;
        }

		public string[][] Porcess()
		{
			var context = string.Format("Convert {0}", this.src.Display().MaxItemLength(10).MaxItems(3));
			using (var etw = ElapsedTimeWatcher.Create(Log, "process", context))
			{
				if (this.result != null) { return this.result; }

				this.weaks = new List<int>[src.Length];
				for (var i = 0; i < src.Length; i++)
				{
					this.weaks[i] = FindWeakColumnBreaks(src[i], this.BigSpaceMinLength);
				}
				etw.Debug("Find weaks column-breaks done");

				this.normal = FindNormalColumnBreaks(this.weaks, this.WeakToNormalThreshold);
				etw.InfoFormat("Normal column-breaks {0}", this.normal.Display());

				this.result = new string[src.Length][];
				for (var i = 0; i < src.Length; i++)
				{
					var hard = FindHardColumnBreaks(this.normal, this.weaks[i], this.WeakDistance);
					this.result[i] = StringTools.Split(src[i], hard, this.TrimCells);
				}

				return this.result;
			}
		}

		
		/// <summary>
		/// Return the weak column break of the string row
		/// </summary>
		public static List<int> FindWeakColumnBreaks(string row, int bigSpaceMinLength=4)
		{
			var resu = new List<int>();
			var found = Regex.Matches(row, " {" + bigSpaceMinLength + "}[^ ]", RegexOptions.IgnoreCase);

			var n = found.Count;
			for (int i = 0; i< n; i++) 
			{
				var m = found[i];
				resu.Add(m.Index + bigSpaceMinLength);
			}
            return resu;
		}

		/// <summary>
		/// Find all the weaks column break which occurs more than 60% (occurenceThreshold)
		/// </summary>
		public static List<int> FindNormalColumnBreaks(List<int>[] weaks, double occurenceThreshold = 0.6)
		{
			var n = weaks.Length;

			//count number occurences of the same column break across every lines
			var stats = new Dictionary<int, int>();
			for (var i = 0; i < n; i++)
			{
				foreach (int pos in weaks[i])
				{
					if (stats.ContainsKey(pos))
					{
						stats[pos]++;
					}
					else
					{
						stats[pos] = 1;
					}
				}
			}

			var resu = new List<int>();

			//filter by occurenceThreshold
			foreach (var pos in stats.Keys)
			{
				if ((double)stats[pos] / n > occurenceThreshold)
				{
					resu.Add(pos);
				}
			}

			return resu;
		}

		/// <summary>
		/// For each normal column-break to we will promote it to hard column-break, except if there is a weak column-break near it
		/// (winthin the weakDistance) then weak column-break will be promoted instead. 
		/// </summary>
		public static SortedSet<int> FindHardColumnBreaks(List<int> normal, List<int> weak, int weakDistance = 4)
		{
			var resu = new SortedSet<int>();

			foreach (int pos in normal)
			{
				//find the neareast weak colum break near it
				int nearestPos, minDist;
				GetNeareastPosition(weak, pos, out nearestPos, out minDist);
				resu.Add(minDist <= weakDistance ? nearestPos : pos);
			}

			return resu;
		}

		/// <summary>
		/// Find the nearest value of pos in arr
		/// Example: given arr = {1, 5, 40, 96}
		/// The neareast value for pos=48 is 40 and the distance (minDist) is 8
		/// </summary>
		public static void GetNeareastPosition(List<int> arr, int pos, out int nearestPos, out int minDist)
		{
			nearestPos = arr[0];
			minDist = Math.Abs(nearestPos - pos);

			for (var i = 1; i < arr.Count; i++)
			{
				var wp = arr[i];
				var dist = Math.Abs(wp - pos);
				if (dist < minDist)
				{
					minDist = dist;
					nearestPos = wp;
				}
			}
		}
	}
}

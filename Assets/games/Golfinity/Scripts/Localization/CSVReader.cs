using UnityEngine;
using System.Linq;

namespace Games.Golfinity
{
	public static class CSVReader {
	    // splits a CSV file into a 2D string array
	    public static string[,] SplitCsvGrid(string csvText) {
	        string[] lines = System.Text.RegularExpressions.Regex.Split(csvText, "[\r\n]+");

	        // finds the max width of row
	        int width = 0;
	        for (int i = 0; i < lines.Length; i++) {
	            string[] row = SplitCsvLine(lines[i]);
	            width = Mathf.Max(width, row.Length);
	        }

	        // creates new 2D string grid to output to
	        string[,] outputGrid = new string[width + 2, lines.Length + 1];
	        for (int y = 0; y < lines.Length; y++) {
	            string[] row = SplitCsvLine(lines[y]);
	            for (int x = 0; x < row.Length; x++) {
	                outputGrid[x, y] = row[x];

	                // This line was to replace "" with " in my output. 
	                // Include or edit it as you wish.
	                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
	            }
	        }

	        return outputGrid;
	    }

	    // splits a CSV row 
	    private static string[] SplitCsvLine(string line) {
	        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
	        @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
	        System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
	                select m.Groups[1].Value).ToArray();
	    }
	}
}

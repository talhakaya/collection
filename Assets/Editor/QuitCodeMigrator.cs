using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Collection.EditorTools
{
	/// <summary>
	/// Rewrites a freshly imported game's exit code, which is written on the assumption that
	/// the game is the whole application.
	///
	/// Two different things get done, because they are two different intentions:
	///
	/// An `if (... Escape ...) { Application.Quit(); }` is deleted outright. The collection
	/// already binds its own exit combination, so a bare Escape closing everything is both
	/// redundant and a way to lose the collection by accident.
	///
	/// Any other Application.Quit() - a game-over screen, a quit button - becomes
	/// GlobalInputManager.ReturnToMainMenu(). A game inside the collection is never the thing
	/// that should be closing the application, and finishing one should land you back in the
	/// list rather than on the desktop.
	///
	/// This is a best-effort pass over source text, like InputCodeMigrator: it reports what it
	/// changed so an import can be checked rather than trusted. Anything it cannot match is
	/// left alone and shows up in a later grep for Application.Quit.
	/// </summary>
	public static class QuitCodeMigrator
	{
		private const string ReturnCall = "GlobalInputManager.ReturnToMainMenu();";
		private const string RequiredUsing = "using Collection.Controls;";

		/// <summary>
		/// An if-statement, with no else, whose condition mentions Escape and whose body does
		/// nothing but quit. Deliberately narrow: a block that also does something else is a
		/// decision for a human, and falls through to the replacement pass instead.
		///
		/// The condition is matched greedily to the end of the line rather than to a closing
		/// paren, because the usual spelling is Input.GetKey(KeyCode.Escape) - stopping at the
		/// first ')' finds the inner one, and the pattern then matches nothing at all.
		/// </summary>
		private static readonly Regex EscapeQuitPattern = new Regex(
			@"[ \t]*if[ \t]*\(.*Escape.*\)[ \t]*\r?\n" +                  // if (... Escape ...)
			@"[ \t]*\{[ \t]*\r?\n" +                                      // {
			@"[ \t]*Application\s*\.\s*Quit\s*\(\s*\)\s*;[ \t]*\r?\n" +   //     Application.Quit();
			@"[ \t]*\}[ \t]*\r?\n",                                       // }
			RegexOptions.Compiled);

		private static readonly Regex QuitCallPattern = new Regex(
			@"Application\s*\.\s*Quit\s*\(\s*\)\s*;", RegexOptions.Compiled);

		private static readonly Regex LastUsingPattern = new Regex(
			@"^using[^\r\n]*;[ \t]*$", RegexOptions.Multiline | RegexOptions.Compiled);

		public static int MigrateFolder(string folderPath, List<string> log)
		{
			string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
			if (!Directory.Exists(absoluteFolder))
			{
				log.Add($"Folder '{folderPath}' does not exist.");
				return 0;
			}

			int removed = 0;
			int redirected = 0;
			int filesChanged = 0;

			foreach (string scriptFile in Directory.GetFiles(absoluteFolder, "*.cs", SearchOption.AllDirectories))
			{
				string original = File.ReadAllText(scriptFile);
				if (!QuitCallPattern.IsMatch(original))
				{
					continue;
				}

				string rewritten = EscapeQuitPattern.Replace(original, match =>
				{
					removed++;
					// The indentation of the `if` is reused so the note sits where the code did.
					string indent = Regex.Match(match.Value, @"^[ \t]*").Value;
					string newLine = match.Value.Contains("\r\n") ? "\r\n" : "\n";
					return
						indent + "// Escape used to quit the application here. The collection binds its own exit" + newLine +
						indent + "// combination, so a bare Escape quitting is both redundant and a way to lose" + newLine +
						indent + "// the whole collection by accident." + newLine;
				});

				int before = redirected;
				rewritten = QuitCallPattern.Replace(rewritten, match =>
				{
					redirected++;
					return ReturnCall;
				});

				if (redirected > before && !rewritten.Contains(RequiredUsing))
				{
					rewritten = AddUsing(rewritten);
				}

				if (rewritten != original)
				{
					File.WriteAllText(scriptFile, rewritten);
					filesChanged++;
				}
			}

			if (removed > 0 || redirected > 0)
			{
				log.Add($"Quit handling: removed {removed} Escape-quits, redirected {redirected} " +
					$"Application.Quit() call(s) to the main menu, across {filesChanged} file(s).");
			}

			return removed + redirected;
		}

		/// Slots the using in after the file's existing ones, so the block stays together.
		private static string AddUsing(string source)
		{
			MatchCollection usings = LastUsingPattern.Matches(source);
			if (usings.Count == 0)
			{
				return RequiredUsing + "\n" + source;
			}

			Match last = usings[usings.Count - 1];
			return source.Insert(last.Index + last.Length, "\n" + RequiredUsing);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibPegasus.Parsers.Scp
{
	public static class ScpParser
	{
		private static bool IsFileMapData(String input)
		{
			var regex = new Regex("world[0-9]+-[a-z]+\\.scp", RegexOptions.IgnoreCase);
			return regex.IsMatch(input);
		}
		public static void Parse(Dictionary<string, Dictionary<string, Dictionary<string, string>>> config, string path)
		{
			if (File.Exists(path))
			{
				bool fileMapData = IsFileMapData(path);
				Serilog.Log.Information($"Loading {path}...");
				string[]? section = null;
				Dictionary<string, Dictionary<string, string>>? sectDict = null;
				foreach (var line in File.ReadLines(path))
				{
					if(!String.IsNullOrWhiteSpace(line))
					{
						if (line.StartsWith('['))
						{
							if(sectDict != null && section != null)
							{
								if (config.ContainsKey(section[0]))
									throw new Exception("Already contains key");
								config[section[0]] = sectDict;
							}
								
							section = line.Split(null);
							if(fileMapData)
							{
								section[0] = section[0].Replace("]", "");
								section[0] = section[0] + Regex.Match(path, @"\d+").Value + "]";
							}
							sectDict = new();
						}
						else
						{
							Dictionary<string, string> dict = new();
							string? key = null;
							int i = 0;
							if(section == null)
							{
								throw new NullReferenceException();
							}
							foreach (var token in line.Split(null))
							{
								if (i == 0)
								{
									key = token;
								}
								else
								{
									dict[section[i]] = token;
								}
								i++;
							}
							if (key == null)
							{
								throw new NullReferenceException();
							}
							sectDict[key] = dict;
						}
					}
				}
				if (sectDict != null && section != null)
				{
					if (config.ContainsKey(section[0]))
						throw new Exception("Already contains key");
					config[section[0]] = sectDict;
				}

			}
			else
			{
				throw new FileNotFoundException(path);
			}
		}
	}
}

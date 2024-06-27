#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Architecture;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.InputSystem.LowLevel;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Neofetch")]
	public class Neofetch : ScriptableCommand
	{
		private readonly string asciiArt = @"
                     %                           @                        
                    #                              @                      
                  @@                                @                     
                 @@                                 @@                    
                 @@         @@            @         @@                    
                 @@@     @@                  @,     @@@                   
                 @@   @@@                     %@@   @@@                   
                    *@@@                       .@@@                       
                    @@@/                        @@@@                      
             @@    @@@@@@                      @@@@@     @                
        @@@@@@     @@@@@                       @@@*@     @@@@@@           
     @@@@@@        @ ,  @@@@@@@@        @@@@@@@&   @         @@@@@        
   @@@,                @@@@@@@@@&      @@@@@@@@@@  %             @@@      
  @@                   @@@@@@@@@   #    @@@@@@@@                   @@.    
 @                      @@@@/     @#@      @@@@                      @    
@                    @ (@@@@@   .@@  .@   @@@@@@ @@                   @   
#                      @@@@@@    @    @    @@@@@/                         
                                  @        @                              
                                                                          
                             @@@@/@#@&@ @@@                               
                                                                          
                              @@        @                                 
                            @@@,        @@@/                              
                        @@@&                @@@                           ";


		private static readonly string dataFormat = @"
{system}
{divider}
{information}

{colors}
";

		private readonly string colors = "\x1b[40m   \x1b[41m   \x1b[42m   \x1b[43m   \x1b[44m   \x1b[45m   \x1b[46m   \x1b[47m   \x1b[0m\r\n\x1b[100m   \x1b[101m   \x1b[102m   \x1b[103m   \x1b[104m   \x1b[105m   \x1b[106m   \x1b[107m   \x1b[0m";
		
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			var systemInformation = new Dictionary<string, string>();
			
			systemInformation.Add("Game version", $"{Application.productName} {Application.version}");
			systemInformation.Add("Unity version", Application.unityVersion);
			systemInformation.Add("Host", SystemInfo.operatingSystem);
			systemInformation.Add("Engine up-time", CalculateEngineUptime());
			systemInformation.Add("CPU", SystemInfo.processorType);
			systemInformation.Add("GPU", SystemInfo.graphicsDeviceName);
			systemInformation.Add("Resolution", $"{Screen.width}x{Screen.height}");
			systemInformation.Add("FPS", Math.Floor(1 / Time.deltaTime).ToString());
			systemInformation.Add("Graphics pipeline", SystemInfo.graphicsDeviceType.ToString());

			string text = BuildText(UserName, HostName, systemInformation);
			
			Console.WriteLine(text);
			return Task.CompletedTask;
		}

		private string BuildText(string username, string hostname, Dictionary<string, string> systemInformation)
		{
			// TODO: Color
			var sb = new StringBuilder();

			// Generate the username and hostname text
			sb.Append(username);
			sb.Append("@");
			sb.Append(hostname);

			// Save it.
			var hostLine = sb.ToString();
			
			// Clear it.
			sb.Length = 0;
			
			// Generate a line of hyphens of the same character length
			for (var i = 0; i < hostLine.Length; i++)
				sb.Append("-");

			// Save it.
			var hyphens = sb.ToString();

			// Clear it.
			sb.Length = 0;
			
			// Fill in system info
			foreach (string key in systemInformation.Keys)
			{
				sb.Append(key);
				sb.Append(": ");
				sb.AppendLine(systemInformation[key]);
			}

			// Save it.
			var systemInfoText = sb.ToString();
			
			// You get the idea.
			sb.Length = 0;
			
			// Format the text
			string formattedSysInfo = dataFormat.Replace("{system}", hostLine.Trim())
				.Replace("{divider}", hyphens)
				.Replace("{information}", systemInfoText)
				.Replace("{colors}", colors);

			// Split the formatted information
			string[] dataLines = Console.Normalize(formattedSysInfo).Split(Environment.NewLine);

			// Split the ASCII lines
			string[] asciiLines = Console.Normalize(asciiArt).Split(Environment.NewLine);
			
			// Find the longest line in the ASCII art
			int longestAsciiLine = asciiLines.Select(x => x.Length)
				.OrderByDescending(x => x)
				.First();
			
			// Find the longest array
			int longestArray = Math.Max(asciiLines.Length, dataLines.Length);
			
			// Iterate through both arrays, it's time to build the final text
			for (var i = 0; i < longestArray; i++)
			{
				var asciiLength = 0;

				if (i < asciiLines.Length)
				{
					asciiLength = asciiLines[i].Length;
					sb.Append(asciiLines[i]);
				}

				if (i >= dataLines.Length)
				{
					sb.AppendLine();
					continue;
				}

				int spacesToAdd = longestAsciiLine - asciiLength;

				for (var j = 0; j < spacesToAdd; j++)
					sb.Append(" ");

				sb.AppendLine(dataLines[i]);
			}
			
			
			return sb.ToString();
		}

		public static string CalculateEngineUptime()
		{
			float time = Time.time;
			TimeSpan timespan = TimeSpan.FromSeconds(time);
			var sb = new StringBuilder();

			if (timespan.TotalDays >= 7)
				sb.Append("Over one week");
			else
			{
				if (timespan.TotalDays >= 1)
				{
					int days = (int) Math.Floor(timespan.TotalDays);
					sb.Append(days);
					sb.Append(" ");
					sb.Append(days == 1 ? "day" : "days");
				}

				if (sb.Length > 0)
					sb.Append(", ");

				sb.Append(timespan.Hours);
				sb.Append(" ");
				sb.Append(timespan.Hours == 1 ? "hour, " : "hours, ");
				
				sb.Append(timespan.Minutes);
				sb.Append(" ");
				sb.Append(timespan.Minutes == 1 ? "minute, " : "minutes, ");
				
				sb.Append(timespan.Seconds);
				sb.Append(" ");
				sb.Append(timespan.Seconds == 1 ? "second" : "seconds");
			}

			return sb.ToString();
		}
	}
}
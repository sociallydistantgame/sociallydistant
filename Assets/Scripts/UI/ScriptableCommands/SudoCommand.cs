#nullable enable
using System.Threading.Tasks;
using Architecture;
using OS.Devices;
using UI.Shell;
using UnityEngine;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Sudo")]
	public class SudoCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override async Task OnExecute()
		{
			bool isPlayer = Process.User.Computer is PlayerComputer;
			bool requirePassword = (Process.User.PrivilegeLevel != PrivilegeLevel.Root && !isPlayer);
			
			// Print the sysadmin bible
			Console.WriteLine(@"We trust you have received the usual lecture from the local System
Administrator. It usually boils down to these three things:

    #1) Respect the privacy of others.
    #2) Think before you type.
    #3) With great power comes great responsibility.

");

			if (!await CheckAccess(requirePassword))
				return;
			
			// Check our privilege level. Must be admin to run a command.
			if (Process.User.PrivilegeLevel != PrivilegeLevel.Admin)
			{
				Console.WriteLine($"{UserName} is not an administrator.  This incident will be reported.");
				return;
			}

			ISystemProcess fork = Process.ForkAsUser(Process.User.Computer.SuperUser);

			var shell = new InteractiveShell(null);

			shell.Setup(fork, this.Console.Device);

			if (Arguments.Length > 0)
			{
				await shell.RunScript(string.Join(" ", Arguments));
			}
			else
			{
				await shell.Run();
			}
		}

		private async Task<bool> CheckAccess(bool requirePassword)
		{
			var result = false;
			
			if (requirePassword)
			{
				var hasResult = false;
				var attemptsLeft = 4;
				var incorrectAttemptss = 0;
				
				while (!hasResult)
				{
					Console.Write($"[{Name}] password for {this.UserName}: ");

					string password = await Console.ReadPasswordAsync();

					await Task.Delay(1000);
					
					if (Process.User.CheckPassword(password))
					{
						hasResult = true;
						result = true;
					}

					attemptsLeft--;
					incorrectAttemptss++;
					
					if (attemptsLeft == 0)
					{
						hasResult = true;
						result = false;
						Console.WriteLine($"[{Name}] {incorrectAttemptss} incorrect password attempts");
					}
					else
					{
						Console.WriteLine("Sorry, try again.");
					}
				}
			}
			else
			{
				var hasResult = false;

				while (!hasResult)
				{
					Console.Write($"[{Name}] Are you sure you want to run this command as root? [y/N] ");

					string answer = await Console.ReadLineAsync();

					if (string.IsNullOrWhiteSpace(answer))
						continue;

					// ow, my garbage collector has IBS
					// but I don't fucking give a fuck, take some damn gas-x and give me the first char
					char firstChar = answer.ToLower().Trim()[0];

					if (firstChar == 'y')
					{
						result = true;
						hasResult = true;
					}
					else if (firstChar == 'n')
					{
						result = false;
						hasResult = true;
					}
				}
			}

			return result;
		}
	}
}
#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using Architecture.AssetTypes;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

namespace Architecture
{
	public class HumanIdentityGenerator
	{
		private readonly List<string> firstNamesMale = new List<string>();
		private readonly List<string> firstNamesFemale = new List<string>();
		private readonly List<string> firstNamesGenderIndependent = new List<string>();
		private readonly List<string> lastNames = new List<string>();
		
		
		public void AddNameList(IEnumerable<string> nameSource, NameListType type)
		{
			switch (type)
			{
				case NameListType.LastName:
					lastNames.AddRange(nameSource);
					break;
				case NameListType.MaleFirstName:
					firstNamesMale.AddRange(nameSource);
					break;
				case NameListType.FemaleFirstName:
					firstNamesFemale.AddRange(nameSource);
					break;
				case NameListType.GenderIndependentFirstName:
					firstNamesGenderIndependent.AddRange(nameSource);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
		
		public void GenerateNewIdentity(ref string fullName, ref Gender genderIdentity)
		{
			int diceRoll1 = Random.Range(1, 6);
			int diceRoll2 = Random.Range(1, 6);
			int diceTotal = diceRoll1 + diceRoll2;

			genderIdentity = (diceTotal) switch
			{
				{ } when diceTotal > 6 && diceTotal % 2 == 1 => Gender.Unknown,
				{ } when diceTotal % 2 == 1 => Gender.Female,
				_ => Gender.Male
			};

			var nameBuilder = new StringBuilder();

			switch (genderIdentity)
			{
				case Gender.Male:
					nameBuilder.Append(GetName(firstNamesMale, firstNamesGenderIndependent));
					break;
				case Gender.Female:
					nameBuilder.Append(GetName(firstNamesFemale, firstNamesGenderIndependent));
					break;
				case Gender.Unknown:
					nameBuilder.Append(GetName(firstNamesFemale, firstNamesGenderIndependent, firstNamesFemale));
					break;
			}

			nameBuilder.Append(" ");
			nameBuilder.Append(GetName(lastNames));
			
			fullName = nameBuilder.ToString();
		}

		private string GetName(params IReadOnlyList<string>[] sources)
		{
			int listIndex = Random.Range(0, sources.Length);
			int sourceLength = sources[listIndex].Count;

			return sources[listIndex][Random.Range(0, sourceLength)];
		}
		
		public enum NameListType
		{
			LastName,
			MaleFirstName,
			FemaleFirstName,
			GenderIndependentFirstName
		}
	}
}
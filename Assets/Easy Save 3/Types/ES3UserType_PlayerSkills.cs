using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("highlightDuration", "skill1Cooldown", "highlightLayerName", "shelfHighlightLayerName", "skill1Level", "skill1InitialPrice", "skill1CurrentPrice", "skill2Cooldown", "skill2Level", "skill2InitialPrice", "skill2CurrentPrice", "skill3Cooldown", "skill3Level", "skill3InitialPrice", "skill3CurrentPrice", "totalPointsSpent", "skillUsed", "highlightTimer", "skill1CooldownTimer", "skill2CooldownTimer", "skill3CooldownTimer", "highlightLayer", "shelfHighlightLayer")]
	public class ES3UserType_PlayerSkills : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerSkills() : base(typeof(PlayerSkills)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerSkills)obj;
			
			writer.WritePrivateField("highlightDuration", instance);
			writer.WritePrivateField("skill1Cooldown", instance);
			writer.WritePrivateField("highlightLayerName", instance);
			writer.WritePrivateField("shelfHighlightLayerName", instance);
			writer.WritePrivateField("skill1Level", instance);
			writer.WritePrivateField("skill1InitialPrice", instance);
			writer.WritePrivateField("skill1CurrentPrice", instance);
			writer.WritePrivateField("skill2Cooldown", instance);
			writer.WritePrivateField("skill2Level", instance);
			writer.WritePrivateField("skill2InitialPrice", instance);
			writer.WritePrivateField("skill2CurrentPrice", instance);
			writer.WritePrivateField("skill3Cooldown", instance);
			writer.WritePrivateField("skill3Level", instance);
			writer.WritePrivateField("skill3InitialPrice", instance);
			writer.WritePrivateField("skill3CurrentPrice", instance);
			writer.WritePrivateField("totalPointsSpent", instance);
			writer.WritePrivateField("skillUsed", instance);
			writer.WritePrivateField("highlightTimer", instance);
			writer.WritePrivateField("skill1CooldownTimer", instance);
			writer.WritePrivateField("skill2CooldownTimer", instance);
			writer.WritePrivateField("skill3CooldownTimer", instance);
			writer.WritePrivateField("highlightLayer", instance);
			writer.WritePrivateField("shelfHighlightLayer", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerSkills)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "highlightDuration":
					instance = (PlayerSkills)reader.SetPrivateField("highlightDuration", reader.Read<System.Single>(), instance);
					break;
					case "skill1Cooldown":
					instance = (PlayerSkills)reader.SetPrivateField("skill1Cooldown", reader.Read<System.Single>(), instance);
					break;
					case "highlightLayerName":
					instance = (PlayerSkills)reader.SetPrivateField("highlightLayerName", reader.Read<System.String>(), instance);
					break;
					case "shelfHighlightLayerName":
					instance = (PlayerSkills)reader.SetPrivateField("shelfHighlightLayerName", reader.Read<System.String>(), instance);
					break;
					case "skill1Level":
					instance = (PlayerSkills)reader.SetPrivateField("skill1Level", reader.Read<System.Int32>(), instance);
					break;
					case "skill1InitialPrice":
					instance = (PlayerSkills)reader.SetPrivateField("skill1InitialPrice", reader.Read<System.Int32>(), instance);
					break;
					case "skill1CurrentPrice":
					instance = (PlayerSkills)reader.SetPrivateField("skill1CurrentPrice", reader.Read<System.Int32>(), instance);
					break;
					case "skill2Cooldown":
					instance = (PlayerSkills)reader.SetPrivateField("skill2Cooldown", reader.Read<System.Single>(), instance);
					break;
					case "skill2Level":
					instance = (PlayerSkills)reader.SetPrivateField("skill2Level", reader.Read<System.Int32>(), instance);
					break;
					case "skill2InitialPrice":
					instance = (PlayerSkills)reader.SetPrivateField("skill2InitialPrice", reader.Read<System.Int32>(), instance);
					break;
					case "skill2CurrentPrice":
					instance = (PlayerSkills)reader.SetPrivateField("skill2CurrentPrice", reader.Read<System.Int32>(), instance);
					break;
					case "skill3Cooldown":
					instance = (PlayerSkills)reader.SetPrivateField("skill3Cooldown", reader.Read<System.Single>(), instance);
					break;
					case "skill3Level":
					instance = (PlayerSkills)reader.SetPrivateField("skill3Level", reader.Read<System.Int32>(), instance);
					break;
					case "skill3InitialPrice":
					instance = (PlayerSkills)reader.SetPrivateField("skill3InitialPrice", reader.Read<System.Int32>(), instance);
					break;
					case "skill3CurrentPrice":
					instance = (PlayerSkills)reader.SetPrivateField("skill3CurrentPrice", reader.Read<System.Int32>(), instance);
					break;
					case "totalPointsSpent":
					instance = (PlayerSkills)reader.SetPrivateField("totalPointsSpent", reader.Read<System.Int32>(), instance);
					break;
					case "skillUsed":
					instance = (PlayerSkills)reader.SetPrivateField("skillUsed", reader.Read<System.Boolean>(), instance);
					break;
					case "highlightTimer":
					instance = (PlayerSkills)reader.SetPrivateField("highlightTimer", reader.Read<System.Single>(), instance);
					break;
					case "skill1CooldownTimer":
					instance = (PlayerSkills)reader.SetPrivateField("skill1CooldownTimer", reader.Read<System.Single>(), instance);
					break;
					case "skill2CooldownTimer":
					instance = (PlayerSkills)reader.SetPrivateField("skill2CooldownTimer", reader.Read<System.Single>(), instance);
					break;
					case "skill3CooldownTimer":
					instance = (PlayerSkills)reader.SetPrivateField("skill3CooldownTimer", reader.Read<System.Single>(), instance);
					break;
					case "highlightLayer":
					instance = (PlayerSkills)reader.SetPrivateField("highlightLayer", reader.Read<System.Int32>(), instance);
					break;
					case "shelfHighlightLayer":
					instance = (PlayerSkills)reader.SetPrivateField("shelfHighlightLayer", reader.Read<System.Int32>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void ResetStaticVariables()
		{
			Instance = null;
		}
	}


	public class ES3UserType_PlayerSkillsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PlayerSkillsArray() : base(typeof(PlayerSkills[]), ES3UserType_PlayerSkills.Instance)
		{
			Instance = this;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void ResetStaticVariables()
		{
			Instance = null;
		}
	}
}
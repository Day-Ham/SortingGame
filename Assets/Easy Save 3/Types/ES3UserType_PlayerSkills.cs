using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("playerInteraction", "currencyManager", "highlightDuration", "skill1Cooldown", "highlightLayerName", "shelfHighlightLayerName", "skill1Level", "skill1Price", "skill2Cooldown", "skill2Level", "skill2Price", "skill3Cooldown", "skill3Level", "skill3Price", "highlightTimer", "skill1CooldownTimer", "skill2CooldownTimer", "skill3CooldownTimer", "skill3Used", "highlightLayer", "shelfHighlightLayer", "highlightedObjects", "originalLayers")]
	public class ES3UserType_PlayerSkills : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerSkills() : base(typeof(PlayerSkills)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerSkills)obj;
			
			writer.WritePrivateFieldByRef("playerInteraction", instance);
			writer.WritePrivateFieldByRef("currencyManager", instance);
			writer.WritePrivateField("highlightDuration", instance);
			writer.WritePrivateField("skill1Cooldown", instance);
			writer.WritePrivateField("highlightLayerName", instance);
			writer.WritePrivateField("shelfHighlightLayerName", instance);
			writer.WritePrivateField("skill1Level", instance);
			writer.WritePrivateField("skill1Price", instance);
			writer.WritePrivateField("skill2Cooldown", instance);
			writer.WritePrivateField("skill2Level", instance);
			writer.WritePrivateField("skill2Price", instance);
			writer.WritePrivateField("skill3Cooldown", instance);
			writer.WritePrivateField("skill3Level", instance);
			writer.WritePrivateField("skill3Price", instance);
			writer.WritePrivateField("highlightTimer", instance);
			writer.WritePrivateField("skill1CooldownTimer", instance);
			writer.WritePrivateField("skill2CooldownTimer", instance);
			writer.WritePrivateField("skill3CooldownTimer", instance);
			writer.WritePrivateField("skill3Used", instance);
			writer.WritePrivateField("highlightLayer", instance);
			writer.WritePrivateField("shelfHighlightLayer", instance);
			writer.WritePrivateField("highlightedObjects", instance);
			writer.WritePrivateField("originalLayers", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerSkills)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "playerInteraction":
					instance = (PlayerSkills)reader.SetPrivateField("playerInteraction", reader.Read<PlayerInteraction>(), instance);
					break;
					case "currencyManager":
					instance = (PlayerSkills)reader.SetPrivateField("currencyManager", reader.Read<CurrencyManager>(), instance);
					break;
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
					case "skill1Price":
					instance = (PlayerSkills)reader.SetPrivateField("skill1Price", reader.Read<System.Int32>(), instance);
					break;
					case "skill2Cooldown":
					instance = (PlayerSkills)reader.SetPrivateField("skill2Cooldown", reader.Read<System.Single>(), instance);
					break;
					case "skill2Level":
					instance = (PlayerSkills)reader.SetPrivateField("skill2Level", reader.Read<System.Int32>(), instance);
					break;
					case "skill2Price":
					instance = (PlayerSkills)reader.SetPrivateField("skill2Price", reader.Read<System.Int32>(), instance);
					break;
					case "skill3Cooldown":
					instance = (PlayerSkills)reader.SetPrivateField("skill3Cooldown", reader.Read<System.Single>(), instance);
					break;
					case "skill3Level":
					instance = (PlayerSkills)reader.SetPrivateField("skill3Level", reader.Read<System.Int32>(), instance);
					break;
					case "skill3Price":
					instance = (PlayerSkills)reader.SetPrivateField("skill3Price", reader.Read<System.Int32>(), instance);
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
					case "skill3Used":
					instance = (PlayerSkills)reader.SetPrivateField("skill3Used", reader.Read<System.Boolean>(), instance);
					break;
					case "highlightLayer":
					instance = (PlayerSkills)reader.SetPrivateField("highlightLayer", reader.Read<System.Int32>(), instance);
					break;
					case "shelfHighlightLayer":
					instance = (PlayerSkills)reader.SetPrivateField("shelfHighlightLayer", reader.Read<System.Int32>(), instance);
					break;
					case "highlightedObjects":
					instance = (PlayerSkills)reader.SetPrivateField("highlightedObjects", reader.Read<System.Collections.Generic.List<UnityEngine.GameObject>>(), instance);
					break;
					case "originalLayers":
					instance = (PlayerSkills)reader.SetPrivateField("originalLayers", reader.Read<System.Collections.Generic.Dictionary<UnityEngine.GameObject, System.Int32>>(), instance);
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
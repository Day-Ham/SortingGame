using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("playerCamera", "holdPoint", "backpackPoint", "pickupRange", "interactLayer", "pickUpLayer", "obstacleLayer", "pickupSpeed", "outlineLayerName", "maxHeldItems", "switchDelay", "throwForce", "robotBuddyShelfPromp", "throwTargetLayers")]
	public class ES3UserType_PlayerInteraction : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerInteraction() : base(typeof(PlayerInteraction)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerInteraction)obj;
			
			writer.WritePrivateFieldByRef("playerCamera", instance);
			writer.WritePrivateFieldByRef("holdPoint", instance);
			writer.WritePrivateFieldByRef("backpackPoint", instance);
			writer.WritePrivateField("pickupRange", instance);
			writer.WritePrivateField("interactLayer", instance);
			writer.WritePrivateField("pickUpLayer", instance);
			writer.WritePrivateField("obstacleLayer", instance);
			writer.WritePrivateField("pickupSpeed", instance);
			writer.WritePrivateField("outlineLayerName", instance);
			writer.WritePrivateField("maxHeldItems", instance);
			writer.WritePrivateField("switchDelay", instance);
			writer.WritePrivateField("throwForce", instance);
			writer.WritePrivateFieldByRef("robotBuddyShelfPromp", instance);
			writer.WritePrivateField("throwTargetLayers", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerInteraction)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "playerCamera":
					instance = (PlayerInteraction)reader.SetPrivateField("playerCamera", reader.Read<UnityEngine.Camera>(), instance);
					break;
					case "holdPoint":
					instance = (PlayerInteraction)reader.SetPrivateField("holdPoint", reader.Read<UnityEngine.Transform>(), instance);
					break;
					case "backpackPoint":
					instance = (PlayerInteraction)reader.SetPrivateField("backpackPoint", reader.Read<UnityEngine.Transform>(), instance);
					break;
					case "pickupRange":
					instance = (PlayerInteraction)reader.SetPrivateField("pickupRange", reader.Read<System.Single>(), instance);
					break;
					case "interactLayer":
					instance = (PlayerInteraction)reader.SetPrivateField("interactLayer", reader.Read<UnityEngine.LayerMask>(), instance);
					break;
					case "pickUpLayer":
					instance = (PlayerInteraction)reader.SetPrivateField("pickUpLayer", reader.Read<UnityEngine.LayerMask>(), instance);
					break;
					case "obstacleLayer":
					instance = (PlayerInteraction)reader.SetPrivateField("obstacleLayer", reader.Read<UnityEngine.LayerMask>(), instance);
					break;
					case "pickupSpeed":
					instance = (PlayerInteraction)reader.SetPrivateField("pickupSpeed", reader.Read<System.Single>(), instance);
					break;
					case "outlineLayerName":
					instance = (PlayerInteraction)reader.SetPrivateField("outlineLayerName", reader.Read<System.String>(), instance);
					break;
					case "maxHeldItems":
					instance = (PlayerInteraction)reader.SetPrivateField("maxHeldItems", reader.Read<System.Int32>(), instance);
					break;
					case "switchDelay":
					instance = (PlayerInteraction)reader.SetPrivateField("switchDelay", reader.Read<System.Single>(), instance);
					break;
					case "throwForce":
					instance = (PlayerInteraction)reader.SetPrivateField("throwForce", reader.Read<System.Single>(), instance);
					break;
					case "robotBuddyShelfPromp":
					instance = (PlayerInteraction)reader.SetPrivateField("robotBuddyShelfPromp", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "throwTargetLayers":
					instance = (PlayerInteraction)reader.SetPrivateField("throwTargetLayers", reader.Read<UnityEngine.LayerMask>(), instance);
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


	public class ES3UserType_PlayerInteractionArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PlayerInteractionArray() : base(typeof(PlayerInteraction[]), ES3UserType_PlayerInteraction.Instance)
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
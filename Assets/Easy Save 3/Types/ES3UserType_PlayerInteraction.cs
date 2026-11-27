using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("pickupRange", "maxHeldItems")]
	public class ES3UserType_PlayerInteraction : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerInteraction() : base(typeof(PlayerInteraction)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerInteraction)obj;
			
			writer.WritePrivateField("pickupRange", instance);
			writer.WritePrivateField("maxHeldItems", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerInteraction)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "pickupRange":
					instance = (PlayerInteraction)reader.SetPrivateField("pickupRange", reader.Read<System.Single>(), instance);
					break;
					case "maxHeldItems":
					instance = (PlayerInteraction)reader.SetPrivateField("maxHeldItems", reader.Read<System.Int32>(), instance);
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
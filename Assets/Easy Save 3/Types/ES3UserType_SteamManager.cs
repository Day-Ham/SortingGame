using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("instance", "itemsShelved", "shelvesCompleted", "stolenFromRobotBuddy")]
	public class ES3UserType_SteamManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SteamManager() : base(typeof(SteamManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SteamManager)obj;
			
			writer.WritePropertyByRef("instance", SteamManager.instance);
			writer.WritePrivateField("itemsShelved", instance);
			writer.WritePrivateField("shelvesCompleted", instance);
			writer.WritePrivateField("stolenFromRobotBuddy", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SteamManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "instance":
						SteamManager.instance = reader.Read<SteamManager>();
						break;
					case "itemsShelved":
					instance = (SteamManager)reader.SetPrivateField("itemsShelved", reader.Read<System.Int32>(), instance);
					break;
					case "shelvesCompleted":
					instance = (SteamManager)reader.SetPrivateField("shelvesCompleted", reader.Read<System.Int32>(), instance);
					break;
					case "stolenFromRobotBuddy":
					instance = (SteamManager)reader.SetPrivateField("stolenFromRobotBuddy", reader.Read<System.Int32>(), instance);
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


	public class ES3UserType_SteamManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SteamManagerArray() : base(typeof(SteamManager[]), ES3UserType_SteamManager.Instance)
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
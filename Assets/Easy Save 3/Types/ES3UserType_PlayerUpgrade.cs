using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("capacityCurrentPrice", "upgradeCapacity", "sprintCurrentPrice", "canSprint", "reachCurrentPrice", "upgradeReach", "jumpCurrentPrice", "upgradeJump", "robotBuddyCurrentPrice", "boughtRobotBuddy", "totalPointsSpent")]
	public class ES3UserType_PlayerUpgrade : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerUpgrade() : base(typeof(PlayerUpgrade)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerUpgrade)obj;
			
			writer.WritePrivateField("capacityCurrentPrice", instance);
			writer.WritePrivateField("upgradeCapacity", instance);
			writer.WritePrivateField("sprintCurrentPrice", instance);
			writer.WritePrivateField("canSprint", instance);
			writer.WritePrivateField("reachCurrentPrice", instance);
			writer.WritePrivateField("upgradeReach", instance);
			writer.WritePrivateField("jumpCurrentPrice", instance);
			writer.WritePrivateField("upgradeJump", instance);
			writer.WritePrivateField("robotBuddyCurrentPrice", instance);
			writer.WritePrivateField("boughtRobotBuddy", instance);
			writer.WritePrivateField("totalPointsSpent", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerUpgrade)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "capacityCurrentPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("capacityCurrentPrice", reader.Read<System.Int32>(), instance);
					break;
					case "upgradeCapacity":
					instance = (PlayerUpgrade)reader.SetPrivateField("upgradeCapacity", reader.Read<System.Int32>(), instance);
					break;
					case "sprintCurrentPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("sprintCurrentPrice", reader.Read<System.Int32>(), instance);
					break;
					case "canSprint":
					instance = (PlayerUpgrade)reader.SetPrivateField("canSprint", reader.Read<System.Boolean>(), instance);
					break;
					case "reachCurrentPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("reachCurrentPrice", reader.Read<System.Int32>(), instance);
					break;
					case "upgradeReach":
					instance = (PlayerUpgrade)reader.SetPrivateField("upgradeReach", reader.Read<System.Int32>(), instance);
					break;
					case "jumpCurrentPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("jumpCurrentPrice", reader.Read<System.Int32>(), instance);
					break;
					case "upgradeJump":
					instance = (PlayerUpgrade)reader.SetPrivateField("upgradeJump", reader.Read<System.Int32>(), instance);
					break;
					case "robotBuddyCurrentPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("robotBuddyCurrentPrice", reader.Read<System.Int32>(), instance);
					break;
					case "boughtRobotBuddy":
					instance = (PlayerUpgrade)reader.SetPrivateField("boughtRobotBuddy", reader.Read<System.Int32>(), instance);
					break;
					case "totalPointsSpent":
					instance = (PlayerUpgrade)reader.SetPrivateField("totalPointsSpent", reader.Read<System.Int32>(), instance);
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


	public class ES3UserType_PlayerUpgradeArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PlayerUpgradeArray() : base(typeof(PlayerUpgrade[]), ES3UserType_PlayerUpgrade.Instance)
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
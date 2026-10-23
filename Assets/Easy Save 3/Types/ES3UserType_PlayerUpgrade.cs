using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("capacityPrice", "sprintPrice", "reachPrice", "jumpPrice", "throwToShelfPrice", "robotBuddyPrice", "upgradeCapacity", "upgradeSprint", "upgradeReach", "upgradeJump", "canThrowToShelf", "boughtRobotBuddy")]
	public class ES3UserType_PlayerUpgrade : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerUpgrade() : base(typeof(PlayerUpgrade)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerUpgrade)obj;
			
			writer.WritePrivateField("capacityPrice", instance);
			writer.WritePrivateField("sprintPrice", instance);
			writer.WritePrivateField("reachPrice", instance);
			writer.WritePrivateField("jumpPrice", instance);
			writer.WritePrivateField("throwToShelfPrice", instance);
			writer.WritePrivateField("robotBuddyPrice", instance);
			writer.WritePrivateField("upgradeCapacity", instance);
			writer.WritePrivateField("upgradeSprint", instance);
			writer.WritePrivateField("upgradeReach", instance);
			writer.WritePrivateField("upgradeJump", instance);
			writer.WritePrivateField("canThrowToShelf", instance);
			writer.WritePrivateField("boughtRobotBuddy", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerUpgrade)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "capacityPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("capacityPrice", reader.Read<System.Int32>(), instance);
					break;
					case "sprintPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("sprintPrice", reader.Read<System.Int32>(), instance);
					break;
					case "reachPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("reachPrice", reader.Read<System.Int32>(), instance);
					break;
					case "jumpPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("jumpPrice", reader.Read<System.Int32>(), instance);
					break;
					case "throwToShelfPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("throwToShelfPrice", reader.Read<System.Int32>(), instance);
					break;
					case "robotBuddyPrice":
					instance = (PlayerUpgrade)reader.SetPrivateField("robotBuddyPrice", reader.Read<System.Int32>(), instance);
					break;
					case "upgradeCapacity":
					instance = (PlayerUpgrade)reader.SetPrivateField("upgradeCapacity", reader.Read<System.Int32>(), instance);
					break;
					case "upgradeSprint":
					instance = (PlayerUpgrade)reader.SetPrivateField("upgradeSprint", reader.Read<System.Single>(), instance);
					break;
					case "upgradeReach":
					instance = (PlayerUpgrade)reader.SetPrivateField("upgradeReach", reader.Read<System.Single>(), instance);
					break;
					case "upgradeJump":
					instance = (PlayerUpgrade)reader.SetPrivateField("upgradeJump", reader.Read<System.Int32>(), instance);
					break;
					case "canThrowToShelf":
					instance = (PlayerUpgrade)reader.SetPrivateField("canThrowToShelf", reader.Read<System.Boolean>(), instance);
					break;
					case "boughtRobotBuddy":
					instance = (PlayerUpgrade)reader.SetPrivateField("boughtRobotBuddy", reader.Read<System.Single>(), instance);
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
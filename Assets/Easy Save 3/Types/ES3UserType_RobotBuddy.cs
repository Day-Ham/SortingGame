using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("holdPoint", "pickupSpeed", "stoppingDistFromShelf", "putDownSpeed", "targetDestination", "targetItem", "heldObject", "roamWaitTimeMax", "roamWaitTimeMin", "roamWaitTime", "roamTimer", "patrolPoints")]
	public class ES3UserType_RobotBuddy : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_RobotBuddy() : base(typeof(RobotBuddy)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (RobotBuddy)obj;
			
			writer.WritePrivateFieldByRef("holdPoint", instance);
			writer.WritePrivateField("pickupSpeed", instance);
			writer.WritePrivateField("stoppingDistFromShelf", instance);
			writer.WritePrivateField("putDownSpeed", instance);
			writer.WritePrivateField("targetDestination", instance);
			writer.WritePropertyByRef("targetItem", instance.targetItem);
			writer.WritePrivateFieldByRef("heldObject", instance);
			writer.WritePrivateField("roamWaitTimeMax", instance);
			writer.WritePrivateField("roamWaitTimeMin", instance);
			writer.WritePrivateField("roamWaitTime", instance);
			writer.WritePrivateField("roamTimer", instance);
			writer.WriteProperty("patrolPoints", instance.patrolPoints, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<UnityEngine.Transform>)));
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (RobotBuddy)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "holdPoint":
					instance = (RobotBuddy)reader.SetPrivateField("holdPoint", reader.Read<UnityEngine.Transform>(), instance);
					break;
					case "pickupSpeed":
					instance = (RobotBuddy)reader.SetPrivateField("pickupSpeed", reader.Read<System.Single>(), instance);
					break;
					case "stoppingDistFromShelf":
					instance = (RobotBuddy)reader.SetPrivateField("stoppingDistFromShelf", reader.Read<System.Single>(), instance);
					break;
					case "putDownSpeed":
					instance = (RobotBuddy)reader.SetPrivateField("putDownSpeed", reader.Read<System.Single>(), instance);
					break;
					case "targetDestination":
					instance = (RobotBuddy)reader.SetPrivateField("targetDestination", reader.Read<UnityEngine.Vector3>(), instance);
					break;
					case "targetItem":
						instance.targetItem = reader.Read<Item>(ES3UserType_Item.Instance);
						break;
					case "heldObject":
					instance = (RobotBuddy)reader.SetPrivateField("heldObject", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "roamWaitTimeMax":
					instance = (RobotBuddy)reader.SetPrivateField("roamWaitTimeMax", reader.Read<System.Single>(), instance);
					break;
					case "roamWaitTimeMin":
					instance = (RobotBuddy)reader.SetPrivateField("roamWaitTimeMin", reader.Read<System.Single>(), instance);
					break;
					case "roamWaitTime":
					instance = (RobotBuddy)reader.SetPrivateField("roamWaitTime", reader.Read<System.Single>(), instance);
					break;
					case "roamTimer":
					instance = (RobotBuddy)reader.SetPrivateField("roamTimer", reader.Read<System.Single>(), instance);
					break;
					case "patrolPoints":
						instance.patrolPoints = reader.Read<System.Collections.Generic.List<UnityEngine.Transform>>();
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


	public class ES3UserType_RobotBuddyArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_RobotBuddyArray() : base(typeof(RobotBuddy[]), ES3UserType_RobotBuddy.Instance)
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
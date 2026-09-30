using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("itemData", "rb", "stopSpeed", "stopTime", "stoppedTimer", "<IsHeld>k__BackingField", "IsPlaced", "m_CancellationTokenSource", "IsHeld", "enabled", "name")]
	public class ES3UserType_Item : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item() : base(typeof(Item)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Item)obj;
			
			writer.WritePrivateFieldByRef("itemData", instance);
			writer.WritePrivateFieldByRef("rb", instance);
			writer.WritePrivateField("stopSpeed", instance);
			writer.WritePrivateField("stopTime", instance);
			writer.WritePrivateField("stoppedTimer", instance);
			writer.WritePrivateField("<IsHeld>k__BackingField", instance);
			writer.WriteProperty("IsPlaced", instance.IsPlaced, ES3Type_bool.Instance);
			writer.WritePrivateField("m_CancellationTokenSource", instance);
			writer.WritePrivateProperty("IsHeld", instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Item)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "itemData":
					instance = (Item)reader.SetPrivateField("itemData", reader.Read<ItemData>(), instance);
					break;
					case "rb":
					instance = (Item)reader.SetPrivateField("rb", reader.Read<UnityEngine.Rigidbody>(), instance);
					break;
					case "stopSpeed":
					instance = (Item)reader.SetPrivateField("stopSpeed", reader.Read<System.Single>(), instance);
					break;
					case "stopTime":
					instance = (Item)reader.SetPrivateField("stopTime", reader.Read<System.Single>(), instance);
					break;
					case "stoppedTimer":
					instance = (Item)reader.SetPrivateField("stoppedTimer", reader.Read<System.Single>(), instance);
					break;
					case "<IsHeld>k__BackingField":
					instance = (Item)reader.SetPrivateField("<IsHeld>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					case "IsPlaced":
						instance.IsPlaced = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "m_CancellationTokenSource":
					instance = (Item)reader.SetPrivateField("m_CancellationTokenSource", reader.Read<System.Threading.CancellationTokenSource>(), instance);
					break;
					case "IsHeld":
					instance = (Item)reader.SetPrivateProperty("IsHeld", reader.Read<System.Boolean>(), instance);
					break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
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


	public class ES3UserType_ItemArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ItemArray() : base(typeof(Item[]), ES3UserType_Item.Instance)
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
using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("itemData", "IsHeld", "HeldBy", "IsPlaced", "enabled", "name")]
	public class ES3UserType_Item : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item() : base(typeof(Item)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Item)obj;
			
			writer.WritePrivateFieldByRef("itemData", instance);
			writer.WriteProperty("IsHeld", instance.IsHeld, ES3Type_bool.Instance);
			writer.WritePropertyByRef("HeldBy", instance.HeldBy);
			writer.WriteProperty("IsPlaced", instance.IsPlaced, ES3Type_bool.Instance);
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
					case "IsHeld":
						instance.IsHeld = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "HeldBy":
						instance.HeldBy = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "IsPlaced":
						instance.IsPlaced = reader.Read<System.Boolean>(ES3Type_bool.Instance);
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
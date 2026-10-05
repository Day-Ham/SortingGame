using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("itemType", "shelfType", "shelfArea", "shelfPositions", "heldItems", "displayName", "currentItem", "maxItems", "displayNameTxt", "background", "player", "spawner", "currency", "m_CancellationTokenSource", "enabled", "name")]
	public class ES3UserType_ItemChecker : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ItemChecker() : base(typeof(ItemChecker)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (ItemChecker)obj;
			
			writer.WritePrivateField("itemType", instance);
			writer.WritePrivateField("shelfType", instance);
			writer.WritePrivateFieldByRef("shelfArea", instance);
			writer.WritePrivateField("shelfPositions", instance);
			writer.WriteProperty("heldItems", instance.heldItems, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<Item>)));
			writer.WritePrivateField("displayName", instance);
			writer.WritePrivateFieldByRef("currentItem", instance);
			writer.WritePrivateField("maxItems", instance);
			writer.WritePrivateFieldByRef("displayNameTxt", instance);
			writer.WritePrivateFieldByRef("background", instance);
			writer.WritePrivateFieldByRef("player", instance);
			writer.WritePrivateFieldByRef("spawner", instance);
			writer.WritePrivateFieldByRef("currency", instance);
			writer.WritePrivateField("m_CancellationTokenSource", instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (ItemChecker)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "itemType":
					instance = (ItemChecker)reader.SetPrivateField("itemType", reader.Read<ItemType>(), instance);
					break;
					case "shelfType":
					instance = (ItemChecker)reader.SetPrivateField("shelfType", reader.Read<ShelfType>(), instance);
					break;
					case "shelfArea":
					instance = (ItemChecker)reader.SetPrivateField("shelfArea", reader.Read<UnityEngine.BoxCollider>(), instance);
					break;
					case "shelfPositions":
					instance = (ItemChecker)reader.SetPrivateField("shelfPositions", reader.Read<System.Collections.Generic.List<UnityEngine.Vector3>>(), instance);
					break;
					case "heldItems":
						instance.heldItems = reader.Read<System.Collections.Generic.List<Item>>();
						break;
					case "displayName":
					instance = (ItemChecker)reader.SetPrivateField("displayName", reader.Read<System.String>(), instance);
					break;
					case "currentItem":
					instance = (ItemChecker)reader.SetPrivateField("currentItem", reader.Read<Item>(), instance);
					break;
					case "maxItems":
					instance = (ItemChecker)reader.SetPrivateField("maxItems", reader.Read<System.Int32>(), instance);
					break;
					case "displayNameTxt":
					instance = (ItemChecker)reader.SetPrivateField("displayNameTxt", reader.Read<TMPro.TMP_Text>(), instance);
					break;
					case "background":
					instance = (ItemChecker)reader.SetPrivateField("background", reader.Read<UnityEngine.UI.Image>(), instance);
					break;
					case "player":
					instance = (ItemChecker)reader.SetPrivateField("player", reader.Read<PlayerInteraction>(), instance);
					break;
					case "spawner":
					instance = (ItemChecker)reader.SetPrivateField("spawner", reader.Read<ItemSpawner>(), instance);
					break;
					case "currency":
					instance = (ItemChecker)reader.SetPrivateField("currency", reader.Read<CurrencyManager>(), instance);
					break;
					case "m_CancellationTokenSource":
					instance = (ItemChecker)reader.SetPrivateField("m_CancellationTokenSource", reader.Read<System.Threading.CancellationTokenSource>(), instance);
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


	public class ES3UserType_ItemCheckerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ItemCheckerArray() : base(typeof(ItemChecker[]), ES3UserType_ItemChecker.Instance)
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
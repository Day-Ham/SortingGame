using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("items", "currentItem", "itemsSpawned", "spawnArea", "spawnHeight", "maxAttempts")]
	public class ES3UserType_ItemSpawner : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ItemSpawner() : base(typeof(ItemSpawner)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (ItemSpawner)obj;
			
			writer.WriteProperty("items", instance.items, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<ItemSpawnEntry>)));
			writer.WritePrivateFieldByRef("currentItem", instance);
			writer.WritePrivateField("itemsSpawned", instance);
			writer.WritePrivateFieldByRef("spawnArea", instance);
			writer.WritePrivateField("spawnHeight", instance);
			writer.WritePrivateField("maxAttempts", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (ItemSpawner)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "items":
						instance.items = reader.Read<System.Collections.Generic.List<ItemSpawnEntry>>();
						break;
					case "currentItem":
					instance = (ItemSpawner)reader.SetPrivateField("currentItem", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "itemsSpawned":
					instance = (ItemSpawner)reader.SetPrivateField("itemsSpawned", reader.Read<System.Boolean>(), instance);
					break;
					case "spawnArea":
					instance = (ItemSpawner)reader.SetPrivateField("spawnArea", reader.Read<UnityEngine.BoxCollider>(), instance);
					break;
					case "spawnHeight":
					instance = (ItemSpawner)reader.SetPrivateField("spawnHeight", reader.Read<System.Single>(), instance);
					break;
					case "maxAttempts":
					instance = (ItemSpawner)reader.SetPrivateField("maxAttempts", reader.Read<System.Int32>(), instance);
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


	public class ES3UserType_ItemSpawnerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ItemSpawnerArray() : base(typeof(ItemSpawner[]), ES3UserType_ItemSpawner.Instance)
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
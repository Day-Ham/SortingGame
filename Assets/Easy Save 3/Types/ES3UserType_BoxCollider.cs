using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("center", "size", "enabled", "isTrigger", "contactOffset", "hasModifiableContacts", "providesContacts", "layerOverridePriority", "excludeLayers", "includeLayers", "sharedMaterial", "material", "name")]
	public class ES3UserType_BoxCollider : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_BoxCollider() : base(typeof(UnityEngine.BoxCollider)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.BoxCollider)obj;
			
			writer.WriteProperty("center", instance.center, ES3Type_Vector3.Instance);
			writer.WriteProperty("size", instance.size, ES3Type_Vector3.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("isTrigger", instance.isTrigger, ES3Type_bool.Instance);
			writer.WriteProperty("contactOffset", instance.contactOffset, ES3Type_float.Instance);
			writer.WriteProperty("hasModifiableContacts", instance.hasModifiableContacts, ES3Type_bool.Instance);
			writer.WriteProperty("providesContacts", instance.providesContacts, ES3Type_bool.Instance);
			writer.WriteProperty("layerOverridePriority", instance.layerOverridePriority, ES3Type_int.Instance);
			writer.WriteProperty("excludeLayers", instance.excludeLayers, ES3Type_LayerMask.Instance);
			writer.WriteProperty("includeLayers", instance.includeLayers, ES3Type_LayerMask.Instance);
			writer.WritePropertyByRef("sharedMaterial", instance.sharedMaterial);
			writer.WritePropertyByRef("material", instance.material);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.BoxCollider)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "center":
						instance.center = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "size":
						instance.size = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isTrigger":
						instance.isTrigger = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "contactOffset":
						instance.contactOffset = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "hasModifiableContacts":
						instance.hasModifiableContacts = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "providesContacts":
						instance.providesContacts = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "layerOverridePriority":
						instance.layerOverridePriority = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "excludeLayers":
						instance.excludeLayers = reader.Read<UnityEngine.LayerMask>(ES3Type_LayerMask.Instance);
						break;
					case "includeLayers":
						instance.includeLayers = reader.Read<UnityEngine.LayerMask>(ES3Type_LayerMask.Instance);
						break;
					case "sharedMaterial":
						instance.sharedMaterial = reader.Read<UnityEngine.PhysicsMaterial>(ES3Type_PhysicsMaterial.Instance);
						break;
					case "material":
						instance.material = reader.Read<UnityEngine.PhysicsMaterial>(ES3Type_PhysicsMaterial.Instance);
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


	public class ES3UserType_BoxColliderArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_BoxColliderArray() : base(typeof(UnityEngine.BoxCollider[]), ES3UserType_BoxCollider.Instance)
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
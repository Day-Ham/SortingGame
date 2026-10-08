using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("stage1NewGameCreated", "stage2NewGameCreated", "stage3NewGameCreated")]
	public class ES3UserType_TitleScreenManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TitleScreenManager() : base(typeof(TitleScreenManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (TitleScreenManager)obj;
			
			writer.WritePrivateField("stage1NewGameCreated", instance);
			writer.WritePrivateField("stage2NewGameCreated", instance);
			writer.WritePrivateField("stage3NewGameCreated", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (TitleScreenManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "stage1NewGameCreated":
					instance = (TitleScreenManager)reader.SetPrivateField("stage1NewGameCreated", reader.Read<System.Boolean>(), instance);
					break;
					case "stage2NewGameCreated":
					instance = (TitleScreenManager)reader.SetPrivateField("stage2NewGameCreated", reader.Read<System.Boolean>(), instance);
					break;
					case "stage3NewGameCreated":
					instance = (TitleScreenManager)reader.SetPrivateField("stage3NewGameCreated", reader.Read<System.Boolean>(), instance);
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


	public class ES3UserType_TitleScreenManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TitleScreenManagerArray() : base(typeof(TitleScreenManager[]), ES3UserType_TitleScreenManager.Instance)
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
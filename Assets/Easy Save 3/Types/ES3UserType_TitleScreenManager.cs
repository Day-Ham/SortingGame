using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("resumeGameBtn", "stageData")]
	public class ES3UserType_TitleScreenManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TitleScreenManager() : base(typeof(TitleScreenManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (TitleScreenManager)obj;
			
			writer.WritePrivateFieldByRef("resumeGameBtn", instance);
			writer.WritePrivateField("stageData", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (TitleScreenManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "resumeGameBtn":
					instance = (TitleScreenManager)reader.SetPrivateField("resumeGameBtn", reader.Read<UnityEngine.UI.Button>(), instance);
					break;
					case "stageData":
					instance = (TitleScreenManager)reader.SetPrivateField("stageData", reader.Read<System.Collections.Generic.List<StageData>>(), instance);
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
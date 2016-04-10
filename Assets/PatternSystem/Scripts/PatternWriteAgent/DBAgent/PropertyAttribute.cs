
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DBAgent
{
	[AttributeUsage(AttributeTargets.Field,AllowMultiple = true)]
	public abstract class DBFieldAttribute : System.Attribute
	{
		static HashSet<System.Type> s_Attributes = new HashSet<Type>();
		static HashSet<string> s_AttributesName = new HashSet<string>();
		static public string GetDBAttributes(System.Reflection.FieldInfo info)
		{
			string attributes = "";
			if (s_Attributes.Count == 0) {
				s_Attributes.Add (typeof(NOT_NULL));
				s_Attributes.Add (typeof(PRIMARY_KEY));
				s_Attributes.Add (typeof(AUTOINCREMENT));
				s_Attributes.Add (typeof(UNIQUE));


				s_AttributesName.Add(NOT_NULL.s_key);
				s_AttributesName.Add(PRIMARY_KEY.s_key);
				s_AttributesName.Add(AUTOINCREMENT.s_key);
				s_AttributesName.Add(UNIQUE.s_key);
			}
			HashSet<string> xorAttributesName = new HashSet<string>();
			foreach (string name in s_AttributesName)
			{
				if(attributes.Length > 0)
					attributes += " ";
				attributes += name;
				xorAttributesName.Add(name);
			}

			foreach (Attribute att in info.GetCustomAttributes(true))
			{
				if (s_Attributes.Contains (att.GetType()) ) 
				{
					DBFieldAttribute dbAtt = att as DBFieldAttribute;
					xorAttributesName.Remove (dbAtt.GetKeyWord ());
				}
			}
			foreach (string name in xorAttributesName)
			{
				attributes = attributes.Replace(name,"");
			}
			return attributes;
		}
		protected DBFieldAttribute(){}
		public abstract string GetKeyWord();

	}

	public class AUTOINCREMENT : DBFieldAttribute
	{
		public static string s_key = "AUTOINCREMENT";
		public AUTOINCREMENT(){}
		public override string GetKeyWord ()
		{
			return s_key;
		}
	}

	public class PRIMARY_KEY : DBFieldAttribute
	{
		public static string s_key = "PRIMARY KEY";
		public PRIMARY_KEY(){}
		public override string GetKeyWord()
		{
			return s_key;
		}
	}

	public class NOT_NULL : DBFieldAttribute
	{
		public static string s_key = "NOT NULL";
		public NOT_NULL(){}
		public override string GetKeyWord()
		{
			return s_key;
		}
	}

	public class UNIQUE : DBFieldAttribute
	{
		public static string s_key = "NOT NULL";
		public UNIQUE(){}
		public override string GetKeyWord()
		{
			return "NOT NULL";
		}
	}
}


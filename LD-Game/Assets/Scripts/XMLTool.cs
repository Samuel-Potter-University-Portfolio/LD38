using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class XML
{
	public static int GetInt(XmlAttribute attrib, int defaultValue = 0)
	{
		if (attrib == null)
			return defaultValue;
		else
			return System.Int32.Parse(attrib.Value);
	}

	public static uint GetUInt(XmlAttribute attrib, uint defaultValue = 0)
	{
		if (attrib == null)
			return defaultValue;
		else
			return System.UInt32.Parse(attrib.Value);
	}

	public static bool GetBool(XmlAttribute attrib, bool defaultValue = false)
	{
		if (attrib == null)
			return defaultValue;
		else
			return attrib.Value.ToLower().Equals("true");
	}

}


using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System;

/// <summary>
/// saves games into xml
/// </summary>
public class XMLGameSaver
{
    /// <summary>
    /// serialize a object
    /// </summary>
    /// <param name="gObj">the object need to be serialized</param>
    /// <returns>serialized string</returns>
    public static string SerializeObject(object gObj)
    {
        string xmlizedString = null;
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(gObj.GetType());
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xmlTextWriter.Formatting = Formatting.Indented;

        xs.Serialize(xmlTextWriter, gObj);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        xmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());

        return xmlizedString;

    }

    /// <summary>
    /// deserialize a object
    /// </summary>
    /// <typeparam name="T">some object type</typeparam>
    /// <param name="xmlString">some xml</param>
    /// <returns>more object type</returns>
    public static T DeserializeObject<T>(string xmlString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(T));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xmlString));

        return (T)xs.Deserialize(memoryStream);
    }

    /// <summary>
    /// save game into file
    /// </summary>
    /// <param name="fileLocation">location of file</param>
    /// <param name="fileName">file name</param>
    /// <param name="xmlData">game data</param>
    public static void SaveGame(string fileLocation, string fileName, string xmlData)
    {
        StreamWriter writer;
        FileInfo f = new FileInfo(fileLocation + "\\" + fileName);

        if (!f.Exists)
        {
            writer = f.CreateText();
        }
        else
        {
            f.Delete();
            writer = f.CreateText();
        }

        writer.Write(xmlData);
        writer.Close();
    }

    /// <summary>
    /// save game into file
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="xmlData">game data</param>
    public static void SaveGame(string fileName, string xmlData)
    {
        StreamWriter writer;
        FileInfo f = new FileInfo(fileName);

        if (!f.Exists)
        {
            writer = f.CreateText();
        }
        else
        {
      //      f.Delete();
            writer = f.CreateText();
        }

        writer.Write(xmlData);
        writer.Close();
    }

    /// <summary>
    /// load game from file
    /// </summary>
    /// <param name="fileLocation">location of file</param>
    /// <param name="fileName">file name</param>
    /// <returns>some xml</returns>
    public static string LoadGame(string fileLocation, string fileName)
    {
        StreamReader sr = File.OpenText(fileLocation + "\\" + fileName);
        string xmlData = sr.ReadToEnd();
        sr.Close();
        return xmlData;
    }

    /// <summary>
    /// load game from file
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <returns>a strings</returns>
    public static string LoadGame(string fileName)
    {
        StreamReader sr = File.OpenText(fileName);
        string xmlData = sr.ReadToEnd();
        sr.Close();
        return xmlData;
    }

    /// <summary>
    /// transform a UTF8 byte array to string
    /// used in SerializeObject function
    /// </summary>
    /// <param name="characters">some characters</param>
    /// <returns>some magic</returns>
    public static string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
        return constructedString;
    }

    /// <summary>
    /// transform a string to UTF8 byte array
    /// used in DeserializeObject function
    /// </summary>
    /// <param name="xmlString">some xml</param>
    /// <returns>no idea</returns>
    public static byte[] StringToUTF8ByteArray(string xmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(xmlString);
        return byteArray;
    }
}

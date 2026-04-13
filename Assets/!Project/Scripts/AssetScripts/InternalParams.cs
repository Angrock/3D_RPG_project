using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Enfity.SaveAndLoad
{
    /// <summary>
    /// InternalParams is a class that saves game settings and values as key-value pairs.<br/>
    /// It can store string, integer, float, boolean and Vector3 values in the special save file in the project folder.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Features:<br/>
    /// Can save data with the same key name, but in different data types.<br/>
    /// Guaranteed to work on PC and Android (including Meta Quest 2/3).<br/>
    /// Has a protection system against manual corruption of the save file:<br/>
    /// - a completely corrupted save file without separators does not causes bugs in the InternalParams operation;<br/>
    /// - extraneous lines are ignored by the InternalParams;<br/>
    /// - a corrupted key-value pair is perceived as missing from the save file and ignored by the InternalParams;<br/>
    /// - in the case of a duplicated key-value pair, the first such pair is used in the methods operation;<br/>
    /// - if there is a corrupted value in the key-value pair, then this pair is replaced with a pair with the default value for the data type of this pair;<br/>
    /// - a key-value pair with a corrupted data type is perceived as missing from the save file and ignored by the InternalParams;<br/>
    /// - when null values are passed to methods, their operation has no effect and stops prematurely (it also returns the default value or false in the case of non-void methods).<br/>
    /// The protection system against manual corruption of the save file has logging with 'Debug.LogWarning()'.<br/>
    /// The protection system against manual corruption of the save file has an impact only if the save file content is intentionally manually corrupted.
    /// </para>
    /// <para>
    /// Recommendations:<br/>
    /// It is recommended not to damage the save file contents manually to avoid possible loss of saved data.<br/>
    /// It is recommended not to touch the separators in the save file, and also not to copy key-value pairs because:<br/>
    /// - InternalParams uses special system control characters which are not displayed in the clipboard;<br/>
    /// - if you try to manually copy one pair and paste it into the save file, InternalParams will ignore it.
    /// </para>
    /// <para>
    /// Notes:<br/>
    /// Version 9 (Latest version as of April 12, 2026)<br/>
    /// GitHub: https://github.com/EnfityBro/InternalParams
    /// </para>
    /// </remarks>
    public static class InternalParams
    {
        #region Properties, Fields, Enumerations And Constructor

        /// <summary>
        /// The name of the save file in which the key-value pairs are stored.<br/>
        /// Default save file name is 'InternalParams.enfity'.
        /// </summary>
        public static string SaveFileName { get; private set; } =
            (Application.platform == RuntimePlatform.Android)
            ? $"{Application.persistentDataPath}/{defaultSaveFileName}"
            : defaultSaveFileName;

        /// <summary>
        /// The separator that separates the data in the key-value pairs in the save file.
        /// </summary>
        private static readonly string separator;

        /// <summary>
        /// The default save file name.
        /// </summary>
        private const string defaultSaveFileName = "InternalParams.enfity";

        /// <summary>
        /// Contains the data types for private methods parameters.
        /// </summary>
        private enum DataTypes
        {
            String,
            Int,
            Float,
            Bool,
            Vector3
        }

        static InternalParams()
        {
            separator = $"~{char.ConvertFromUtf32(0x001F)}~";
        }

        #endregion


        #region String Methods

        /// <summary>
        /// Sets a string value identified by the given key.
        /// </summary>
        public static void SetString(string key, string value) => SetValue(key, value);

        /// <summary>
        /// Returns the string value corresponding to key if it exists.<br/>
        /// If it does not exist, it returns empty string.
        /// </summary>
        public static string GetString(string key) => (string)GetValue(key, DataTypes.String, false);

        /// <summary>
        /// Returns the string value corresponding to key if it exists.<br/>
        /// If it does not exist, it creates a key-value pair if createIfMissing parameter is true, and returns empty string.
        /// </summary>
        public static string GetString(string key, bool createIfMissing) => (string)GetValue(key, DataTypes.String, createIfMissing);

        /// <summary>
        /// Returns true if the given key with string value exists, otherwise returns false.
        /// </summary>
        public static bool HasKeyString(string key) => HasKeyByType(key, DataTypes.String);

        /// <summary>
        /// Removes the given key with string value.<br/>
        /// If no such key exists, DeleteKeyString has no impact.
        /// </summary>
        public static void DeleteKeyString(string key) => DeleteKeyByType(key, DataTypes.String);

        #endregion


        #region Int Methods

        /// <summary>
        /// Sets an integer value identified by the given key.
        /// </summary>
        public static void SetInt(string key, int value) => SetValue(key, value);

        /// <summary>
        /// Returns the integer value corresponding to key if it exists.<br/>
        /// If it does not exist, it returns 0.
        /// </summary>
        public static int GetInt(string key) => (int)GetValue(key, DataTypes.Int, false);

        /// <summary>
        /// Returns the integer value corresponding to key if it exists.<br/>
        /// If it does not exist, it creates a key-value pair if createIfMissing parameter is true, and returns 0.
        /// </summary>
        public static int GetInt(string key, bool createIfMissing) => (int)GetValue(key, DataTypes.Int, createIfMissing);

        /// <summary>
        /// Returns true if the given key with integer value exists, otherwise returns false.
        /// </summary>
        public static bool HasKeyInt(string key) => HasKeyByType(key, DataTypes.Int);

        /// <summary>
        /// Removes the given key with integer value.<br/>
        /// If no such key exists, DeleteKeyInt has no impact.
        /// </summary>
        public static void DeleteKeyInt(string key) => DeleteKeyByType(key, DataTypes.Int);

        #endregion


        #region Float Methods

        /// <summary>
        /// Sets a float value identified by the given key.
        /// </summary>
        public static void SetFloat(string key, float value) => SetValue(key, value);

        /// <summary>
        /// Returns the float value corresponding to key if it exists.<br/>
        /// If it does not exist, it returns 0.0f.
        /// </summary>
        public static float GetFloat(string key) => (float)GetValue(key, DataTypes.Float, false);

        /// <summary>
        /// Returns the float value corresponding to key if it exists.<br/>
        /// If it does not exist, it creates a key-value pair if createIfMissing parameter is true, and returns 0.0f.
        /// </summary>
        public static float GetFloat(string key, bool createIfMissing) => (float)GetValue(key, DataTypes.Float, createIfMissing);

        /// <summary>
        /// Returns true if the given key with float value exists, otherwise returns false.
        /// </summary>
        public static bool HasKeyFloat(string key) => HasKeyByType(key, DataTypes.Float);

        /// <summary>
        /// Removes the given key with float value.<br/>
        /// If no such key exists, DeleteKeyFloat has no impact.
        /// </summary>
        public static void DeleteKeyFloat(string key) => DeleteKeyByType(key, DataTypes.Float);

        #endregion


        #region Bool Methods

        /// <summary>
        /// Sets a boolean value identified by the given key.
        /// </summary>
        public static void SetBool(string key, bool value) => SetValue(key, value);

        /// <summary>
        /// Returns the boolean value corresponding to key if it exists.<br/>
        /// If it does not exist, it returns false.
        /// </summary>
        public static bool GetBool(string key) => (bool)GetValue(key, DataTypes.Bool, false);

        /// <summary>
        /// Returns the boolean value corresponding to key if it exists.<br/>
        /// If it does not exist, it creates a key-value pair if createIfMissing parameter is true, and returns false.
        /// </summary>
        public static bool GetBool(string key, bool createIfMissing) => (bool)GetValue(key, DataTypes.Bool, createIfMissing);

        /// <summary>
        /// Returns true if the given key with boolean value exists, otherwise returns false.
        /// </summary>
        public static bool HasKeyBool(string key) => HasKeyByType(key, DataTypes.Bool);

        /// <summary>
        /// Removes the given key with boolean value.<br/>
        /// If no such key exists, DeleteKeyBool has no impact.
        /// </summary>
        public static void DeleteKeyBool(string key) => DeleteKeyByType(key, DataTypes.Bool);

        #endregion


        #region Vector3 Methods

        /// <summary>
        /// Sets a Vector3 value identified by the given key.
        /// </summary>
        public static void SetVector3(string key, Vector3 value) => SetValue(key, value);

        /// <summary>
        /// Returns the Vector3 value corresponding to key if it exists.<br/>
        /// If it does not exist, it returns Vector3.zero.
        /// </summary>
        public static Vector3 GetVector3(string key) => (Vector3)GetValue(key, DataTypes.Vector3, false);

        /// <summary>
        /// Returns the Vector3 value corresponding to key if it exists.<br/>
        /// If it does not exist, it creates a key-value pair if createIfMissing parameter is true, and returns Vector3.zero.
        /// </summary>
        public static Vector3 GetVector3(string key, bool createIfMissing) => (Vector3)GetValue(key, DataTypes.Vector3, createIfMissing);

        /// <summary>
        /// Returns true if the given key with Vector3 value exists, otherwise returns false.
        /// </summary>
        public static bool HasKeyVector3(string key) => HasKeyByType(key, DataTypes.Vector3);

        /// <summary>
        /// Removes the given key with Vector3 value.<br/>
        /// If no such key exists, DeleteKeyVector3 has no impact.
        /// </summary>
        public static void DeleteKeyVector3(string key) => DeleteKeyByType(key, DataTypes.Vector3);

        #endregion


        #region Public Methods

        /// <summary>
        /// Deletes all existing keys and values.<br/>
        /// If there are no key-value pairs, then DeleteAll will has no impact.
        /// </summary>
        public static void DeleteAll()
        {
            using (FileStream file = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                file.SetLength(0);
            }
        }

        /// <summary>
        /// Deletes all existing key-value pairs with the given key.<br/>
        /// If there are no such keys it has no impact.
        /// </summary>
        public static void DeleteAllKeys(string key)
        {
            if (key == null)
            {
                Debug.LogWarning($"Warning from DeleteAllKeys method: You are trying to delete null keys. " +
                    $"The method operation had no impact and was terminated prematurely.");

                return;
            }
            key = StringEscapeUtility.Escape(key);

            CheckOrCreateSaveFile();

            try
            {
                List<string> lines = File.ReadAllLines(SaveFileName).ToList();

                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    if (lines[i].Split(separator)[1] == key)
                        lines.RemoveAt(i);
                }

                File.WriteAllLines(SaveFileName, lines);
            }
            catch (IndexOutOfRangeException exception)
            {
                Debug.LogWarning($"Exception from DeleteAllKeys method with '{((key != null) ? key : "null")}' key: {exception.Message}. " +
                    $"Due to this exception, the method operation had no impact. " +
                    $"This message appears only when you try to manually corrupt the save file content.");
            }
        }

        /// <summary>
        /// Returns true if the given key exists, otherwise returns false.
        /// </summary>
        public static bool HasKey(string key)
        {
            if (key == null)
            {
                Debug.LogWarning($"Warning from HasKey method: You are trying to get a null key. " +
                    $"The method returns false and was terminated prematurely.");

                return false;
            }
            key = StringEscapeUtility.Escape(key);

            CheckOrCreateSaveFile();

            try
            {
                List<string> lines = File.ReadAllLines(SaveFileName).ToList();

                foreach (string line in lines)
                {
                    if (line.Split(separator)[1] == key)
                        return true;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                Debug.LogWarning($"Exception from HasKey method with '{((key != null) ? key : "null")}' key: {exception.Message}. " +
                    $"Due to this exception, the method returns false. " +
                    $"This message appears only when you try to manually corrupt the save file content.");
            }

            return false;
        }

        /// <summary>
        /// Returns the number of all existing key-value pairs.
        /// </summary>
        public static int PairsCount()
        {
            CheckOrCreateSaveFile();

            int count = 0;

            try
            {
                List<string> lines = File.ReadAllLines(SaveFileName).ToList();

                foreach (string line in lines)
                {
                    if (!IsPairCorrupted(line.Split(separator)))
                        count++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                Debug.LogWarning($"Exception from PairsCount method: {exception.Message}. " +
                    $"Due to this exception, the method returns 0. " +
                    $"This message appears only when you try to manually corrupt the save file content.");
            }

            return count;
        }

        /// <summary>
        /// Sets (for subsequent calls) the save file name in which the key-value pairs will be saved.
        /// </summary>
        /// <remarks>
        /// Sets the name only for subsequent calls, useful when working with multiple save files.<br/>
        /// Default save file name is 'InternalParams.enfity'.
        /// </remarks>
        public static void SetSaveFileName(string newSaveFileName)
        {
            if (IsValidSaveFileName(newSaveFileName))
            {
                SaveFileName = (Application.platform == RuntimePlatform.Android)
                    ? $"{Application.persistentDataPath}/{newSaveFileName}"
                    : newSaveFileName;
            }
            else
            {
                Debug.LogWarning($"Warning from SetSaveFileName method: You are trying to set a null or invalid save file name. " +
                    $"The method operation had no impact. The current save file name remains the same.");
            }
        }

        /// <summary>
        /// Returns a list with all key-value pairs stored in the save file.<br/>
        /// If there are no key-value pairs, it returns an empty list.
        /// </summary>
        /// <returns>A List with elements that consist of a KeyValuePair with a string key and a value with object data type.</returns>
        public static List<KeyValuePair<string, object>> GetAllKeyValuePairs()
        {
            CheckOrCreateSaveFile();

            List<KeyValuePair<string, object>> keyValuePairs = new List<KeyValuePair<string, object>>();

            if (PairsCount() == 0)
                return keyValuePairs;

            List<string> lines = File.ReadAllLines(SaveFileName).ToList();

            foreach (string line in lines)
            {
                if (!IsPairCorrupted(line.Split(separator)))
                {
                    string key = line.Split(separator)[1];
                    object value = null;
                    DataTypes type = (DataTypes)Enum.Parse(typeof(DataTypes), line.Split(separator)[3]);

                    TryGetValue(key, ref value, type, line.Split(separator)[2]);
                    keyValuePairs.Add(new KeyValuePair<string, object>(key, value));
                }
            }

            return keyValuePairs;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Sets a given value identified by the given key.
        /// </summary>
        private static void SetValue(string key, object value)
        {
            if ((key == null) || (value == null))
            {
                Debug.LogWarning($"Warning from Set* methods: You are trying to set a {((key == null) ? "null key" : "null value")}. " +
                    $"The method operation had no impact and was terminated prematurely.");
                return;
            }
            key = StringEscapeUtility.Escape(key);

            int? lineIndex = FindLineIndex(key, GetDataType(value));
            DataTypes type = GetDataType(value);

            if (type == DataTypes.String)
                value = StringEscapeUtility.Escape((string)value);

            if (lineIndex == null)
            {
                Write(key, value);
            }
            else
            {
                List<string> lines = File.ReadAllLines(SaveFileName).ToList();
                lines.Remove(lines[(int)lineIndex]);

                if (type == DataTypes.Float)
                    lines.Add(separator + key + separator + ((float)value).ToString(CultureInfo.InvariantCulture) + separator + GetDataType(value).ToString() + separator);
                else if (type == DataTypes.Vector3)
                    lines.Add(separator + key + separator + ((Vector3)value).ToString("F7", CultureInfo.InvariantCulture) + separator + GetDataType(value).ToString() + separator);
                else
                    lines.Add(separator + key + separator + $"{value}" + separator + GetDataType(value).ToString() + separator);

                File.WriteAllLines(SaveFileName, lines);
            }
        }

        /// <summary>
        /// Returns the value of a specific data type corresponding to key if it exists.<br/>
        /// If it does not exist, it creates a key-value pair with corresponding data type if createIfMissing parameter is true, and returns default value for this data type.
        /// </summary>
        private static object GetValue(string key, DataTypes type, bool createIfMissing)
        {
            object value = null;

            if (key == null)
            {
                Debug.LogWarning($"Warning from Get* methods: You are trying to get a null key. " +
                    $"The method returns the default value for the {type} type and was terminated prematurely.");

                GetDefaultValue(ref value, type);
                return value;
            }
            key = StringEscapeUtility.Escape(key);

            int? lineIndex = FindLineIndex(key, type);

            if (lineIndex == null)
            {
                GetDefaultValue(ref value, type);

                if (createIfMissing)
                    Write(key, value);
            }
            else
            {
                List<string> lines = File.ReadAllLines(SaveFileName).ToList();
                string[] necessaryLine = lines[(int)lineIndex].Split(separator);

                TryGetValue(key, ref value, type, necessaryLine[2]);
            }

            return value;
        }

        /// <summary>
        /// Returns true if the given key with a value of given data type exists, otherwise returns false.
        /// </summary>
        private static bool HasKeyByType(string key, DataTypes type)
        {
            if (key == null)
            {
                Debug.LogWarning($"Warning from HasKey{type} method: You are trying to get a null key. " +
                    $"The method returns false and was terminated prematurely.");

                return false;
            }
            key = StringEscapeUtility.Escape(key);

            CheckOrCreateSaveFile();

            try
            {
                List<string> lines = File.ReadAllLines(SaveFileName).ToList();

                foreach (string line in lines)
                {
                    string[] pair = line.Split(separator);

                    if ((pair[1] == key) && (pair[3] == type.ToString()))
                        return true;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                Debug.LogWarning($"Exception from HasKey (by type) methods with '{((key != null) ? key : "null")}' key and {type} type: {exception.Message}. " +
                    $"Due to this exception, the method returns false. " +
                    $"This message appears only when you try to manually corrupt the save file content.");
            }

            return false;
        }

        /// <summary>
        /// Removes the given key with a value of given data type.<br/>
        /// If no such key exists, DeleteKeyByType has no impact.
        /// </summary>
        private static void DeleteKeyByType(string key, DataTypes type)
        {
            if (key == null)
            {
                Debug.LogWarning($"Warning from DeleteKey{type} method: You are trying to delete a null key. " +
                    $"The method operation had no impact and was terminated prematurely.");

                return;
            }
            key = StringEscapeUtility.Escape(key);

            int? lineIndex = FindLineIndex(key, type);

            if (lineIndex != null)
            {
                List<string> lines = File.ReadAllLines(SaveFileName).ToList();
                lines.Remove(lines[(int)lineIndex]);

                File.WriteAllLines(SaveFileName, lines);
            }
        }

        /// <summary>
        /// Creates a key-value pair with a given value identified by the given key and writes it to a safe file.
        /// </summary>
        private static void Write(string key, object value)
        {
            using (FileStream file = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                DataTypes type = GetDataType(value);
                byte[] keyArray = Encoding.Default.GetBytes(separator + key + separator);
                byte[] valueArray;

                if (type == DataTypes.Float)
                    valueArray = Encoding.Default.GetBytes(((float)value).ToString(CultureInfo.InvariantCulture) + separator + type.ToString() + separator + "\n");
                else if (type == DataTypes.Vector3)
                    valueArray = Encoding.Default.GetBytes(((Vector3)value).ToString("F7", CultureInfo.InvariantCulture) + separator + type.ToString() + separator + "\n");
                else
                    valueArray = Encoding.Default.GetBytes($"{value}" + separator + type.ToString() + separator + "\n");

                file.Seek(0, SeekOrigin.End);
                file.Write(keyArray, 0, keyArray.Length);
                file.Write(valueArray, 0, valueArray.Length);
            }
        }

        /// <summary>
        /// Returns the value of a specific data type corresponding to key.<br/>
        /// If the value is corrupted, then the pair with this value is replaced with a pair with the default value for the data type of this pair.
        /// </summary>
        private static void TryGetValue(string key, ref object value, DataTypes type, string inputValue)
        {
            switch (type)
            {
                case DataTypes.String:
                    {
                        value = StringEscapeUtility.Unescape(inputValue);
                        break;
                    }
                case DataTypes.Int:
                    {
                        try
                        {
                            value = Convert.ToInt32(inputValue);
                        }
                        catch (Exception exception)
                        {
                            value = 0;

                            DeleteKeyByType(key, type);
                            Write(key, value);

                            Debug.LogWarning($"Exception from Get* methods with '{((key != null) ? key : "null")}' key and {type} type: {exception.Message}. " +
                                $"The key-value pair has been replaced with a pair with the default value for the data type of this pair. " +
                                $"This message appears only when you try to manually corrupt the save file content.");
                        }

                        break;
                    }
                case DataTypes.Float:
                    {
                        try
                        {
                            inputValue = inputValue.Replace(',', '.');
                            value = float.Parse(inputValue, CultureInfo.InvariantCulture);
                        }
                        catch (Exception exception)
                        {
                            value = 0.0f;

                            DeleteKeyByType(key, type);
                            Write(key, value);

                            Debug.LogWarning($"Exception from Get* methods with '{((key != null) ? key : "null")}' key and {type} type: {exception.Message}. " +
                                $"The key-value pair has been replaced with a pair with the default value for the data type of this pair. " +
                                $"This message appears only when you try to manually corrupt the save file content.");
                        }

                        break;
                    }
                case DataTypes.Bool:
                    {
                        if ((inputValue == "True") || (inputValue == "true"))
                            value = true;
                        else if ((inputValue == "False") || (inputValue == "false"))
                            value = false;
                        else
                        {
                            value = false;

                            DeleteKeyByType(key, type);
                            Write(key, value);
                        }

                        break;
                    }
                case DataTypes.Vector3:
                    {
                        try
                        {
                            string[] vectorLine = inputValue.Substring(1, inputValue.Length - 2).Split(", ");
                            string[] vectorValues = new string[]
                            {
                                vectorLine[0].Replace(',', '.'),
                                vectorLine[1].Replace(',', '.'),
                                vectorLine[2].Replace(',', '.')
                            };

                            value = new Vector3(
                                float.Parse(vectorValues[0], CultureInfo.InvariantCulture),
                                float.Parse(vectorValues[1], CultureInfo.InvariantCulture),
                                float.Parse(vectorValues[2], CultureInfo.InvariantCulture));
                        }
                        catch (Exception exception)
                        {
                            value = Vector3.zero;

                            DeleteKeyByType(key, type);
                            Write(key, value);

                            Debug.LogWarning($"Exception from Get* methods with '{((key != null) ? key : "null")}' key and {type} type: {exception.Message}. " +
                                $"The key-value pair has been replaced with a pair with the default value for the data type of this pair. " +
                                $"This message appears only when you try to manually corrupt the save file content.");
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Returns the default value for the given data type.
        /// </summary>
        private static void GetDefaultValue(ref object value, DataTypes type)
        {
            switch (type)
            {
                case DataTypes.String:
                    value = "";
                    break;
                case DataTypes.Int:
                    value = 0;
                    break;
                case DataTypes.Float:
                    value = 0.0f;
                    break;
                case DataTypes.Bool:
                    value = false;
                    break;
                case DataTypes.Vector3:
                    value = Vector3.zero;
                    break;
            }
        }

        /// <summary>
        /// Returns key-value pair index if the given key with value of given data type exists, otherwise returns null.
        /// </summary>
        private static int? FindLineIndex(string key, DataTypes type)
        {
            CheckOrCreateSaveFile();

            try
            {
                List<string> lines = File.ReadAllLines(SaveFileName).ToList();

                foreach (string line in lines)
                {
                    string[] pair = line.Split(separator);

                    if (!IsPairCorrupted(pair))
                    {
                        if ((pair[1] == key) && (pair[3] == type.ToString()))
                            return lines.IndexOf(line);
                    }
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                Debug.LogWarning($"Exception from Set*, Get* or DeleteKey (by type) methods with '{((key != null) ? key : "null")}' key and {type} type: {exception.Message}. " +
                    $"Due to this exception, the method: " +
                    $"1. Creates a new pair with '{((key != null) ? key : "null")}' key and {type} type if Set*, " +
                    $"2. Returns default value for {type} type (and creates a default pair for the {type} type if createIfMissing parameter is true) if Get*, " +
                    $"3. Operation has no impact if DeleteKey (by type). " +
                    $"This message appears only when you try to manually corrupt the save file content.");
            }

            return null;
        }

        /// <summary>
        /// Opens and closes the FileStream and creates a save file if it does not exist.<br/>
        /// If the save file already exists, CheckOrCreateSaveFile has no impact.
        /// </summary>
        private static void CheckOrCreateSaveFile()
        {
            if (File.Exists(SaveFileName))
                return;

            using (FileStream file = new FileStream(SaveFileName, FileMode.OpenOrCreate)) { }
        }

        /// <summary>
        /// Returns the data type corresponding to the DataTypes enumeration for the given value.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// An exception can never be raised due solely to the internal workings of the method with passing the correct parameters.
        /// </remarks>
        private static DataTypes GetDataType(object value)
        {
            switch (value)
            {
                case string:
                    return DataTypes.String;
                case int:
                    return DataTypes.Int;
                case Single:
                    return DataTypes.Float;
                case bool:
                    return DataTypes.Bool;
                case Vector3:
                    return DataTypes.Vector3;
                default:
                    throw new ArgumentException($"Unsupported type: {value.GetType()}");
            }
        }

        /// <summary>
        /// Returns true if the given key-value pair is corrupted, otherwise returns false.<br/>
        /// Ensures that the key-value pair length is correct and that the data type is found in the pair.
        /// </summary>
        private static bool IsPairCorrupted(string[] pair)
        {
            List<string> types = new List<string>()
            {
                DataTypes.String.ToString(),
                DataTypes.Int.ToString(),
                DataTypes.Float.ToString(),
                DataTypes.Bool.ToString(),
                DataTypes.Vector3.ToString(),
            };

            if ((pair.Length == 5) && types.Contains(pair[3]))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Returns true if the save file name is valid and not null, otherwise returns false.
        /// </summary>
        private static bool IsValidSaveFileName(string saveFileName)
        {
            if (saveFileName == defaultSaveFileName)
                return true;

            if ((saveFileName != null) && (saveFileName != "") && (saveFileName != " "))
            {
                List<char> invalidChars = new List<char>() { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

                for (int i = 0; i < invalidChars.Count; i++)
                {
                    if (saveFileName.Contains(invalidChars[i]))
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region Utilities

        /// <summary>
        /// Provides escape and recovery capabilities for control sequences and Unicode characters.
        /// </summary>
        private static class StringEscapeUtility
        {
            /// <summary>
            /// Escape dictionary that contains control sequences and its string representations.
            /// </summary>
            private static readonly Dictionary<char, string> escapeMap = new Dictionary<char, string>
            {
                ['\n'] = "\\n",
                ['\r'] = "\\r",
                ['\t'] = "\\t",
                ['\b'] = "\\b",
                ['\f'] = "\\f",
                ['\a'] = "\\a",
                ['\v'] = "\\v",
                ['\0'] = "\\0",
                ['\''] = "\\'",
                ['\"'] = "\\\"",
                ['\\'] = "\\\\"
            };

            /// <summary>
            /// Reverse dictionary for recovery that contains string representations and its control sequences.
            /// </summary>
            private static readonly Dictionary<string, char> unescapeMap = new Dictionary<string, char>();

            static StringEscapeUtility()
            {
                foreach (KeyValuePair<char, string> pair in escapeMap)
                    unescapeMap[pair.Value] = pair.Key;
            }

            /// <summary>
            /// Returns escaped control characters and Unicode characters converted to \uXXXX.
            /// </summary>
            public static string Escape(string input, bool escapeNonAscii = false)
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                StringBuilder result = new StringBuilder();

                foreach (char c in input)
                {
                    if (escapeMap.TryGetValue(c, out string escaped))
                    {
                        result.Append(escaped);
                    }
                    else if (escapeNonAscii && (c > 0x7F))
                    {
                        result.Append($"\\u{(int)c:X4}");
                    }
                    else if (char.IsControl(c))
                    {
                        result.Append($"\\u{(int)c:X4}");
                    }
                    else
                    {
                        result.Append(c);
                    }
                }

                return result.ToString();
            }

            /// <summary>
            /// Returns restored escaped sequences, including Unicode characters.
            /// </summary>
            public static string Unescape(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                StringBuilder result = new StringBuilder();

                for (int i = 0; i < input.Length; i++)
                {
                    if ((input[i] == '\\') && (i + 1 < input.Length))
                    {
                        if ((input[i + 1] == 'u') && (i + 5 < input.Length))
                        {
                            string hex = input.Substring(i + 2, 4);
                            if (int.TryParse(hex, NumberStyles.HexNumber, null, out int codePoint))
                            {
                                result.Append((char)codePoint);
                                i += 5;
                                continue;
                            }
                        }

                        string escapeSequence = input.Substring(i, 2);
                        if (unescapeMap.ContainsKey(escapeSequence))
                        {
                            result.Append(unescapeMap[escapeSequence]);
                            i++;
                            continue;
                        }
                    }

                    result.Append(input[i]);
                }

                return result.ToString();
            }
        }

        #endregion
    }
}
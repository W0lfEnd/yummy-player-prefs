using System;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    public abstract class PlayerPrefsProviderBase : INotifyPropertyChanged
    {
        protected const string FirstLaunchName = "FirstLaunch";
        protected const string LaunchCountName = "LaunchCount";

        protected const string IntType = "Int";
        protected const string FloatType = "Float";
        protected const string StringType = "String";
        protected const string BooleanType = "Boolean";
        protected const string CharType = "Char";
        protected const string DateTimeType = "DateTime";
        protected const string ValueSuffix = "Value";
        protected const string TypeSuffix = "Type";

        public DeferredPlayerPrefsSaver DeferredPlayerPrefsSaver { get; set; }

        public int LaunchCount
        {
            get
            {
                if (!ContainsPref(LaunchCountName))
                {
                    FirstLaunch = DateTime.Now;
                }

                return (int) GetValueOrDefault(LaunchCountName, 0);
            }
            set { Save(LaunchCountName, value); }
        }

        public DateTime FirstLaunch
        {
            get 
            {
                var now = DateTime.Now;
                if (!ContainsPref(FirstLaunchName))
                {
                    Save(FirstLaunchName, now);
                }
                return (DateTime)GetValueOrDefault(FirstLaunchName, now);
            }
            private set { Save(FirstLaunchName, value); }
        }

#region Int

        private void SetInt(string propertyName, int value)
        {
            PlayerPrefs.SetInt(PropKey(propertyName), value);
            PlayerPrefs.SetString(TypeKey(propertyName), IntType);
        }

        private int LoadInt(string propertyName)
        {
            return PlayerPrefs.GetInt(PropKey(propertyName));
        }

#endregion

#region Float

        private void SetFloat(string propertyName, float value)
        {
            PlayerPrefs.SetFloat(PropKey(propertyName), value); 
            PlayerPrefs.SetString(TypeKey(propertyName), FloatType);
        }

        private float LoadFloat(string propertyName)
        {
            return PlayerPrefs.GetFloat(PropKey(propertyName));
        }

#endregion

#region String

        private void SetString(string propertyName, String value)
        {
            PlayerPrefs.SetString(PropKey(propertyName), value);
            PlayerPrefs.SetString(TypeKey(propertyName), StringType);
        }

        private string LoadString(string propertyName)
        {
            return PlayerPrefs.GetString(PropKey(propertyName));
        }

#endregion

#region Boolean

        private void SetBoolean(string propertyName, Boolean value)
        {
            PlayerPrefs.SetInt(PropKey(propertyName), value ? 1 : 0);
            PlayerPrefs.SetString(TypeKey(propertyName), BooleanType);
        }

        private bool LoadBoolean(string propertyName)
        {
            return PlayerPrefs.GetInt(PropKey(propertyName)) == 1;
        }

#endregion

#region Char

        private void SetChar(string propertyName, Char value)
        {
            PlayerPrefs.SetString(PropKey(propertyName), value.ToString());
            PlayerPrefs.SetString(TypeKey(propertyName), CharType);
        }

        private char LoadChar(string propertyName)
        {
            return PlayerPrefs.GetString(PropKey(propertyName))[0];
        }

#endregion

#region DateTime

        protected void SetDateTime(string propertyName, DateTime value)
        {
            string dateTime = value.ToString(CultureInfo.InvariantCulture);
            PlayerPrefs.SetString(PropKey(propertyName), dateTime);
            PlayerPrefs.SetString(TypeKey(propertyName), DateTimeType);
        }

        protected DateTime LoadDateTime(string propertyName)
        {
            return DateTime.Parse(PlayerPrefs.GetString(PropKey(propertyName)), CultureInfo.InvariantCulture);
        }

#endregion

        protected virtual void Save(string propertyName, object value)
        {
            if (value is Int16 || value is Int32)
            {
                SetInt(propertyName, (int) value);
            }
            if (value is float)
            {
                SetFloat(propertyName, (float) value);
            }
            if (value as string != null)
            {
                SetString(propertyName, (string) value);
            }
            if (value is bool)
            {
                SetBoolean(propertyName, (bool) value);
            }
            if (value is char)
            {
                SetChar(propertyName, (char) value);
            }
            if (value is DateTime)
            {
                SetDateTime(propertyName, (DateTime) value);
            }

            if (DeferredPlayerPrefsSaver)
                DeferredPlayerPrefsSaver.MarkDirty();
            else
                PlayerPrefs.Save();

        }

        protected virtual object GetValueOrDefault(string propertyName, object defaultValue)
        {
            string type = TypeKey(propertyName);
            if (ContainsPref(propertyName) && !String.IsNullOrEmpty(PlayerPrefs.GetString(type)))
            {
                string typeName = PlayerPrefs.GetString(type);
                if (typeName == IntType)
                {
                    return LoadInt(propertyName);
                }
                if (typeName == FloatType)
                {
                    return LoadFloat(propertyName);
                }
                if (typeName == StringType)
                {
                    return LoadString(propertyName);
                }
                if (typeName == BooleanType)
                {
                    return LoadBoolean(propertyName);
                }
                if (typeName == CharType)
                {
                    return LoadChar(propertyName);
                }
                if (typeName == DateTimeType)
                {
                    return LoadDateTime(propertyName);
                }
                Debug.LogError("GetValueOrDefault: typeName " + typeName + " is not supported.");
                return defaultValue;
            }
            return defaultValue;
        }

        protected bool ContainsPref(string propertyName)
        {
            var type = TypeKey(propertyName);
            return PlayerPrefs.HasKey(PropKey(propertyName)) && PlayerPrefs.HasKey(type);
        }

        protected void DeletePref(string propertyName)
        {
            PlayerPrefs.DeleteKey(PropKey(propertyName));
            PlayerPrefs.DeleteKey(TypeKey(propertyName));
        }

        protected string PropKey(string propertyName)
        {
            return propertyName + ValueSuffix;
        }

        protected string TypeKey(string propertyName)
        {
            var typeKey = "";
            typeKey += propertyName + TypeSuffix;
            return typeKey;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
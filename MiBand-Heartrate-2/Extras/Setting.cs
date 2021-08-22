using System;
using Microsoft.Win32;

namespace MiBand_Heartrate_2.Extras
{
    public static class Setting
    {
        const string regPath = @"Software\PAWZ\Heartrate";


        public static T Get<T>(string key, T defaultValue)
        {
            T result = defaultValue;
            
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(regPath);
                
                if (regKey != null)
                {
                    if (defaultValue is bool)
                    {
                        string value = (string)regKey.GetValue(key, defaultValue.ToString());
                        result = (T)(object)(value == "True");
                    }
                    else
                    {
                        result = (T)regKey.GetValue(key, defaultValue);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (regKey != null)
                {
                    regKey.Close();
                }
            }

            return result;
        }

        public static void Set(string key, object value, RegistryValueKind valueKind = RegistryValueKind.String)
        {
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(regPath, true);

                if (regKey == null)
                {
                    regKey = Registry.CurrentUser.CreateSubKey(regPath);
                }

                if (regKey != null)
                {
                    regKey.SetValue(key, value, valueKind);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (regKey != null)
                {
                    regKey.Close();
                }
            }
        }

        public static void Set(string key, int value)
        {
            Set(key, value, RegistryValueKind.DWord);
        }
    }
}

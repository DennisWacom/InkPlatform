using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using InkPlatform.Hardware;
using InkPlatform.Hardware.Wacom;
using InkPlatform.UserInterface;

namespace InkPlatform.Ink
{
    public delegate void SendLog(string msg, int alertType);

    public class ContextPenData
    {
        List<InkData> _penData;
        SerializablePenDevice _penDevice;
        Dictionary<string, string> _dictionary;
        Layout _layout;

        int _errorCode = 0;
        string _errorMessage = "";

        public SendLog LogFunction;

        public ContextPenData(int errorCode, string errorMessage)
        {
            _errorCode = errorCode;
            _errorMessage = errorMessage;
            _dictionary = new Dictionary<string, string>();
        }

        public ContextPenData(PenDevice penDevice, List<InkData> penData, Layout layout)
        {
            _penData = penData;
            _penDevice = penDevice.Serialize();
            _dictionary = new Dictionary<string, string>();
            _layout = layout;
        }

        public void Log(string msg)
        {
            Log(msg, 0);
        }

        public void Log(string msg, int alertType)
        {
            if(LogFunction != null)
            {
                LogFunction(msg, alertType);
            }
        }

        public override string ToString()
        {
            if(_errorCode != 0)
            {
                return _errorCode.ToString() + ":" + _errorMessage;
            }

            string result = "";
            
            string dict = "";
            foreach(string key in _dictionary.Keys)
            {
                try
                {
                    string value = "\"" + key + "\":\"" + _dictionary[key] + "\"";
                    if(dict.Length > 0)
                    {
                        dict = "," + dict + value;
                    }
                    else
                    {
                        dict = value;
                    }
                }
                catch (Exception ex)
                {
                    Log("Error converting dictionary - " + ex.Message);
                }
            }
            dict = "\"dictionary\":{" + dict + "}";

            string deviceString = "";
            try
            {
                deviceString = "\"PenDevice\":" + _penDevice.Serialize();
            }
            catch (Exception ex)
            {
                Log("Error converting device string - " + ex.Message);
            }

            string penDataString = "";
            try
            {
                penDataString = "\"PenData\":" + InkProcessor.SerializeInkDataListToJson(_penData);
            }
            catch (Exception ex)
            {
                Log("Error converting pen data - " + ex.Message);
            }

            string layoutString = "";
            try
            {
                layoutString = "\"Layout\":" + JSONSerializer.SerializeLayout(_layout);
            }
            catch (Exception ex)
            {
                Log("Error converting layout - " + ex.Message);
            }

            result = "{" + dict + "," + deviceString + "," + penDataString + "," + layoutString + "}";

            return result;
        }

        public int ErrorCode
        {
            get { return _errorCode; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public List<InkData> PenData
        {
            get { return _penData; }
        }

        public SerializablePenDevice PenDevice
        {
            get { return _penDevice; }
        }

        public Dictionary<string, string> Dictionary
        {
            get { return _dictionary; }
        }

        public bool AddData(string key, string value)
        {
            if (_dictionary.ContainsKey(key))
            {
                return false;
            }
            else
            {
                _dictionary[key] = value;
                return true;
            }
        }

        public bool RemoveData(string key)
        {
            if (_dictionary.ContainsKey(key))
            {
                return false;
            }
            else
            {
                return _dictionary.Remove(key);
            }
        }

        public string GetData(string key)
        {
            if (_dictionary.ContainsKey(key))
            {
                return _dictionary[key];
            }
            else
            {
                return null;
            }
        }
    }
}

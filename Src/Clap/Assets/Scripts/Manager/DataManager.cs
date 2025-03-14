using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace GameServer.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        internal string DataPath;
        public Dictionary<int, ItemDefine> Items = null;

        public DataManager()
        {
            DataPath = Path.Combine(Application.dataPath, "Common/Tables/Data/");
        }

        internal void Load()
        {
            string json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
            this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);
        }

        public IEnumerator LoadData()
        {
            string json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
            this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);
            yield return null;
        }
    }
}
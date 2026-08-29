using System;
using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UI框架View配置提供服务 ,组合优于继承，不把所有的东西塞到UIManager中
    /// </summary>
    public class ViewConfigServices
    {
        private Dictionary<System.Type, string> type2ConfigIDDic = new Dictionary<System.Type, string>();
        private Dictionary<string, System.Type> viewConfigName2TypeDic = new Dictionary<string, System.Type>();
        private Dictionary<string, ViewConfig> cfgID2ConfigDic = new Dictionary<string, ViewConfig>();
        public ViewConfigServices()
        {
        }

        private ViewConfig GetViewConfigByCfgID(string inCfgID)
        {
            if (this.cfgID2ConfigDic.TryGetValue(inCfgID, out var config))
            {
                return config;
            }
            return new ViewConfig();
        }

        public ViewConfig GetViewConfig<T>() where T : View
        {
            var type = typeof(T);
            return this.GetViewConfigByType(type);
        }

        public ViewConfig GetViewConfig(View inView)
        {
            return this.GetViewConfigByType(inView.GetType());
        }

        public ViewConfig GetViewConfigByType(System.Type inType)
        {
            if (this.type2ConfigIDDic.TryGetValue(inType, out var configName))
            {
                return this.GetViewConfigByCfgID(configName);
            }
            return null;
        }

        public ViewConfig GetViewConfig(string inViewName)
        {
            if (this.viewConfigName2TypeDic.TryGetValue(inViewName, out var type))
            {
                return this.GetViewConfigByType(type);
            }
            return null;
        }

        public Type GetViewType(string inCfgID)
        {
            if (this.viewConfigName2TypeDic.TryGetValue(inCfgID, out var type))
            {
                return type;
            }
            return null;
        }

        internal void AddViewConfig(Type item1, string item2, ViewConfig item3)
        {
            this.type2ConfigIDDic.Add(item1, item2);
            this.viewConfigName2TypeDic.Add(item2, item1);
            this.cfgID2ConfigDic.Add(item2, item3);
        }
    }
}
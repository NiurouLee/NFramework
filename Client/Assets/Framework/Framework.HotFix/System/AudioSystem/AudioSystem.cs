using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using NFramework;
using  NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 声音管理器
    /// </summary>
    public class AudioSystem : FrameworkSystemModuleBase, IRendererUpdateSystem
    {
        private GameObject _gameObject;

        #region 字段与属性

        /// <summary>
        /// 所有声音组的字典
        /// </summary>
        private readonly Dictionary<string, AudioGroup> _audioGroups = new();

        /// <summary>
        /// 正在加载的声音
        /// </summary>
        private readonly List<int> _audiosBeingLoaded = new();

        /// <summary>
        /// 需要在加载后释放的声音
        /// </summary>
        private readonly HashSet<int> _audiosToReleaseOnLoad = new();

        /// <summary>
        /// 序列编号
        /// </summary>
        private int _serial = 0;

        #endregion

        #region 生命周期

        public override void Awake()
        {
            _gameObject = new GameObject("AudioManager");
            _gameObject.GetOrAddComponent<AudioListener>();

            this.Log?.Print("AudioManager初始化");
            // AudioGroup 的初始化移到 InitAudioGroups()，由热更层调用
        }

        /// <summary>
        /// 初始化音频组（由热更层调用）
        /// </summary>
        /// <param name="groupConfigs">音频组配置列表</param>
        public void InitAudioGroups(List<AudioGroupConfig> groupConfigs)
        {
            if (groupConfigs == null || groupConfigs.Count == 0)
            {
                this.Warning?.Print("AudioGroupConfigs is null or empty");
                return;
            }

            for (int i = 0; i < groupConfigs.Count; i++)
            {
                AudioGroupConfig config = groupConfigs[i];
                if (!AddAudioGroup(config.Name, config.AgentCount, config.AvoidBeingReplacedBySamePriority, config.Mute,
                        config.Volume))
                {
                    this.Warning?.Print("Add audio group '{0}' failure.", config.Name);
                }
            }

            this.Log?.Print("AudioManager 音频组初始化完成，共 {0} 个组", groupConfigs.Count);
        }

        /// <summary>
        /// 关闭并清理声音管理器
        /// </summary>
        public override void Destroy()
        {
            StopAllLoadedAudios();
            _audioGroups.Clear();
            _audiosBeingLoaded.Clear();
            _audiosToReleaseOnLoad.Clear();
        }

        public void RendererUpdate(float deltaTime)
        {
            foreach (AudioGroup audioGroup in _audioGroups.Values)
            {
                audioGroup.Update();
            }
        }

        #endregion

        #region 声音组相关方法

        /// <summary>
        /// 是否存在指定声音组
        /// </summary>
        /// <param name="audioGroupName">声音组名称</param>
        /// <returns>指定声音组是否存在</returns>
        private bool HasAudioGroup(string audioGroupName)
        {
            if (string.IsNullOrEmpty(audioGroupName))
            {
                this.Error?.Print("要检查是否存在的声音组名称为空");
                return false;
            }

            return _audioGroups.ContainsKey(audioGroupName);
        }

        /// <summary>
        /// 获取指定声音组
        /// </summary>
        /// <param name="audioGroupName">声音组名称</param>
        /// <returns>要获取的声音组</returns>
        public AudioGroup GetAudioGroup(string audioGroupName)
        {
            if (string.IsNullOrEmpty(audioGroupName))
            {
                this.Error?.Print("要获取的声音组名称为空");
                return null;
            }

            return _audioGroups.GetValueOrDefault(audioGroupName);
        }


        /// <summary>
        /// 增加声音组
        /// </summary>
        /// <param name="audioGroupName">声音组名称</param>
        /// <param name="audioAgentCount">声音代理数量</param>
        /// <param name="audioGroupAvoidBeingReplacedBySamePriority">声音组中的声音是否避免被同优先级声音替换</param>
        /// <param name="audioGroupMute">声音组是否静音</param>
        /// <param name="audioGroupVolume">声音组音量</param>
        /// <returns>是否增加声音组成功</returns>
        private bool AddAudioGroup(string audioGroupName, int audioAgentCount,
            bool audioGroupAvoidBeingReplacedBySamePriority = false, bool audioGroupMute = false,
            float audioGroupVolume = 1f)
        {
            if (HasAudioGroup(audioGroupName))
            {
                this.Error?.Print("要增加的声音组已存在：" + audioGroupName);
                return false;
            }

            if (string.IsNullOrEmpty(audioGroupName))
            {
                this.Error?.Print("要增加的声音组名称为空");
                return false;
            }

            //创建声音组
            AudioGroup audioGroup = new AudioGroup(audioGroupName, _gameObject.transform)
            {
                AvoidBeingReplacedBySamePriority = audioGroupAvoidBeingReplacedBySamePriority,
                Mute = audioGroupMute,
                Volume = audioGroupVolume
            };
            _audioGroups.Add(audioGroupName, audioGroup);

            //添加声音代理
            for (int i = 0; i < audioAgentCount; i++)
            {
                audioGroup.AddAudioAgent(i);
            }

            return true;
        }

        #endregion

        #region 声音相关方法

        /// <summary>
        /// 是否正在加载声音
        /// </summary>
        /// <param name="serialId">声音序列编号</param>
        /// <returns>是否正在加载声音</returns>
        private bool IsLoadingAudio(int serialId)
        {
            return _audiosBeingLoaded.Contains(serialId);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <param name="audioGroupName">声音组名称</param>
        /// <param name="priority">资源优先级</param>
        /// <param name="playAudioParams">播放声音参数</param>
        /// <returns>声音的序列编号</returns>
        public async UniTask<int?> PlayAudio(string audioAssetName, string audioGroupName, uint priority,
            PlayAudioParams playAudioParams = null)
        {
            if (playAudioParams == null)
            {
                playAudioParams = PlayAudioParams.Create();
            }

            int serialId = _serial++;
            //获取声音组
            AudioGroup audioGroup = GetAudioGroup(audioGroupName);
            //加载声音
            _audiosBeingLoaded.Add(serialId);
            AudioClip audioClip = NFROOT.I.GetSystem<ResSystem>().LoadAsync<AudioClip>(audioAssetName, priority)
                .AssetObject;
            //加载结果
            if (audioClip)
            {
                _audiosBeingLoaded.Remove(serialId);
                if (_audiosToReleaseOnLoad.Contains(serialId))
                {
                    this.Error?.Print(string.Format("需要释放的声音：{0} 加载成功", serialId));
                    _audiosToReleaseOnLoad.Remove(serialId);
                    NFROOT.I.GetSystem<ResSystem>().Free(audioClip);
                    playAudioParams.FreeToPool();
                    return null;
                }

                //播放声音
                AudioAgent audioAgent = audioGroup.PlayAudio(serialId, audioClip, playAudioParams);

                if (audioAgent == null)
                {
                    //绑定的实体或位置
                    _audiosToReleaseOnLoad.Remove(serialId);
                    NFROOT.I.GetSystem<ResSystem>().Free(audioClip);
                }
            }
            else
            {
                _audiosToReleaseOnLoad.Remove(serialId);
                this.Error?.Print("播放声音：{0} 失败，错误信息：{1}", audioAssetName);
                NFROOT.I.GetSystem<ResSystem>().Free(audioClip);
                playAudioParams.FreeToPool();
                return null;
            }

            playAudioParams.FreeToPool();
            return serialId;
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="serialId">要暂停播放声音的序列编号</param>
        public void PauseAudio(int serialId)
        {
            foreach (AudioGroup audioGroup in _audioGroups.Values)
            {
                if (audioGroup.PauseAudio(serialId))
                {
                    return;
                }
            }

            this.Error?.Print("没找到要暂停的声音：" + serialId);
        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="serialId">要恢复播放声音的序列编号</param>
        public void ResumeAudio(int serialId)
        {
            foreach (AudioGroup audioGroup in _audioGroups.Values)
            {
                if (audioGroup.ResumeAudio(serialId))
                {
                    return;
                }
            }

            this.Error?.Print("没找到要恢复的声音：" + serialId);
        }

        #region 停止播放声音

        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="serialId">要停止播放声音的序列编号</param>
        /// <returns>是否停止播放声音成功</returns>
        public bool StopAudio(int serialId)
        {
            if (IsLoadingAudio(serialId))
            {
                _audiosToReleaseOnLoad.Add(serialId);
                return true;
            }

            foreach (AudioGroup audioGroup in _audioGroups.Values)
            {
                if (audioGroup.StopAudio(serialId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 停止所有已加载的声音
        /// </summary>
        public void StopAllLoadedAudios()
        {
            foreach (AudioGroup audioGroup in _audioGroups.Values)
            {
                audioGroup.StopAllLoadedAudios();
            }
        }

        /// <summary>
        /// 停止所有正在加载的声音
        /// </summary>
        public void StopAllLoadingAudios()
        {
            foreach (int serialId in _audiosBeingLoaded)
            {
                _audiosToReleaseOnLoad.Add(serialId);
            }
        }

        #endregion

        #endregion
    }
}
using System;
using System.Text;
using UnityEngine;

namespace NFramework.ModuleSystem.Combat
{
    /// <summary>
    ///  游戏标签 - 类似于虚幻引擎的GamePlay tag
    ///  支持层级结构，例如“Ability.Attack.Melee"
    /// </summary>
    [Serializable]
    public struct GamePlayTag : IEquatable<GamePlayTag>
    {
        [SerializeField] private string _name;
        [SerializeField] private int _hashCode;
        [SerializeField] private string _shortName;
        [SerializeField] private int[] _ancestorHashCodes;
        [SerializeField] private string[] _ancestorNames;

        /// <summary>
        /// 完整标签
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// 标签hash值，用于快速
        /// </summary>
        public int HashCode => _hashCode;

        /// <summary>
        /// 短名字 （最后一级）如Melee
        /// </summary>
        public string ShortName => _shortName;

        /// <summary>
        /// 祖先标签hash值
        /// </summary>
        public int[] AncestorHashCodes => _ancestorHashCodes;

        /// <summary>
        /// 祖先标签名字
        /// </summary>
        public string[] AncestorNames => _ancestorNames;

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid => !IsEmpty;

        /// <summary>
        /// 检查是否为空（于IsValid）相反
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(_name);

        /// <summary>
        /// 深度 - 有多少级祖先 
        /// </summary>
        public int Depth => _ancestorNames?.Length ?? 0;

        /// <summary>
        /// 空标签
        /// </summary>
        public static GamePlayTag None => new GamePlayTag(string.Empty);


        public GamePlayTag(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                _name = string.Empty;
                _hashCode = 0;
                _shortName = string.Empty;
                _ancestorHashCodes = Array.Empty<int>();
                _ancestorNames = Array.Empty<string>();
                return;
            }
            _name = name;
            _hashCode = name.GetHashCode();
            //解析层级结构
            var parts = name.Split('.');
            _shortName = parts[parts.Length - 1];
            // 构建祖先列表（不包含自身）：等价于遍历 parts[0..^1]；此处用计数+索引避免 Unity 下 AsSpan 的程序集歧义
            int ancestorCount = parts.Length - 1;
            if (ancestorCount > 0)
            {
                _ancestorHashCodes = new int[ancestorCount];
                _ancestorNames = new string[ancestorCount];

                var pathBuilder = new StringBuilder(name.Length);
                for (int i = 0; i < ancestorCount; i++)
                {
                    if (i > 0)
                    {
                        pathBuilder.Append('.');
                    }

                    pathBuilder.Append(parts[i]);
                    string path = pathBuilder.ToString();
                    _ancestorNames[i] = path;
                    _ancestorHashCodes[i] = path.GetHashCode();
                }
            }
            else
            {
                _ancestorHashCodes = Array.Empty<int>();
                _ancestorNames = Array.Empty<string>();
            }
        }

        //// <summary>
        /// 检查是否拥有指定标签（支持层级匹配）
        /// 例如标签”Ability.Attack.Melee"拥有“Ability”，“Ability.Attack" ,"Ability,Attack,Melee"
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(GamePlayTag tag)
        {
            if (!tag.IsValid) return false;
            if (!IsValid) return false;
            //完全匹配
            if (_hashCode == tag.HashCode) return true;
            //检查是否是祖先
            if (_ancestorHashCodes != null)
            {
                for (int i = 0; i < _ancestorHashCodes.Length; i++)
                {
                    if (_ancestorHashCodes[i] == tag.HashCode) return true;
                }
            }
            return false;
        }

        /// <summary>   
        ///检查是否是一个标签的后代
        /// <summary>
        public bool IsDescendantOf(GamePlayTag other)
        {
            if (!other.IsValid) return false;
            if (!IsValid) return false;
            //完全匹配
            return HasTag(other) && _hashCode != other.HashCode;
        }

        /// <summary>
        /// 检查是否是另一个标签的祖先
        /// </summary>
        /// <returns></returns>
        public bool IsAncestorOf(GamePlayTag other)
        {
            return other.IsDescendantOf(this);
        }

        /// <summary>
        /// 获取父标签
        /// </summary>
        /// <returns></returns>
        public GamePlayTag GetParent()
        {
            if (_ancestorNames != null || _ancestorNames.Length == 0)
            {
                return None;
            }
            return new GamePlayTag(_ancestorNames[_ancestorNames.Length - 1]);
        }

        #region Operators

        public bool Equals(GamePlayTag other)
        {
            return _hashCode == other._hashCode;
        }
        public override bool Equals(object obj)
        {
            return obj is GamePlayTag tag && Equals(tag);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static bool operator ==(GamePlayTag left, GamePlayTag right)
        {
            return left._hashCode == right._hashCode;
        }

        public static bool operator !=(GamePlayTag left, GamePlayTag right)
        {
            return left._hashCode != right._hashCode;
        }

        #endregion
        public override string ToString()
        {
            return _name;
        }

        public static implicit operator GamePlayTag(string name)
        {
            return new GamePlayTag(name);
        }

        public static implicit operator string(GamePlayTag tag)
        {
            return tag._name;
        }

    }
}
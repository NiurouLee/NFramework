using System;
using System.Collections.Generic;
using UnityEngine;

namespace NFramework.ModuleSystem.Combat
{
    /// <summary>
    /// 游戏标签容器- 可变标签集合（带引用计数）
    /// 用于游戏运行时动态添加/移除标签
    /// </summary>
    [Serializable]
    public class GamePlayTagContainer
    {

        [SerializeField]
        private List<GamePlayTag> _tags = new List<GamePlayTag>();
        /// <summary>
        /// 标签引用计数
        /// </summary>
        public Dictionary<int, int> _tagCounts = new Dictionary<int, int>();

        /// <summary>
        /// 标签变化事件(通用)
        /// </summary>
        public event Action OnTagChanged;


        /// <summary>
        /// 标签首次添加事件， 计数0->1时触发
        /// </summary>
        public event Action<GamePlayTag> OnTagAdded;

        public event Action<GamePlayTag> OnTagRemoved;

        public event Action<GamePlayTag, int, int> OnTagCountChanged;

        public IReadOnlyList<GamePlayTag> Tags => _tags;

        public int Count => Tags.Count;

        public bool IsEmpty => _tags.Count == 0;

        public int GetTagCount(GamePlayTag tag)
        {
            if (!tag.IsValid)
            {
                return 0;
            }
            if (_tagCounts.TryGetValue(tag.HashCode, out int count))
            {
                return count;
            }
            return 0;
        }
        public void AddTag(GamePlayTag tag)
        {
            if (!tag.IsValid) return;
            int hashCode = tag.HashCode;
            int oldCount = _tagCounts.TryGetValue(hashCode, out int count) ? count : 0;
            var newCount = oldCount + 1;
            _tagCounts[hashCode] = newCount;
            if (oldCount == 0)
            {
                _tags.Add(tag);
                OnTagAdded?.Invoke(tag);
            }
            OnTagCountChanged?.Invoke(tag, oldCount, newCount);
            OnTagChanged?.Invoke();
        }

        public void AddTags(GamePlayTagSet tagSet)
        {
            if (tagSet.IsEmpty) return;
            for (int i = 0; i < tagSet.Count; i++)
            {
                AddTag(tagSet[i]);
            }
        }

        public bool RemoveTag(GamePlayTag tag)
        {
            if (!tag.IsValid) return false;
            int hashCode = tag.GetHashCode();
            if (!_tagCounts.TryGetValue(hashCode, out int oldCount) || oldCount <= 0) return false;
            int newCount = oldCount - 1;
            if (newCount <= 0)
            {
                _tagCounts.Remove(hashCode);
                for (int i = _tags.Count - 1; i >= 0; i--)
                {
                    if (_tags[i].HashCode == hashCode)
                    {
                        _tags.RemoveAt(i);
                        break;
                    }
                }
                OnTagRemoved?.Invoke(tag);
            }
            else
            {
                _tagCounts[hashCode] = newCount;
            }
            OnTagCountChanged?.Invoke(tag, oldCount, newCount);
            OnTagChanged?.Invoke();
            return true;
        }

        public void RemoveTags(GamePlayTagSet tagSet)
        {
            if (tagSet.IsEmpty) return;
            for (int i = 0; i < tagSet.Count; i++)
            {
                RemoveTag(tagSet[i]);
            }
        }

        public void Clear()
        {
            if (_tags.Count > 0)
            {
                foreach (var tag in _tags)
                {
                    OnTagRemoved?.Invoke(tag);
                }
                _tags.Clear();
                _tagCounts.Clear();
                OnTagChanged?.Invoke();
            }
        }

        public bool HasTag(GamePlayTag tag)
        {
            if (!tag.IsValid || IsEmpty) return false;
            for (int i = 0; i < _tags.Count; i++)
            {
                if (_tags[i].HasTag(tag))
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasTagExact(GamePlayTag tag)
        {
            if (!tag.IsValid || IsEmpty) return false;
            for (int i = 0; i < _tags.Count; i++)
            {
                if (_tags[i] == tag)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasAllTags(GamePlayTagSet other)
        {
            if (other.IsEmpty) return true;
            if (IsEmpty) return false;

            for (int i = 0; i < other.Count; i++)
            {
                if (!HasTag(other.Tags[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasAnyTags(GamePlayTagSet other)
        {
            if (this.IsEmpty || other.IsEmpty) return false;
            for (int i = 0; i < other.Count; i++)
            {
                if (HasTag(other.Tags[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasNoneTags(GamePlayTagSet other)
        {
            return !HasAnyTags(other);
        }

        public GamePlayTagSet ToTagSet()
        {
            return new GamePlayTagSet(_tags);
        }

        public void SetFormTagSet(GamePlayTagSet tagSet)
        {
            Clear();
            if (!tagSet.IsEmpty)
            {
                foreach (var tag in tagSet.Tags)
                {
                    AddTag(tag);
                }
            }
        }

        public override string ToString()
        {
            if (IsEmpty) return "[]";
            return $"[{string.Join(", ", _tags.ConvertAll(tag => tag.ToString()))}]";
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NFramework.Core;

namespace NFramework.ModuleSystem.Combat
{
    public struct GamePlayTagSet : IEquatable<GamePlayTagSet>
    {

        [UnityEngine.SerializeField]
        private GamePlayTag[] _tags;
        public GamePlayTag[] Tags => _tags;

        public int Count => _tags?.Length ?? 0;
        public bool IsEmpty => _tags == null || _tags.Length == 0;

        public static GamePlayTagSet Empty => new GamePlayTagSet(Array.Empty<GamePlayTag>());


        public GamePlayTagSet(params string[] tagNames)
        {
            if (tagNames == null || tagNames.Length == 0)
            {
                _tags = Array.Empty<GamePlayTag>();
                return;
            }

            _tags = new GamePlayTag[tagNames.Length];
            for (int i = 0; i < tagNames.Length; i++)
            {
                Tags[i] = new GamePlayTag(tagNames[i]);

            }
        }

        public GamePlayTagSet(IList<GamePlayTag> tags)
        {
            _tags = tags?.ToArray() ?? Array.Empty<GamePlayTag>();
        }

        public bool HasTag(GamePlayTag tag)
        {
            if (!tag.IsValid || this.IsEmpty)
            {
                return false;
            }
            for (int i = 0; i < _tags.Length; i++)
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
            if (!tag.IsValid || this.IsEmpty)
            {
                return false;
            }
            for (int i = 0; i < _tags.Length; i++)
            {
                if (_tags[i].HashCode == tag.HashCode)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasAllTags(GamePlayTagSet other)
        {
            if (other.IsEmpty) return true;
            if (this.IsEmpty) return false;
            for (int i = 0; i < other._tags.Length; i++)
            {
                if (!HasTag(other._tags[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasAnyTags(GamePlayTagSet other)
        {
            if (other.IsEmpty || IsEmpty) return false;
            for (int i = 0; i < other._tags.Length; i++)
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

        /// <summary>
        /// 交集 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public GamePlayTagSet Intersect(GamePlayTagSet other)
        {
            if (IsEmpty || other.IsEmpty) return Empty;

            var result = new StructList<GamePlayTag>();
            GamePlayTagSet resultSet = Empty;
            using (result)
            {
                for (int i = 0; i < _tags.Length; i++)
                {
                    if (other.HasTagExact(Tags[i]))
                    {
                        result.Add(Tags[i]);
                    }
                }
                resultSet = new GamePlayTagSet(result);
            }
            return resultSet;
        }

        /// <summary>
        /// 并集
        /// </summary>
        public GamePlayTagSet Union(GamePlayTagSet other)
        {
            if (this.IsEmpty) return other;
            if (other.IsEmpty) return this;
            var result = new StructList<GamePlayTag>(_tags);

            GamePlayTagSet resultSet = Empty;
            using (result)
            {
                for (int i = 0; i < _tags.Length; i++)
                {
                    if (!this.HasTagExact(other.Tags[i]))
                    {
                        result.Add(other.Tags[i]);
                    }
                }
            }
            return resultSet;
        }


        /// <summary>
        /// 差集合
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public GamePlayTagSet Except(GamePlayTagSet other)
        {
            if (this.IsEmpty) return Empty;
            if (other.IsEmpty) return this;
            var result = new StructList<GamePlayTag>(_tags);
            GamePlayTagSet resultSet = Empty;
            using (result)
            {
                for (int i = 0; i < _tags.Length; i++)
                {
                    if (!other.HasTagExact(Tags[i]))
                    {
                        result.Add(Tags[i]);
                    }
                }
                resultSet = new GamePlayTagSet(result);
            }
            return resultSet;
        }

        public GamePlayTagSet AddTag(GamePlayTag tag)
        {
            if (!tag.IsValid) return this;
            if (HasTagExact(tag)) return this;

            var result = new StructList<GamePlayTag>(_tags);
            GamePlayTagSet resultSet = Empty;
            using (result)
            {
                result.Add(tag);
                resultSet = new GamePlayTagSet(result);
            }
            return resultSet;
        }

        public GamePlayTagSet RemoveTag(GamePlayTag tag)
        {
            if (!tag.IsValid || this.IsEmpty) return this;
            if (!HasTagExact(tag)) return this;
            var result = new StructList<GamePlayTag>(_tags);
            GamePlayTagSet resultSet = Empty;
            using (result)
            {
                for (int i = 0; i < _tags.Length; i++)
                {
                    if (_tags[i].HashCode != tag.HashCode)
                    {
                        result.Add(Tags[i]);
                        break;
                    }
                }
                resultSet = new GamePlayTagSet(result);
            }
            return resultSet;
        }

        public bool Equals(GamePlayTagSet other)
        {
            if (Count != other.Count) return false;
            if (IsEmpty && other.IsEmpty) return true;

            for (int i = 0; i < Count; i++)
            {
                if (!HasTagExact(other.Tags[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(object obj)
        {
            return obj is GamePlayTagSet other && Equals(other);
        }

        public override int GetHashCode()
        {
            if (IsEmpty)
            {
                return 0;
            }
            int hash = 17;
            for (int i = 0; i < Count; i++)
            {
                hash = hash * 31 + _tags[i].GetHashCode();
            }
            return hash;
        }

        public static bool operator ==(GamePlayTagSet left, GamePlayTagSet right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(GamePlayTagSet left, GamePlayTagSet right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            if (IsEmpty) return "[]";
            return $"[{string.Join(", ", _tags.Select(tag => tag.ToString()))}]";
        }

        public GamePlayTag this[int index] => _tags[index];
    }



}
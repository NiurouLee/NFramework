using System;

namespace  NFramework.Core
{
    [Serializable]
    public struct BitField32 : IEquatable<BitField32>
    {
        public static int MinBitCount = 0;
        public static int MidBitCount = 16;
        public static int MaxBitCount = 32;
        public static uint HighMask = 0xFFFF0000;
        public static uint LowMask = 0x0000FFFF;
        private uint _value;

        #region 构造函数
        public BitField32(uint value = 0)
        {
            _value = value;
        }

        public BitField32(ushort lowSkill, ushort highSkill)
        {
            _value = ((uint)highSkill << MidBitCount) | lowSkill;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置低16位技能值
        /// </summary>
        public ushort Low
        {
            get => (ushort)(_value & LowMask);
            set => _value = (_value & HighMask) | value;
        }

        /// <summary>
        /// 获取或设置高16位技能值
        /// </summary>
        public ushort High
        {
            get => (ushort)(_value >> MidBitCount);
            set => _value = (_value & LowMask) | ((uint)value << MidBitCount);
        }

        /// <summary>
        /// 获取完整的32位值
        /// </summary>
        public uint Value => _value;

        #endregion

        #region 位操作方法

        /// <summary>
        /// 获取指定位置的技能状态
        /// </summary>
        /// <param name="position">位置 (0-31)</param>
        /// <returns>技能状态</returns>
        public bool GetBit(int position)
        {
            if (position < MinBitCount || position >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be between 0 and 31");

            return (_value & (1u << position)) != 0;
        }

        /// <summary>
        /// 设置指定位置的技能状态
        /// </summary>
        /// <param name="position">位置 (0-31)</param>
        /// <param name="value">要设置的状态</param>
        public void SetBit(int position, bool value)
        {
            if (position < MinBitCount || position >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be between 0 and 31");

            if (value)
                _value |= (1u << position);
            else
                _value &= ~(1u << position);
        }

        /// <summary>
        /// 判断
        /// </summary>
        public bool Has(int skillId)
        {
            if (skillId < MinBitCount || skillId >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(skillId));

            return GetBit(skillId);
        }

        /// <summary>
        /// 标记
        /// </summary>
        public void Learn(int skillId)
        {
            if (skillId < MinBitCount || skillId >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(skillId));

            SetBit(skillId, true);
        }

        /// <summary>
        /// 遗忘
        /// </summary>
        public void Forget(int skillId)
        {
            if (skillId < MinBitCount || skillId >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(skillId));

            SetBit(skillId, false);
        }

        /// <summary>
        /// 清除所有低位技能（0-15位）
        /// </summary>
        public void ClearLowSkills()
        {
            _value &= HighMask;
        }

        /// <summary>
        /// 清除所有高位技能（16-31位）
        /// </summary>
        public void ClearHighSkills()
        {
            _value &= LowMask;
        }

        /// <summary>
        /// 清除所有技能
        /// </summary>
        public void Clear()
        {
            _value = 0;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 获取指定范围
        /// </summary>
        public uint GetRange(int startBit, int endBit)
        {
            if (startBit < MinBitCount || endBit >= MaxBitCount || startBit > endBit)
                throw new ArgumentException("Invalid range");

            int length = endBit - startBit + 1;
            uint mask = ((1u << length) - 1) << startBit;
            return (_value & mask) >> startBit;
        }

        /// <summary>
        /// 设置
        /// </summary>
        public void SetRange(int startBit, int endBit, uint value)
        {
            if (startBit < MinBitCount || endBit >= MaxBitCount || startBit > endBit)
                throw new ArgumentException("Invalid range");

            int length = endBit - startBit + 1;
            uint mask = ((1u << length) - 1) << startBit;
            _value = (_value & ~mask) | ((value << startBit) & mask);
        }

        #endregion

        #region 运算符重载

        public static implicit operator uint(BitField32 field32) => field32._value;
        public static explicit operator BitField32(uint value) => new BitField32(value);

        public static bool operator ==(BitField32 left, BitField32 right)
            => left._value == right._value;

        public static bool operator !=(BitField32 left, BitField32 right)
            => left._value != right._value;


        public override int GetHashCode()
            => _value.GetHashCode();

        public override string ToString()
            => $"LowSkill: 0x{Low:X4}, HighSkill: 0x{High:X4}";

        public bool Equals(BitField32 other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is BitField32 other && Equals(other);
        }

        #endregion
    }
}
using System;

namespace  NFramework.Core
{
    [Serializable]
    public struct BitField64 : IEquatable<BitField64>
    {
        public static int MinBitCount = 0;
        public static int MidBitCount = 32;
        public static int MaxBitCount = 64;
        public static ulong HighMask = 0xFFFFFFFF00000000UL;
        public static ulong LowMask = 0x00000000FFFFFFFFUL;
        private ulong _value;

        #region 构造函数
        public BitField64(ulong value = 0)
        {
            _value = value;
        }

        public BitField64(uint lowBits, uint highBits)
        {
            _value = ((ulong)highBits << MidBitCount) | lowBits;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置低32位值
        /// </summary>
        public uint Low
        {
            get => (uint)(_value & LowMask);
            set => _value = (_value & HighMask) | value;
        }

        /// <summary>
        /// 获取或设置高32位值
        /// </summary>
        public uint High
        {
            get => (uint)((_value & HighMask) >> MidBitCount);
            set => _value = (_value & LowMask) | ((ulong)value << MidBitCount);
        }

        /// <summary>
        /// 获取完整的64位值
        /// </summary>
        public ulong Value => _value;

        #endregion

        #region 位操作方法

        /// <summary>
        /// 获取指定位置的位状态
        /// </summary>
        /// <param name="position">位置 (0-63)</param>
        /// <returns>位状态</returns>
        public bool GetBit(int position)
        {
            if (position < MinBitCount || position >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be between 0 and 63");

            return (_value & (1UL << position)) != 0;
        }

        /// <summary>
        /// 设置指定位置的位状态
        /// </summary>
        /// <param name="position">位置 (0-63)</param>
        /// <param name="value">要设置的状态</param>
        public void SetBit(int position, bool value)
        {
            if (position < MinBitCount || position >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be between 0 and 63");

            if (value)
                _value |= (1UL << position);
            else
                _value &= ~(1UL << position);
        }

        /// <summary>
        /// 判断
        /// </summary>
        public bool Has(int bitId)
        {
            if (bitId < MinBitCount || bitId >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(bitId));

            return GetBit(bitId);
        }

        /// <summary>
        /// 标记
        /// </summary>
        public void Learn(int bitId)
        {
            if (bitId < MinBitCount || bitId >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(bitId));

            SetBit(bitId, true);
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Forget(int bitId)
        {
            if (bitId < MinBitCount || bitId >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(bitId));

            SetBit(bitId, false);
        }

        /// <summary>
        /// 清除所有低位（0-31位）
        /// </summary>
        public void ClearLowBits()
        {
            _value &= HighMask;
        }

        /// <summary>
        /// 清除所有高位（32-63位）
        /// </summary>
        public void ClearHighBits()
        {
            _value &= LowMask;
        }

        /// <summary>
        /// 清除所有位
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
        public ulong GetRange(int startBit, int endBit)
        {
            if (startBit < MinBitCount || endBit >= MaxBitCount || startBit > endBit)
                throw new ArgumentException("Invalid range");

            int length = endBit - startBit + 1;
            ulong mask = ((1UL << length) - 1) << startBit;
            return (_value & mask) >> startBit;
        }

        /// <summary>
        /// 设置指定范围
        /// </summary>
        public void SetRange(int startBit, int endBit, ulong value)
        {
            if (startBit < MinBitCount || endBit >= MaxBitCount || startBit > endBit)
                throw new ArgumentException("Invalid range");

            int length = endBit - startBit + 1;
            ulong mask = ((1UL << length) - 1) << startBit;
            _value = (_value & ~mask) | ((value << startBit) & mask);
        }

        #endregion

        #region 运算符重载

        public static implicit operator ulong(BitField64 field64) => field64._value;
        public static explicit operator BitField64(ulong value) => new BitField64(value);

        public static bool operator ==(BitField64 left, BitField64 right)
            => left._value == right._value;

        public static bool operator !=(BitField64 left, BitField64 right)
            => left._value != right._value;

        public override int GetHashCode()
            => _value.GetHashCode();

        public override string ToString()
            => $"Low: 0x{Low:X8}, High: 0x{High:X8}, Value: 0x{Value:X16}";

        public bool Equals(BitField64 other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is BitField64 other && Equals(other);
        }

        #endregion
    }
}


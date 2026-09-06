using System;

namespace  NFramework.Core
{
    [Serializable]
    public struct BitField16 : IEquatable<BitField16>
    {
        public static int MinBitCount = 0;
        public static int MidBitCount = 8;
        public static int MaxBitCount = 16;
        public static ushort HighMask = 0xFF00;
        public static ushort LowMask = 0x00FF;
        private ushort _value;

        #region 构造函数
        public BitField16(ushort value = 0)
        {
            _value = value;
        }

        public BitField16(byte lowBits, byte highBits)
        {
            _value = (ushort)(((ushort)highBits << MidBitCount) | lowBits);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置低8位值
        /// </summary>
        public byte Low
        {
            get => (byte)(_value & LowMask);
            set => _value = (ushort)((_value & HighMask) | value);
        }

        /// <summary>
        /// 获取或设置高8位值
        /// </summary>
        public byte High
        {
            get => (byte)((_value & HighMask) >> MidBitCount);
            set => _value = (ushort)((_value & LowMask) | ((ushort)value << MidBitCount));
        }

        /// <summary>
        /// 获取完整的16位值
        /// </summary>
        public ushort Value => _value;

        #endregion

        #region 位操作方法

        /// <summary>
        /// 获取指定位置的位状态
        /// </summary>
        /// <param name="position">位置 (0-15)</param>
        /// <returns>位状态</returns>
        public bool GetBit(int position)
        {
            if (position < MinBitCount || position >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be between 0 and 15");

            return (_value & (1u << position)) != 0;
        }

        /// <summary>
        /// 设置指定位置的位状态
        /// </summary>
        /// <param name="position">位置 (0-15)</param>
        /// <param name="value">要设置的状态</param>
        public void SetBit(int position, bool value)
        {
            if (position < MinBitCount || position >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be between 0 and 15");

            if (value)
                _value |= (ushort)(1 << position);
            else
                _value &= (ushort)~(1 << position);
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
        /// 清除所有低位（0-7位）
        /// </summary>
        public void ClearLowBits()
        {
            _value &= HighMask;
        }

        /// <summary>
        /// 清除所有高位（8-15位）
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
        public ushort GetRange(int startBit, int endBit)
        {
            if (startBit < MinBitCount || endBit >= MaxBitCount || startBit > endBit)
                throw new ArgumentException("Invalid range");

            int length = endBit - startBit + 1;
            ushort mask = (ushort)(((1 << length) - 1) << startBit);
            return (ushort)((_value & mask) >> startBit);
        }

        /// <summary>
        /// 设置指定范围
        /// </summary>
        public void SetRange(int startBit, int endBit, ushort value)
        {
            if (startBit < MinBitCount || endBit >= MaxBitCount || startBit > endBit)
                throw new ArgumentException("Invalid range");

            int length = endBit - startBit + 1;
            ushort mask = (ushort)(((1 << length) - 1) << startBit);
            _value = (ushort)((_value & ~mask) | ((value << startBit) & mask));
        }

        #endregion

        #region 运算符重载

        public static implicit operator ushort(BitField16 field16) => field16._value;
        public static explicit operator BitField16(ushort value) => new BitField16(value);

        public static bool operator ==(BitField16 left, BitField16 right)
            => left._value == right._value;

        public static bool operator !=(BitField16 left, BitField16 right)
            => left._value != right._value;

        public override int GetHashCode()
            => _value.GetHashCode();

        public override string ToString()
            => $"Low: 0x{Low:X2}, High: 0x{High:X2}, Value: 0x{Value:X4}";

        public bool Equals(BitField16 other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is BitField16 other && Equals(other);
        }

        #endregion
    }
}
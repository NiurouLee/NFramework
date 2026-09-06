using System;

namespace  NFramework.Core
{
    [Serializable]
    public struct BitField8 : IEquatable<BitField8>
    {
        public static int MinBitCount = 0;
        public static int MidBitCount = 4;
        public static int MaxBitCount = 8;
        public static byte HighMask = 0xF0;
        public static byte LowMask = 0x0F;
        private byte _value;

        #region 构造函数
        public BitField8(byte value = 0)
        {
            _value = value;
        }

        public BitField8(byte lowBits, byte highBits)
        {
            _value = (byte)((highBits << MidBitCount) | (lowBits & LowMask));
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置低4位值
        /// </summary>
        public byte Low
        {
            get => (byte)(_value & LowMask);
            set => _value = (byte)((_value & HighMask) | (value & LowMask));
        }

        /// <summary>
        /// 获取或设置高4位值
        /// </summary>
        public byte High
        {
            get => (byte)((_value & HighMask) >> MidBitCount);
            set => _value = (byte)((_value & LowMask) | ((value & 0x0F) << MidBitCount));
        }

        /// <summary>
        /// 获取完整的8位值
        /// </summary>
        public byte Value => _value;

        #endregion

        #region 位操作方法

        /// <summary>
        /// 获取指定位置的位状态
        /// </summary>
        /// <param name="position">位置 (0-7)</param>
        /// <returns>位状态</returns>
        public bool GetBit(int position)
        {
            if (position < MinBitCount || position >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be between 0 and 7");

            return (_value & (1 << position)) != 0;
        }

        /// <summary>
        /// 设置指定位置的位状态
        /// </summary>
        /// <param name="position">位置 (0-7)</param>
        /// <param name="value">要设置的状态</param>
        public void SetBit(int position, bool value)
        {
            if (position < MinBitCount || position >= MaxBitCount)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be between 0 and 7");

            if (value)
                _value |= (byte)(1 << position);
            else
                _value &= (byte)~(1 << position);
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
        /// 清除所有低位（0-3位）
        /// </summary>
        public void ClearLowBits()
        {
            _value &= HighMask;
        }

        /// <summary>
        /// 清除所有高位（4-7位）
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
        public byte GetRange(int startBit, int endBit)
        {
            if (startBit < MinBitCount || endBit >= MaxBitCount || startBit > endBit)
                throw new ArgumentException("Invalid range");

            int length = endBit - startBit + 1;
            byte mask = (byte)(((1 << length) - 1) << startBit);
            return (byte)((_value & mask) >> startBit);
        }

        /// <summary>
        /// 设置指定范围
        /// </summary>
        public void SetRange(int startBit, int endBit, byte value)
        {
            if (startBit < MinBitCount || endBit >= MaxBitCount || startBit > endBit)
                throw new ArgumentException("Invalid range");

            int length = endBit - startBit + 1;
            byte mask = (byte)(((1 << length) - 1) << startBit);
            _value = (byte)((_value & ~mask) | ((value << startBit) & mask));
        }

        #endregion

        #region 运算符重载

        public static implicit operator byte(BitField8 field8) => field8._value;
        public static explicit operator BitField8(byte value) => new BitField8(value);

        public static bool operator ==(BitField8 left, BitField8 right)
            => left._value == right._value;

        public static bool operator !=(BitField8 left, BitField8 right)
            => left._value != right._value;

        public override int GetHashCode()
            => _value.GetHashCode();

        public override string ToString()
            => $"Low: 0x{Low:X1}, High: 0x{High:X1}, Value: 0x{Value:X2}";

        public bool Equals(BitField8 other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is BitField8 other && Equals(other);
        }

        #endregion
    }
}


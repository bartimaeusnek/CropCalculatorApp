﻿/*===============================[ XorShiftPlus ]==============================
  ==-------------[ (c) 2018 R. Wildenhaus - Licensed under MIT ]-------------==
  ============================================================================= */

using System;

namespace CropApp.Backend
{
    /// <summary>
    ///   Generates pseudorandom primitive types with a 64-bit implementation
    ///   of the XorShift algorithm.
    /// </summary>
    public class XorShiftRandom
    {
#region Data Members

        // Constants
        private const double DOUBLE_UNIT = 1.0 / (int.MaxValue + 1.0);

        // State Fields
        private ulong x_;
        private ulong y_;

        // Buffer for optimized bit generation.
        private ulong buffer_;
        private ulong bufferMask_;

#endregion

#region Constructor

        /// <summary>
        ///   Constructs a new  generator using two
        ///   random Guid hash codes as a seed.
        /// </summary>
        public XorShiftRandom()
        {
            this.x_ = (ulong) Guid.NewGuid().GetHashCode();
            this.y_ = (ulong) Guid.NewGuid().GetHashCode();
        }

        /// <summary>
        ///   Constructs a new  generator
        ///   with the supplied seed.
        /// </summary>
        /// <param name="seed">
        ///   The seed value.
        /// </param>
        public XorShiftRandom(ulong seed)
        {
            this.x_ = seed << 3;
            this.x_ = seed >> 3;
        }

#endregion

#region Public Methods

        /// <summary>
        ///   Generates a pseudorandom boolean.
        /// </summary>
        /// <returns>
        ///   A pseudorandom boolean.
        /// </returns>
        public bool NextBoolean()
        {
            bool _;
            if (this.bufferMask_ > 0)
            {
                _                =   (this.buffer_ & this.bufferMask_) == 0;
                this.bufferMask_ >>= 1;
                return _;
            }

            ulong temp_x, temp_y;
            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            this.buffer_ = temp_y + this.y_;
            this.x_      = temp_x;
            this.y_      = temp_y;

            this.bufferMask_ = 0x8000000000000000;
            return (this.buffer_ & 0xF000000000000000) == 0;
        }

        /// <summary>
        ///   Generates a pseudorandom byte.
        /// </summary>
        /// <returns>
        ///   A pseudorandom byte.
        /// </returns>
        public byte NextByte()
        {
            if (this.bufferMask_ >= 8)
            {
                var _ = (byte) this.buffer_;
                this.buffer_     >>= 8;
                this.bufferMask_ >>= 8;
                return _;
            }

            ulong temp_x, temp_y;
            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            this.buffer_ = temp_y + this.y_;
            this.x_      = temp_x;
            this.y_      = temp_y;

            this.bufferMask_ = 0x8000000000000;
            return (byte) (this.buffer_ >>= 8);
        }

        /// <summary>
        ///   Generates a pseudorandom 16-bit signed integer.
        /// </summary>
        /// <returns>
        ///   A pseudorandom 16-bit signed integer.
        /// </returns>
        public short NextInt16()
        {
            short _;
            ulong temp_x, temp_y;

            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            _ = (short) (temp_y + this.y_);

            this.x_ = temp_x;
            this.y_ = temp_y;

            return _;
        }

        /// <summary>
        ///   Generates a pseudorandom 16-bit unsigned integer.
        /// </summary>
        /// <returns>
        ///   A pseudorandom 16-bit unsigned integer.
        /// </returns>
        public ushort NextUInt16()
        {
            ushort _;
            ulong  temp_x, temp_y;

            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            _ = (ushort) (temp_y + this.y_);

            this.x_ = temp_x;
            this.y_ = temp_y;

            return _;
        }

        /// <summary>
        ///   Generates a pseudorandom 32-bit signed integer.
        /// </summary>
        /// <returns>
        ///   A pseudorandom 32-bit signed integer.
        /// </returns>
        public int NextInt32()
        {
            int   _;
            ulong temp_x, temp_y;

            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            _ = (int) (temp_y + this.y_);

            this.x_ = temp_x;
            this.y_ = temp_y;

            return _;
        }

        /// <summary>
        ///   Generates a pseudorandom 32-bit unsigned integer.
        /// </summary>
        /// <returns>
        ///   A pseudorandom 32-bit unsigned integer.
        /// </returns>
        public uint NextUInt32()
        {
            uint  _;
            ulong temp_x, temp_y;

            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            _ = (uint) (temp_y + this.y_);

            this.x_ = temp_x;
            this.y_ = temp_y;

            return _;
        }

        /// <summary>
        ///   Generates a pseudorandom 64-bit signed integer.
        /// </summary>
        /// <returns>
        ///   A pseudorandom 64-bit signed integer.
        /// </returns>
        public long NextInt64()
        {
            long  _;
            ulong temp_x, temp_y;

            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            _ = (long) (temp_y + this.y_);

            this.x_ = temp_x;
            this.y_ = temp_y;

            return _;
        }

        /// <summary>
        ///   Generates a pseudorandom 64-bit unsigned integer.
        /// </summary>
        /// <returns>
        ///   A pseudorandom 64-bit unsigned integer.
        /// </returns>
        public ulong NextUInt64()
        {
            ulong _;
            ulong temp_x, temp_y;

            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            _ = (ulong) (temp_y + this.y_);

            this.x_ = temp_x;
            this.y_ = temp_y;

            return _;
        }

        /// <summary>
        ///   Generates a pseudorandom double between
        ///   0 and 1 non-inclusive.
        /// </summary>
        /// <returns>
        ///   A pseudorandom double.
        /// </returns>
        public double NextDouble()
        {
            double _;
            ulong  temp_x, temp_y, temp_z;

            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            temp_z = temp_y + this.y_;
            _      = DOUBLE_UNIT * (0x7FFFFFFF & temp_z);

            this.x_ = temp_x;
            this.y_ = temp_y;

            return _;
        }

        /// <summary>
        ///   Generates a pseudorandom decimal between
        ///   0 and 1 non-inclusive.
        /// </summary>
        /// <returns>
        ///   A pseudorandom decimal.
        /// </returns>
        public decimal NextDecimal()
        {
            decimal _;
            int     l,      m,      h;
            ulong   temp_x, temp_y, temp_z;

            temp_x  =  this.y_;
            this.x_ ^= this.x_ << 23;
            temp_y  =  this.x_ ^ this.y_ ^ (this.x_ >> 17) ^ (this.y_ >> 26);

            temp_z = temp_y + this.y_;

            h = (int) (temp_z & 0x1FFFFFFF);
            m = (int) (temp_z >> 16);
            l = (int) (temp_z >> 32);

            _ = new decimal(l, m, h, false, 28);

            this.x_ = temp_x;
            this.y_ = temp_y;

            return _;
        }

#endregion
    }
}
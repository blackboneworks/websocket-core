using System;
using System.Collections;
using System.Collections.Generic;

namespace WebSocketCore
{
    internal class PayloadData : IEnumerable<byte>
    {
        #region Private Fields

        private byte[] _data;
        private long _extDataLength;
        private long _length;

        #endregion

        #region Public Fields

        /// <summary>
        /// Represents the empty payload data.
        /// </summary>
        public static readonly PayloadData Empty;

        /// <summary>
        /// Represents the allowable max length.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   A <see cref="WebSocketException"/> will occur if the payload data length is
        ///   greater than the value of this field.
        ///   </para>
        ///   <para>
        ///   If you would like to change the value, you must set it to a value between
        ///   <c>WebSocket.FragmentLength</c> and <c>Int64.MaxValue</c> inclusive.
        ///   </para>
        /// </remarks>
        public static readonly ulong MaxLength;

        #endregion

        #region Static Constructor

        static PayloadData()
        {
            Empty = new PayloadData();
            MaxLength = Int64.MaxValue;
        }

        #endregion

        #region Internal Constructors

        internal PayloadData()
        {
            _data = WebSocket.EmptyBytes;
        }

        internal PayloadData(byte[] data)
            : this(data, data.Length)
        {
        }

        internal PayloadData(byte[] data, long length)
        {
            _data = data;
            _length = length;
        }

        #endregion

        #region Internal Properties

        internal long ExtensionDataLength
        {
            get { return _extDataLength; }

            set { _extDataLength = value; }
        }

        internal bool IncludesReservedCloseStatusCode
        {
            get { return _length > 1 && _data.SubArray(0, 2).ToUInt16(ByteOrder.Big).IsReserved(); }
        }

        #endregion

        #region Public Properties

        public byte[] ApplicationData => _extDataLength > 0
            ? _data.SubArray((int) _extDataLength, (int) _length - (int) _extDataLength)
            : _data;

        public byte[] ExtensionData => _extDataLength > 0
            ? _data.SubArray(0, (int) _extDataLength)
            : WebSocket.EmptyBytes;

        public ulong Length => (ulong) _length;

        #endregion

        #region Internal Methods

        internal void Mask(byte[] key)
        {
            for (long i = 0; i < _length; i++)
                _data[i] = (byte) (_data[i] ^ key[i % 4]);
        }

        #endregion

        #region Public Methods

        public IEnumerator<byte> GetEnumerator()
        {
            foreach (var b in _data)
                yield return b;
        }

        public byte[] ToArray()
        {
            return _data;
        }

        public override string ToString()
        {
            return BitConverter.ToString(_data);
        }

        #endregion

        #region Explicit Interface Implementations

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
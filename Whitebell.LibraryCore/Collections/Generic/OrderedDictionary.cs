﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
using Whitebell.Library.Extension;

namespace Whitebell.Library.Collections.Generic
{
    /// <summary>キーと値のペアのインデックス付きジェネリック コレクションを表します。</summary>
    /// <typeparam name="TKey">ディクショナリ内のキーの型。</typeparam>
    /// <typeparam name="TValue">ディクショナリ内の値の型。</typeparam>
    public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IOrderedDictionary
    {
        /// <summary>指定したインデックスにある要素を取得または設定します。</summary>
        /// <param name="index">取得または設定する要素の、0 から始まるインデックス番号。</param>
        /// <returns>指定したインデックス位置にある要素。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="ICollection{T}.Count"/> 以上になっています。</exception>
        new TValue this[int index] { get; set; }

        /// <summary>コレクション内の指定したインデックス位置に、キーと値のペアを挿入します。</summary>
        /// <param name="index">キーと値のペアを挿入する位置の、0 から始まるインデックス。</param>
        /// <param name="key">追加する要素のキーとして使用するオブジェクト。</param>
        /// <param name="value">追加する要素の値として使用するオブジェクト。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="ICollection{T}.Count"/> より大きくなっています。</exception>
        /// <exception cref="ArgumentException">同じキーを持つ要素がコレクションに既に存在しています。</exception>
        void Insert(int index, TKey key, TValue value);
    }

    /// <summary>キーまたはインデックスからアクセスできる、順序付けられたキーと値のコレクションを表します。</summary>
    /// <typeparam name="TKey">ディクショナリ内のキーの型。</typeparam>
    /// <typeparam name="TValue">ディクショナリ内の値の型。</typeparam>
    [Serializable]
    [ComVisible(false)]
    public class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>,
        ISerializable, IDeserializationCallback
    {
        #region Fields

        private readonly Dictionary<TKey, TValue> dic;
        private readonly List<TKey> list;
        private int initialCapacity;
        private IEqualityComparer<TKey> eqComparer;
        private KeyCollection keys;
        private ValueCollection values;
        private readonly SerializationInfo si;
        private int version;

        [NonSerialized]
        private object syncRoot;

        private const string VersionName = "Version";
        private const string EqualityComparerName = "EqualityComparer";
        private const string InitialCapacityName = "InitialCapacity";
        private const string KeyValuePairsName = "KeyValuePairs";

        #endregion

        #region Constructors

        /// <summary>空で、既定の初期量を備え、キーの型の既定の等値比較子を使用する、 <see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。</summary>
        public OrderedDictionary()
            : this(0, null)
        { }

        /// <summary>指定した <see cref="IDictionary{TKey, TValue}"/> から要素をコピーして格納し、キーの型の既定の等値比較子を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。</summary>
        /// <param name="dictionary">新しい <see cref="OrderedDictionary{TKey, TValue}"/> に要素がコピーされる <see cref="IDictionary{TKey, TValue}"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> は null です。</exception>
        public OrderedDictionary(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        { }

        /// <summary>指定した <see cref="IDictionary{TKey, TValue}"/> から要素をコピーして格納し、指定した <see cref="IEqualityComparer{T}"/> を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。</summary>
        /// <param name="dictionary">新しい <see cref="OrderedDictionary{TKey, TValue}"/> に要素がコピーされる <see cref="IDictionary{TKey, TValue}"/></param>
        /// <param name="comparer">キーの比較時に使用する <see cref="IEqualityComparer{T}"/> 実装。キーの型の既定の <see cref="EqualityComparer{T}"/> を使用する場合は null。</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> は null です。</exception>
        public OrderedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            initialCapacity = dictionary.Count;
            eqComparer = comparer ?? EqualityComparer<TKey>.Default;

            dic = new Dictionary<TKey, TValue>(initialCapacity, eqComparer);
            list = new List<TKey>();

            foreach (var e in dictionary)
                Add(e.Key, e.Value);
        }

        /// <summary>空で、既定の初期量を備え、指定した <see cref="IEqualityComparer{T}"/> を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。</summary>
        /// <param name="comparer">キーの比較時に使用する <see cref="IEqualityComparer{T}"/> 実装。キーの型の既定の <see cref="EqualityComparer{T}"/> を使用する場合は null。</param>
        public OrderedDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        { }

        /// <summary>空で、指定した初期量を備え、キーの型の既定の等値比較子を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。</summary>
        /// <param name="capacity"><see cref="OrderedDictionary{TKey, TValue}"/> が格納できる要素数の初期値。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> が 0 未満です。</exception>
        public OrderedDictionary(int capacity)
            : this(capacity, null)
        { }

        /// <summary>空で、指定した初期量を備え、指定した <see cref="IEqualityComparer{T}"/> を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。</summary>
        /// <param name="capacity"><see cref="OrderedDictionary{TKey, TValue}"/> が格納できる要素数の初期値。</param>
        /// <param name="comparer">キーの比較時に使用する <see cref="IEqualityComparer{T}"/> 実装。キーの型の既定の <see cref="EqualityComparer{T}"/> を使用する場合は null。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> が 0 未満です。</exception>
        public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            initialCapacity = capacity;
            eqComparer = comparer ?? EqualityComparer<TKey>.Default;

            dic = new Dictionary<TKey, TValue>(initialCapacity, eqComparer);
            list = new List<TKey>(initialCapacity);
        }

        /// <summary>シリアル化したデータを使用して、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。</summary>
        /// <param name="info"><see cref="OrderedDictionary{TKey, TValue}"/> をシリアル化するために必要な情報を格納している <see cref="SerializationInfo"/> オブジェクト。</param>
        /// <param name="context"><see cref="OrderedDictionary{TKey, TValue}"/> に関連付けられているシリアル化ストリームの送信元および送信先を格納している <see cref="StreamingContext"/> 構造体。</param>
        protected OrderedDictionary(SerializationInfo info, StreamingContext context) => si = info;

        #endregion

        #region Properties

        /// <summary>ディクショナリのキーが等しいかどうかを確認するために使用する <see cref="IEqualityComparer{T}"/> を取得します。</summary>
        /// <value>現在の <see cref="OrderedDictionary{TKey, TValue}"/> のキーが等しいかどうかを確認し、キーのハッシュ値を提供するために使用する <see cref="IEqualityComparer{T}"/> ジェネリック インターフェイスの実装。</value>
        public IEqualityComparer<TKey> Comparer => eqComparer ?? EqualityComparer<TKey>.Default;

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> に格納されているキー/値ペアの数を取得します。</summary>
        /// <value><see cref="OrderedDictionary{TKey, TValue}"/> に格納されているキー/値ペアの数。</value>
        public int Count => list.Count;

        /// <summary>指定したインデックスにある要素を取得または設定します。</summary>
        /// <param name="index">取得または設定する要素の、0 から始まるインデックス番号。</param>
        /// <returns>指定したインデックス位置にある要素。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="Count"/> 以上になっています。</exception>
        public TValue this[int index]
        {
            get
            {
                if (index < 0 || list.Count <= index)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return dic[list[index]];
            }
            set
            {
                if (index < 0 || list.Count <= index)
                    throw new ArgumentOutOfRangeException(nameof(index));

                dic[list[index]] = value;
                version++;
            }
        }

        /// <summary>指定されたキーに関連付けられている値を取得または設定します。</summary>
        /// <param name="key">取得または設定する値のキー。</param>
        /// <returns>指定されたキーに関連付けられている値。 指定したキーが見つからなかった場合、get 操作は <see cref="KeyNotFoundException"/> をスローし、set 操作は指定したキーを持つ新しい要素を作成します。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> が null です。</exception>
        /// <exception cref="KeyNotFoundException">プロパティが取得されましたが、コレクション内に <paramref name="key"/> が存在しません。</exception>
        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                if (!dic.ContainsKey(key))
                    throw new KeyNotFoundException(nameof(key));

                return dic[key];
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                if (dic.ContainsKey(key))
                {
                    dic[key] = value;
                    var idx = list.FindIndex(e => eqComparer.Equals(e, key));
                    list[idx] = key;
                }
                else
                {
                    dic[key] = value;
                    list.Add(key);
                }
                version++;
            }
        }

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> 内のキーを格納しているコレクションを取得します。</summary>
        /// <value><see cref="OrderedDictionary{TKey, TValue}"/> 内のキーを格納している <see cref="KeyCollection"/>。</value>
        public KeyCollection Keys => keys ?? (keys = new KeyCollection(this));

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> 内の値を格納しているコレクションを取得します。</summary>
        /// <value><see cref="OrderedDictionary{TKey, TValue}"/> 内の値を格納している <see cref="ValueCollection"/>。</value>
        public ValueCollection Values => values ?? (values = new ValueCollection(this));

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        #endregion

        #region Methods

        /// <summary>指定したキーと値をディクショナリに追加します。</summary>
        /// <param name="key">追加する要素のキー。</param>
        /// <param name="value">追加する要素の値。 参照型の場合は null の値を使用できます。</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> は null です。</exception>
        /// <exception cref="ArgumentException">同じキーを持つ要素が、<see cref="OrderedDictionary{TKey, TValue}"/> に既に存在します。</exception>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (dic.ContainsKey(key))
                throw new ArgumentException(nameof(key));

            list.Add(key);
            dic.Add(key, value);
            version++;
        }

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> からすべてのキーと値を削除します。</summary>
        public void Clear()
        {
            dic.Clear();
            list.Clear();
            version++;
        }

        /// <summary>指定したキーが <see cref="OrderedDictionary{TKey, TValue}"/> に格納されているかどうかを判断します。</summary>
        /// <param name="key"><see cref="OrderedDictionary{TKey, TValue}"/> 内で検索されるキー。</param>
        /// <returns>指定したキーを持つ要素が <see cref="OrderedDictionary{TKey, TValue}"/> に格納されている場合は true。それ以外の場合は false。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> は null です。</exception>
        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return dic.ContainsKey(key);
        }

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> に特定の値が格納されているかどうかを判断します。</summary>
        /// <param name="value"><see cref="OrderedDictionary{TKey, TValue}"/> 内で検索される値。参照型の場合は null の値を使用できます。</param>
        /// <returns>指定した値を持つ要素が <see cref="OrderedDictionary{TKey, TValue}"/> に格納されている場合は true。それ以外の場合は false。</returns>
        public bool ContainsValue(TValue value) => dic.ContainsValue(value);

        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0 || array.Length <= index)
                throw new ArgumentNullException(nameof(index));
            if (array.Length - index < list.Count)
                throw new ArgumentException();

            for (var i = 0; i < list.Count; i++)
                array[index++] = new KeyValuePair<TKey, TValue>(list[i], dic[list[i]]);
        }

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> を反復処理する列挙子を返します。</summary>
        /// <returns><see cref="OrderedDictionary{TKey, TValue}"/> の <see cref="Enumerator"/> 構造体。</returns>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary><see cref="ISerializable"/> インターフェイスを実装し、<see cref="OrderedDictionary{TKey, TValue}"/> インスタンスをシリアル化するために必要なデータを返します。</summary>
        /// <param name="info"><see cref="OrderedDictionary{TKey, TValue}"/> インスタンスをシリアル化するために必要な情報を格納する <see cref="SerializationInfo"/> オブジェクト。</param>
        /// <param name="context"><see cref="OrderedDictionary{TKey, TValue}"/> インスタンスに関連付けられているシリアル化ストリームの転送元および転送先を格納する <see cref="StreamingContext"/> 構造体。</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> は null です。</exception>
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(InitialCapacityName, initialCapacity);
            info.AddValue(VersionName, version);
            info.AddValue(EqualityComparerName, eqComparer, typeof(IEqualityComparer<TKey>));
            if (list.Count != 0)
            {
                var ary = list.Select(e => new KeyValuePair<TKey, TValue>(e, dic[e])).ToArray();
                info.AddValue(KeyValuePairsName, ary, typeof(KeyValuePair<TKey, TValue>[]));
            }
        }

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> コレクションの指定したインデックス位置に、指定したキーと値を持つ新しいエントリを挿入します。</summary>
        /// <param name="index">要素を挿入する位置の、0 から始まるインデックス番号。</param>
        /// <param name="key">追加するエントリのキー。</param>
        /// <param name="value">追加するエントリの値。値は  null に設定できます。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="Count"/> より大きくなっています。</exception>
        /// <exception cref="ArgumentException">同じキーを持つ要素がコレクションに既に存在しています。</exception>
        public void Insert(int index, TKey key, TValue value)
        {
            if (index < 0 || list.Count <= index)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (dic.ContainsKey(key))
                throw new ArgumentException(nameof(key));

            list.Insert(index, key);
            dic[key] = value;
            version++;
        }

        /// <summary><see cref="ISerializable"/> インターフェイスを実装し、逆シリアル化が完了したときに逆シリアル化イベントを発生させます。</summary>
        /// <param name="sender">逆シリアル化イベントのソース。</param>
        /// <exception cref="SerializationException"><see cref="OrderedDictionary{TKey, TValue}"/> オブジェクトに現在関連付けられている <see cref="SerializationInfo"/> インスタンスが無効です。</exception>
        public virtual void OnDeserialization(object sender)
        {
            if (si == null)
                throw new SerializationException();

            eqComparer = si.GetValue<IEqualityComparer<TKey>>(EqualityComparerName);
            initialCapacity = si.GetInt32(InitialCapacityName);
            var realVersion = si.GetInt32(VersionName);
            var kvps = si.GetValue<KeyValuePair<TKey, TValue>[]>(KeyValuePairsName) ?? Array.Empty<KeyValuePair<TKey, TValue>>();

            foreach (var e in kvps)
                Add(e.Key, e.Value);
            version = realVersion;
        }

        /// <summary>指定したキーを持つ値を <see cref="OrderedDictionary{TKey, TValue}"/> から削除します。</summary>
        /// <param name="key">削除する要素のキー。</param>
        /// <returns>要素が見つかり、正常に削除された場合は true。それ以外の場合は false。このメソッドは、<paramref name="key"/> が <see cref="OrderedDictionary{TKey, TValue}"/> に見つからない場合、false を返します。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> は null です。</exception>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (dic.Remove(key) && list.Remove(key))
            {
                version++;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>指定したインデックス位置にある要素を削除します。</summary>
        /// <param name="index">削除する要素の 0 から始まるインデックス。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="Count"/> 以上になっています。</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || list.Count <= index)
                throw new ArgumentOutOfRangeException(nameof(index));

            var k = list[index];
            dic.Remove(k);
            list.RemoveAt(index);
            version++;
        }

        /// <summary>指定したキーに関連付けられている値を取得します。</summary>
        /// <param name="key">取得する値のキー。</param>
        /// <param name="value">このメソッドから制御が戻るときに、キーが見つかった場合は、指定したキーに関連付けられている値が格納されます。それ以外の場合は <paramref name="value"/> パラメーターの型に対する既定の値です。このパラメーターは初期化せずに渡されます。</param>
        /// <returns>指定したキーを持つ要素が <see cref="OrderedDictionary{TKey, TValue}"/> に格納されている場合は true。それ以外の場合は false。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> は null です。</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (dic.ContainsKey(key))
            {
                value = dic[key];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// 指定したインデックス位置にある要素に対応するキーを取得します。
        /// </summary>
        /// <param name="index">キーを取得する要素の 0 から始まるインデックス。</param>
        /// <returns>指定したインデックスにある要素に対応するキーの値。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="Count"/> 以上になっています。</exception>
        public TKey KeyAt(int index)
        {
            if (index < 0 || list.Count <= index)
                throw new ArgumentOutOfRangeException(nameof(index));

            return list[index];
        }

        #endregion

        #region EIMI

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => dic.ContainsKey(item.Key) && dic[item.Key].Equals(item.Value);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index) => CopyTo(array, index);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => dic.ContainsKey(item.Key) && dic[item.Key].Equals(item.Value) && Remove(item.Key);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IDictionary.Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null && default(TValue) != null) // TValue is struct.
                throw new ArgumentNullException(nameof(value));

            if (!(key is TKey tk))
                throw new ArgumentException(nameof(key));
            if (!(value is TValue tv))
                throw new ArgumentException(nameof(value));

            Add(tk, tv);
        }

        bool IDictionary.Contains(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return key is TKey t && ContainsKey(t);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this);

        void IDictionary.Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (key is TKey t)
                Remove(t);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException(nameof(array));
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException(nameof(array));
            if (index < 0 || array.Length <= index)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Length - index < list.Count)
                throw new ArgumentException();

            if (array is KeyValuePair<TKey, TValue>[] ks)
            {
                CopyTo(ks, index);
            }
            else if (array is DictionaryEntry[] ds)
            {
                for (var i = 0; i < list.Count; i++)
                    ds[index++] = new DictionaryEntry(list[i], dic[list[i]]);
            }
            else if (array is object[] os)
            {
                try
                {
                    for (var i = 0; i < list.Count; i++)
                        os[index++] = new KeyValuePair<TKey, TValue>(list[i], dic[list[i]]);
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                    Interlocked.CompareExchange<object>(ref syncRoot, new object(), null);
                return syncRoot;
            }
        }

        ICollection IDictionary.Keys => Keys;

        ICollection IDictionary.Values => Values;

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        object IDictionary.this[object key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                return key is TKey tk && dic.ContainsKey(tk) ? dic[tk] : null as object;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));
                if (value == null && default(TValue) != null) // TValue is struct.
                    throw new ArgumentNullException(nameof(value));

                if (!(key is TKey tk))
                    throw new ArgumentException(nameof(key));
                if (!(value is TValue tv))
                    throw new ArgumentException(nameof(value));

                this[tk] = tv;
                version++;
            }
        }

        object IOrderedDictionary.this[int index]
        {
            get => this[index];
            set
            {
                if (value == null && default(TValue) != null) // TValue is struct.
                    throw new ArgumentNullException(nameof(value));

                if (!(value is TValue tv))
                    throw new ArgumentException(nameof(value));

                this[index] = tv;
                version++;
            }
        }

        void IOrderedDictionary.Insert(int index, object key, object value)
        {
            if (index < 0 || list.Count <= index)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null && default(TValue) != null) // TValue is struct.
                throw new ArgumentNullException(nameof(value));

            if (!(key is TKey tk))
                throw new ArgumentException(nameof(key));
            if (dic.ContainsKey(tk))
                throw new ArgumentException(nameof(key));
            if (!(value is TValue tv))
                throw new ArgumentException(nameof(value));

            Insert(index, tk, tv);
        }

        IDictionaryEnumerator IOrderedDictionary.GetEnumerator() => GetEnumerator();

        #endregion

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> の要素を列挙します。</summary>
        [Serializable]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly OrderedDictionary<TKey, TValue> dic;
            private int index;
            private readonly int version;

            internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
            {
                dic = dictionary;
                index = 0;
                version = dictionary.version;
                Current = new KeyValuePair<TKey, TValue>();
            }

            /// <summary>列挙子の現在位置の要素を取得します。</summary>
            /// <value><see cref="OrderedDictionary{TKey, TValue}"/> のうち、列挙子の現在位置にある要素。</value>
            public KeyValuePair<TKey, TValue> Current { get; private set; }

            /// <summary><see cref="Enumerator"/> によって使用されているすべてのリソースを解放します。</summary>
            public void Dispose()
            { }

            /// <summary>列挙子を <see cref="OrderedDictionary{TKey, TValue}"/> の次の要素に進めます。</summary>
            /// <returns>列挙子が次の要素に正常に進んだ場合は true。列挙子がコレクションの末尾を越えた場合は false。</returns>
            /// <exception cref="InvalidOperationException">コレクションは、列挙子の作成後に変更されました。</exception>
            public bool MoveNext()
            {
                if (version != dic.version)
                    throw new InvalidOperationException();

                while ((uint)index < (uint)dic.Count)
                {
                    var k = dic.list[index];
                    Current = new KeyValuePair<TKey, TValue>(k, dic.dic[k]);
                    index++;
                    return true;
                }
                index = dic.Count + 1;
                Current = new KeyValuePair<TKey, TValue>();
                return false;
            }

            void IEnumerator.Reset()
            {
                if (version != dic.version)
                    throw new InvalidOperationException();

                index = 0;
                Current = new KeyValuePair<TKey, TValue>();
            }

            object IEnumerator.Current => Current;

            object IDictionaryEnumerator.Key => Current.Key;

            object IDictionaryEnumerator.Value => Current.Value;

            DictionaryEntry IDictionaryEnumerator.Entry => new DictionaryEntry(Current.Key, Current.Value);
        }

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> 内のキーのコレクションを表します。 このクラスは継承できません。</summary>
        [Serializable]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public sealed class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>, ICollection
        {
            private readonly OrderedDictionary<TKey, TValue> d;

            /// <summary>指定した <see cref="OrderedDictionary{TKey, TValue}"/> 内のキーを反映する、<see cref="KeyCollection"/> クラスの新しいインスタンスを初期化します。</summary>
            /// <param name="dictionary">新しい <see cref="KeyCollection"/> にキーが反映される <see cref="OrderedDictionary{TKey, TValue}"/>。</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> は null です。</exception>
            public KeyCollection(OrderedDictionary<TKey, TValue> dictionary) => d = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            /// <summary><see cref="KeyCollection"/> を反復処理する列挙子を返します。</summary>
            /// <returns><see cref="KeyCollection"/> の <see cref="Enumerator"/>。</returns>
            public Enumerator GetEnumerator() => new Enumerator(d);

            /// <summary><see cref="KeyCollection"/> の要素を既存の 1 次元の <see cref="Array"/> にコピーします。コピー操作は、配列内の指定したインデックスから始まります。</summary>
            /// <param name="array"><see cref="KeyCollection"/> から要素がコピーされる 1 次元の <see cref="Array"/>。<see cref="Array"/>には、0 から始まるインデックス番号が必要です。</param>
            /// <param name="arrayIndex">コピーの開始位置とする <paramref name="array"/> のインデックス (0 から始まる)。</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> は null です。</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> が 0 未満です。</exception>
            /// <exception cref="ArgumentException">コピー元の <see cref="KeyCollection"/> の要素数が、<paramref name="arrayIndex"/> からコピー先の <paramref name="array"/> の末尾までに格納できる数を超えています。</exception>
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0 || array.Length <= arrayIndex)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length - arrayIndex < d.list.Count)
                    throw new ArgumentException();

                d.list.CopyTo(array, arrayIndex);
            }

            /// <summary><see cref="KeyCollection"/> に格納されている要素の数を取得します。</summary>
            /// <value><see cref="KeyCollection"/> に格納されている要素の数。このプロパティ値を取得することは、O(1) 操作になります。</value>
            public int Count => d.list.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

            void ICollection<TKey>.Clear() => throw new NotSupportedException();

            bool ICollection<TKey>.Contains(TKey item) => d.dic.ContainsKey(item);

            bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(nameof(array));
                if (array.GetLowerBound(0) != 0)
                    throw new ArgumentException(nameof(array));
                if (index < 0 || array.Length <= index)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length - index < d.list.Count)
                    throw new ArgumentException();

                if (array is TKey[] ks)
                {
                    CopyTo(ks, index);
                }
                else if (array is object[] os)
                {
                    try
                    {
                        for (var i = 0; i < d.list.Count; i++)
                            os[index++] = d.list[i];
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException();
                    }
                }
            }

            bool ICollection.IsSynchronized => false;

            [SuppressMessage("Usage", "RCS1202:Avoid NullReferenceException.", Justification = "<Pending>")]
            object ICollection.SyncRoot => (d as ICollection).SyncRoot;

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => new Enumerator(d);

            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(d);

            /// <summary><see cref="KeyCollection"/> の要素を列挙します。</summary>
            [Serializable]
            public struct Enumerator : IEnumerator<TKey>
            {
                private readonly OrderedDictionary<TKey, TValue> d;
                private int index;
                private readonly int version;

                internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
                {
                    d = dictionary;
                    index = 0;
                    version = dictionary.version;
                    Current = default;
                }

                /// <summary><see cref="Enumerator"/> によって使用されているすべてのリソースを解放します。</summary>
                public void Dispose()
                { }

                /// <summary>列挙子を <see cref="KeyCollection"/> の次の要素に進めます。</summary>
                /// <returns>列挙子が次の要素に正常に進んだ場合は true。列挙子がコレクションの末尾を越えた場合は false。</returns>
                /// <exception cref="InvalidOperationException">コレクションは、列挙子の作成後に変更されました。</exception>
                public bool MoveNext()
                {
                    if (d.version != version)
                        throw new InvalidOperationException();

                    while ((uint)index < (uint)d.list.Count)
                    {
                        Current = d.list[index];
                        index++;
                        return true;
                    }

                    index = d.list.Count + 1;
                    Current = default;
                    return false;
                }

                /// <summary>列挙子の現在位置の要素を取得します。</summary>
                /// <value><see cref="KeyCollection"/> のうち、列挙子の現在位置にある要素。</value>
                public TKey Current { get; private set; }

                object IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || index == d.list.Count + 1)
                            throw new InvalidOperationException();
                        return Current;
                    }
                }

                void IEnumerator.Reset()
                {
                    index = 0;
                    Current = default;
                }
            }
        }

        /// <summary><see cref="OrderedDictionary{TKey, TValue}"/> の値のコレクションを表します。 このクラスは継承できません。</summary>
        [Serializable]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        public sealed class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>, ICollection
        {
            private readonly OrderedDictionary<TKey, TValue> d;

            /// <summary>指定した <see cref="OrderedDictionary{TKey, TValue}"/> 内の値を反映する、<see cref="ValueCollection"/> クラスの新しいインスタンスを初期化します。</summary>
            /// <param name="dictionary">新しい <see cref="ValueCollection"/> にキーが反映される <see cref="OrderedDictionary{TKey, TValue}"/>。</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> は null です。</exception>
            public ValueCollection(OrderedDictionary<TKey, TValue> dictionary) => d = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            /// <summary><see cref="ValueCollection"/> を反復処理する列挙子を返します。</summary>
            /// <returns><see cref="ValueCollection"/> の <see cref="Enumerator"/>。</returns>
            public Enumerator GetEnumerator() => new Enumerator(d);

            /// <summary><see cref="ValueCollection"/> の要素を既存の 1 次元の <see cref="Array"/> にコピーします。コピー操作は、配列内の指定したインデックスから始まります。</summary>
            /// <param name="array"><see cref="ValueCollection"/> から要素がコピーされる 1 次元の <see cref="Array"/>。<see cref="Array"/>には、0 から始まるインデックス番号が必要です。</param>
            /// <param name="arrayIndex">コピーの開始位置とする <paramref name="array"/> のインデックス (0 から始まる)。</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> は null です。</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> が 0 未満です。</exception>
            /// <exception cref="ArgumentException">コピー元の <see cref="ValueCollection"/> の要素数が、<paramref name="arrayIndex"/> からコピー先の <paramref name="array"/> の末尾までに格納できる数を超えています。</exception>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0 || array.Length <= arrayIndex)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length - arrayIndex < d.list.Count)
                    throw new ArgumentException();

                for (var i = 0; i < d.list.Count; i++)
                    array[arrayIndex++] = d.dic[d.list[i]];
            }

            /// <summary><see cref="ValueCollection"/> に格納されている要素の数を取得します。</summary>
            /// <value><see cref="ValueCollection"/> に格納されている要素の数。</value>
            public int Count => d.list.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

            bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();

            void ICollection<TValue>.Clear() => throw new NotSupportedException();

            bool ICollection<TValue>.Contains(TValue item) => d.dic.ContainsValue(item);

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(nameof(array));
                if (array.GetLowerBound(0) != 0)
                    throw new ArgumentException(nameof(array));
                if (index < 0 || array.Length <= index)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length - index < d.list.Count)
                    throw new ArgumentException();

                if (array is TValue[] vs)
                {
                    CopyTo(vs, index);
                }
                else if (array is object[] os)
                {
                    try
                    {
                        for (var i = 0; i < d.list.Count; i++)
                            os[index++] = d.dic[d.list[i]];
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            bool ICollection.IsSynchronized => false;

            [SuppressMessage("Usage", "RCS1202:Avoid NullReferenceException.", Justification = "<Pending>")]
            object ICollection.SyncRoot => (d as ICollection).SyncRoot;

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => new Enumerator(d);

            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(d);

            /// <summary><see cref="ValueCollection"/> の要素を列挙します。</summary>
            [Serializable]
            public struct Enumerator : IEnumerator<TValue>
            {
                private readonly OrderedDictionary<TKey, TValue> d;
                private int index;
                private readonly int version;

                internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
                {
                    d = dictionary;
                    index = 0;
                    version = dictionary.version;
                    Current = default;
                }

                /// <summary><see cref="Enumerator"/> によって使用されているすべてのリソースを解放します。</summary>
                public void Dispose()
                { }

                /// <summary><see cref="ValueCollection"/> の次の要素に進めます。</summary>
                /// <returns>列挙子が次の要素に正常に進んだ場合は true。列挙子がコレクションの末尾を越えた場合は false。</returns>
                /// <exception cref="InvalidOperationException">コレクションは、列挙子の作成後に変更されました。</exception>
                public bool MoveNext()
                {
                    if (d.version != version)
                        throw new InvalidOperationException();

                    while ((uint)index < (uint)d.list.Count)
                    {
                        Current = d.dic[d.list[index]];
                        index++;
                        return true;
                    }

                    index = d.list.Count + 1;
                    Current = default;
                    return false;
                }

                /// <summary>列挙子の現在位置の要素を取得します。</summary>
                /// <value><see cref="ValueCollection"/> のうち、列挙子の現在位置にある要素。</value>
                public TValue Current { get; private set; }

                object IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (uint)index == (uint)d.list.Count + 1u)
                            throw new InvalidOperationException();

                        return Current;
                    }
                }

                void IEnumerator.Reset()
                {
                    index = 0;
                    Current = default;
                }
            }
        }
    }
}

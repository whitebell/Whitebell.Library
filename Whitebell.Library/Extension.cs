using System;
using System.Runtime.Serialization;

namespace Whitebell.Library
{
    public static class Extension
    {
        #region System.Runtime.Serialization.SerializationInfo

        /// <summary><see cref="SerializationInfo"/> ストアから値を取得します。</summary>
        /// <typeparam name="T">取得する値の <see cref="Type"/>。格納された値がこの型に変換できない場合は、<see cref="InvalidCastException"/> がスローされます。</typeparam>
        /// <param name="si"></param>
        /// <param name="name">取得する値に関連付けられた名前。</param>
        /// <returns><paramref name="name"/> に関連付けられた、指定した <see cref="Type"/> のオブジェクト。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> が null です。</exception>
        /// <exception cref="InvalidCastException"><paramref name="name"/> に関連付けられた値を <typeparamref name="T"/> に変換できません。</exception>
        /// <exception cref="SerializationException">指定した名前の要素が、現在のインスタンス内に見つかりません。</exception>
        public static T GetValue<T>(this SerializationInfo si, string name) => (T)si.GetValue(name, typeof(T));

        #endregion
    }
}

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitebell.Library.Extension
{
    public static class Extension
    {
        #region where T : System.IComparable<T>

        /// <summary>
        /// 最小値、最大値を指定し、このインスタンスがその範囲外の場合は指定された最小値あるいは最大値を返します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="min"/> または <paramref name="max"/> が null です。</exception>
        /// <exception cref="ArgumentException"><paramref name="min"/> が <paramref name="max"/> よりも大きい値です。</exception>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val == null)
                throw new ArgumentNullException(nameof(val));
            if (min == null)
                throw new ArgumentNullException(nameof(min));
            if (max == null)
                throw new ArgumentNullException(nameof(max));
            if (min.CompareTo(max) > 0)
                throw new ArgumentException();

            return val.CompareTo(min) < 0 ? min
                 : val.CompareTo(max) > 0 ? max
                 : val;
        }

        /// <summary>
        /// 最小値、最大値を指定し、このインスタンスがその範囲に含まれているかを示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <returns>最小値と最大値の範囲内に含まれている場合は true。それ以外の場合は false。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="min"/> または <paramref name="max"/> が null です。</exception>
        /// <exception cref="ArgumentException"><paramref name="min"/> が <paramref name="max"/> よりも大きい値です。</exception>
        public static bool InRange<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val == null)
                throw new ArgumentNullException(nameof(val));
            if (min == null)
                throw new ArgumentNullException(nameof(min));
            if (max == null)
                throw new ArgumentNullException(nameof(max));
            if (min.CompareTo(max) > 0)
                throw new ArgumentException($"\"{nameof(min)}\" greater than \"{nameof(max)}\"");

            return val.CompareTo(min) >= 0 && val.CompareTo(max) <= 0;
        }

        #endregion

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

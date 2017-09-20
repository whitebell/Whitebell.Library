using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Whitebell.Library.Extension
{
    /// <summary>
    /// Indicates which type of conversion to perform
    /// </summary>
    [Flags]
    public enum StringConvert
    {
        /// <summary>
        /// Performs no conversion.
        /// </summary>
        None = 0,
        /// <summary>
        /// Converts the string to uppercase characters.
        /// </summary>
        Uppercase = 1,
        /// <summary>
        /// Converts the string to lowercase characters.
        /// </summary>
        Lowercase = 2,
        /// <summary>
        /// Converts the first letter of every word in the string to uppercase.
        /// </summary>
        TitleCase = 3,
        /// <summary>
        /// Converts narrow (single-byte) characters in the string to wide (double-byte) characters. Applies to Asian locales.
        /// </summary>
        FullWidth = 4,
        /// <summary>
        /// Converts wide (double-byte) characters in the string to narrow (single-byte) characters. Applies to Asian locales.
        /// </summary>
        HalfWidth = 8,
        /// <summary>
        /// Converts Hiragana characters in the string to Katakana characters. Applies to Japanese locale only.
        /// </summary>
        Katakana = 16,
        /// <summary>
        /// Converts Katakana characters in the string to Hiragana characters. Applies to Japanese locale only.
        /// </summary>
        Hiragana = 32,
        /// <summary>
        /// Converts the string to Simplified Chinese characters.
        /// </summary>
        SimplifiedChinese = 256,
        /// <summary>
        /// Converts the string to Traditional Chinese characters.
        /// </summary>
        TraditionalChinese = 512,
        /// <summary>
        /// Converts the string from file system rules for casing to linguistic rules.
        /// </summary>
        LinguisticCasing = 1024,
    }

    /// <summary>
    /// 区間の端点の扱いを示す。
    /// </summary>
    public enum Interval
    {
        /// <summary>閉区間</summary>
        Closed,
        /// <summary>開区間</summary>
        Open,
        /// <summary>左閉半開区間</summary>
        LeftClosedRightOpen,
        /// <summary>右閉半開区間</summary>
        LeftOpenRightClosed,
    }

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
                throw new ArgumentException($"\"{nameof(min)}\" greater than \"{nameof(max)}\"");

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
        [Obsolete("use InRange<T>(T, T, T, Interval)")]
        public static bool InRange<T>(this T val, T min, T max) where T : IComparable<T> => InRange(val, min, max, Interval.Closed);

        /// <summary>
        /// 最小値、最大値を指定し、このインスタンスがその区間に含まれているかを示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <param name="interval">区間の端点の扱いを示します。省略した場合は<see cref="Interval.Closed"/>（閉区間）として扱います。</param>
        /// <returns>最小値と最大値の範囲内に含まれている場合は true。それ以外の場合は false。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="min"/> または <paramref name="max"/> が null です。</exception>
        /// <exception cref="ArgumentException"><paramref name="min"/> が <paramref name="max"/> よりも大きい値です。</exception>
        public static bool InRange<T>(this T val, T min, T max, Interval interval) where T : IComparable<T>
        {
            if (val == null)
                throw new ArgumentNullException(nameof(val));
            if (min == null)
                throw new ArgumentNullException(nameof(min));
            if (max == null)
                throw new ArgumentNullException(nameof(max));
            if (min.CompareTo(max) > 0)
                throw new ArgumentException($"\"{nameof(min)}\" greater than \"{nameof(max)}\"");

            switch (interval)
            {
                case Interval.Closed:
                    return val.CompareTo(min) >= 0 && val.CompareTo(max) <= 0;
                case Interval.Open:
                    return val.CompareTo(min) > 0 && val.CompareTo(max) < 0;
                case Interval.LeftClosedRightOpen:
                    return val.CompareTo(min) >= 0 && val.CompareTo(max) < 0;
                case Interval.LeftOpenRightClosed:
                    return val.CompareTo(min) > 0 && val.CompareTo(max) <= 0;
                default:
                    throw new ArgumentException(nameof(interval));
            }
        }

        #endregion

        #region System.Char

        #region from Unicode::EastAsianWidth 1.33 by ☻ 唐鳳 ☺ Audrey Tang http://search.cpan.org/~audreyt/Unicode-EastAsianWidth-1.33/

        public static bool InFullwidth(this char chr)
            => chr.InFullwidth(eastAsian: true);

        public static bool InFullwidth(this char chr, bool eastAsian)
        {
            if (eastAsian)
                return chr.InEastAsianFullwidth() || chr.InEastAsianWide() || chr.InEastAsianAmbiguous();
            else
                return chr.InEastAsianFullwidth() || chr.InEastAsianWide();
        }

        public static bool InFullwidth(this int codePoint)
            => codePoint.InFullwidth(eastAsian: true);

        public static bool InFullwidth(this int codePoint, bool eastAsian)
        {
            if (eastAsian)
                return codePoint.InEastAsianFullwidth() || codePoint.InEastAsianWide() || codePoint.InEastAsianAmbiguous();
            else
                return codePoint.InEastAsianFullwidth() || codePoint.InEastAsianWide();
        }

        public static bool InHalfwidth(this char chr)
            => chr.InHalfwidth(eastAsian: true);

        public static bool InHalfwidth(this char chr, bool eastAsian)
        {
            if (eastAsian)
                return chr.InEastAsianHalfwidth() || chr.InEastAsianNarrow() || chr.InEastAsianNeutral();
            else
                return chr.InEastAsianHalfwidth() || chr.InEastAsianNarrow() || chr.InEastAsianNeutral() || chr.InEastAsianAmbiguous();
        }

        public static bool InHalfwidth(this int codePoint)
            => codePoint.InHalfwidth(eastAsian: true);

        public static bool InHalfwidth(this int codePoint, bool eastAsian)
        {
            if (eastAsian)
                return codePoint.InEastAsianHalfwidth() || codePoint.InEastAsianNarrow() || codePoint.InEastAsianNeutral();
            else
                return codePoint.InEastAsianHalfwidth() || codePoint.InEastAsianNarrow() || codePoint.InEastAsianNeutral() || codePoint.InEastAsianAmbiguous();
        }

        public static bool InEastAsianAmbiguous(this char chr)
        {
            ushort cp = chr;
            //return (codePoint == 0x00a1 || codePoint == 0x00a4 || codePoint == 0x00a7 || codePoint == 0x00a8 || codePoint == 0x00aa || codePoint == 0x00ad || codePoint == 0x00ae || (0x00b0 <= codePoint && codePoint <= 0x00b4) || (0x00b6 <= codePoint && codePoint <= 0x00ba) || (0x00bc <= codePoint && codePoint <= 0x00bf) || codePoint == 0x00c6 || codePoint == 0x00d0 || codePoint == 0x00d7 || codePoint == 0x00d8 || (0x00de <= codePoint && codePoint <= 0x00e1) || codePoint == 0x00e6 || (0x00e8 <= codePoint && codePoint <= 0x00ea) || codePoint == 0x00ec || codePoint == 0x00ed || codePoint == 0x00f0 || codePoint == 0x00f2 || codePoint == 0x00f3 || (0x00f7 <= codePoint && codePoint <= 0x00fa) || codePoint == 0x00fc || codePoint == 0x00fe || codePoint == 0x0101 || codePoint == 0x0111 || codePoint == 0x0113 || codePoint == 0x011b || codePoint == 0x0126 || codePoint == 0x0127 || codePoint == 0x012b || (0x0131 <= codePoint && codePoint <= 0x0133) || codePoint == 0x0138 || (0x013f <= codePoint && codePoint <= 0x0142) || codePoint == 0x0144 || (0x0148 <= codePoint && codePoint <= 0x014b) || codePoint == 0x014d || codePoint == 0x0152 || codePoint == 0x0153 || codePoint == 0x0166 || codePoint == 0x0167 || codePoint == 0x016b || codePoint == 0x01ce || codePoint == 0x01d0 || codePoint == 0x01d2 || codePoint == 0x01d4 || codePoint == 0x01d6 || codePoint == 0x01d8 || codePoint == 0x01da || codePoint == 0x01dc || codePoint == 0x0251 || codePoint == 0x0261 || codePoint == 0x02c4 || codePoint == 0x02c7 || (0x02c9 <= codePoint && codePoint <= 0x02cb) || codePoint == 0x02cd || codePoint == 0x02d0 || (0x02d8 <= codePoint && codePoint <= 0x02db) || codePoint == 0x02dd || codePoint == 0x02df || (0x0300 <= codePoint && codePoint <= 0x036f) || (0x0391 <= codePoint && codePoint <= 0x03a9) || (0x03b1 <= codePoint && codePoint <= 0x03c1) || (0x03c3 <= codePoint && codePoint <= 0x03c9) || codePoint == 0x0401 || (0x0410 <= codePoint && codePoint <= 0x044f) || codePoint == 0x0451 || codePoint == 0x2010 || (0x2013 <= codePoint && codePoint <= 0x2016) || codePoint == 0x2018 || codePoint == 0x2019 || codePoint == 0x201c || codePoint == 0x201d || (0x2020 <= codePoint && codePoint <= 0x2022) || (0x2024 <= codePoint && codePoint <= 0x2027) || codePoint == 0x2030 || codePoint == 0x2032 || codePoint == 0x2033 || codePoint == 0x2035 || codePoint == 0x203b || codePoint == 0x203e || codePoint == 0x2074 || codePoint == 0x207f || (0x2081 <= codePoint && codePoint <= 0x2084) || codePoint == 0x20ac || codePoint == 0x2103 || codePoint == 0x2105 || codePoint == 0x2109 || codePoint == 0x2113 || codePoint == 0x2116 || codePoint == 0x2121 || codePoint == 0x2122 || codePoint == 0x2126 || codePoint == 0x212b || codePoint == 0x2153 || codePoint == 0x2154 || (0x215b <= codePoint && codePoint <= 0x215e) || (0x2160 <= codePoint && codePoint <= 0x216b) || (0x2170 <= codePoint && codePoint <= 0x2179) || (0x2189 <= codePoint && codePoint <= 0x2199) || codePoint == 0x21b8 || codePoint == 0x21b9 || codePoint == 0x21d2 || codePoint == 0x21d4 || codePoint == 0x21e7 || codePoint == 0x2200 || codePoint == 0x2202 || codePoint == 0x2203 || codePoint == 0x2207 || codePoint == 0x2208 || codePoint == 0x220b || codePoint == 0x220f || codePoint == 0x2211 || codePoint == 0x2215 || codePoint == 0x221a || (0x221d <= codePoint && codePoint <= 0x2220) || codePoint == 0x2223 || codePoint == 0x2225 || (0x2227 <= codePoint && codePoint <= 0x222c) || codePoint == 0x222e || (0x2234 <= codePoint && codePoint <= 0x2237) || codePoint == 0x223c || codePoint == 0x223d || codePoint == 0x2248 || codePoint == 0x224c || codePoint == 0x2252 || codePoint == 0x2260 || codePoint == 0x2261 || (0x2264 <= codePoint && codePoint <= 0x2267) || codePoint == 0x226a || codePoint == 0x226b || codePoint == 0x226e || codePoint == 0x226f || codePoint == 0x2282 || codePoint == 0x2283 || codePoint == 0x2286 || codePoint == 0x2287 || codePoint == 0x2295 || codePoint == 0x2299 || codePoint == 0x22a5 || codePoint == 0x22bf || codePoint == 0x2312 || (0x2460 <= codePoint && codePoint <= 0x24e9) || (0x24eb <= codePoint && codePoint <= 0x254b) || (0x2550 <= codePoint && codePoint <= 0x2573) || (0x2580 <= codePoint && codePoint <= 0x258f) || (0x2592 <= codePoint && codePoint <= 0x2595) || codePoint == 0x25a0 || codePoint == 0x25a1 || (0x25a3 <= codePoint && codePoint <= 0x25a9) || codePoint == 0x25b2 || codePoint == 0x25b3 || codePoint == 0x25b6 || codePoint == 0x25b7 || codePoint == 0x25bc || codePoint == 0x25bd || codePoint == 0x25c0 || codePoint == 0x25c1 || (0x25c6 <= codePoint && codePoint <= 0x25c8) || codePoint == 0x25cb || (0x25ce <= codePoint && codePoint <= 0x25d1) || (0x25e2 <= codePoint && codePoint <= 0x25e5) || codePoint == 0x25ef || codePoint == 0x2605 || codePoint == 0x2606 || codePoint == 0x2609 || codePoint == 0x260e || codePoint == 0x260f || codePoint == 0x2614 || codePoint == 0x2615 || codePoint == 0x261c || codePoint == 0x261e || codePoint == 0x2640 || codePoint == 0x2642 || codePoint == 0x2660 || codePoint == 0x2661 || (0x2663 <= codePoint && codePoint <= 0x2665) || (0x2667 <= codePoint && codePoint <= 0x266a) || codePoint == 0x266c || codePoint == 0x266d || codePoint == 0x266f || codePoint == 0x269e || codePoint == 0x269f || codePoint == 0x26be || codePoint == 0x26bf || (0x26c4 <= codePoint && codePoint <= 0x26ff) || codePoint == 0x273d || codePoint == 0x2757 || (0x2776 <= codePoint && codePoint <= 0x277f) || (0x2b55 <= codePoint && codePoint <= 0x2b59) || (0x3248 <= codePoint && codePoint <= 0x324f) || (0xe000 <= codePoint && codePoint <= 0xf8ff) || (0xfe00 <= codePoint && codePoint <= 0xfe0f) || codePoint == 0xfffd || (0x1f100 <= codePoint && codePoint <= 0x1f12d) || (0x1f131 <= codePoint && codePoint <= 0x1f190) || (0xe0100 <= codePoint && codePoint <= 0xe01ef) || (0xf0000 <= codePoint && codePoint <= 0xffffd) || (0x100000 <= codePoint && codePoint <= 0x10fffd))
            return cp == 0x00a1 || cp == 0x00a4 || cp == 0x00a7 || cp == 0x00a8 || cp == 0x00aa || cp == 0x00ad || cp == 0x00ae || (0x00b0 <= cp && cp <= 0x00b4) || (0x00b6 <= cp && cp <= 0x00ba) || (0x00bc <= cp && cp <= 0x00bf) || cp == 0x00c6 || cp == 0x00d0 || cp == 0x00d7 || cp == 0x00d8 || (0x00de <= cp && cp <= 0x00e1) || cp == 0x00e6 || (0x00e8 <= cp && cp <= 0x00ea) || cp == 0x00ec || cp == 0x00ed || cp == 0x00f0 || cp == 0x00f2 || cp == 0x00f3 || (0x00f7 <= cp && cp <= 0x00fa) || cp == 0x00fc || cp == 0x00fe || cp == 0x0101 || cp == 0x0111 || cp == 0x0113 || cp == 0x011b || cp == 0x0126 || cp == 0x0127 || cp == 0x012b || (0x0131 <= cp && cp <= 0x0133) || cp == 0x0138 || (0x013f <= cp && cp <= 0x0142) || cp == 0x0144 || (0x0148 <= cp && cp <= 0x014b) || cp == 0x014d || cp == 0x0152 || cp == 0x0153 || cp == 0x0166 || cp == 0x0167 || cp == 0x016b || cp == 0x01ce || cp == 0x01d0 || cp == 0x01d2 || cp == 0x01d4 || cp == 0x01d6 || cp == 0x01d8 || cp == 0x01da || cp == 0x01dc || cp == 0x0251 || cp == 0x0261 || cp == 0x02c4 || cp == 0x02c7 || (0x02c9 <= cp && cp <= 0x02cb) || cp == 0x02cd || cp == 0x02d0 || (0x02d8 <= cp && cp <= 0x02db) || cp == 0x02dd || cp == 0x02df || (0x0300 <= cp && cp <= 0x036f) || (0x0391 <= cp && cp <= 0x03a9) || (0x03b1 <= cp && cp <= 0x03c1) || (0x03c3 <= cp && cp <= 0x03c9) || cp == 0x0401 || (0x0410 <= cp && cp <= 0x044f) || cp == 0x0451 || cp == 0x2010 || (0x2013 <= cp && cp <= 0x2016) || cp == 0x2018 || cp == 0x2019 || cp == 0x201c || cp == 0x201d || (0x2020 <= cp && cp <= 0x2022) || (0x2024 <= cp && cp <= 0x2027) || cp == 0x2030 || cp == 0x2032 || cp == 0x2033 || cp == 0x2035 || cp == 0x203b || cp == 0x203e || cp == 0x2074 || cp == 0x207f || (0x2081 <= cp && cp <= 0x2084) || cp == 0x20ac || cp == 0x2103 || cp == 0x2105 || cp == 0x2109 || cp == 0x2113 || cp == 0x2116 || cp == 0x2121 || cp == 0x2122 || cp == 0x2126 || cp == 0x212b || cp == 0x2153 || cp == 0x2154 || (0x215b <= cp && cp <= 0x215e) || (0x2160 <= cp && cp <= 0x216b) || (0x2170 <= cp && cp <= 0x2179) || (0x2189 <= cp && cp <= 0x2199) || cp == 0x21b8 || cp == 0x21b9 || cp == 0x21d2 || cp == 0x21d4 || cp == 0x21e7 || cp == 0x2200 || cp == 0x2202 || cp == 0x2203 || cp == 0x2207 || cp == 0x2208 || cp == 0x220b || cp == 0x220f || cp == 0x2211 || cp == 0x2215 || cp == 0x221a || (0x221d <= cp && cp <= 0x2220) || cp == 0x2223 || cp == 0x2225 || (0x2227 <= cp && cp <= 0x222c) || cp == 0x222e || (0x2234 <= cp && cp <= 0x2237) || cp == 0x223c || cp == 0x223d || cp == 0x2248 || cp == 0x224c || cp == 0x2252 || cp == 0x2260 || cp == 0x2261 || (0x2264 <= cp && cp <= 0x2267) || cp == 0x226a || cp == 0x226b || cp == 0x226e || cp == 0x226f || cp == 0x2282 || cp == 0x2283 || cp == 0x2286 || cp == 0x2287 || cp == 0x2295 || cp == 0x2299 || cp == 0x22a5 || cp == 0x22bf || cp == 0x2312 || (0x2460 <= cp && cp <= 0x24e9) || (0x24eb <= cp && cp <= 0x254b) || (0x2550 <= cp && cp <= 0x2573) || (0x2580 <= cp && cp <= 0x258f) || (0x2592 <= cp && cp <= 0x2595) || cp == 0x25a0 || cp == 0x25a1 || (0x25a3 <= cp && cp <= 0x25a9) || cp == 0x25b2 || cp == 0x25b3 || cp == 0x25b6 || cp == 0x25b7 || cp == 0x25bc || cp == 0x25bd || cp == 0x25c0 || cp == 0x25c1 || (0x25c6 <= cp && cp <= 0x25c8) || cp == 0x25cb || (0x25ce <= cp && cp <= 0x25d1) || (0x25e2 <= cp && cp <= 0x25e5) || cp == 0x25ef || cp == 0x2605 || cp == 0x2606 || cp == 0x2609 || cp == 0x260e || cp == 0x260f || cp == 0x2614 || cp == 0x2615 || cp == 0x261c || cp == 0x261e || cp == 0x2640 || cp == 0x2642 || cp == 0x2660 || cp == 0x2661 || (0x2663 <= cp && cp <= 0x2665) || (0x2667 <= cp && cp <= 0x266a) || cp == 0x266c || cp == 0x266d || cp == 0x266f || cp == 0x269e || cp == 0x269f || cp == 0x26be || cp == 0x26bf || (0x26c4 <= cp && cp <= 0x26ff) || cp == 0x273d || cp == 0x2757 || (0x2776 <= cp && cp <= 0x277f) || (0x2b55 <= cp && cp <= 0x2b59) || (0x3248 <= cp && cp <= 0x324f) || (0xe000 <= cp && cp <= 0xf8ff) || (0xfe00 <= cp && cp <= 0xfe0f) || cp == 0xfffd;
        }

        public static bool InEastAsianAmbiguous(this int codePoint)
            => codePoint == 0x00a1 || codePoint == 0x00a4 || codePoint == 0x00a7 || codePoint == 0x00a8 || codePoint == 0x00aa || codePoint == 0x00ad || codePoint == 0x00ae || (0x00b0 <= codePoint && codePoint <= 0x00b4) || (0x00b6 <= codePoint && codePoint <= 0x00ba) || (0x00bc <= codePoint && codePoint <= 0x00bf) || codePoint == 0x00c6 || codePoint == 0x00d0 || codePoint == 0x00d7 || codePoint == 0x00d8 || (0x00de <= codePoint && codePoint <= 0x00e1) || codePoint == 0x00e6 || (0x00e8 <= codePoint && codePoint <= 0x00ea) || codePoint == 0x00ec || codePoint == 0x00ed || codePoint == 0x00f0 || codePoint == 0x00f2 || codePoint == 0x00f3 || (0x00f7 <= codePoint && codePoint <= 0x00fa) || codePoint == 0x00fc || codePoint == 0x00fe || codePoint == 0x0101 || codePoint == 0x0111 || codePoint == 0x0113 || codePoint == 0x011b || codePoint == 0x0126 || codePoint == 0x0127 || codePoint == 0x012b || (0x0131 <= codePoint && codePoint <= 0x0133) || codePoint == 0x0138 || (0x013f <= codePoint && codePoint <= 0x0142) || codePoint == 0x0144 || (0x0148 <= codePoint && codePoint <= 0x014b) || codePoint == 0x014d || codePoint == 0x0152 || codePoint == 0x0153 || codePoint == 0x0166 || codePoint == 0x0167 || codePoint == 0x016b || codePoint == 0x01ce || codePoint == 0x01d0 || codePoint == 0x01d2 || codePoint == 0x01d4 || codePoint == 0x01d6 || codePoint == 0x01d8 || codePoint == 0x01da || codePoint == 0x01dc || codePoint == 0x0251 || codePoint == 0x0261 || codePoint == 0x02c4 || codePoint == 0x02c7 || (0x02c9 <= codePoint && codePoint <= 0x02cb) || codePoint == 0x02cd || codePoint == 0x02d0 || (0x02d8 <= codePoint && codePoint <= 0x02db) || codePoint == 0x02dd || codePoint == 0x02df || (0x0300 <= codePoint && codePoint <= 0x036f) || (0x0391 <= codePoint && codePoint <= 0x03a9) || (0x03b1 <= codePoint && codePoint <= 0x03c1) || (0x03c3 <= codePoint && codePoint <= 0x03c9) || codePoint == 0x0401 || (0x0410 <= codePoint && codePoint <= 0x044f) || codePoint == 0x0451 || codePoint == 0x2010 || (0x2013 <= codePoint && codePoint <= 0x2016) || codePoint == 0x2018 || codePoint == 0x2019 || codePoint == 0x201c || codePoint == 0x201d || (0x2020 <= codePoint && codePoint <= 0x2022) || (0x2024 <= codePoint && codePoint <= 0x2027) || codePoint == 0x2030 || codePoint == 0x2032 || codePoint == 0x2033 || codePoint == 0x2035 || codePoint == 0x203b || codePoint == 0x203e || codePoint == 0x2074 || codePoint == 0x207f || (0x2081 <= codePoint && codePoint <= 0x2084) || codePoint == 0x20ac || codePoint == 0x2103 || codePoint == 0x2105 || codePoint == 0x2109 || codePoint == 0x2113 || codePoint == 0x2116 || codePoint == 0x2121 || codePoint == 0x2122 || codePoint == 0x2126 || codePoint == 0x212b || codePoint == 0x2153 || codePoint == 0x2154 || (0x215b <= codePoint && codePoint <= 0x215e) || (0x2160 <= codePoint && codePoint <= 0x216b) || (0x2170 <= codePoint && codePoint <= 0x2179) || (0x2189 <= codePoint && codePoint <= 0x2199) || codePoint == 0x21b8 || codePoint == 0x21b9 || codePoint == 0x21d2 || codePoint == 0x21d4 || codePoint == 0x21e7 || codePoint == 0x2200 || codePoint == 0x2202 || codePoint == 0x2203 || codePoint == 0x2207 || codePoint == 0x2208 || codePoint == 0x220b || codePoint == 0x220f || codePoint == 0x2211 || codePoint == 0x2215 || codePoint == 0x221a || (0x221d <= codePoint && codePoint <= 0x2220) || codePoint == 0x2223 || codePoint == 0x2225 || (0x2227 <= codePoint && codePoint <= 0x222c) || codePoint == 0x222e || (0x2234 <= codePoint && codePoint <= 0x2237) || codePoint == 0x223c || codePoint == 0x223d || codePoint == 0x2248 || codePoint == 0x224c || codePoint == 0x2252 || codePoint == 0x2260 || codePoint == 0x2261 || (0x2264 <= codePoint && codePoint <= 0x2267) || codePoint == 0x226a || codePoint == 0x226b || codePoint == 0x226e || codePoint == 0x226f || codePoint == 0x2282 || codePoint == 0x2283 || codePoint == 0x2286 || codePoint == 0x2287 || codePoint == 0x2295 || codePoint == 0x2299 || codePoint == 0x22a5 || codePoint == 0x22bf || codePoint == 0x2312 || (0x2460 <= codePoint && codePoint <= 0x24e9) || (0x24eb <= codePoint && codePoint <= 0x254b) || (0x2550 <= codePoint && codePoint <= 0x2573) || (0x2580 <= codePoint && codePoint <= 0x258f) || (0x2592 <= codePoint && codePoint <= 0x2595) || codePoint == 0x25a0 || codePoint == 0x25a1 || (0x25a3 <= codePoint && codePoint <= 0x25a9) || codePoint == 0x25b2 || codePoint == 0x25b3 || codePoint == 0x25b6 || codePoint == 0x25b7 || codePoint == 0x25bc || codePoint == 0x25bd || codePoint == 0x25c0 || codePoint == 0x25c1 || (0x25c6 <= codePoint && codePoint <= 0x25c8) || codePoint == 0x25cb || (0x25ce <= codePoint && codePoint <= 0x25d1) || (0x25e2 <= codePoint && codePoint <= 0x25e5) || codePoint == 0x25ef || codePoint == 0x2605 || codePoint == 0x2606 || codePoint == 0x2609 || codePoint == 0x260e || codePoint == 0x260f || codePoint == 0x2614 || codePoint == 0x2615 || codePoint == 0x261c || codePoint == 0x261e || codePoint == 0x2640 || codePoint == 0x2642 || codePoint == 0x2660 || codePoint == 0x2661 || (0x2663 <= codePoint && codePoint <= 0x2665) || (0x2667 <= codePoint && codePoint <= 0x266a) || codePoint == 0x266c || codePoint == 0x266d || codePoint == 0x266f || codePoint == 0x269e || codePoint == 0x269f || codePoint == 0x26be || codePoint == 0x26bf || (0x26c4 <= codePoint && codePoint <= 0x26ff) || codePoint == 0x273d || codePoint == 0x2757 || (0x2776 <= codePoint && codePoint <= 0x277f) || (0x2b55 <= codePoint && codePoint <= 0x2b59) || (0x3248 <= codePoint && codePoint <= 0x324f) || (0xe000 <= codePoint && codePoint <= 0xf8ff) || (0xfe00 <= codePoint && codePoint <= 0xfe0f) || codePoint == 0xfffd || (0x1f100 <= codePoint && codePoint <= 0x1f12d) || (0x1f131 <= codePoint && codePoint <= 0x1f190) || (0xe0100 <= codePoint && codePoint <= 0xe01ef) || (0xf0000 <= codePoint && codePoint <= 0xffffd) || (0x100000 <= codePoint && codePoint <= 0x10fffd);

        public static bool InEastAsianFullwidth(this char chr)
        {
            ushort cp = chr;
            return cp == 0x3000 || (0xff01 <= cp && cp <= 0xff60) || (0xffe0 <= cp && cp <= 0xffe6);
        }

        public static bool InEastAsianFullwidth(this int codePoint)
            => codePoint == 0x3000 || (0xff01 <= codePoint && codePoint <= 0xff60) || (0xffe0 <= codePoint && codePoint <= 0xffe6);

        public static bool InEastAsianHalfwidth(this char chr)
        {
            ushort cp = chr;
            return cp == 0x20a9 || (0xff61 <= cp && cp <= 0xffdc) || (0xffe8 <= cp && cp <= 0xffee);
        }

        public static bool InEastAsianHalfwidth(this int codePoint)
            => codePoint == 0x20a9 || (0xff61 <= codePoint && codePoint <= 0xffdc) || (0xffe8 <= codePoint && codePoint <= 0xffee);

        public static bool InEastAsianNarrow(this char chr)
        {
            ushort cp = chr;
            return (0x0020 <= cp && cp <= 0x007e) || cp == 0x00a2 || cp == 0x00a3 || cp == 0x00a5 || cp == 0x00a6 || cp == 0x00ac || cp == 0x00af || (0x27e6 <= cp && cp <= 0x27ed) || cp == 0x2985 || cp == 0x2986;
        }

        public static bool InEastAsianNarrow(this int codePoint)
            => (0x0020 <= codePoint && codePoint <= 0x007e) || codePoint == 0x00a2 || codePoint == 0x00a3 || codePoint == 0x00a5 || codePoint == 0x00a6 || codePoint == 0x00ac || codePoint == 0x00af || (0x27e6 <= codePoint && codePoint <= 0x27ed) || codePoint == 0x2985 || codePoint == 0x2986;

        public static bool InEastAsianNeutral(this char chr)
        {
            ushort cp = chr;
            //return ((0x0000 <= codePoint && codePoint <= 0x001f) || (0x007f <= codePoint && codePoint <= 0x00a0) || codePoint == 0x00a9 || codePoint == 0x00ab || codePoint == 0x00b5 || codePoint == 0x00bb || (0x00c0 <= codePoint && codePoint <= 0x00c5) || (0x00c7 <= codePoint && codePoint <= 0x00cf) || (0x00d1 <= codePoint && codePoint <= 0x00d6) || (0x00d9 <= codePoint && codePoint <= 0x00dd) || (0x00e2 <= codePoint && codePoint <= 0x00e5) || codePoint == 0x00e7 || codePoint == 0x00eb || codePoint == 0x00ee || codePoint == 0x00ef || codePoint == 0x00f1 || (0x00f4 <= codePoint && codePoint <= 0x00f6) || codePoint == 0x00fb || codePoint == 0x00fd || codePoint == 0x00ff || codePoint == 0x0100 || (0x0102 <= codePoint && codePoint <= 0x0110) || codePoint == 0x0112 || (0x0114 <= codePoint && codePoint <= 0x011a) || (0x011c <= codePoint && codePoint <= 0x0125) || (0x0128 <= codePoint && codePoint <= 0x012a) || (0x012c <= codePoint && codePoint <= 0x0130) || (0x0134 <= codePoint && codePoint <= 0x0137) || (0x0139 <= codePoint && codePoint <= 0x013e) || codePoint == 0x0143 || (0x0145 <= codePoint && codePoint <= 0x0147) || codePoint == 0x014c || (0x014e <= codePoint && codePoint <= 0x0151) || (0x0154 <= codePoint && codePoint <= 0x0165) || (0x0168 <= codePoint && codePoint <= 0x016a) || (0x016c <= codePoint && codePoint <= 0x01cd) || codePoint == 0x01cf || codePoint == 0x01d1 || codePoint == 0x01d3 || codePoint == 0x01d5 || codePoint == 0x01d7 || codePoint == 0x01d9 || codePoint == 0x01db || (0x01dd <= codePoint && codePoint <= 0x0250) || (0x0252 <= codePoint && codePoint <= 0x0260) || (0x0262 <= codePoint && codePoint <= 0x02c3) || codePoint == 0x02c5 || codePoint == 0x02c6 || codePoint == 0x02c8 || codePoint == 0x02cc || codePoint == 0x02ce || codePoint == 0x02cf || (0x02d1 <= codePoint && codePoint <= 0x02d7) || codePoint == 0x02dc || codePoint == 0x02de || (0x02e0 <= codePoint && codePoint <= 0x02ff) || (0x0370 <= codePoint && codePoint <= 0x0390) || (0x03aa <= codePoint && codePoint <= 0x03b0) || codePoint == 0x03c2 || (0x03ca <= codePoint && codePoint <= 0x0400) || (0x0402 <= codePoint && codePoint <= 0x040f) || codePoint == 0x0450 || (0x0452 <= codePoint && codePoint <= 0x10fc) || (0x1160 <= codePoint && codePoint <= 0x11a2) || (0x11a8 <= codePoint && codePoint <= 0x11f9) || (0x1200 <= codePoint && codePoint <= 0x200f) || codePoint == 0x2011 || codePoint == 0x2012 || codePoint == 0x2017 || codePoint == 0x201a || codePoint == 0x201b || codePoint == 0x201e || codePoint == 0x201f || codePoint == 0x2023 || (0x2028 <= codePoint && codePoint <= 0x202f) || codePoint == 0x2031 || codePoint == 0x2034 || (0x2036 <= codePoint && codePoint <= 0x203a) || codePoint == 0x203c || codePoint == 0x203d || (0x203f <= codePoint && codePoint <= 0x2071) || (0x2075 <= codePoint && codePoint <= 0x207e) || codePoint == 0x2080 || (0x2085 <= codePoint && codePoint <= 0x20a8) || codePoint == 0x20aa || codePoint == 0x20ab || (0x20ad <= codePoint && codePoint <= 0x2102) || codePoint == 0x2104 || (0x2106 <= codePoint && codePoint <= 0x2108) || (0x210a <= codePoint && codePoint <= 0x2112) || codePoint == 0x2114 || codePoint == 0x2115 || (0x2117 <= codePoint && codePoint <= 0x2120) || (0x2123 <= codePoint && codePoint <= 0x2125) || (0x2127 <= codePoint && codePoint <= 0x212a) || (0x212c <= codePoint && codePoint <= 0x2152) || (0x2155 <= codePoint && codePoint <= 0x215a) || codePoint == 0x215f || (0x216c <= codePoint && codePoint <= 0x216f) || (0x217a <= codePoint && codePoint <= 0x2188) || (0x219a <= codePoint && codePoint <= 0x21b7) || (0x21ba <= codePoint && codePoint <= 0x21d1) || codePoint == 0x21d3 || (0x21d5 <= codePoint && codePoint <= 0x21e6) || (0x21e8 <= codePoint && codePoint <= 0x21ff) || codePoint == 0x2201 || (0x2204 <= codePoint && codePoint <= 0x2206) || codePoint == 0x2209 || codePoint == 0x220a || (0x220c <= codePoint && codePoint <= 0x220e) || codePoint == 0x2210 || (0x2212 <= codePoint && codePoint <= 0x2214) || (0x2216 <= codePoint && codePoint <= 0x2219) || codePoint == 0x221b || codePoint == 0x221c || codePoint == 0x2221 || codePoint == 0x2222 || codePoint == 0x2224 || codePoint == 0x2226 || codePoint == 0x222d || (0x222f <= codePoint && codePoint <= 0x2233) || (0x2238 <= codePoint && codePoint <= 0x223b) || (0x223e <= codePoint && codePoint <= 0x2247) || (0x2249 <= codePoint && codePoint <= 0x224b) || (0x224d <= codePoint && codePoint <= 0x2251) || (0x2253 <= codePoint && codePoint <= 0x225f) || codePoint == 0x2262 || codePoint == 0x2263 || codePoint == 0x2268 || codePoint == 0x2269 || codePoint == 0x226c || codePoint == 0x226d || (0x2270 <= codePoint && codePoint <= 0x2281) || codePoint == 0x2284 || codePoint == 0x2285 || (0x2288 <= codePoint && codePoint <= 0x2294) || (0x2296 <= codePoint && codePoint <= 0x2298) || (0x229a <= codePoint && codePoint <= 0x22a4) || (0x22a6 <= codePoint && codePoint <= 0x22be) || (0x22c0 <= codePoint && codePoint <= 0x2311) || (0x2313 <= codePoint && codePoint <= 0x2328) || (0x232b <= codePoint && codePoint <= 0x244a) || codePoint == 0x24ea || (0x254c <= codePoint && codePoint <= 0x254f) || (0x2574 <= codePoint && codePoint <= 0x257f) || codePoint == 0x2590 || codePoint == 0x2591 || (0x2596 <= codePoint && codePoint <= 0x259f) || codePoint == 0x25a2 || (0x25aa <= codePoint && codePoint <= 0x25b1) || codePoint == 0x25b4 || codePoint == 0x25b5 || (0x25b8 <= codePoint && codePoint <= 0x25bb) || codePoint == 0x25be || codePoint == 0x25bf || (0x25c2 <= codePoint && codePoint <= 0x25c5) || codePoint == 0x25c9 || codePoint == 0x25ca || codePoint == 0x25cc || codePoint == 0x25cd || (0x25d2 <= codePoint && codePoint <= 0x25e1) || (0x25e6 <= codePoint && codePoint <= 0x25ee) || (0x25f0 <= codePoint && codePoint <= 0x2604) || codePoint == 0x2607 || codePoint == 0x2608 || (0x260a <= codePoint && codePoint <= 0x260d) || (0x2610 <= codePoint && codePoint <= 0x2613) || (0x2616 <= codePoint && codePoint <= 0x261b) || codePoint == 0x261d || (0x261f <= codePoint && codePoint <= 0x263f) || codePoint == 0x2641 || (0x2643 <= codePoint && codePoint <= 0x265f) || codePoint == 0x2662 || codePoint == 0x2666 || codePoint == 0x266b || codePoint == 0x266e || (0x2670 <= codePoint && codePoint <= 0x269d) || (0x26a0 <= codePoint && codePoint <= 0x26bd) || (0x26c0 <= codePoint && codePoint <= 0x26c3) || (0x2701 <= codePoint && codePoint <= 0x273c) || (0x273e <= codePoint && codePoint <= 0x2756) || (0x2758 <= codePoint && codePoint <= 0x2775) || (0x2780 <= codePoint && codePoint <= 0x27e5) || (0x27ee <= codePoint && codePoint <= 0x2984) || (0x2987 <= codePoint && codePoint <= 0x2b54) || (0x2c00 <= codePoint && codePoint <= 0x2e31) || codePoint == 0x303f || (0x4dc0 <= codePoint && codePoint <= 0x4dff) || (0xa4d0 <= codePoint && codePoint <= 0xa95f) || (0xa980 <= codePoint && codePoint <= 0xabf9) || (0xd800 <= codePoint && codePoint <= 0xdb7f) || (0xdb80 <= codePoint && codePoint <= 0xdbff) || (0xdc00 <= codePoint && codePoint <= 0xdfff) || (0xfb00 <= codePoint && codePoint <= 0xfdfd) || (0xfe20 <= codePoint && codePoint <= 0xfe26) || (0xfe70 <= codePoint && codePoint <= 0xfeff) || (0xfff9 <= codePoint && codePoint <= 0xfffc) || (0x10000 <= codePoint && codePoint <= 0x1f093) || codePoint == 0x1f12e || (0xe0001 <= codePoint && codePoint <= 0xe007f))
            return (0x0000 <= cp && cp <= 0x001f) || (0x007f <= cp && cp <= 0x00a0) || cp == 0x00a9 || cp == 0x00ab || cp == 0x00b5 || cp == 0x00bb || (0x00c0 <= cp && cp <= 0x00c5) || (0x00c7 <= cp && cp <= 0x00cf) || (0x00d1 <= cp && cp <= 0x00d6) || (0x00d9 <= cp && cp <= 0x00dd) || (0x00e2 <= cp && cp <= 0x00e5) || cp == 0x00e7 || cp == 0x00eb || cp == 0x00ee || cp == 0x00ef || cp == 0x00f1 || (0x00f4 <= cp && cp <= 0x00f6) || cp == 0x00fb || cp == 0x00fd || cp == 0x00ff || cp == 0x0100 || (0x0102 <= cp && cp <= 0x0110) || cp == 0x0112 || (0x0114 <= cp && cp <= 0x011a) || (0x011c <= cp && cp <= 0x0125) || (0x0128 <= cp && cp <= 0x012a) || (0x012c <= cp && cp <= 0x0130) || (0x0134 <= cp && cp <= 0x0137) || (0x0139 <= cp && cp <= 0x013e) || cp == 0x0143 || (0x0145 <= cp && cp <= 0x0147) || cp == 0x014c || (0x014e <= cp && cp <= 0x0151) || (0x0154 <= cp && cp <= 0x0165) || (0x0168 <= cp && cp <= 0x016a) || (0x016c <= cp && cp <= 0x01cd) || cp == 0x01cf || cp == 0x01d1 || cp == 0x01d3 || cp == 0x01d5 || cp == 0x01d7 || cp == 0x01d9 || cp == 0x01db || (0x01dd <= cp && cp <= 0x0250) || (0x0252 <= cp && cp <= 0x0260) || (0x0262 <= cp && cp <= 0x02c3) || cp == 0x02c5 || cp == 0x02c6 || cp == 0x02c8 || cp == 0x02cc || cp == 0x02ce || cp == 0x02cf || (0x02d1 <= cp && cp <= 0x02d7) || cp == 0x02dc || cp == 0x02de || (0x02e0 <= cp && cp <= 0x02ff) || (0x0370 <= cp && cp <= 0x0390) || (0x03aa <= cp && cp <= 0x03b0) || cp == 0x03c2 || (0x03ca <= cp && cp <= 0x0400) || (0x0402 <= cp && cp <= 0x040f) || cp == 0x0450 || (0x0452 <= cp && cp <= 0x10fc) || (0x1160 <= cp && cp <= 0x11a2) || (0x11a8 <= cp && cp <= 0x11f9) || (0x1200 <= cp && cp <= 0x200f) || cp == 0x2011 || cp == 0x2012 || cp == 0x2017 || cp == 0x201a || cp == 0x201b || cp == 0x201e || cp == 0x201f || cp == 0x2023 || (0x2028 <= cp && cp <= 0x202f) || cp == 0x2031 || cp == 0x2034 || (0x2036 <= cp && cp <= 0x203a) || cp == 0x203c || cp == 0x203d || (0x203f <= cp && cp <= 0x2071) || (0x2075 <= cp && cp <= 0x207e) || cp == 0x2080 || (0x2085 <= cp && cp <= 0x20a8) || cp == 0x20aa || cp == 0x20ab || (0x20ad <= cp && cp <= 0x2102) || cp == 0x2104 || (0x2106 <= cp && cp <= 0x2108) || (0x210a <= cp && cp <= 0x2112) || cp == 0x2114 || cp == 0x2115 || (0x2117 <= cp && cp <= 0x2120) || (0x2123 <= cp && cp <= 0x2125) || (0x2127 <= cp && cp <= 0x212a) || (0x212c <= cp && cp <= 0x2152) || (0x2155 <= cp && cp <= 0x215a) || cp == 0x215f || (0x216c <= cp && cp <= 0x216f) || (0x217a <= cp && cp <= 0x2188) || (0x219a <= cp && cp <= 0x21b7) || (0x21ba <= cp && cp <= 0x21d1) || cp == 0x21d3 || (0x21d5 <= cp && cp <= 0x21e6) || (0x21e8 <= cp && cp <= 0x21ff) || cp == 0x2201 || (0x2204 <= cp && cp <= 0x2206) || cp == 0x2209 || cp == 0x220a || (0x220c <= cp && cp <= 0x220e) || cp == 0x2210 || (0x2212 <= cp && cp <= 0x2214) || (0x2216 <= cp && cp <= 0x2219) || cp == 0x221b || cp == 0x221c || cp == 0x2221 || cp == 0x2222 || cp == 0x2224 || cp == 0x2226 || cp == 0x222d || (0x222f <= cp && cp <= 0x2233) || (0x2238 <= cp && cp <= 0x223b) || (0x223e <= cp && cp <= 0x2247) || (0x2249 <= cp && cp <= 0x224b) || (0x224d <= cp && cp <= 0x2251) || (0x2253 <= cp && cp <= 0x225f) || cp == 0x2262 || cp == 0x2263 || cp == 0x2268 || cp == 0x2269 || cp == 0x226c || cp == 0x226d || (0x2270 <= cp && cp <= 0x2281) || cp == 0x2284 || cp == 0x2285 || (0x2288 <= cp && cp <= 0x2294) || (0x2296 <= cp && cp <= 0x2298) || (0x229a <= cp && cp <= 0x22a4) || (0x22a6 <= cp && cp <= 0x22be) || (0x22c0 <= cp && cp <= 0x2311) || (0x2313 <= cp && cp <= 0x2328) || (0x232b <= cp && cp <= 0x244a) || cp == 0x24ea || (0x254c <= cp && cp <= 0x254f) || (0x2574 <= cp && cp <= 0x257f) || cp == 0x2590 || cp == 0x2591 || (0x2596 <= cp && cp <= 0x259f) || cp == 0x25a2 || (0x25aa <= cp && cp <= 0x25b1) || cp == 0x25b4 || cp == 0x25b5 || (0x25b8 <= cp && cp <= 0x25bb) || cp == 0x25be || cp == 0x25bf || (0x25c2 <= cp && cp <= 0x25c5) || cp == 0x25c9 || cp == 0x25ca || cp == 0x25cc || cp == 0x25cd || (0x25d2 <= cp && cp <= 0x25e1) || (0x25e6 <= cp && cp <= 0x25ee) || (0x25f0 <= cp && cp <= 0x2604) || cp == 0x2607 || cp == 0x2608 || (0x260a <= cp && cp <= 0x260d) || (0x2610 <= cp && cp <= 0x2613) || (0x2616 <= cp && cp <= 0x261b) || cp == 0x261d || (0x261f <= cp && cp <= 0x263f) || cp == 0x2641 || (0x2643 <= cp && cp <= 0x265f) || cp == 0x2662 || cp == 0x2666 || cp == 0x266b || cp == 0x266e || (0x2670 <= cp && cp <= 0x269d) || (0x26a0 <= cp && cp <= 0x26bd) || (0x26c0 <= cp && cp <= 0x26c3) || (0x2701 <= cp && cp <= 0x273c) || (0x273e <= cp && cp <= 0x2756) || (0x2758 <= cp && cp <= 0x2775) || (0x2780 <= cp && cp <= 0x27e5) || (0x27ee <= cp && cp <= 0x2984) || (0x2987 <= cp && cp <= 0x2b54) || (0x2c00 <= cp && cp <= 0x2e31) || cp == 0x303f || (0x4dc0 <= cp && cp <= 0x4dff) || (0xa4d0 <= cp && cp <= 0xa95f) || (0xa980 <= cp && cp <= 0xabf9) || (0xd800 <= cp && cp <= 0xdb7f) || (0xdb80 <= cp && cp <= 0xdbff) || (0xdc00 <= cp && cp <= 0xdfff) || (0xfb00 <= cp && cp <= 0xfdfd) || (0xfe20 <= cp && cp <= 0xfe26) || (0xfe70 <= cp && cp <= 0xfeff) || (0xfff9 <= cp && cp <= 0xfffc);
        }

        public static bool InEastAsianNeutral(this int codePoint)
            => (0x0000 <= codePoint && codePoint <= 0x001f) || (0x007f <= codePoint && codePoint <= 0x00a0) || codePoint == 0x00a9 || codePoint == 0x00ab || codePoint == 0x00b5 || codePoint == 0x00bb || (0x00c0 <= codePoint && codePoint <= 0x00c5) || (0x00c7 <= codePoint && codePoint <= 0x00cf) || (0x00d1 <= codePoint && codePoint <= 0x00d6) || (0x00d9 <= codePoint && codePoint <= 0x00dd) || (0x00e2 <= codePoint && codePoint <= 0x00e5) || codePoint == 0x00e7 || codePoint == 0x00eb || codePoint == 0x00ee || codePoint == 0x00ef || codePoint == 0x00f1 || (0x00f4 <= codePoint && codePoint <= 0x00f6) || codePoint == 0x00fb || codePoint == 0x00fd || codePoint == 0x00ff || codePoint == 0x0100 || (0x0102 <= codePoint && codePoint <= 0x0110) || codePoint == 0x0112 || (0x0114 <= codePoint && codePoint <= 0x011a) || (0x011c <= codePoint && codePoint <= 0x0125) || (0x0128 <= codePoint && codePoint <= 0x012a) || (0x012c <= codePoint && codePoint <= 0x0130) || (0x0134 <= codePoint && codePoint <= 0x0137) || (0x0139 <= codePoint && codePoint <= 0x013e) || codePoint == 0x0143 || (0x0145 <= codePoint && codePoint <= 0x0147) || codePoint == 0x014c || (0x014e <= codePoint && codePoint <= 0x0151) || (0x0154 <= codePoint && codePoint <= 0x0165) || (0x0168 <= codePoint && codePoint <= 0x016a) || (0x016c <= codePoint && codePoint <= 0x01cd) || codePoint == 0x01cf || codePoint == 0x01d1 || codePoint == 0x01d3 || codePoint == 0x01d5 || codePoint == 0x01d7 || codePoint == 0x01d9 || codePoint == 0x01db || (0x01dd <= codePoint && codePoint <= 0x0250) || (0x0252 <= codePoint && codePoint <= 0x0260) || (0x0262 <= codePoint && codePoint <= 0x02c3) || codePoint == 0x02c5 || codePoint == 0x02c6 || codePoint == 0x02c8 || codePoint == 0x02cc || codePoint == 0x02ce || codePoint == 0x02cf || (0x02d1 <= codePoint && codePoint <= 0x02d7) || codePoint == 0x02dc || codePoint == 0x02de || (0x02e0 <= codePoint && codePoint <= 0x02ff) || (0x0370 <= codePoint && codePoint <= 0x0390) || (0x03aa <= codePoint && codePoint <= 0x03b0) || codePoint == 0x03c2 || (0x03ca <= codePoint && codePoint <= 0x0400) || (0x0402 <= codePoint && codePoint <= 0x040f) || codePoint == 0x0450 || (0x0452 <= codePoint && codePoint <= 0x10fc) || (0x1160 <= codePoint && codePoint <= 0x11a2) || (0x11a8 <= codePoint && codePoint <= 0x11f9) || (0x1200 <= codePoint && codePoint <= 0x200f) || codePoint == 0x2011 || codePoint == 0x2012 || codePoint == 0x2017 || codePoint == 0x201a || codePoint == 0x201b || codePoint == 0x201e || codePoint == 0x201f || codePoint == 0x2023 || (0x2028 <= codePoint && codePoint <= 0x202f) || codePoint == 0x2031 || codePoint == 0x2034 || (0x2036 <= codePoint && codePoint <= 0x203a) || codePoint == 0x203c || codePoint == 0x203d || (0x203f <= codePoint && codePoint <= 0x2071) || (0x2075 <= codePoint && codePoint <= 0x207e) || codePoint == 0x2080 || (0x2085 <= codePoint && codePoint <= 0x20a8) || codePoint == 0x20aa || codePoint == 0x20ab || (0x20ad <= codePoint && codePoint <= 0x2102) || codePoint == 0x2104 || (0x2106 <= codePoint && codePoint <= 0x2108) || (0x210a <= codePoint && codePoint <= 0x2112) || codePoint == 0x2114 || codePoint == 0x2115 || (0x2117 <= codePoint && codePoint <= 0x2120) || (0x2123 <= codePoint && codePoint <= 0x2125) || (0x2127 <= codePoint && codePoint <= 0x212a) || (0x212c <= codePoint && codePoint <= 0x2152) || (0x2155 <= codePoint && codePoint <= 0x215a) || codePoint == 0x215f || (0x216c <= codePoint && codePoint <= 0x216f) || (0x217a <= codePoint && codePoint <= 0x2188) || (0x219a <= codePoint && codePoint <= 0x21b7) || (0x21ba <= codePoint && codePoint <= 0x21d1) || codePoint == 0x21d3 || (0x21d5 <= codePoint && codePoint <= 0x21e6) || (0x21e8 <= codePoint && codePoint <= 0x21ff) || codePoint == 0x2201 || (0x2204 <= codePoint && codePoint <= 0x2206) || codePoint == 0x2209 || codePoint == 0x220a || (0x220c <= codePoint && codePoint <= 0x220e) || codePoint == 0x2210 || (0x2212 <= codePoint && codePoint <= 0x2214) || (0x2216 <= codePoint && codePoint <= 0x2219) || codePoint == 0x221b || codePoint == 0x221c || codePoint == 0x2221 || codePoint == 0x2222 || codePoint == 0x2224 || codePoint == 0x2226 || codePoint == 0x222d || (0x222f <= codePoint && codePoint <= 0x2233) || (0x2238 <= codePoint && codePoint <= 0x223b) || (0x223e <= codePoint && codePoint <= 0x2247) || (0x2249 <= codePoint && codePoint <= 0x224b) || (0x224d <= codePoint && codePoint <= 0x2251) || (0x2253 <= codePoint && codePoint <= 0x225f) || codePoint == 0x2262 || codePoint == 0x2263 || codePoint == 0x2268 || codePoint == 0x2269 || codePoint == 0x226c || codePoint == 0x226d || (0x2270 <= codePoint && codePoint <= 0x2281) || codePoint == 0x2284 || codePoint == 0x2285 || (0x2288 <= codePoint && codePoint <= 0x2294) || (0x2296 <= codePoint && codePoint <= 0x2298) || (0x229a <= codePoint && codePoint <= 0x22a4) || (0x22a6 <= codePoint && codePoint <= 0x22be) || (0x22c0 <= codePoint && codePoint <= 0x2311) || (0x2313 <= codePoint && codePoint <= 0x2328) || (0x232b <= codePoint && codePoint <= 0x244a) || codePoint == 0x24ea || (0x254c <= codePoint && codePoint <= 0x254f) || (0x2574 <= codePoint && codePoint <= 0x257f) || codePoint == 0x2590 || codePoint == 0x2591 || (0x2596 <= codePoint && codePoint <= 0x259f) || codePoint == 0x25a2 || (0x25aa <= codePoint && codePoint <= 0x25b1) || codePoint == 0x25b4 || codePoint == 0x25b5 || (0x25b8 <= codePoint && codePoint <= 0x25bb) || codePoint == 0x25be || codePoint == 0x25bf || (0x25c2 <= codePoint && codePoint <= 0x25c5) || codePoint == 0x25c9 || codePoint == 0x25ca || codePoint == 0x25cc || codePoint == 0x25cd || (0x25d2 <= codePoint && codePoint <= 0x25e1) || (0x25e6 <= codePoint && codePoint <= 0x25ee) || (0x25f0 <= codePoint && codePoint <= 0x2604) || codePoint == 0x2607 || codePoint == 0x2608 || (0x260a <= codePoint && codePoint <= 0x260d) || (0x2610 <= codePoint && codePoint <= 0x2613) || (0x2616 <= codePoint && codePoint <= 0x261b) || codePoint == 0x261d || (0x261f <= codePoint && codePoint <= 0x263f) || codePoint == 0x2641 || (0x2643 <= codePoint && codePoint <= 0x265f) || codePoint == 0x2662 || codePoint == 0x2666 || codePoint == 0x266b || codePoint == 0x266e || (0x2670 <= codePoint && codePoint <= 0x269d) || (0x26a0 <= codePoint && codePoint <= 0x26bd) || (0x26c0 <= codePoint && codePoint <= 0x26c3) || (0x2701 <= codePoint && codePoint <= 0x273c) || (0x273e <= codePoint && codePoint <= 0x2756) || (0x2758 <= codePoint && codePoint <= 0x2775) || (0x2780 <= codePoint && codePoint <= 0x27e5) || (0x27ee <= codePoint && codePoint <= 0x2984) || (0x2987 <= codePoint && codePoint <= 0x2b54) || (0x2c00 <= codePoint && codePoint <= 0x2e31) || codePoint == 0x303f || (0x4dc0 <= codePoint && codePoint <= 0x4dff) || (0xa4d0 <= codePoint && codePoint <= 0xa95f) || (0xa980 <= codePoint && codePoint <= 0xabf9) || (0xd800 <= codePoint && codePoint <= 0xdb7f) || (0xdb80 <= codePoint && codePoint <= 0xdbff) || (0xdc00 <= codePoint && codePoint <= 0xdfff) || (0xfb00 <= codePoint && codePoint <= 0xfdfd) || (0xfe20 <= codePoint && codePoint <= 0xfe26) || (0xfe70 <= codePoint && codePoint <= 0xfeff) || (0xfff9 <= codePoint && codePoint <= 0xfffc) || (0x10000 <= codePoint && codePoint <= 0x1f093) || codePoint == 0x1f12e || (0xe0001 <= codePoint && codePoint <= 0xe007f);

        public static bool InEastAsianWide(this char chr)
        {
            ushort cp = chr;
            //return ((0x1100 <= codePoint && codePoint <= 0x115f) || (0x11a3 <= codePoint && codePoint <= 0x11a7) || (0x11fa <= codePoint && codePoint <= 0x11ff) || codePoint == 0x2329 || codePoint == 0x232a || (0x2e80 <= codePoint && codePoint <= 0x2ffb) || (0x3001 <= codePoint && codePoint <= 0x303e) || (0x3041 <= codePoint && codePoint <= 0x3247) || (0x3250 <= codePoint && codePoint <= 0x33ff) || (0x3400 <= codePoint && codePoint <= 0x4db5) || (0x4db6 <= codePoint && codePoint <= 0x4dbf) || (0x4e00 <= codePoint && codePoint <= 0x9fcb) || (0x9fcc <= codePoint && codePoint <= 0x9fff) || (0xa000 <= codePoint && codePoint <= 0xa4c6) || (0xa960 <= codePoint && codePoint <= 0xa97c) || (0xac00 <= codePoint && codePoint <= 0xd7a3) || (0xd7b0 <= codePoint && codePoint <= 0xd7fb) || (0xf900 <= codePoint && codePoint <= 0xfa2d) || codePoint == 0xfa2e || codePoint == 0xfa2f || (0xfa30 <= codePoint && codePoint <= 0xfa6d) || codePoint == 0xfa6e || codePoint == 0xfa6f || (0xfa70 <= codePoint && codePoint <= 0xfad9) || (0xfada <= codePoint && codePoint <= 0xfaff) || (0xfe10 <= codePoint && codePoint <= 0xfe19) || (0xfe30 <= codePoint && codePoint <= 0xfe6b) || (0x1f200 <= codePoint && codePoint <= 0x1f248) || (0x20000 <= codePoint && codePoint <= 0x2a6d6) || (0x2a6d7 <= codePoint && codePoint <= 0x2a6ff) || (0x2a700 <= codePoint && codePoint <= 0x2b734) || (0x2b735 <= codePoint && codePoint <= 0x2f7ff) || (0x2f800 <= codePoint && codePoint <= 0x2fa1d) || (0x2fa1e <= codePoint && codePoint <= 0x2fffd) || (0x30000 <= codePoint && codePoint <= 0x3fffd))
            return (0x1100 <= cp && cp <= 0x115f) || (0x11a3 <= cp && cp <= 0x11a7) || (0x11fa <= cp && cp <= 0x11ff) || cp == 0x2329 || cp == 0x232a || (0x2e80 <= cp && cp <= 0x2ffb) || (0x3001 <= cp && cp <= 0x303e) || (0x3041 <= cp && cp <= 0x3247) || (0x3250 <= cp && cp <= 0x33ff) || (0x3400 <= cp && cp <= 0x4db5) || (0x4db6 <= cp && cp <= 0x4dbf) || (0x4e00 <= cp && cp <= 0x9fcb) || (0x9fcc <= cp && cp <= 0x9fff) || (0xa000 <= cp && cp <= 0xa4c6) || (0xa960 <= cp && cp <= 0xa97c) || (0xac00 <= cp && cp <= 0xd7a3) || (0xd7b0 <= cp && cp <= 0xd7fb) || (0xf900 <= cp && cp <= 0xfa2d) || cp == 0xfa2e || cp == 0xfa2f || (0xfa30 <= cp && cp <= 0xfa6d) || cp == 0xfa6e || cp == 0xfa6f || (0xfa70 <= cp && cp <= 0xfad9) || (0xfada <= cp && cp <= 0xfaff) || (0xfe10 <= cp && cp <= 0xfe19) || (0xfe30 <= cp && cp <= 0xfe6b);
        }

        public static bool InEastAsianWide(this int codePoint)
            => (0x1100 <= codePoint && codePoint <= 0x115f) || (0x11a3 <= codePoint && codePoint <= 0x11a7) || (0x11fa <= codePoint && codePoint <= 0x11ff) || codePoint == 0x2329 || codePoint == 0x232a || (0x2e80 <= codePoint && codePoint <= 0x2ffb) || (0x3001 <= codePoint && codePoint <= 0x303e) || (0x3041 <= codePoint && codePoint <= 0x3247) || (0x3250 <= codePoint && codePoint <= 0x33ff) || (0x3400 <= codePoint && codePoint <= 0x4db5) || (0x4db6 <= codePoint && codePoint <= 0x4dbf) || (0x4e00 <= codePoint && codePoint <= 0x9fcb) || (0x9fcc <= codePoint && codePoint <= 0x9fff) || (0xa000 <= codePoint && codePoint <= 0xa4c6) || (0xa960 <= codePoint && codePoint <= 0xa97c) || (0xac00 <= codePoint && codePoint <= 0xd7a3) || (0xd7b0 <= codePoint && codePoint <= 0xd7fb) || (0xf900 <= codePoint && codePoint <= 0xfa2d) || codePoint == 0xfa2e || codePoint == 0xfa2f || (0xfa30 <= codePoint && codePoint <= 0xfa6d) || codePoint == 0xfa6e || codePoint == 0xfa6f || (0xfa70 <= codePoint && codePoint <= 0xfad9) || (0xfada <= codePoint && codePoint <= 0xfaff) || (0xfe10 <= codePoint && codePoint <= 0xfe19) || (0xfe30 <= codePoint && codePoint <= 0xfe6b) || (0x1f200 <= codePoint && codePoint <= 0x1f248) || (0x20000 <= codePoint && codePoint <= 0x2a6d6) || (0x2a6d7 <= codePoint && codePoint <= 0x2a6ff) || (0x2a700 <= codePoint && codePoint <= 0x2b734) || (0x2b735 <= codePoint && codePoint <= 0x2f7ff) || (0x2f800 <= codePoint && codePoint <= 0x2fa1d) || (0x2fa1e <= codePoint && codePoint <= 0x2fffd) || (0x30000 <= codePoint && codePoint <= 0x3fffd);

        #endregion

        #endregion

        #region System.String

        /// <summary>指定にしたがって変換された文字列型の値を返します。</summary>
        /// <param name="str">変換する文字列</param>
        /// <param name="conversion">変換方法を指定する <see cref="StringConvert"/> 列挙値。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static string Convert(this string str, StringConvert conversion) => Convert(str, conversion, Thread.CurrentThread.CurrentCulture);

        /// <summary>指定に従って変換された文字列型の値を返します。</summary>
        /// <param name="str">変換する文字列</param>
        /// <param name="conversion">変換方法を指定する <see cref="StringConvert"/> 列挙値。</param>
        /// <param name="localeId">現在のシステムのロケールと異なるロケールにしたがって変換する場合のロケールID。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static string Convert(this string str, StringConvert conversion, int localeId)
        {
            CultureInfo ci;
            if (localeId == 0 || localeId == 1)
                ci = Thread.CurrentThread.CurrentCulture;
            else
                ci = CultureInfo.GetCultureInfo(localeId & 65535);

            return Convert(str, conversion, ci);
        }

        /// <summary>指定にしたがって変換された文字列型の値を返します。</summary>
        /// <param name="str">変換する文字列</param>
        /// <param name="conversion">変換方法を指定する <see cref="StringConvert"/> 列挙値。</param>
        /// <param name="ci">現在のシステムの <see cref="CultureInfo"/> と異なる <see cref="CultureInfo"/> にしたがって変換する場合の <see cref="CultureInfo"/>。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static string Convert(this string str, StringConvert conversion, CultureInfo ci)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (ci == null)
                throw new ArgumentNullException(nameof(ci));
            if ((conversion & ~(StringConvert.Uppercase | StringConvert.Lowercase | StringConvert.TitleCase | StringConvert.FullWidth | StringConvert.HalfWidth | StringConvert.Katakana | StringConvert.Hiragana | StringConvert.SimplifiedChinese | StringConvert.TraditionalChinese | StringConvert.LinguisticCasing)) != StringConvert.None)
                throw new ArgumentOutOfRangeException(nameof(conversion));

            if (str.Length == 0 || conversion == StringConvert.None)
                return str;

            var flag = Kernel32.MapFlag.None;

            switch (conversion & StringConvert.TitleCase)
            {
                case StringConvert.TitleCase:
                    flag |= Kernel32.MapFlag.LowerCase;
                    //flag &= ~MapFlag.UpperCase;
                    break;
                case StringConvert.Uppercase:
                    flag |= Kernel32.MapFlag.UpperCase;
                    break;
                case StringConvert.Lowercase:
                    flag |= Kernel32.MapFlag.LowerCase;
                    break;
            }

            switch (conversion & (StringConvert.FullWidth | StringConvert.HalfWidth))
            {
                case StringConvert.FullWidth:
                    flag |= Kernel32.MapFlag.FullWidth;
                    break;
                case StringConvert.HalfWidth:
                    flag |= Kernel32.MapFlag.HalfWidth;
                    break;
                case StringConvert.None:
                    break;
                default:
                    throw new ArgumentException("StringConvert.FullWidth and StringConvert.HalfWidth are exclusive.", nameof(conversion));
            }

            switch (conversion & (StringConvert.Katakana | StringConvert.Hiragana))
            {
                case StringConvert.Katakana:
                    flag |= Kernel32.MapFlag.Katakana;
                    break;
                case StringConvert.Hiragana:
                    flag |= Kernel32.MapFlag.Hiragana;
                    break;
                case StringConvert.None:
                    break;
                default:
                    throw new ArgumentException("StringConvert.Katakana and StringConvert.Hiragana are exclusive.", nameof(conversion));
            }

            switch (conversion & (StringConvert.SimplifiedChinese | StringConvert.TraditionalChinese))
            {
                case StringConvert.SimplifiedChinese:
                    flag |= Kernel32.MapFlag.SimplifiedChinese;
                    break;
                case StringConvert.TraditionalChinese:
                    flag |= Kernel32.MapFlag.TraditionalChinese;
                    break;
                case StringConvert.None:
                    break;
                default:
                    throw new ArgumentException("StringConvert.SimplifiedChinese and StringConvert.TraditionalChinese are exclusive.", nameof(conversion));
            }

            if ((conversion & StringConvert.LinguisticCasing) == StringConvert.LinguisticCasing)
            {
                if ((conversion & StringConvert.TitleCase) != StringConvert.None)
                    flag |= Kernel32.MapFlag.LinguisticCasing;
                else
                    throw new ArgumentOutOfRangeException(nameof(conversion));
            }

            var len = str.Length;
            var dest = new char[Math.Min(len * 6, Int32.MaxValue)];
            var count = Kernel32.LCMapStringEx(ci.Name, flag, str, len, dest, dest.Length);
            if (count == 0)
                throw new Win32Exception();

            var cs = new char[count];
            Array.ConstrainedCopy(dest, 0, cs, 0, count);
            var ret = new string(cs);

            if ((conversion & (StringConvert.TitleCase)) == StringConvert.TitleCase)
                return ci.TextInfo.ToTitleCase(ret);
            return ret;
        }

        /// <summary>
        /// タブ文字'\t'を半角スペースに展開します。
        /// </summary>
        /// <param name="str">タブ文字を展開する文字列</param>
        /// <returns>タブを半角スペースで展開した文字列</returns>
        public static string TabExpand(this string str)
            => TabExpand(str, 8, true);

        /// <summary>
        /// タブ文字'\t'を半角スペースに展開します。
        /// </summary>
        /// <param name="str">タブ文字を展開する文字列</param>
        /// <param name="tabStop">タブストップ。</param>
        /// <returns>タブを半角スペースで展開した文字列</returns>
        public static string TabExpand(this string str, int tabStop)
            => TabExpand(str, tabStop, true);

        /// <summary>
        /// タブ文字'\t'を半角スペースに展開します。
        /// </summary>
        /// <param name="str">タブ文字を展開する文字列</param>
        /// <param name="eastAsian">東アジア仕様の全角/半角チェック。</param>
        /// <returns>タブを半角スペースで展開した文字列</returns>
        public static string TabExpand(this string str, bool eastAsian)
            => TabExpand(str, 8, eastAsian);

        /// <summary>
        /// タブ文字'\t'を半角スペースに展開します。
        /// </summary>
        /// <param name="str">タブ文字を展開する文字列</param>
        /// <param name="tabStop">タブストップ。</param>
        /// <param name="eastAsian">東アジア仕様の全角/半角チェック。</param>
        /// <returns>タブを半角スペースで展開した文字列</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="tabStop"/> が 0 未満です。</exception>
        public static string TabExpand(this string str, int tabStop, bool eastAsian)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (tabStop < 0)
                throw new ArgumentOutOfRangeException(nameof(tabStop));

            if (str.Length == 0 || !str.Contains("\t"))
                return str;

            // for surrogate pair, use TextElementEnumerator.
            var tee = StringInfo.GetTextElementEnumerator(str);
            var sb = new StringBuilder();

            var ci = 0;
            while (tee.MoveNext())
            {
                var cur = tee.GetTextElement();

                if (cur == "\n")
                {
                    ci = 0;
                    sb.Append(cur);
                }
                else if (cur == "\t")
                {
                    var len = tabStop - (ci % tabStop);
                    ci += len;
                    sb.Append(' ', len);
                }
                else
                {
                    if (cur.Length == 1)
                    {
                        ci += cur[0].InHalfwidth(eastAsian) ? 1 : 2;
                    }
                    else if (cur.Length == 2 && Char.IsHighSurrogate(cur[0]) && Char.IsLowSurrogate(cur[1])) // surrogate pair
                    {
                        int codePoint = ((cur[0] - 0xd800) * 0x400) + (cur[1] - 0xdc00) + 0x10000;
                        ci += codePoint.InHalfwidth(eastAsian) ? 1 : 2;
                    }
                    else
                    {
                        // ここにくるケースはあるのか。とりあえず例外投げるようにしておく。
                        throw new NotImplementedException();
                    }

                    sb.Append(cur);
                }
            }

            return sb.ToString();
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

        #region System.Windows.Forms.Control

        /// <summary>このコントロール内に含まれているすべての子孫コントロールを列挙します。</summary>
        /// <param name="top"></param>
        /// <returns>このコントロールに含まれるすべての子孫コントロールを含む <see cref="IEnumerable{Control}"/></returns>
        public static IEnumerable<Control> EnumerateAllChildControls(this Control top)
        {
            foreach (Control c in top.Controls)
            {
                yield return c;
                if (c.HasChildren)
                {
                    foreach (var cc in EnumerateAllChildControls(c))
                        yield return cc;
                }
            }
        }

        #endregion
    }
}

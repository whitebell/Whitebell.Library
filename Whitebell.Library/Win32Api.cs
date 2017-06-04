﻿using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitebell.Library
{
    /// <summary>
    /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd318702.aspx
    /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd318144.aspx
    /// </summary>
    [CLSCompliant(false)]
    [Flags]
    public enum MapFlag : uint
    {
        None = 0,
        /// <summary>This flag is used only with the LCMAP_SORTKEY flag. Ignore case. For many scripts (notably Latin scripts), NORM_IGNORECASE coincides with LINGUISTIC_IGNORECASE. NOTE: NORM_IGNORECASE ignores any tertiary distinction, whether it is actually linguistic case or not. For example, in Arabic and Indic scripts, this flag distinguishes alternate forms of a character, but the differences do not correspond to linguistic case. LINGUISTIC_IGNORECASE causes the function to ignore only actual linguistic casing, instead of ignoring the third sorting weight. NOTE: For double-byte character _set (DBCS) locales, NORM_IGNORECASE has an effect on all Unicode characters as well as narrow (one-byte) characters, including Greek and Cyrillic characters.</summary>
        NormIgnoreCase = 0x00000001,
        /// <summary>This flag can be used alone, with one another, or with the LCMAP_SORTKEY and/or LCMAP_BYTEREV flags. Ignore nonspacing characters. For many scripts (notably Latin scripts), NORM_IGNORENONSPACE coincides with LINGUISTIC_IGNOREDIACRITIC. Note  NORM_IGNORENONSPACE ignores any secondary distinction, whether it is a diacritic or not. Scripts for Korean, Japanese, Chinese, and Indic languages, among others, use this distinction for purposes other than diacritics. LINGUISTIC_IGNOREDIACRITIC causes the function to ignore only actual diacritics, instead of ignoring the second sorting weight.</summary>
        NormIgnoreNonspace = 0x00000002,
        /// <summary>This flag can be used alone, with one another, or with the LCMAP_SORTKEY and/or LCMAP_BYTEREV flags. Ignore symbols and punctuation.</summary>
        NormIgnoreSymbols = 0x00000004,
        /// <summary>This flag is used only with the LCMAP_SORTKEY flag. Windows 7: Treat digits as numbers during sorting, for example, sort "2" before "10".</summary>
        SortDigitsAsNumbers = 0x00000008,
        /// <summary>This flag is used only with the LCMAP_SORTKEY flag. Ignore case, as linguistically appropriate.</summary>
        LinguisticIgnoreCase = 0x00000010,
        /// <summary>This flag is used only with the LCMAP_SORTKEY flag. Ignore nonspacing characters, as linguistically appropriate. NOTE This flag does not always produce predictable results when used with decomposed characters, that is, characters in which a base character and one or more nonspacing characters each have distinct code point values.</summary>
        LinguisticIgnoreDiacritic = 0x00000020,
        /// <summary>For locales and scripts capable of handling uppercase and lowercase, map all characters to lowercase.</summary>
        LowerCase = 0x00000100,
        /// <summary>For locales and scripts capable of handling uppercase and lowercase, map all characters to uppercase.</summary>
        UpperCase = 0x00000200,
        /// <summary>Windows 7: Map all characters to title case, in which the first letter of each major word is capitalized.</summary>
        TitleCase = 0x00000300,
        /// <summary>Produce a normalized sort key. If the LCMAP_SORTKEY flag is not specified, the function performs string mapping. For details of sort key generation and string mapping, see the Remarks section.</summary>
        SortKey = 0x00000400,
        /// <summary>Use byte reversal. For example, if the application passes in 0x3450 0x4822, the result is 0x5034 0x2248.</summary>
        ByteReverse = 0x00000800,
        /// <summary>This flag is used only with the LCMAP_SORTKEY flag. Treat punctuation the same as symbols.</summary>
        StringSort = 0x00001000,
        /// <summary>This flag is used only with the LCMAP_SORTKEY flag. Do not differentiate between hiragana and katakana characters. Corresponding hiragana and katakana characters compare as equal.</summary>
        NormIgnoreKanaType = 0x00010000,
        /// <summary>This flag is used only with the LCMAP_SORTKEY flag. Ignore the difference between half-width and full-width characters, for example, C a t == cat. The full-width form is a formatting distinction used in Chinese and Japanese scripts.</summary>
        NormIgnoreWidth = 0x00020000,
        /// <summary>Map all katakana characters to hiragana. This flag and LCMAP_KATAKANA are mutually exclusive.</summary>
        Hiragana = 0x00100000,
        /// <summary>Map all hiragana characters to katakana. This flag and LCMAP_HIRAGANA are mutually exclusive.</summary>
        Katakana = 0x00200000,
        /// <summary>Use narrow characters where applicable. This flag and LCMAP_FULLWIDTH are mutually exclusive.</summary>
        HalfWidth = 0x00400000,
        /// <summary>Use Unicode (wide) characters where applicable. This flag and LCMAP_HALFWIDTH are mutually exclusive.</summary>
        FullWidth = 0x00800000,
        /// <summary>Use linguistic rules for casing, instead of file system rules (default). This flag is valid with LCMAP_LOWERCASE or LCMAP_UPPERCASE only.</summary>
        LinguisticCasing = 0x01000000,
        /// <summary>Map traditional Chinese characters to simplified Chinese characters. This flag and LCMAP_TRADITIONAL_CHINESE are mutually exclusive.</summary>
        SimplifiedChinese = 0x02000000,
        /// <summary>Map simplified Chinese characters to traditional Chinese characters. This flag and LCMAP_SIMPLIFIED_CHINESE are mutually exclusive.</summary>
        TraditionalChinese = 0x04000000,
        /// <summary>This flag is used only with the LCMAP_SORTKEY flag. Use linguistic rules for casing, instead of file system rules (default).</summary>
        NormLinguisticCasing = 0x08000000
    }

    public static class Kernel32
    {
        #region LCMapStringEx (Vista/Server 2008)

        /// <summary>
        /// For a locale specified by name, maps an input character string to another using a specified transformation, or generates a sort key for the input string.
        /// </summary>
        /// <param name="lpLocaleName">Pointer to a locale name (http://msdn.microsoft.com/en-us/library/dd318702%28v=vs.85%29.aspx), or one of the following predefined values.</param>
        /// <param name="dwMapFlags">Flag specifying the type of transformation to use during string mapping or the type of sort key to generate.</param>
        /// <param name="lpSrcStr">Source string that the function maps or uses for sort key generation. This string cannot have a size of 0.</param>
        /// <param name="cchSrc">Size, in characters, of the source string indicated by lpSrcStr. The size of the source string can include the terminating null character, but does not have to. If the terminating null character is included, the mapping behavior of the function is not greatly affected because the terminating null character is considered to be unsortable and always maps to itself. The application can _set this parameter to any negative value to specify that the source string is null-terminated. In this case, if LCMapStringEx is being used in its string-mapping mode, the function calculates the string length itself, and null-terminates the mapped string indicated by lpDestStr. The application cannot _set this parameter to 0.</param>
        /// <param name="lpDestStr">Pointer to a buffer in which this function retrieves the mapped string or sort key. If the application specifies LCMAP_SORTKEY, the function stores a sort key in the buffer as an opaque array of byte values that can include embedded 0 bytes. Note  If the function fails, the destination buffer might contain either partial results or no results at all. In this case, it is recommended for your application to consider any results invalid.</param>
        /// <param name="cchDest">Size, in characters, of the buffer indicated by lpDestStr. If the application is using the function for string mapping, it supplies a character count for this parameter. If space for a terminating null character is included in cchSrc, cchDest must also include space for a terminating null character. If the application is using the function to generate a sort key, it supplies a byte count for the size. This byte count must include space for the sort key 0x00 terminator. The application can _set cchDest to 0. In this case, the function does not use the lpDestStr parameter and returns the required buffer size for the mapped string or sort key.</param>
        /// <param name="lpVersionInformation">Reserved; must be NULL.</param>
        /// <param name="lpReserved">Reserved; must be NULL.</param>
        /// <param name="sortHandle">Reserved; must be 0.</param>
        /// <returns>Returns the number of characters or bytes in the translated string or sort key, including a terminating null character, if successful. If the function succeeds and the value of cchDest is 0, the return value is the size of the buffer required to hold the translated string or sort key, including a terminating null character if the input was null terminated.</returns>
        [DllImport("Kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int LCMapStringEx(string lpLocaleName, uint dwMapFlags, string lpSrcStr, int cchSrc, char[] lpDestStr, int cchDest, IntPtr lpVersionInformation, IntPtr lpReserved, IntPtr sortHandle);
        // int LCMapStringEx(_In_opt_ LPCWSTR lpLocaleName, _In_ DWORD dwMapFlags, _In_ LPCWSTR lpSrcStr, _In_ int cchSrc, _Out_opt_ LPWSTR lpDestStr, _In_ int cchDest, _In_opt_ LPNLSVERSIONINFO lpVersionInformation, _In_opt_ LPVOID lpReserved, _In_opt_ LPARAM sortHandle);

        /// <summary>
        /// For a locale specified by name, maps an input character string to another using a specified transformation, or generates a sort key for the input string.
        /// </summary>
        /// <param name="lpLocaleName">Pointer to a locale name (http://msdn.microsoft.com/en-us/library/dd318702%28v=vs.85%29.aspx), or one of the following predefined values.</param>
        /// <param name="dwMapFlags">Flag specifying the type of transformation to use during string mapping or the type of sort key to generate.</param>
        /// <param name="lpSrcStr">Source string that the function maps or uses for sort key generation. This string cannot have a size of 0.</param>
        /// <param name="cchSrc">Size, in characters, of the source string indicated by lpSrcStr. The size of the source string can include the terminating null character, but does not have to. If the terminating null character is included, the mapping behavior of the function is not greatly affected because the terminating null character is considered to be unsortable and always maps to itself. The application can _set this parameter to any negative value to specify that the source string is null-terminated. In this case, if LCMapStringEx is being used in its string-mapping mode, the function calculates the string length itself, and null-terminates the mapped string indicated by lpDestStr. The application cannot _set this parameter to 0.</param>
        /// <param name="lpDestStr">Pointer to a buffer in which this function retrieves the mapped string or sort key. If the application specifies LCMAP_SORTKEY, the function stores a sort key in the buffer as an opaque array of byte values that can include embedded 0 bytes. Note  If the function fails, the destination buffer might contain either partial results or no results at all. In this case, it is recommended for your application to consider any results invalid.</param>
        /// <param name="cchDest">Size, in characters, of the buffer indicated by lpDestStr. If the application is using the function for string mapping, it supplies a character count for this parameter. If space for a terminating null character is included in cchSrc, cchDest must also include space for a terminating null character. If the application is using the function to generate a sort key, it supplies a byte count for the size. This byte count must include space for the sort key 0x00 terminator. The application can _set cchDest to 0. In this case, the function does not use the lpDestStr parameter and returns the required buffer size for the mapped string or sort key.</param>
        /// <returns>Returns the number of characters or bytes in the translated string or sort key, including a terminating null character, if successful. If the function succeeds and the value of cchDest is 0, the return value is the size of the buffer required to hold the translated string or sort key, including a terminating null character if the input was null terminated.</returns>
        [CLSCompliant(false)]
        public static int LCMapStringEx(string lpLocaleName, MapFlag dwMapFlags, string lpSrcStr, int cchSrc, char[] lpDestStr, int cchDest)
            => LCMapStringEx(lpLocaleName, (uint)dwMapFlags, lpSrcStr, cchSrc, lpDestStr, cchDest, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

        #endregion
    }
}

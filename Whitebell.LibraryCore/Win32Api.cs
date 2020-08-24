using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Whitebell.Library
{
    internal static class Constants
    {
        public const int MaxPath = 260;
    }

    public static class Kernel32
    {
        [CLSCompliant(false)]
        [Flags]
        public enum EComparationFlags : uint
        {
            None = 0x0,

            /// <summary>Ignore case</summary>
            NormIgnoreCase = 0x1,

            /// <summary>Ignore nonspacing chars</summary>
            NormIgnoreNonspace = 0x2,

            /// <summary>Ignore symbols</summary>
            NormIgnoreSymbols = 0x4,

            /// <summary>Use digits as numbers sort method</summary>
            SortDigitAsNumbers = 0x8,

            /// <summary>Linguistically appropriate 'ignore case'</summary>
            LinguisticIgnoreCase = 0x10,

            /// <summary>Linguistically appropriate 'ignore nonspace'</summary>
            LinguisticIgnoreDiacritic = 0x20,

            /// <summary>Use string sort method</summary>
            SortStringSort = 0x1000,

            /// <summary>Ignore kanatype</summary>
            NormIgnoreKanaType = 0x10000,

            /// <summary>Ignore width</summary>
            NormIgnoreWidth = 0x20000,

            /// <summary>Use linguistic rules for casing</summary>
            NormLinguisticCasing = 0x8000000
        }

        // http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd318702.aspx
        // http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd318144.aspx
        [CLSCompliant(false)]
        [Flags]
        public enum MapFlag : uint
        {
            None = 0,

            /// <summary>
            /// This flag is used only with the <see cref="SortKey"/> flag.
            /// Ignore case. For many scripts (notably Latin scripts), <see cref="NormIgnoreCase"/> coincides with <see cref="LinguisticIgnoreCase"/>.
            /// NOTE: <see cref="NormIgnoreCase"/> ignores any tertiary distinction, whether it is actually linguistic case or not.
            /// For example, in Arabic and Indic scripts, this flag distinguishes alternate forms of a character, but the differences do not correspond to linguistic case.
            /// <see cref="LinguisticIgnoreCase"/> causes the function to ignore only actual linguistic casing, instead of ignoring the third sorting weight.
            /// NOTE: For double-byte character set (DBCS) locales, <see cref="NormIgnoreCase"/> has an effect on all Unicode characters as well as narrow (one-byte) characters, including Greek and Cyrillic characters.
            /// </summary>
            NormIgnoreCase = 0x00000001,

            /// <summary>
            /// This flag can be used alone, with one another, or with the <see cref="SortKey"/> and/or <see cref="ByteReverse"/> flags.
            /// Ignore nonspacing characters. For many scripts (notably Latin scripts), <see cref="NormIgnoreNonspace"/> coincides with <see cref="LinguisticIgnoreDiacritic"/>.
            /// Note: <see cref="NormIgnoreNonspace"/> ignores any secondary distinction, whether it is a diacritic or not.
            /// Scripts for Korean, Japanese, Chinese, and Indic languages, among others, use this distinction for purposes other than diacritics.
            /// <see cref="LinguisticIgnoreDiacritic"/> causes the function to ignore only actual diacritics, instead of ignoring the second sorting weight.
            /// </summary>
            NormIgnoreNonspace = 0x00000002,

            /// <summary>This flag can be used alone, with one another, or with the <see cref="SortKey"/> and/or <see cref="ByteReverse"/> flags. Ignore symbols and punctuation.</summary>
            NormIgnoreSymbols = 0x00000004,

            /// <summary>This flag is used only with the <see cref="SortKey"/> flag. Windows 7: Treat digits as numbers during sorting, for example, sort "2" before "10".</summary>
            SortDigitsAsNumbers = 0x00000008,

            /// <summary>This flag is used only with the <see cref="SortKey"/> flag. Ignore case, as linguistically appropriate.</summary>
            LinguisticIgnoreCase = 0x00000010,

            /// <summary>
            /// This flag is used only with the <see cref="SortKey"/> flag. Ignore nonspacing characters, as linguistically appropriate.
            /// NOTE: This flag does not always produce predictable results when used with decomposed characters,
            /// that is, characters in which a base character and one or more nonspacing characters each have distinct code point values.
            /// </summary>
            LinguisticIgnoreDiacritic = 0x00000020,

            /// <summary>For locales and scripts capable of handling uppercase and lowercase, map all characters to lowercase.</summary>
            LowerCase = 0x00000100,

            /// <summary>For locales and scripts capable of handling uppercase and lowercase, map all characters to uppercase.</summary>
            UpperCase = 0x00000200,

            /// <summary>Windows 7: Map all characters to title case, in which the first letter of each major word is capitalized.</summary>
            TitleCase = 0x00000300,

            /// <summary>Produce a normalized sort key. If the <see cref="SortKey"/> flag is not specified, the function performs string mapping. For details of sort key generation and string mapping, see the Remarks section.</summary>
            SortKey = 0x00000400,

            /// <summary>Use byte reversal. For example, if the application passes in 0x3450 0x4822, the result is 0x5034 0x2248.</summary>
            ByteReverse = 0x00000800,

            /// <summary>This flag is used only with the <see cref="SortKey"/> flag. Treat punctuation the same as symbols.</summary>
            StringSort = 0x00001000,

            /// <summary>This flag is used only with the <see cref="SortKey"/> flag. Do not differentiate between hiragana and katakana characters. Corresponding hiragana and katakana characters compare as equal.</summary>
            NormIgnoreKanaType = 0x00010000,

            /// <summary>
            /// This flag is used only with the <see cref="SortKey"/> flag.
            /// Ignore the difference between half-width and full-width characters, for example, C a t == cat.
            /// The full-width form is a formatting distinction used in Chinese and Japanese scripts.
            /// </summary>
            NormIgnoreWidth = 0x00020000,

            /// <summary>Map all katakana characters to hiragana. This flag and <see cref="Katakana"/> are mutually exclusive.</summary>
            Hiragana = 0x00100000,

            /// <summary>Map all hiragana characters to katakana. This flag and <see cref="Hiragana"/> are mutually exclusive.</summary>
            Katakana = 0x00200000,

            /// <summary>Use narrow characters where applicable. This flag and <see cref="FullWidth"/> are mutually exclusive.</summary>
            HalfWidth = 0x00400000,

            /// <summary>Use Unicode (wide) characters where applicable. This flag and <see cref="HalfWidth"/> are mutually exclusive.</summary>
            FullWidth = 0x00800000,

            /// <summary>Use linguistic rules for casing, instead of file system rules (default). This flag is valid with <see cref="LowerCase"/> or <see cref="UpperCase"/> only.</summary>
            LinguisticCasing = 0x01000000,

            /// <summary>Map traditional Chinese characters to simplified Chinese characters. This flag and <see cref="TraditionalChinese"/> are mutually exclusive.</summary>
            SimplifiedChinese = 0x02000000,

            /// <summary>Map simplified Chinese characters to traditional Chinese characters. This flag and <see cref="SimplifiedChinese"/> are mutually exclusive.</summary>
            TraditionalChinese = 0x04000000,

            /// <summary>This flag is used only with the <see cref="SortKey"/> flag. Use linguistic rules for casing, instead of file system rules (default).</summary>
            NormLinguisticCasing = 0x08000000
        }

        [CLSCompliant(false)]
        public enum SymbolicLink : uint
        {
            /// <summary>The link target is a file.</summary>
            File = 0u,

            /// <summary>The link target is a directory.</summary>
            Directory = 1u,
        }

        #region CompareStringEx (Vista/Server 2008)

        /// <summary>
        /// Compares two Unicode (wide character) strings, for a locale specified by name. Using CompareStringEx incorrectly can compromise the security of your application. Strings that are not compared correctly can produce invalid input. Test strings to make sure they are valid before using them, and provide error handlers. For more information, see http://msdn.microsoft.com/en-us/library/windows/desktop/dd374047(v=vs.85).aspx
        /// </summary>
        /// <param name="lpLocaleName">Pointer to a locale name.</param>
        /// <param name="dwCmpFlags">Flags that indicate how the function compares the two strings. By default, these flags are not set. This parameter can specify a combination of any of the following values, or it can be set to 0 to obtain the default behavior.</param>
        /// <param name="lpString1">Pointer to the first string to compare.</param>
        /// <param name="cchCount1">Length of the string indicated by lpString1, excluding the terminating null character. The application can supply a negative value if the string is null-terminated. In this case, the function determines the length automatically.</param>
        /// <param name="lpString2">Pointer to the second string to compare.</param>
        /// <param name="cchCount2">Length of the string indicated by lpString2, excluding the terminating null character. The application can supply a negative value if the string is null-terminated. In this case, the function determines the length automatically.</param>
        /// <param name="lpVersionInformation">Reserved; must set to NULL.</param>
        /// <param name="lpReserved">Reserved; must set to NULL.</param>
        /// <param name="lParam">Reserved; must be set to 0.</param>
        /// <returns>Returns 1 when the string indicated by lpString1 is less in lexical value than the string indicated by lpString2. Returns 2 when the string indicated by lpString1 is equivalent in lexical value to the string indicated by lpString2. The two strings are equivalent for sorting purposes, although not necessarily identical. Returns 3 when the string indicated by lpString1 is greater in lexical value than the string indicated by lpString2. To maintain the C runtime convention of comparing strings, the value 2 can be subtracted from a nonzero return value. Then, the meaning of &lt;0, ==0, and &gt;0 is consistent with the C runtime. The function returns 0 if it does not succeed. To get extended error information, the application can call GetLastError, which can return one of the following error codes: ERROR_INVALID_FLAGS. The values supplied for flags were invalid. ERROR_INVALID_PARAMETER. Any of the parameter values was invalid.</returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int CompareStringEx(string lpLocaleName, EComparationFlags dwCmpFlags, string lpString1, int cchCount1, string lpString2, int cchCount2, IntPtr lpVersionInformation, IntPtr lpReserved, IntPtr lParam);
        // int CompareStringEx(_In_opt_ LPCWSTR lpLocaleName, _In_ DWORD dwCmpFlags, _In_ LPCWSTR lpString1, _In_ int cchCount1, _In_ LPCWSTR lpString2, _In_ int cchCount2, _In_opt_ LPNLSVERSIONINFO lpVersionInformation, _In_opt_ LPVOID lpReserved, _In_opt_ LPARAM lParam);

        /// <summary>Compares two Unicode (wide character) strings, for a locale specified by name.</summary>
        /// <param name="localeName">Locale name.</param>
        /// <param name="cmpFlags">Flags that indicate how the function compares the two strings. By default, these flags are not set. This parameter can specify a combination of any of the following values, or it can be set to 0 to obtain the default behavior.</param>
        /// <param name="str1">The first string to compare.</param>
        /// <param name="str2">The second string to compare.</param>
        /// <returns>Returns -1 when the string indicated by str1 is less in lexical value than the string indicated by str2. Returns 0 when the string indicated by str1 is equivalent in lexical value to the string indicated by str2. The two strings are equivalent for sorting purposes, although not necessarily identical. Returns 1 when the string indicated by str1 is greater in lexical value than the string indicated by str2.</returns>
        [CLSCompliant(false)]
        public static int CompareStringEx(string localeName, EComparationFlags cmpFlags, string str1, string str2)
        {
            var ret = CompareStringEx(localeName, cmpFlags, str1, str1.Length, str2, str2.Length, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (ret == 0)
                throw new Win32Exception();
            return ret - 2;
        }

        #endregion

        #region CreateHardLink (XP/Server 2003)

        /// <summary>Establishes a hard link between an existing file and a new file. This function is only supported on the NTFS file system, and only for files, not directories.</summary>
        /// <param name="lpFileName">The name of the new file. This parameter cannot specify the name of a directory.</param>
        /// <param name="lpExistingFileName">The name of the existing file. This parameter cannnot specify the name of directory.</param>
        /// <param name="lpSecurityAttributes">Reserved; must be <see cref="IntPtr.Zero"/></param>
        /// <returns>If the function succeeds, the return value is true.
        /// If the function fails, the return value is false. To get extended error information, call <see cref="Marshal.GetLastWin32Error()"/>.
        /// The maximum number of hard links that can be created with this function is 1023 per file.
        /// If more than 1023 links are created for a file, an error results.</returns>
        [DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateHardLink")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);
        // BOOL WINAPI CreateHardLink(_In_ LPCTSTR lpFileName, _In_ LPCTSTR lpExistingFileName, _Reserved_ LPSECURITY_ATTRIBUTES lpSecurityAttributes);

        /// <summary>Establishes a hard link between an existing file and a new file. This function is only supported on the NTFS file system, and only for files, not directories.</summary>
        /// <param name="newFileName">The name of the new file. This parameter cannot specify the name of a directory.</param>
        /// <param name="existingFileName">The name of the existing file. This parameter cannnot specify the name of directory.</param>
        /// <returns>If the function succeeds, the return value is true.
        /// If the function fails, the return value is false. To get extended error information, call <see cref="Marshal.GetLastWin32Error()"/>.
        /// The maximum number of hard links that can be created with this function is 1023 per file.
        /// If more than 1023 links are created for a file, an error results.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool CreateHardLink(string newFileName, string existingFileName)
        {
            if (String.IsNullOrWhiteSpace(newFileName))
                throw new ArgumentException(nameof(newFileName));
            if (String.IsNullOrWhiteSpace(existingFileName))
                throw new ArgumentException(nameof(existingFileName));

            return _CreateHardLink(newFileName, existingFileName, IntPtr.Zero);
        }

        #endregion

        #region CreateSymbolicLink (Vista/Server 2008)

        /// <summary>Creates a symbolic link.</summary>
        /// <param name="lpSymlinkFileName">The Symbolic link to be created.</param>
        /// <param name="lpTargetFileName">The name of the target for the symbolic link to be created.
        /// If lpTargetFileName has a device name associated with it, the link is treated as an absolute link;
        /// otherwise, the link is treated as a relative link.</param>
        /// <param name="dwFlags">Indicates whether the link target, lpTargetFileName, is a directory.</param>
        /// <returns>If the function succeeds, the return value is true. If the function fails, the return value is false.</returns>
        [DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateSymbolicLink")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);
        // BOOLEAN WINAPI CreateSymbolicLink(_In_ LPTSTR lpSymlinkFileName, _In_ LPTSTR loTargetFileName, _In_ DWORD dwFlags);

        /// <summary>Creates a symbolic link.</summary>
        /// <param name="symlinkFileName">The Symbolic link to be created.</param>
        /// <param name="targetFileName">The name of the target for the symbolic link to be created.
        /// If targetFileName has a device name associated with it, the link is treated as an absolute link;
        /// otherwise, the link is treated as a relative link.</param>
        /// <param name="dwFlags">Indicates whether the link target, targetFileName, is a directory.</param>
        /// <returns>If the function succeeds, the return value is true. If the function fails, the return value is false.</returns>
        /// <exception cref="ArgumentException"></exception>
        [CLSCompliant(false)]
        public static bool CreateSymbolicLink(string symlinkFileName, string targetFileName, SymbolicLink dwFlags)
        {
            if (String.IsNullOrWhiteSpace(symlinkFileName))
                throw new ArgumentException(nameof(symlinkFileName));
            if (String.IsNullOrWhiteSpace(targetFileName))
                throw new ArgumentException(nameof(targetFileName));

            return _CreateSymbolicLink(symlinkFileName, targetFileName, dwFlags);
        }
        #endregion

        #region LCMapStringEx (Vista/Server 2008)

        /// <summary>
        /// For a locale specified by name, maps an input character string to another using a specified transformation, or generates a sort key for the input string.
        /// </summary>
        /// <param name="lpLocaleName">Pointer to a locale name (http://msdn.microsoft.com/en-us/library/dd318702%28v=vs.85%29.aspx), or one of the following predefined values.</param>
        /// <param name="dwMapFlags">Flag specifying the type of transformation to use during string mapping or the type of sort key to generate.</param>
        /// <param name="lpSrcStr">Source string that the function maps or uses for sort key generation. This string cannot have a size of 0.</param>
        /// <param name="cchSrc">Size, in characters, of the source string indicated by lpSrcStr. The size of the source string can include the terminating null character, but does not have to. If the terminating null character is included, the mapping behavior of the function is not greatly affected because the terminating null character is considered to be unsortable and always maps to itself. The application can set this parameter to any negative value to specify that the source string is null-terminated. In this case, if LCMapStringEx is being used in its string-mapping mode, the function calculates the string length itself, and null-terminates the mapped string indicated by lpDestStr. The application cannot set this parameter to 0.</param>
        /// <param name="lpDestStr">Pointer to a buffer in which this function retrieves the mapped string or sort key. If the application specifies LCMAP_SORTKEY, the function stores a sort key in the buffer as an opaque array of byte values that can include embedded 0 bytes. Note  If the function fails, the destination buffer might contain either partial results or no results at all. In this case, it is recommended for your application to consider any results invalid.</param>
        /// <param name="cchDest">Size, in characters, of the buffer indicated by lpDestStr. If the application is using the function for string mapping, it supplies a character count for this parameter. If space for a terminating null character is included in cchSrc, cchDest must also include space for a terminating null character. If the application is using the function to generate a sort key, it supplies a byte count for the size. This byte count must include space for the sort key 0x00 terminator. The application can set cchDest to 0. In this case, the function does not use the lpDestStr parameter and returns the required buffer size for the mapped string or sort key.</param>
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
        /// <param name="cchSrc">Size, in characters, of the source string indicated by lpSrcStr. The size of the source string can include the terminating null character, but does not have to. If the terminating null character is included, the mapping behavior of the function is not greatly affected because the terminating null character is considered to be unsortable and always maps to itself. The application can set this parameter to any negative value to specify that the source string is null-terminated. In this case, if LCMapStringEx is being used in its string-mapping mode, the function calculates the string length itself, and null-terminates the mapped string indicated by lpDestStr. The application cannot set this parameter to 0.</param>
        /// <param name="lpDestStr">Pointer to a buffer in which this function retrieves the mapped string or sort key. If the application specifies LCMAP_SORTKEY, the function stores a sort key in the buffer as an opaque array of byte values that can include embedded 0 bytes. Note  If the function fails, the destination buffer might contain either partial results or no results at all. In this case, it is recommended for your application to consider any results invalid.</param>
        /// <param name="cchDest">Size, in characters, of the buffer indicated by lpDestStr. If the application is using the function for string mapping, it supplies a character count for this parameter. If space for a terminating null character is included in cchSrc, cchDest must also include space for a terminating null character. If the application is using the function to generate a sort key, it supplies a byte count for the size. This byte count must include space for the sort key 0x00 terminator. The application can set cchDest to 0. In this case, the function does not use the lpDestStr parameter and returns the required buffer size for the mapped string or sort key.</param>
        /// <returns>Returns the number of characters or bytes in the translated string or sort key, including a terminating null character, if successful. If the function succeeds and the value of cchDest is 0, the return value is the size of the buffer required to hold the translated string or sort key, including a terminating null character if the input was null terminated.</returns>
        [CLSCompliant(false)]
        public static int LCMapStringEx(string lpLocaleName, MapFlag dwMapFlags, string lpSrcStr, int cchSrc, char[] lpDestStr, int cchDest)
            => LCMapStringEx(lpLocaleName, (uint)dwMapFlags, lpSrcStr, cchSrc, lpDestStr, cchDest, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

        #endregion
    }

    public static class Shlwapi
    {
        #region PathCanonicalize (2000 Professional/2000 Server)

        /// <summary>
        /// Simplifies a path by removing navigation elements such as "." and ".." to produce a direct, well-formed path. Note  Misuse of this function can lead to a buffer overrun. We recommend the use of the safer PathCchCanonicalize or PathCchCanonicalizeEx function in its place.
        /// </summary>
        /// <param name="lpszDst">A pointer to a string that receives the canonicalized path. You must set the size of this buffer to MAX_PATH to ensure that it is large enough to hold the returned string.</param>
        /// <param name="lpszSrc">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path to be canonicalized.</param>
        /// <returns>Returns TRUE if a result has been computed and the content of the lpszDst output buffer is valid. Returns FALSE otherwise, and the contents of the buffer pointed to by lpszDst are invalid. To get extended error information, call GetLastError.</returns>
        /// <remarks>This function allows the user to specify what to remove from a path by inserting special character sequences into the path. The ".." sequence indicates to remove a path segment from the current position to the previous path segment. The "." sequence indicates to skip over the next path segment to the following path segment. The root segment of the path cannot be removed. If there are more ".." sequences than there are path segments, the function returns TRUE and contents of the buffer pointed to by lpszDst contains just the root, "\".</remarks>
        [DllImport("shlwapi", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PathCanonicalize([Out]StringBuilder lpszDst, string lpszSrc);
        // BOOL PathCanonicalize(_Out_ LPTSTR lpszDst, _In_ LPCTSTR lpszSrc);

        /// <summary>
        /// Simplifies a path by removing navigation elements such as "." and ".." to produce a direct, well-formed path.
        /// </summary>
        /// <param name="path">A string of maximum length MAX_PATH that contains the path to be canonicalized.</param>
        /// <returns>A canonicalized path.</returns>
        public static string PathCanonicalize(string path)
        {
            var sb = new StringBuilder(Constants.MaxPath);
            if (!PathCanonicalize(sb, path))
                throw new Win32Exception();
            return sb.ToString();
        }

        #endregion

        #region PathRelativePathTo (2000 Professional/2000 Server)

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="pszPath">A pointer to a string that receives the relative path. This buffer must be at least MAX_PATH characters in size.</param>
        /// <param name="pszFrom">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path that defines the start of the relative path.</param>
        /// <param name="dwAttrFrom">The file attributes of pszFrom. If this value contains FILE_ATTRIBUTE_DIRECTORY, pszFrom is assumed to be a directory; otherwise, pszFrom is assumed to be a file.</param>
        /// <param name="pszTo">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path that defines the endpoint of the relative path.</param>
        /// <param name="dwAttrTo">The file attributes of pszTo. If this value contains FILE_ATTRIBUTE_DIRECTORY, pszTo is assumed to be directory; otherwise, pszTo is assumed to be a file.</param>
        /// <returns>Returns TRUE if successful, or FALSE otherwise.</returns>
        /// <remarks>This function takes a pair of paths and generates a relative path from one to the other. The paths do not have to be fully qualified, but they must have a common prefix, or the function will fail and return FALSE. For example, let the starting point, pszFrom, be "c:\FolderA\FolderB\FolderC", and the ending point, pszTo, be "c:\FolderA\FolderD\FolderE". PathRelativePathTo will return the relative path from pszFrom to pszTo as: "..\..\FolderD\FolderE". You will get the same result if you set pszFrom to "\FolderA\FolderB\FolderC" and pszTo to "\FolderA\FolderD\FolderE". On the other hand, "c:\FolderA\FolderB" and "a:\FolderA\FolderD do not share a common prefix, and the function will fail. Note that "\\" is not considered a prefix and is ignored. If you set pszFrom to "\\FolderA\FolderB", and pszTo to "\\FolderC\FolderD", the function will fail.</remarks>
        [DllImport("shlwapi", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool PathRelativePathTo([Out]StringBuilder pszPath, [In]string pszFrom, [In]FileAttributes dwAttrFrom, [In]string pszTo, [In]FileAttributes dwAttrTo);
        // BOOL PathRelativePathTo(_Out_ LPTSTR pszPath, _In_ LPCTSTR pszFrom, _In_ DWORD dwAttrFrom, _In_ LPCTSTR pszTo, _In_ DWORD dwAttrTo);

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="basePath">A string that contains the path that defines the start of the relative path.</param>
        /// <param name="absolutePath">A string that contains the path that defines the endpoint of the relative path.</param>
        /// <returns>A relative path.</returns>
        /// <exception cref="Win32Exception"></exception>
        public static string PathRelativePathTo(string basePath, string absolutePath)
        {
            if (!Directory.Exists(basePath))
                basePath = new FileInfo(basePath).DirectoryName;
            var sb = new StringBuilder(Constants.MaxPath);
            var res = PathRelativePathTo(sb, basePath, FileAttributes.Directory, absolutePath, FileAttributes.Normal);
            if (!res)
                throw new Win32Exception();
            return sb.ToString();
        }

        #endregion

        #region PathRemoveExtension (2000 Professional/2000 Server)

        /// <summary>
        /// Removes the file name extension from a path, if one is present.
        /// </summary>
        /// <param name="pszFile">A pointer to a null-terminated string of length MAX_PATH from which to remove the extension.</param>
        /// <returns>This function does not return a value.</returns>
        [DllImport("shlwapi", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void PathRemoveExtension([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszFile);
        // void PathRemoveExtension(_Inout_ LPTSTR pszPath);

        /// <summary>
        /// Removes the file name extension from a path, if one is present.
        /// </summary>
        /// <param name="path">A path to remove the extension.</param>
        /// <returns>The path without the extension.</returns>
        public static string PathRemoveExtension(string path)
        {
            var sb = new StringBuilder(path);
            PathRemoveExtension(sb);
            return sb.ToString();
        }

        #endregion

        #region StrCmpLogicalW (XP/Server 2003)

        /// <summary>
        /// Compares two Unicode strings.
        /// Digits in the strings are considered as numerical content rather than text.
        /// This test is not case-sensitive.
        /// </summary>
        /// <param name="psz1">A pointer to the first null-terminated string to be compared.</param>
        /// <param name="psz2">A pointer to the second null-terminated string to be compared.</param>
        /// <returns>Returns zero if the strings are identical.
        /// Returns 1 if the string pointed to by psz1 has a greater value than that pointed to by psz2.
        /// Returns -1 if the string pointed to by psz1 has a lesser value than that pointed to by psz2.
        /// </returns>
        [DllImport("shlwapi", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "StrCmpLogicalW")]
        private static extern int _StrCmpLogicalW(string psz1, string psz2);
        // int StrCmpLogicalW(_In_ PCWSTR psz1, _In_ PCWSTR psz2);

        /// <summary>
        /// Compares two Unicode strings.
        /// Digits in the strings are considered as numerical content rather than text.
        /// This test is not case-sensitive.
        /// </summary>
        /// <param name="psz1">A pointer to the first null-terminated string to be compared.</param>
        /// <param name="psz2">A pointer to the second null-terminated string to be compared.</param>
        /// <returns>Returns zero if the strings are identical.
        /// Returns 1 if the string pointed to by psz1 has a greater value than that pointed to by psz2.
        /// Returns -1 if the string pointed to by psz1 has a lesser value than that pointed to by psz2.
        /// </returns>
        public static int StrCmpLogicalW(string psz1, string psz2)
        {
            if (psz1 == null)
                throw new ArgumentNullException(nameof(psz1));
            if (psz2 == null)
                throw new ArgumentNullException(nameof(psz2));

            return _StrCmpLogicalW(psz1, psz2);
        }

        #endregion
    }

    public static class User32
    {
        #region GetKeyState (2000 Professional/2000 Server)

        /// <summary>
        /// Retrieves the status of the specified virtual key.
        /// The status specifies whether the key is up, down, or toggled (on, off—alternating each time the key is pressed).
        /// </summary>
        /// <param name="nVirtKey">
        /// A virtual key. If the desired virtual key is a letter or digit (A through Z, a through z, or 0 through 9), nVirtKey must be set to the ASCII value of that character.
        /// For other keys, it must be a virtual-key code.
        /// If a non-English keyboard layout is used, virtual keys with values in the range ASCII A through Z and 0 through 9 are used to specify most of the character keys.
        /// For example, for the German keyboard layout, the virtual key of value ASCII O(0x4F) refers to the "o" key, whereas VK_OEM_1 refers to the "o with umlaut" key.</param>
        /// <returns>
        /// <para>The return value specifies the status of the specified virtual key, as follows:</para>
        /// <para>If the high-order bit is 1, the key is down; otherwise, it is up.</para>
        /// <para>If the low-order bit is 1, the key is toggled.A key, such as the CAPS LOCK key, is toggled if it is turned on.The key is off and untoggled if the low-order bit is 0. A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key is untoggled.</para>
        /// </returns>
        [DllImport("User32.dll")]
        public static extern short GetKeyState(int nVirtKey);
        //SHORT GetKeyState(int nVirtKey);
        #endregion
    }

    public static class Winmm
    {
        [CLSCompliant(false)]
        public enum TimErr : uint
        {
            NoError = 0,
            Base = 96,
            Struct = 129,
            NoCanDo = 97,
        }

        #region timeGetTime (2000 Professional/2000 Server)

        /// <summary>
        /// <para>The timeGetTime function retrieves the system time, in milliseconds.</para><para>The system time is the time elapsed since Windows was started.</para>
        /// </summary>
        /// <returns>Returns the system time, in milliseconds.</returns>
        [CLSCompliant(false)]
        [DllImport("Winmm", EntryPoint = "timeGetTime")]
        public static extern uint TimeGetTime();
        // DWORD timeGetTime(void);

        #endregion

        #region timeBeginPeriod (2000 Professional/2000 Server)

        /// <summary>
        /// The timeBeginPeriod function requests a minimum resolution for periodic timers.
        /// </summary>
        /// <param name="uPeriod"><para>Minimum timer resolution, in milliseconds, for the application or device driver.</para>
        /// <para>A lower value specifies a higher (more accurate) resolution.</para></param>
        /// <returns>Returns <see cref="TimErr.NoError"/> if successful or <see cref="TimErr.NoCanDo"/> if the resolution specified in uPeriod is out of range.</returns>
        [DllImport("Winmm", EntryPoint = "timeBeginPeriod")]
        internal static extern uint _timeBeginPeriod(uint uPeriod);
        // MMRESULT timeBeginPeriod(UINT uPeriod);

        /// <summary>
        /// The timeBeginPeriod function requests a minimum resolution for periodic timers.
        /// </summary>
        /// <param name="uPeriod"><para>Minimum timer resolution, in milliseconds, for the application or device driver.</para>
        /// <para>A lower value specifies a higher (more accurate) resolution.</para></param>
        /// <returns>Returns <see cref="TimErr.NoError"/> if successful or <see cref="TimErr.NoCanDo"/> if the resolution specified in uPeriod is out of range.</returns>
        [CLSCompliant(false)]
        public static TimErr TimeBeginPeriod(uint uPeriod)
        {
            switch (_timeBeginPeriod(uPeriod))
            {
                case 0:
                    return TimErr.NoError;
                case 97:
                    return TimErr.NoCanDo;
                default:
                    throw new AccessViolationException();
            }
        }

        #endregion

        #region timeEndPeriod (2000 Professional/2000 Server)

        /// <summary>
        /// The timeEndPeriod function clears a previously set minimum timer resolution.
        /// </summary>
        /// <param name="uPeriod">Minimum timer resolution specified in the previous call to the timeBeginPeriod function.</param>
        /// <returns>Returns <see cref="TimErr.NoError"/> if successful or <see cref="TimErr.NoCanDo"/> if the resolution specified in uPeriod is out of range.</returns>
        [DllImport("Winmm", EntryPoint = "timeEndPeriod")]
        internal static extern uint _timeEndPeriod(uint uPeriod);
        // MMRESULT timeEndPeriod(UINT uPeriod);

        /// <summary>
        /// The timeEndPeriod function clears a previously set minimum timer resolution.
        /// </summary>
        /// <param name="uPeriod">Minimum timer resolution specified in the previous call to the timeBeginPeriod function.</param>
        /// <returns>Returns <see cref="TimErr.NoError"/> if successful or <see cref="TimErr.NoCanDo"/> if the resolution specified in uPeriod is out of range.</returns>
        [CLSCompliant(false)]
        public static TimErr TimeEndPeriod(uint uPeriod) => (_timeEndPeriod(uPeriod)) switch
        {
            0 => TimErr.NoError,
            97 => TimErr.NoCanDo,
            _ => throw new AccessViolationException(),
        };

        #endregion
    }
}

namespace Whitebell.Library.Win32Api.Wrap
{
    public static class WinmmTimer
    {
        [SuppressMessage("スタイル", "IDE0044:読み取り専用修飾子を追加します")]
        private static volatile WinmmTimerImpl t;

        static WinmmTimer() => t = new WinmmTimerImpl();

        [CLSCompliant(false)]
        public static uint TickCount => Winmm.TimeGetTime();

        private class WinmmTimerImpl
        {
            public WinmmTimerImpl() => Winmm._timeBeginPeriod(1);

            ~WinmmTimerImpl() => Winmm._timeEndPeriod(1);
        }
    }
}

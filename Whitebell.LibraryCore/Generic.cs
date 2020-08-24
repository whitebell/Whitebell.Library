namespace Whitebell.Library
{
    public static class Generics
    {
        /// <summary>二つの変数の値を入れ替えます。</summary>
        /// <typeparam name="T">値を入れ替える変数の型。</typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}

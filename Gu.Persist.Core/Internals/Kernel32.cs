namespace Gu.Persist.Core
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Calls to Kernel32.dll.
    /// </summary>
    internal static class Kernel32
    {
        /// <summary>
        /// Moves an existing file or directory, including its children, with various move options.
        /// </summary>
        /// <param name="lpExistingFileName">The current name of the file or directory on the local computer.</param>
        /// <param name="lpNewFileName">The new name of the file or directory on the local computer.</param>
        /// <param name="dwFlags">The <see cref="MoveFileFlags"/>.</param>
        /// <returns>True if the function succeeds.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);
    }
}

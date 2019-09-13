// ReSharper disable InconsistentNaming
namespace Gu.Persist.Core
{
    using System;

    [Flags]
    internal enum MoveFileFlags
    {
        /// <summary>
        /// If a file named lpNewFileName exists, the function replaces its contents with the contents of the lpExistingFileName file, provided that security requirements regarding access control lists (ACLs) are met. For more information, see the Remarks section of this topic.
        /// This value cannot be used if lpNewFileName or lpExistingFileName names a directory.
        /// </summary>
        REPLACE_EXISTING = 0x00000001,

        /// <summary>
        /// If the file is to be moved to a different volume, the function simulates the move by using the CopyFile and DeleteFile functions.
        /// If the file is successfully copied to a different volume and the original file is unable to be deleted, the function succeeds leaving the source file intact.
        /// This value cannot be used with MOVEFILE_DELAY_UNTIL_REBOOT.
        /// </summary>
        COPY_ALLOWED = 0x00000002,

        /// <summary>
        /// The system does not move the file until he operating system is restarted. The system moves the file immediately after AUTOCHK is executed, but before creating any paging files. Consequently, this parameter enables the function to delete paging files from previous startups.
        /// This value can be used only if the process is in the context of a user who belongs to the administrators group or the LocalSystem account.
        /// This value cannot be used with MOVEFILE_COPY_ALLOWED.
        /// </summary>
        DELAY_UNTIL_REBOOT = 0x00000004,

        /// <summary>
        /// The function does not return until the file is actually moved on the disk.Setting this value guarantees that a move performed as a copy and delete operation is flushed to disk before the function returns.
        /// The flush occurs at the end of the copy operation.
        /// This value has no effect if MOVEFILE_DELAY_UNTIL_REBOOT is set.
        /// </summary>
        WRITE_THROUGH = 0x00000008,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        CREATE_HARDLINK = 0x00000010,

        /// <summary>
        /// The function fails if the source file is a link source, but the file cannot be tracked after the move. This situation can occur if the destination is a volume formatted with the FAT file system.
        /// </summary>
        FAIL_IF_NOT_TRACKABLE = 0x00000020,
    }
}
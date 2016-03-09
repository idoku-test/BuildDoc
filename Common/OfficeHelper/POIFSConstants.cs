using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class POIFSConstants
    {
        /** Most files use 512 bytes as their big block size */
        public const int SMALLER_BIG_BLOCK_SIZE = 0x0200;

       // public static readonly POIFSBigBlockSize SMALLER_BIG_BLOCK_SIZE_DETAILS =
       //new POIFSBigBlockSize(SMALLER_BIG_BLOCK_SIZE, (short)9);
        /** Some use 4096 bytes */
        public const int LARGER_BIG_BLOCK_SIZE = 0x1000;
        //public static readonly POIFSBigBlockSize LARGER_BIG_BLOCK_SIZE_DETAILS =
        //   new POIFSBigBlockSize(LARGER_BIG_BLOCK_SIZE, (short)12);
        /** Most files use 512 bytes as their big block size */
        //[Obsolete]
        public const int BIG_BLOCK_SIZE = 0x0200;

        /** Most files use 512 bytes as their big block size */
        //[Obsolete]
        public const int MINI_BLOCK_SIZE = 64;
        /** How big a block in the small block stream is. Fixed size */
        public const int SMALL_BLOCK_SIZE = 0x0040;

        /** How big a single property is */
        public const int PROPERTY_SIZE = 0x0080;
        /** 
         * The minimum size of a document before it's stored using 
         *  Big Blocks (normal streams). Smaller documents go in the 
         *  Mini Stream (SBAT / Small Blocks)
         */
        public const int BIG_BLOCK_MINIMUM_DOCUMENT_SIZE = 0x1000;

        /** The highest sector number you're allowed, 0xFFFFFFFA */
        public const int LARGEST_REGULAR_SECTOR_NUMBER = -5;
        /** Indicates the sector holds a FAT block (0xFFFFFFFD) */
        public const int FAT_SECTOR_BLOCK = -3;
        /** Indicates the sector holds a DIFAT block (0xFFFFFFFC) */
        public const int DIFAT_SECTOR_BLOCK = -4;
        /** Indicates the sector is the end of a chain (0xFFFFFFFE) */
        public const int END_OF_CHAIN = -2;
        /** Indicates the sector is not used (0xFFFFFFFF) */
        public const int UNUSED_BLOCK = -1;
        /** The first 4 bytes of an OOXML file, used in detection */
        public static readonly byte[] OOXML_FILE_HEADER =
            new byte[] { 0x50, 0x4B, 0x03, 0x04 };
    }
}

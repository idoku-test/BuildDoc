using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class HeaderBlockConstants
    {
        //public const long _signature = unchecked((long)16220472316735377360L);
        public const long _signature = unchecked((long)0xE11AB1A1E011CFD0L);
        public const int _bat_array_offset = 0x4c;
        //public const int _max_bats_in_header =
        //    (POIFSConstants.BIG_BLOCK_SIZE - _bat_array_offset)
        //    / LittleEndianConsts.INT_SIZE;

        // useful offsets
        public const int _signature_offset = 0;
        public const int _bat_count_offset = 0x2C;
        public const int _property_start_offset = 0x30;
        public const int _sbat_start_offset = 0x3C;
        public const int _sbat_block_count_offset = 0x40;
        public const int _xbat_start_offset = 0x44;
        public const int _xbat_count_offset = 0x48;
    }
}

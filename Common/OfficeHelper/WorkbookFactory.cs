using System;
using System.IO;
using NPOI.Util;

namespace Common
{
    public class WordDocuemntFactory
    {
        public static IWordDocument Create(Stream inputStream)
        {
            if (HasPOIFSHeader(inputStream))
            {
                return new DocumentWith2003(inputStream);
            }
            else if (HasOOXMLHeader(inputStream))
            {
                return new DocumentWith2007(inputStream);
            }
            throw new ArgumentException("Your InputStream was neither an OLE2 stream, nor an OOXML stream.");
        }

        public static bool HasPOIFSHeader(Stream inp)
        {
            inp.Position = 0;
            byte[] header = new byte[8];
            IOUtils.ReadFully(inp, header);
            LongField signature = new LongField(HeaderBlockConstants._signature_offset, header);
            
            return (signature.Value == HeaderBlockConstants._signature);
        }

        public static bool HasOOXMLHeader(Stream inp)
        {
            inp.Position = 0;
            // We want to peek at the first 4 bytes
            byte[] header = new byte[4];
            IOUtils.ReadFully(inp, header);

            // Wind back those 4 bytes
            if (inp is PushbackStream)
            {
                PushbackStream pin = (PushbackStream)inp;
                pin.Position = pin.Position - 4;
            }
            else
            {
                inp.Position = 0;
            }

            // Did it match the ooxml zip signature?
            return (
                    header[0] == POIFSConstants.OOXML_FILE_HEADER[0] &&
                    header[1] == POIFSConstants.OOXML_FILE_HEADER[1] &&
                    header[2] == POIFSConstants.OOXML_FILE_HEADER[2] &&
                    header[3] == POIFSConstants.OOXML_FILE_HEADER[3]
            );
        }
    }
}
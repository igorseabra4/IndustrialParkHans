using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialParkHans.BlockTypes
{
    public class Section_SVID : Block
    {
        [Description("Unknown function.")]
        public int Version { get; set; }

        public Section_SVID()
        {
            sectionIdentifier = Section.SVID;
        }

        public Section_SVID(BinaryReader binaryReader)
        {
            sectionIdentifier = Section.SVID;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            Version = binaryReader.ReadInt32().Switch();

            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = Section.SVID;

            int previousSize = listBytes.Count;
            
            listBytes.AddRange(Version.Reverse());

            bytesUsed = listBytes.Count - previousSize;
            blockSize = bytesUsed;
        }
    }
}

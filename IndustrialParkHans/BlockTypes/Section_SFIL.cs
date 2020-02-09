using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialParkHans.BlockTypes
{
    public class Section_SFIL : Block
    {
        [Description("Unknown function.")]
        public string UnknownText { get; set; }

        public Section_SFIL()
        {
            sectionIdentifier = Section.SFIL;
            UnknownText = "RyanNeil";
        }

        public Section_SFIL(BinaryReader binaryReader)
        {
            sectionIdentifier = Section.SFIL;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            UnknownText = "";

            byte c = binaryReader.ReadByte();
            while (c != 0xBF)
            {
                UnknownText += (char)c;
                c = binaryReader.ReadByte();
            }

            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = Section.SFIL;

            int previousSize = listBytes.Count;

            foreach (char c in UnknownText)
                listBytes.Add((byte)c);

            bytesUsed = listBytes.Count - previousSize;

            for (int i = bytesUsed; i < 36008; i++)
                listBytes.Add(0xBF);

            blockSize = listBytes.Count - previousSize;
        }
    }
}

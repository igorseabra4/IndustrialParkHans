﻿using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialParkHans.BlockTypes
{
    public class Section_PREF_Scoo : Block
    {
        public SoundMode SoundMode { get; set; }
        [Description("0 to 1.")]
        public float MusicVolume { get; set; }
        [Description("0 to 1.")]
        public float SfxVolume { get; set; }
        [Description("Unknown function.")]
        public float UnknownFloat3 { get; set; }
        [Description("Unknown function.")]
        public float UnknownFloat4 { get; set; }
        public Rumble Rumble { get; set; }

        public Section_PREF_Scoo()
        {
            sectionIdentifier = Section.PREF;
        }

        public Section_PREF_Scoo(BinaryReader binaryReader)
        {
            sectionIdentifier = Section.PREF;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            SoundMode = (SoundMode)binaryReader.ReadInt32().Switch();
            MusicVolume = binaryReader.ReadSingle().Switch();
            SfxVolume = binaryReader.ReadSingle().Switch();
            UnknownFloat3 = binaryReader.ReadSingle().Switch();
            UnknownFloat4 = binaryReader.ReadSingle().Switch();
            Rumble = (Rumble)binaryReader.ReadInt32().Switch();

            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = Section.PREF;

            int previousSize = listBytes.Count;
            
            listBytes.AddRange(((int)SoundMode).Reverse());
            listBytes.AddRange(MusicVolume.Reverse());
            listBytes.AddRange(SfxVolume.Reverse());
            listBytes.AddRange(UnknownFloat3.Reverse());
            listBytes.AddRange(UnknownFloat4.Reverse());
            listBytes.AddRange(((int)Rumble).Reverse());

            bytesUsed = listBytes.Count - previousSize;

            for (int i = bytesUsed; i < 0x30; i++)
                listBytes.Add(0xBF);

            blockSize = listBytes.Count - previousSize;
        }
    }
}

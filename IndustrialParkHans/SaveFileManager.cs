using System;
using System.Collections.Generic;
using System.IO;

namespace IndustrialParkHans.BlockTypes
{
    public class SaveFileManager
    {
        private byte[] gciData;
        public List<Block> Blocks { get; set; }
        public string GameName { get; set; }
        public int TotalFileBytes { get; set; }

        public void ReadFile(string fileName)
        {
            ReadFile(new BinaryReader(new FileStream(fileName, FileMode.Open)));
        }

        public void ReadFile(byte[] file)
        {
            ReadFile(new BinaryReader(new MemoryStream(file)));
        }

        public void ReadFile(BinaryReader binaryReader)
        {
            gciData = binaryReader.ReadBytes(0x5880);

            binaryReader.BaseStream.Position += 0x200;

            int stringStart = (int)binaryReader.BaseStream.Position;

            GameName = "";

            byte c = binaryReader.ReadByte();
            while (c != 0)
            {
                GameName += (char)c;
                c = binaryReader.ReadByte();
            }

            binaryReader.BaseStream.Position = stringStart + 0x5C0;

            Blocks = new List<Block>();

            string currentSection;

            bool continueLoop = true;
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length && continueLoop)
            {
                currentSection = new string(binaryReader.ReadChars(4));
                switch (currentSection)
                {
                    case "CNTR":
                        Blocks.Add(new Section_CNTR(binaryReader));
                        break;
                    case "GDAT":
                        Blocks.Add(new Section_GDAT(binaryReader));
                        break;
                    case "LEDR":
                        Blocks.Add(new Section_LEDR(binaryReader));
                        break;
                    case "PLYR":
                        Blocks.Add(new Section_PLYR(binaryReader));
                        break;
                    case "PREF":
                        Blocks.Add(new Section_PREF(binaryReader));
                        break;
                    case "ROOM":
                        Blocks.Add(new Section_ROOM(binaryReader));
                        break;
                    case "SFIL":
                        Blocks.Add(new Section_SFIL(binaryReader));
                        break;
                    case "SVID":
                        Blocks.Add(new Section_SVID(binaryReader));
                        break;
                    default:
                        if (currentSection != "\0\0\0\0")
                            Blocks.Add(new Section_Scene(binaryReader, currentSection));
                        else 
                            continueLoop = false;
                        break;
                }
            }

            binaryReader.Close();
        }

        public void WriteFile(string fileName)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create));

            writer.Write(gciData);

            for (int i = 0; i < 0x200; i++)
                writer.Write((byte)0);

            int stringStart = (int)writer.BaseStream.Position;

            foreach (char c in GameName)
                writer.Write(c);

            while (writer.BaseStream.Length < stringStart + 0x5C0)
                writer.Write((byte)0);

            int gdatUsedPos = 0;

            List<byte> bytes = new List<byte>();
            foreach (Block b in Blocks)
            {
                if (b is Section_GDAT)
                    gdatUsedPos = bytes.Count + 8;
                b.SetBytes(ref bytes);
            }

            bytes[gdatUsedPos] = BitConverter.GetBytes(bytes.Count)[3];
            bytes[gdatUsedPos + 1] = BitConverter.GetBytes(bytes.Count)[2];
            bytes[gdatUsedPos + 2] = BitConverter.GetBytes(bytes.Count)[1];
            bytes[gdatUsedPos + 3] = BitConverter.GetBytes(bytes.Count)[0];

            writer.Write(bytes.ToArray());

            while(writer.BaseStream.Length < 0x14040)
                writer.Write((byte)0);

            writer.Close();
        }

        public void Add(Section section)
        {
            switch (section)
            {
                case Section.CNTR:
                    Blocks.Add(new Section_CNTR());
                    break;
                case Section.GDAT:
                    Blocks.Add(new Section_GDAT());
                    break;
                case Section.LEDR:
                    Blocks.Add(new Section_LEDR());
                    break;
                case Section.PLYR:
                    Blocks.Add(new Section_PLYR());
                    break;
                case Section.PREF:
                    Blocks.Add(new Section_PREF());
                    break;
                    case Section.ROOM:
                    Blocks.Add(new Section_ROOM());
                    break;
                case Section.SFIL:
                    Blocks.Add(new Section_SFIL());
                    break;
                case Section.SVID:
                    Blocks.Add(new Section_SVID());
                    break;
                case Section.Scene:
                    Blocks.Add(new Section_Scene());
                    break;
                default:
                    throw new Exception("Unknown section type");
            }
        }
    }
}

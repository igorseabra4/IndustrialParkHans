﻿using IndustrialParkHans.BlockTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialParkHans
{
    public partial class HansMainForm : Form
    {
        public HansMainForm()
        {
            InitializeComponent();
            saveFileManager = new SaveFileManager();
        }

        private SaveFileManager saveFileManager;
        private string currentFile;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFile.FileName;
                saveFileManager.ReadFile(openFile.FileName);
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                groupBox1.Enabled = true;
                FillBlocksListBox();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileManager.WriteFile(currentFile);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                currentFile = saveFile.FileName;
                saveFileManager.WriteFile(saveFile.FileName);
            }
        }

        private void FillBlocksListBox()
        {
            listBoxBlocks.Items.Clear();
            foreach (Block b in saveFileManager.Blocks)
                if (b is Section_Scene scene)
                    listBoxBlocks.Items.Add(scene.SceneID);
                else
                    listBoxBlocks.Items.Add(b.sectionIdentifier.ToString());
        }
        
        private void listBoxBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1 && listBoxBlocks.SelectedIndex < saveFileManager.Blocks.Count)
                propertyGridSectionEditor.SelectedObject = saveFileManager.Blocks[listBoxBlocks.SelectedIndex];
            else
                propertyGridSectionEditor.SelectedObject = null;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddSectionDialog addSectionDialog = new AddSectionDialog();
            addSectionDialog.ShowDialog();
            if (addSectionDialog.OKed)
            {
                saveFileManager.Add(addSectionDialog.section);

                Block b = saveFileManager.Blocks.Last();
                if (b is Section_Scene scene)
                    listBoxBlocks.Items.Add(scene.SceneID);
                else
                    listBoxBlocks.Items.Add(b.sectionIdentifier.ToString());
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1)
            {
                int removeIndex = listBoxBlocks.SelectedIndex;
                listBoxBlocks.SelectedIndex = -1;
                saveFileManager.Blocks.RemoveAt(removeIndex);
                listBoxBlocks.Items.RemoveAt(removeIndex);
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1)
            {
                Clipboard.SetText(JsonConvert.SerializeObject(
                (
                    saveFileManager.Blocks[listBoxBlocks.SelectedIndex].sectionIdentifier,
                    JsonConvert.SerializeObject(saveFileManager.Blocks[listBoxBlocks.SelectedIndex], Formatting.Indented)
                ), Formatting.Indented));
            }
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            try
            {
                (Section, string) container = JsonConvert.DeserializeObject<(Section, string)>(Clipboard.GetText());
                
                switch (container.Item1)
                {
                    case Section.CNTR:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_CNTR>(container.Item2));
                        break;
                    case Section.GDAT:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_GDAT>(container.Item2));
                        break;
                    case Section.LEDR:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_LEDR>(container.Item2));
                        break;
                    case Section.PLYR:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_PLYR>(container.Item2));
                        break;
                    case Section.PREF:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_PREF>(container.Item2));
                        break;
                    case Section.ROOM:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_ROOM>(container.Item2));
                        break;
                    case Section.SFIL:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_SFIL>(container.Item2));
                        break;
                    case Section.SVID:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_SVID>(container.Item2));
                        break;
                    case Section.Scene:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_Scene>(container.Item2));
                        break;
                    default:
                        throw new Exception("Unknown section type");
                }

                Block b = saveFileManager.Blocks.Last();
                if (b is Section_Scene scene)
                    listBoxBlocks.Items.Add(scene.SceneID);
                else
                    listBoxBlocks.Items.Add(b.sectionIdentifier.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting section: " + ex.Message + "\nAre you sure you have a section copied?");
            }
        }

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1)
            {
                int previndex = listBoxBlocks.SelectedIndex;

                if (previndex > 0)
                {
                    Block previous = saveFileManager.Blocks[previndex - 1];
                    saveFileManager.Blocks[previndex - 1] = saveFileManager.Blocks[previndex];
                    saveFileManager.Blocks[previndex] = previous;
                }

                FillBlocksListBox();
                listBoxBlocks.SelectedIndex = Math.Max(previndex - 1, 0);
            }
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1)
            {
                int previndex = listBoxBlocks.SelectedIndex;

                if (previndex < listBoxBlocks.Items.Count - 1)
                {
                    Block previous = saveFileManager.Blocks[previndex + 1];
                    saveFileManager.Blocks[previndex + 1] = saveFileManager.Blocks[previndex];
                    saveFileManager.Blocks[previndex] = previous;
                }

                FillBlocksListBox();
                listBoxBlocks.SelectedIndex = Math.Min(previndex + 1, listBoxBlocks.Items.Count - 1);
            }
        }

        private void propertyGridSectionEditor_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridSectionEditor.SelectedObject is Section_Scene scene)
                listBoxBlocks.Items[listBoxBlocks.SelectedIndex] = scene.SceneID;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hans v0.1 is a SpongeBob Squarepants: Battle For Bikini Bottom save file editor by igorseabra4; additional credits go to Seil for figuring out the format in the first place!");
        }
    }
}

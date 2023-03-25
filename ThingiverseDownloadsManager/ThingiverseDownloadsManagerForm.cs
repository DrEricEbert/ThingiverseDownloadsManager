using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Ionic.Zip;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ThingiverseDownloadsManager
{
    public partial class ThingiverseDownloadsManagerForm : Form
    {
        public ThingiverseDownloadsManagerForm()
        {
            try
            {
                Directory.Delete("images", true);
            }catch(SystemException) 
            { }
            InitializeComponent();
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void verzeichnisAuswählenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = "C:\\Data\\stl";
            if (fbd.ShowDialog() != DialogResult.OK) return;
            string[] zips = Directory.GetFiles(fbd.SelectedPath, "*.zip",SearchOption.AllDirectories);
            listViewZips.BeginUpdate();
            foreach (string zip in zips) 
            {
                ListViewItem item= listViewZips.Items.Add(Path.GetFileNameWithoutExtension(zip));
                item.Tag = zip;
            }
            listViewZips.EndUpdate();
        }

        private void listViewZips_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageList1.Images.Clear();
            imageList1.ImageSize = new Size(128, 128);
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            listViewImages.Clear();
            try
            {
                Directory.Delete("images", true);
            }
            catch (SystemException)
            { }
            listViewImages.LargeImageList = imageList1;
            if (listViewZips.SelectedItems.Count != 1) return;
            ListViewItem item = listViewZips.SelectedItems[0];
            string path = (string)item.Tag;
            
            using (ZipFile zip = ZipFile.Read(path))
            {
                bool result = zip.ContainsEntry("README.txt");
                if (result)
                {
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.FileName.Contains("images/")) 
                        {
                            entry.Extract(ExtractExistingFileAction.DoNotOverwrite);
                            Bitmap i = new Bitmap(entry.FileName);
                            imageList1.Images.Add(entry.FileName, i);
                            var listViewItem = listViewImages.Items.Add(entry.FileName);
                            // and tell the item which image to use
                            listViewItem.ImageKey = entry.FileName;
                            listViewItem.Tag = i;


                        }
                        if (entry.FileName.Contains("README.txt")) 
                        {
                            entry.Extract(ExtractExistingFileAction.OverwriteSilently);
                            TextReader tr = new StreamReader("README.txt");
                            string txt = tr.ReadToEnd();
                            tr.Close();
                            textBox.Text=txt;
                        }
                    }
                    //zip.ExtractAll(target, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            listViewImages.LargeImageList = imageList1;
           // 
        }

        private void listViewImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewImages.SelectedItems.Count != 1) return;
             Bitmap bmp = (Bitmap)listViewImages.SelectedItems[0].Tag;
            this.pictureBox.Image = bmp;
        }

        private void entpackenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewZips.SelectedItems.Count != 1) return;
            ListViewItem item = listViewZips.SelectedItems[0];
            string path = (string)item.Tag;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = "C:\\Data\\stl";
            if (fbd.ShowDialog() != DialogResult.OK) return;

            using (ZipFile zip = ZipFile.Read(path))
            {
               zip.ExtractAll(fbd.SelectedPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}

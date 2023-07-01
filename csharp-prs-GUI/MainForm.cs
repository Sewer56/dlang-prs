using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csharp_prs;

namespace csharp_prs_GUI
{
    public partial class MainForm : Form
    {
        // Name of the directory relative to application directory to which all files are output.
        public const string TempDirectoryName = "temp-output";

        public MainForm()
        {
            InitializeComponent();
            this.Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            PrepareOutputFolder();
        }

        /*
            -----------
            Drag Events
            -----------
        */

        /*
            Change Mouse Cursor & Target Drop Effect on Drag Enter with Files
        */
        private void btn_CompressDragDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void btn_DecompressDragDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }


        /*
            Process each file in a DragDrop operation.
        */
        private void btn_CompressDragDrop_DragDrop(object sender, DragEventArgs e)
        {
            // Get files dropped onto button and process them.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            int searchBufferSize = (int)nud_SearchBufferSize.Value;
            ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(files, options, file =>
            {
                if (Directory.Exists(file))
                {
                    // If Directory, Do All Relative Files.
                    var directoryName = Path.GetFileName(file);
                    var filesInDirectory = Directory.GetFiles(file, "*.*", SearchOption.AllDirectories);
                    foreach (var fileInDirectory in filesInDirectory)
                    {
                        var relativePath    = Paths.GetRelativePath(fileInDirectory, file);
                        var destinationPath = Paths.AppendRelativePath(relativePath, $"{TempDirectoryName}/{directoryName}");
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        File.WriteAllBytes(destinationPath, Prs.Compress(File.ReadAllBytes(fileInDirectory), searchBufferSize));
                    }
                }
                else
                {
                    // Else compress just the file.
                    byte[] decompressedFile = File.ReadAllBytes(file);
                    byte[] compressedFile = Prs.Compress(ref decompressedFile, searchBufferSize);

                    string fileName = Path.GetFileName(file);
                    File.WriteAllBytes($"{TempDirectoryName}\\{fileName}", compressedFile);
                }
            });

            // Open directory
            Process.Start(TempDirectoryName);
        }

        private void btn_DecompressDragDrop_DragDrop(object sender, DragEventArgs e)
        {
            // Get files dropped onto button and process them.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(files, file =>
            {
                byte[] compressedFile   = File.ReadAllBytes(file);
                byte[] decompressedFile = Prs.Decompress(ref compressedFile);

                string fileName = Path.GetFileName(file);
                File.WriteAllBytes($"{TempDirectoryName}\\{fileName}", decompressedFile);
            });

            // Open directory
            Process.Start(TempDirectoryName);
        }

        /*
            ----------------------------
            Compress/Decompress Routines
            ----------------------------
        */

        private void PrepareOutputFolder()
        {
            if (! Directory.Exists(TempDirectoryName))
                Directory.CreateDirectory(TempDirectoryName);
            else
                ClearDirectory(new DirectoryInfo(TempDirectoryName));
        }

        /// <summary>
        /// Removes all files from a specific directory and subdirectories.
        /// </summary>
        private static void ClearDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
                file.Delete();

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                subDirectory.Delete(true);
        }

        /// <summary>
        /// Cleanup our temp folder.
        /// </summary>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Directory.Exists(TempDirectoryName))
                new DirectoryInfo(TempDirectoryName).Delete(true);
        }
    }
}

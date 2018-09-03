using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using CPP2ALF_Transfomer.TransformationEngine;

namespace CPP2ALF_Transfomer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnViewSrcML_Click(object sender, EventArgs e)
        {
            if (cbCustomCode.Checked)
            {
                string cpp = rtbCPP.Text;
                string fileName = Path.Combine(txtOutputDirectory.Text, "tmp.cpp");
                File.WriteAllText(fileName, cpp);
                string srcML = GenerateSrcMl(fileName);
                Editor.ShowDialog(srcML, "srcML");
                return;
            }
                if (clbCppFilesList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select at least one file");
                return;
            }
            FileObject fo = (FileObject)clbCppFilesList.SelectedItem;
            //MessageBox.Show(fo.srcML);
            Editor.ShowDialog(fo.srcML, fo.fileName);

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtOutputDirectory.Text = Application.StartupPath;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fwd = new FolderBrowserDialog();
            fwd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fwd.ShowDialog() == DialogResult.OK)
            {
                clbCppFilesList.Items.Clear();
                string[] fileNames = Directory.GetFiles(fwd.SelectedPath).Where(x => x.EndsWith(".cpp") || x.EndsWith(".h")).ToArray();
                int filesCount = 0;
                int totalFiles = fileNames.Length;
                foreach (string f in fileNames)
                {
                    FileObject fo = new FileObject();
                    fo.fileName = Path.GetFileName(f);
                    fo.fileExtension = Path.GetExtension(fo.fileName);
                    fo.filePath = f;
                    fo.cppCode = File.ReadAllText(f);
                    fo.srcML = GenerateSrcMl(f);
                    clbCppFilesList.Items.Add(fo);
                    SourceFiles.Add(fo);

                    filesCount++;
                    lbl_SrcMLFiles.Text = $"Source Files ({filesCount}/{totalFiles})";
                }

            }


        }
        private string GenerateSrcMl(string path)
        {
            string programLocation = @"D:\Program Files\srcML 0.9.5\bin\srcml.exe";
            string output_filename = txtOutputDirectory.Text + @"\tmp.tmp";
            string command = $" \"{path}\" -o \"{output_filename}\" option --register-ext h=C++";

            if(!File.Exists(programLocation))
            {
                MessageBox.Show($"SrcML Program not found on the given location: {programLocation}");
                return "";
            }

            Process process = new Process();

            ProcessStartInfo psi = new ProcessStartInfo(programLocation, command);
            process.StartInfo = psi;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            if (!process.Start())
            {
                MessageBox.Show("unable to start process");


            }
            process.WaitForExit();

            //while (!process.HasExited)
            //{

            //}
            return File.ReadAllText(txtOutputDirectory.Text + @"\tmp.tmp");
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private List<FileObject> SourceFiles = new List<FileObject>();
        private class FileObject
        {
            public string fileName;
            public string filePath;
            public string fileExtension;
            public string srcML;
            public string cppCode;
            public string alfCode = "";
            public bool MainFile = false;
            public override string ToString()
            {
                return fileName;

            }
        }

        private void clbCppFilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileObject fo = (FileObject)clbCppFilesList.SelectedItem;
            if (fo != null)
            {
                rtbCPP.Text = fo.cppCode;
                string fnwe = Path.GetFileNameWithoutExtension(fo.fileName);
                rtbALF.Text = SourceFiles.First(x => x.fileName == fnwe + ".cpp").alfCode;
            }
        }

        private void btnGenerateALF_Click(object sender, EventArgs e)
        {
            if (cbCustomCode.Checked)
            {
                string cpp = rtbCPP.Text;
                string fileName = Path.Combine(txtOutputDirectory.Text, "tmp.cpp");
                File.WriteAllText(fileName, cpp);
                string srcML = GenerateSrcMl(fileName);
                Transformer tt = new Transformer();
                tt.CppCode = cpp;
                tt.SrcMLCodeCpp = srcML;
                tt.SrcMLCodeH = "";
                rtbALF.Text = tt.Transform(true);

                return;
            }
            if (clbCppFilesList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select at least one file");
                return;
            }


            FileObject fo = (FileObject)clbCppFilesList.SelectedItem;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fo.fileName);
            FileObject cppObj = SourceFiles.First(x => x.fileName == fileNameOnly + ".cpp");
            FileObject hObj = null;
            if (SourceFiles.Where(x => x.fileName == fileNameOnly + ".h").Count() > 0)
                hObj = SourceFiles.First(x => x.fileName == fileNameOnly + ".h");
            else
                cppObj.MainFile = true;

            if (cppObj.alfCode.Trim().Length > 0)
            {
                if (MessageBox.Show("All modifications will be lost, Continue ?", "Sure", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

            }
            Transformer t = new Transformer();

            foreach (var file in SourceFiles.Where(x => x.fileExtension == ".h"))
            {
                string fn = Path.GetFileNameWithoutExtension(file.fileName);
                t.TypeRules.Add(fn, fn, true);
            }
            //t.rtb = rtbALF;

            t.CppCode = fo.cppCode;
            t.SrcMLCodeCpp = cppObj.srcML;
            t.SrcMLCodeH = (hObj == null) ? "" : hObj.srcML;
            //t.AlfCode = fo.alfCode;

            string result = t.Transform();
            rtbALF.Text = result;
            cppObj.alfCode = result;
            UpdateChecks(fileNameOnly);
            //clbCppFilesList.Items.Cast<FileObject>().First(x => x.fileName == fileNameOnly + ".cpp");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clbCppFilesList.Items.Clear();
            SourceFiles = new List<FileObject>();
            string[] fileNames = Directory.GetFiles(@"D:\Asad_Developer\WorkspaceNeon\CPP_CASESTUDY").Where(x => x.EndsWith(".cpp") || x.EndsWith(".h")).ToArray();
            int filesCount = 0;
            int totalFiles = fileNames.Length;
            foreach (string f in fileNames)
            {
                FileObject fo = new FileObject();
                fo.fileName = Path.GetFileName(f);
                fo.fileExtension = Path.GetExtension(fo.fileName);
                fo.filePath = f;
                fo.cppCode = File.ReadAllText(f);
                fo.srcML = GenerateSrcMl(f);
                clbCppFilesList.Items.Add(fo);
                SourceFiles.Add(fo);

                filesCount++;
                lbl_SrcMLFiles.Text = $"Source Files ({filesCount}/{totalFiles})";
            }
        }

        private void trackBarCpp_Scroll(object sender, EventArgs e)
        {
            rtbCPP.ZoomFactor = (float)trackBarCpp.Value / 10f;
        }

        private void trackBarALF_Scroll(object sender, EventArgs e)
        {
            rtbALF.ZoomFactor = (float)trackBarALF.Value / 10f;
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            int width = Width - lbl_topLeft.Left;
            int height = Height - lbl_topLeft.Top;

            rtbCPP.Width = width / 2 - 5;
            rtbCPP.Height = height - 120;

            rtbALF.Left = rtbCPP.Left + rtbCPP.Width + 10;
            rtbALF.Width = rtbCPP.Width - 30;
            rtbALF.Height = rtbCPP.Height;

            gp_bottom.Top = rtbCPP.Bottom + 5;
        }

        bool AllGeneratingCode = false;
        private void rtbALF_TextChanged(object sender, EventArgs e)
        {
            if (AllGeneratingCode)
                return;
            if (clbCppFilesList.SelectedIndex != -1)
            {
                FileObject fo = (FileObject)clbCppFilesList.SelectedItem;

                string fileNameOnly = Path.GetFileNameWithoutExtension(fo.fileName);
                SourceFiles.First(x => x.fileName == fileNameOnly + ".cpp").alfCode = rtbALF.Text;

            }
        }

        private void UpdateChecks(string fileName)
        {
            for (int i = 0; i < clbCppFilesList.Items.Count; i++)
            {
                string fn = ((FileObject)clbCppFilesList.Items[i]).fileName;
                if (fn == fileName + ".h" || fn == fileName + ".cpp")
                {
                    clbCppFilesList.SetItemChecked(i, true);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string outputPath = Path.Combine(txtOutputDirectory.Text, "ALF Generated Code");

            if (!Directory.Exists(outputPath + "\\DefaultPkg"))
                Directory.CreateDirectory(outputPath + "\\DefaultPkg");
            foreach (FileObject fo in SourceFiles)
            {
                if (fo.alfCode.Trim().Length == 0)
                {
                    var res = MessageBox.Show("Some File's Alf code is not genereted yet. Do you want to continue?", "Sure", MessageBoxButtons.YesNo);
                    if (res == DialogResult.No)
                        return;
                    else
                        break;
                }
            }
            foreach (FileObject fo in SourceFiles.Where(x => x.fileExtension == ".cpp"))
            {
                if (fo.alfCode.Trim().Length > 0)
                {
                    string fnwe = Path.GetFileNameWithoutExtension(fo.fileName);
                    string directory = outputPath;
                    if (!fo.MainFile)
                         directory = Path.Combine(outputPath, "DefaultPkg");

                    string fileName = Path.Combine(directory, fnwe + ".alf");
                    File.WriteAllText(fileName, fo.alfCode);

                }
            }
            MessageBox.Show("Alf Code Generated to DefaultPkg in " + Environment.NewLine + outputPath);

        }

        private void btn_GenerateAll_Click(object sender, EventArgs e)
        {
            if (cbCustomCode.Checked)
            {
                string cpp = rtbCPP.Text;
                string fileName = Path.Combine(txtOutputDirectory.Text, "tmp.cpp");
                File.WriteAllText(fileName, cpp);
                string srcML = GenerateSrcMl(fileName);
                Transformer tt = new Transformer();
                tt.CppCode = cpp;
                tt.SrcMLCodeCpp = srcML;
                tt.SrcMLCodeH = "";
                rtbALF.Text = tt.Transform(true);

                return;
            }
            if (clbCppFilesList.Items.Count == 0)
            {
                MessageBox.Show("No source files are loaded. Please browse for cpp and h files.");
                return;
            }


            //FileObject fo = (FileObject)clbCppFilesList.SelectedItem;
            //foreach (var fo in clbCppFilesList.Items.Cast<FileObject>().Where(x => x.fileName.ToLower().EndsWith(".cpp")))
            AllGeneratingCode = true;
            for(int iterator = 0;iterator<clbCppFilesList.Items.Count;iterator++)
            {
                var fo = (FileObject)clbCppFilesList.Items[iterator];
                if (fo.fileName.EndsWith(".h"))
                    continue;
                string fileNameOnly = Path.GetFileNameWithoutExtension(fo.fileName);
                FileObject cppObj = SourceFiles.First(x => x.fileName == fileNameOnly + ".cpp");
                FileObject hObj = null;
                if (SourceFiles.Where(x => x.fileName == fileNameOnly + ".h").Count() > 0)
                    hObj = SourceFiles.First(x => x.fileName == fileNameOnly + ".h");
                else
                    cppObj.MainFile = true; // we decided main method cpp file as this file wont have any h file.

                if (cppObj.alfCode.Trim().Length > 0)
                {
                    if (MessageBox.Show($"All modifications will be lost for the file {fileNameOnly}, Continue ?", "Sure", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;

                }
                Transformer t = new Transformer();

                foreach (var file in SourceFiles.Where(x => x.fileExtension == ".h"))
                {
                    string fn = Path.GetFileNameWithoutExtension(file.fileName);
                    t.TypeRules.Add(fn, fn, true);
                }
                //t.rtb = rtbALF;

                t.CppCode = fo.cppCode;
                t.SrcMLCodeCpp = cppObj.srcML;
                t.SrcMLCodeH = (hObj == null) ? "" : hObj.srcML;
                //t.AlfCode = fo.alfCode;

                string result = t.Transform();
                rtbALF.Text = result;
                cppObj.alfCode = result;
                UpdateChecks(fileNameOnly);
            }
            clbCppFilesList.SelectedIndex = 0;
            AllGeneratingCode = false;
        }
    }
}

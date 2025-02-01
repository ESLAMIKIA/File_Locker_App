using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace FileLocker
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        // Select File
        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        // Lock File
        private void LockFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                lblStatus.Text = "❌ Please select file and password !";
                return;
            }

            try
            {
                EncryptFile(txtFilePath.Text, txtPassword.Password);
                lblStatus.Text = "✅ The file is locked !";
            }
            catch (Exception )
            {
                lblStatus.Text = "❌ Locking error !";
            }
        }

        // Unlock File
        private void UnlockFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                lblStatus.Text = "❌ Please select file and password !";
                return;
            }

            try
            {
                DecryptFile(txtFilePath.Text, txtPassword.Password);
                lblStatus.Text = "✅  File Unlocked !";
            }
            catch (Exception )
            {
                lblStatus.Text = "❌ Wrong password or eror in finding file !";
            }
        }

        // متد رمزگذاری (AES)
        private void EncryptFile(string inputFile, string password)
        {
            byte[] key = Encoding.UTF8.GetBytes(password.PadRight(32).Substring(0, 32));
            byte[] iv = Encoding.UTF8.GetBytes(password.PadRight(16).Substring(0, 16));

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (FileStream fs = new FileStream(inputFile + ".locked", FileMode.Create))
                using (CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (FileStream inputFs = new FileStream(inputFile, FileMode.Open))
                {
                    inputFs.CopyTo(cs);
                }
            }
            File.Delete(inputFile);
        }

        // unlock method(AES)
        private void DecryptFile(string inputFile, string password)
        {
            byte[] key = Encoding.UTF8.GetBytes(password.PadRight(32).Substring(0, 32));
            byte[] iv = Encoding.UTF8.GetBytes(password.PadRight(16).Substring(0, 16));

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                string outputFile = inputFile.Replace(".locked", "");
                using (FileStream fs = new FileStream(outputFile, FileMode.Create))
                using (CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Write))
                using (FileStream inputFs = new FileStream(inputFile, FileMode.Open))
                {
                    inputFs.CopyTo(cs);
                }
            }
            File.Delete(inputFile);
        }
    }
}

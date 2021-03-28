using Microsoft.Win32;
using PAPrefabToolkit;
using PAPrefabToolkit.Data;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;

namespace UnImageObject_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImgBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image|*.png;*.jpg";
            ofd.ShowDialog();

            if (!string.IsNullOrEmpty(ofd.FileName))
            {
                ImgField.Text = ofd.FileName;
            }
        }

        private void OutBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Prefab|*.lsp";
            sfd.ShowDialog();

            if (!string.IsNullOrEmpty(sfd.FileName))
            {
                OutField.Text = sfd.FileName;
            }
        }

        private void ConvertBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(ImgField.Text))
            {
                MessageBox.Show("Image does not exist in specified location!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Prefab prefab = new Prefab();
                prefab.Name = PrefabNameField.Text;
                prefab.Type = (PrefabType)PrefabTypeCombo.SelectedIndex;

                PrefabObject textObject = new PrefabObject(prefab, PrefabNameField.Text);
                textObject.AutoKillType = PrefabObjectAutoKillType.Fixed;
                textObject.AutoKillOffset = 5.0f;
                textObject.Shape = PrefabObjectShape.Text;

                //load image
                Bitmap bmp = new Bitmap(ImgField.Text);

                using (ImageConverter converter = new ImageConverter(bmp))
                {
                    StringBuilder sb = new StringBuilder();
                    converter.WriteToStringBuilder(sb);

                    textObject.Text = sb.ToString();
                }

                //write to output
                File.WriteAllText(OutField.Text, PrefabBuilder.BuildPrefab(prefab, noValidate: true));

                //feedback
                MessageBox.Show("Image successfully converted to prefab.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}

using iTextSharp.text;
using iTextSharp.text.pdf;
using PDFWizard.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Syroot.Windows.IO;

namespace PDFWizard;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
        cl1.ItemsSource = filesList;
    }

    ObservableCollection<FileC> filesList = new ObservableCollection<FileC> ();
   
    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickMultipleAsync(new PickOptions { FileTypes = FilePickerFileType.Pdf});
        if (result != null)
        {
            foreach(var f in result)
            {
                filesList.Add(new FileC { Filename = f.FileName.ToString(), id = filesList.Count() + 1, Fullpath = f.FullPath.ToString() });
            }
        }
        if (filesList.Count > 1)
        {
            mergeBtn.IsVisible = true;
        }
    }
    private void DownClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        int id = Convert.ToInt16(button.ClassId);

        var targetFile = filesList.FirstOrDefault(file => file.id == id);
        if (filesList.IndexOf(targetFile) < filesList.Count - 1)
        {
            filesList.Move(filesList.IndexOf(targetFile), filesList.IndexOf(targetFile) + 1);
        }
    }
    private void Upclicked(object sender, EventArgs e)
    {
        var Button = (Button)sender;
        int id = Convert.ToInt16(Button.ClassId);
        var targetFile = filesList.FirstOrDefault(file => file.id == id);
        if(filesList.IndexOf(targetFile)>0)
        {
            filesList.Move(filesList.IndexOf(targetFile), filesList.IndexOf(targetFile) - 1);
        }
    }
    private void Delete(object sender, EventArgs e)
    {
        var button = (Button)sender;
        int id = Convert.ToInt16(button.ClassId);
        var targetFile = filesList.FirstOrDefault(file => file.id == id);
        filesList.Remove(targetFile);
        if (filesList.Count < 2)
        {
            mergeBtn.IsVisible = false;
        }
    }
    private async void Merge(object sender, EventArgs e)
    {
        string downloadsPath = KnownFolders.Downloads.Path;
        string targetPath = downloadsPath + "\\" + filesList[0].Filename + "-merged.pdf";
        int fileCount = 0;
        while (File.Exists(targetPath))
        {
           fileCount++;
           targetPath = downloadsPath + "\\" + filesList[0].Filename + "-merged"+"("+fileCount +")"+".pdf";
        }
        cl1.IsVisible = false;
        mergeBtn.IsVisible=false;
        Document document = new Document();
        PdfCopy copy = new PdfCopy(document, new FileStream(targetPath, FileMode.Create));
        document.Open();

        foreach (var f in filesList)
        {
            PdfReader reader = new PdfReader(f.Fullpath);
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                PdfImportedPage page = copy.GetImportedPage(reader, i);
                copy.AddPage(page);
            }
            reader.Close();
        }
        document.Close();
        string action = await DisplayActionSheet("Plik został zapisany! Co zrobić?", "Anuluj", null, "Otwórz lokalizację", "Otwórz plik");
        if(action == "Otwórz lokalizację")
        {
            Process.Start("explorer.exe", downloadsPath);
        }
        else if (action == "Otwórz plik")
        {
            Process.Start("explorer.exe", downloadsPath + "\\" + filesList[0].Filename + "-merged.pdf");
        }
        cl1.IsVisible = true;
        filesList.Clear();
    }
}


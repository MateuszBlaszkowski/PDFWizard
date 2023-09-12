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

    ObservableCollection<file> filesList = new ObservableCollection<file> ();
   
    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickMultipleAsync(new PickOptions { FileTypes = FilePickerFileType.Pdf});
        if (result != null)
        {
            foreach(var f in result)
            {
                filesList.Add(new file { Filename = f.FileName.ToString(), id = filesList.Count() + 1, Fullpath = f.FullPath.ToString() });
            }
        }
        if (filesList.Count > 1)
        {
            MergeBtn.IsVisible = true;
        }
    }
    private void DownClicked(object sender, EventArgs e)
    {
        var Button = (Button)sender;
        int id = Convert.ToInt16(Button.ClassId);

        var targetFile1 = filesList.FirstOrDefault(file => file.id == id);
        if (filesList.IndexOf(targetFile1) < filesList.Count - 1)
        {
            filesList.Move(filesList.IndexOf(targetFile1), filesList.IndexOf(targetFile1) + 1);
        }
    }
    private void Upclicked(object sender, EventArgs e)
    {
        var Button = (Button)sender;
        int id = Convert.ToInt16(Button.ClassId);
        var targetFile1 = filesList.FirstOrDefault(file => file.id == id);
        if(filesList.IndexOf(targetFile1)>0)
        {
            filesList.Move(filesList.IndexOf(targetFile1), filesList.IndexOf(targetFile1) - 1);
        }
    }
    private void Delete(object sender, EventArgs e)
    {
        var Button = (Button)sender;
        int id = Convert.ToInt16(Button.ClassId);
        var targetFile1 = filesList.FirstOrDefault(file => file.id == id);
        filesList.Remove(targetFile1);
        if (filesList.Count < 2)
        {
            MergeBtn.IsVisible = false;
        }
    }
    private async void Merge(object sender, EventArgs e)
    {
        string downloadsPath = KnownFolders.Downloads.Path;
        cl1.IsVisible = false;
        MergeBtn.IsVisible=false;
        Document document = new Document();
        PdfCopy copy = new PdfCopy(document, new FileStream(downloadsPath + "\\" + filesList[0].Filename+"-merged.pdf", FileMode.Create));
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
        DisplayAlert("OK", "Plik został zapisany!", "ok");
        Process.Start("explorer.exe", downloadsPath + "\\" + filesList[0].Filename + "-merged.pdf");
        cl1.IsVisible = true;
        filesList.Clear();
    }
}


using System;
using System.Collections.Generic;
using System.IO;

public class PlayList
{
    public System.Windows.Forms.FolderBrowserDialog fbd;
    public string[] _directory { get; set; }
    public List<string> fileList = new List<string>();
    public int _lenght { get; set; }

	public PlayList()
	{
	}

    public PlayList(System.Windows.Forms.FolderBrowserDialog fbd)
    {
        // TODO: Complete member initialization
        if (fbd.SelectedPath != "")
        {
            _directory = Directory.GetFiles(fbd.SelectedPath);
            this.fbd = fbd;
            this._lenght = fileList.Count;
        }
    }
}

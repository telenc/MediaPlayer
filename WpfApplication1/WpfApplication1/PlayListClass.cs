using System;
using System.Collections.Generic;
using System.IO;

public class PlayList
{
    public System.Windows.Forms.FolderBrowserDialog fbd;
    public string[] _directory { get; set; }
    public List<string> fileList = new List<string>();

	public PlayList()
	{
	}

    public PlayList(System.Windows.Forms.FolderBrowserDialog fbd)
    {
        // TODO: Complete member initialization
        _directory = Directory.GetFiles(fbd.SelectedPath);
        this.fbd = fbd;
    }
}

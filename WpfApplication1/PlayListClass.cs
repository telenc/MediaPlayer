using System;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Timers;
using System.Windows.Threading;
using System.Speech.Recognition;
using MyWindowsMediaPlayerV2;

public class PlayList
{

    string[] _directory;
    System.Windows.Forms.FolderBrowserDialog _fbd;

    public PlayList()
    { }

    public PlayList(System.Windows.Forms.FolderBrowserDialog fbd)
	{
        _fbd = fbd;
        _directory = Directory.GetFiles(fbd.SelectedPath);
	}
}

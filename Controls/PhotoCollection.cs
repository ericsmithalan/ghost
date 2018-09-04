using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Ghost.Controls
{
    public class PhotoCollection : ObservableCollection<Photo>
    {
        public PhotoCollection()
        {
        }

        public PhotoCollection(string path) : this(new DirectoryInfo(path))
        {
        }

        public PhotoCollection(DirectoryInfo directory)
        {
            try
            {
                _directory = directory;
                Update();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        public string Path
        {
            set
            {
                _directory = new DirectoryInfo(value);
                Update();
            }
            get { return _directory.FullName; }
        }

        public DirectoryInfo Directory
        {
            set
            {
                _directory = value;
                Update();
            }
            get { return _directory; }
        }

        private void Update()
        {
            Clear();
            try
            {
                foreach (FileInfo f in _directory.GetFiles("*"))
                {
                    if (f.Extension.ToLower() == ".jpg" || f.Extension.ToLower() == ".gif" || f.Extension.ToLower() == ".png")
                    {
                        Add(new Photo(f.FullName));
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private DirectoryInfo _directory;
    }
}